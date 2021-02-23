using System.Collections;
using System.Collections.Generic;
using FirebaseREST;
using UnityEngine;
using System;
using UniRx;

namespace Host
{
    public class HostModel : MonoBehaviour
    {
        public IObservable<string> SubmitHostPhaseEvent => hostPhaseSubject;
        public IObservable<int> SubmitHostNumberEvent => hostSubmitNumSubject;

        private Subject<string> hostPhaseSubject = new Subject<string>();
        private Subject<int> hostSubmitNumSubject = new Subject<int>();

        private string[] phases = { HostPhase.SelectNum, HostPhase.PresentQuestion, HostPhase.PresentAnswer };
        
        private int nowPhaseNum;

        public void InitModel()
        {
            nowPhaseNum = 0;
            SetHostPhase(HostPhase.SelectNum);
            DeleteAllNumbers();
        }

        // HostのPhaseが変わった時の処理
        public void OnChangeHostPhase(Unit d)
        {
            nowPhaseNum++;
            if (nowPhaseNum >= phases.Length) nowPhaseNum = 0;
            SetHostPhase(phases[nowPhaseNum]);
        }

        public void OnChangeHostBingoNum(Unit d)
        {
            SubmissionNumber();
        }

        private void DeleteAllNumbers()
        {
            DatabaseReference reference = FirebaseDatabase.Instance.GetReference("Host/numbers");
            reference.RemoveValueAsync(10, (e) =>
            {
                if (e.success)
                {
                    Debug.Log("Delete data success");
                }
                else
                {
                    Debug.Log("Delete data failed : " + e.message);
                }
            });
        }

        // DataBaseにあるHostのPhase変更処理とPresenterにイベントの通知
        private void SetHostPhase(string _phase)
        {
            DatabaseReference reference = FirebaseDatabase.Instance.GetReference("Host/phase");
            reference.SetValueAsync(_phase, 10, (res) =>
            {
                if (res.success)
                {
                    Debug.Log("Write success");
                }
                else
                {
                    Debug.Log("Write failed : " + res.message);
                }
            });

            hostPhaseSubject.OnNext(_phase);
        }

        // 数字を送る
        private void SubmissionNumber()
        {
            var num = RandomGenerateNumber();
            DatabaseReference reference = FirebaseDatabase.Instance.GetReference("Host/numbers");
            reference.Push(num, 10, (res)=>{
                if (res.success)
                {
                    Debug.Log("Pushed with id: " + res.data);
                }
                else
                {
                    Debug.Log("Push failed : " + res.message);
                }
            });


            hostSubmitNumSubject.OnNext(num);
        }

        // ビンゴの乱数生成
        private int RandomGenerateNumber()
        {
            return UnityEngine.Random.Range(1, 26);
        }
    }
}