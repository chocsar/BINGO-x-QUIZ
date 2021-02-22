using UnityEngine;
using UnityEngine.UI;
using UniRx;
using System;

namespace Host
{
    public class HostView : MonoBehaviour
    {
        [SerializeField] private Button nextPhaseButton;
        [SerializeField] private Text phaseText;

        public IObservable<Unit> ChangeHostPhaseEvent => changeHostPhaseSubject;

        private Subject<Unit> changeHostPhaseSubject = new Subject<Unit>();

        public void InitView()
        {
            nextPhaseButton.onClick.AddListener(() => NextPhase());
        }

        // HostのPhaseが変わった時の見た目の処理
        public void OnChangeHostPhase(string _phase)
        {
            phaseText.text = $"PHASE : {_phase}";
        }

        // ビンゴの値を受入れ
        public void RecieveHostNum(int _bingoNum)
        {
            //TODO:
            // テキストで受け取った数字の表示
        }

        // Phaseの変更通知
        private void NextPhase()
        {
            changeHostPhaseSubject.OnNext(Unit.Default);
        }

        // 数字生成のイベント通知
        private void GenerateNumber()
        {

        }
        
    }
}

