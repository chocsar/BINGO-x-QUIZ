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
        [SerializeField] private HostFirebaseLoader firebaseLoader;

        public IObservable<string> SubmitHostPhaseEvent => hostPhaseSubject;
        public IObservable<int> SubmitHostNumberEvent => hostSubmitNumSubject;
        public IObservable<Dictionary<string, int>> ClientPhaseEvent => loadClientPhaseSubject;
        public IObservable<List<ClientStatus>> ClientStatusEvent => loadClientStatusSubject;
         
        private Subject<string> hostPhaseSubject = new Subject<string>();
        private Subject<int> hostSubmitNumSubject = new Subject<int>();
        private Subject<Dictionary<string, int>> loadClientPhaseSubject = new Subject<Dictionary<string, int>>();
        private Subject<List<ClientStatus>> loadClientStatusSubject = new Subject<List<ClientStatus>>();

        private string[] phases = { HostPhase.SelectNum, HostPhase.PresentQuestion, HostPhase.PresentAnswer };
        
        private int nowPhaseNum;
        private List<int> useNumberList = new List<int>();

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

        public void OnChangeHostBingoNum(int num)
        {
            SubmissionNumber(num);
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
        private void SubmissionNumber(int num)
        {
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

        public void LoadClientPhase(Unit d)
        {
            var data = new ReactiveProperty<string>();
            DatabaseReference reference = FirebaseDatabase.Instance.GetReference("userphase");
            reference.GetValueAsync(10, (res) =>
            {
                if (res.success)
                {
                    Debug.Log("Success fetched data : " + res.data.GetRawJsonValue());
                    data.Value = res.data.GetRawJsonValue();
                    data.Subscribe(x =>
                    {
                        Dictionary<string, string> phaseDic = new Dictionary<string, string>();
                        phaseDic = Utility.UtilityRestJson.JsonPhaseLoad(x);
                        loadClientPhaseSubject.OnNext(PhaseCheck(phaseDic));
                    });
                }
                else
                {
                    Debug.Log("Fetch data failed : " + res.message);
                }
            });
        }

        public void LoadClientStatus(Unit d)
        {
            var data = new ReactiveProperty<string>();
            DatabaseReference reference = FirebaseDatabase.Instance.GetReference("usernameandstatus");
            reference.GetValueAsync(10, (res) =>
            {
                if (res.success)
                {
                    Debug.Log("Success fetched data : " + res.data.GetRawJsonValue());
                    data.Value = res.data.GetRawJsonValue();
                    data.Subscribe(x =>
                    {
                        var statusList = Utility.UtilityRestJson.JsonStatusLoad(x);
                        //foreach (var item in statusList)
                        //{
                        //    Debug.Log($"status: {item.status}, username: {item.username}");
                        //}
                        loadClientStatusSubject.OnNext(statusList);
                    });
                }
            });
        }

        // ビンゴの乱数生成
        private int RandomGenerateNumber()
        {
            return UnityEngine.Random.Range(1, 26);
        }

        private Dictionary<string, int> PhaseCheck(Dictionary<string, string> dic)
        {
            Debug.Log("check");
            int[] phaseNum = new int[5]; // 1:ready, 2:beforeanswer, 3: answer, 4: afteranswer, 5: open
            foreach (var data in dic)
            {
                Debug.Log("phase : " + data.Value);
                switch (data.Value)
                {
                    case UserBingoPhase.Ready:
                        phaseNum[0]++;
                        break;
                    case UserBingoPhase.BeforeAnswer:
                        phaseNum[1]++;
                        break;
                    case UserBingoPhase.Answer:
                        phaseNum[2]++;
                        break;
                    case UserBingoPhase.AfterAnswer:
                        phaseNum[3]++;
                        break;
                    case UserBingoPhase.Open:
                        phaseNum[4]++;
                        break;
                    default:
                        phaseNum[4]++;
                        break;
                }
            }
            Dictionary<string, int> sendData = new Dictionary<string, int>()
            {
                { UserBingoPhase.Ready, phaseNum[0]},
                { UserBingoPhase.BeforeAnswer, phaseNum[1]},
                { UserBingoPhase.Answer, phaseNum[2]},
                { UserBingoPhase.AfterAnswer, phaseNum[3]},
                { UserBingoPhase.Open, phaseNum[4]},
            };

            return sendData;
        }
    }
}