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
        [SerializeField] private Button generateNumButton;
        [SerializeField] private Text bingoNumText;
        [SerializeField] private InputField sendNumInputField;

        public IObservable<Unit> ChangeHostPhaseEvent => changeHostPhaseSubject;
        public IObservable<int> ChangeHostBingoNumEvent => changeHostBingoSubject;

        private Subject<Unit> changeHostPhaseSubject = new Subject<Unit>();
        private Subject<int> changeHostBingoSubject = new Subject<int>();

        private string nowPhase;

        public void InitView()
        {
            nextPhaseButton.onClick.AddListener(() => NextPhase());
            generateNumButton.onClick.AddListener(() => SubmitNumber());
        }

        // HostのPhaseが変わった時の見た目の処理
        public void OnChangeHostPhase(string _phase)
        {
            nowPhase = _phase;
            phaseText.text = $"PHASE : {_phase}";
        }

        // ビンゴの値を受入れ
        public void RecieveHostNum(int _bingoNum)
        {
            bingoNumText.text = $"BingoNum : {_bingoNum}";
        }

        // Phaseの変更通知
        private void NextPhase()
        {
            changeHostPhaseSubject.OnNext(Unit.Default);
        }

        // 数字生成のイベント通知
        private void SubmitNumber()
        {
            if (nowPhase != HostPhase.SelectNum) return;
            if (string.IsNullOrEmpty(sendNumInputField.text)) return;

            var submitNumber = Int32.Parse(sendNumInputField.text);

            changeHostBingoSubject.OnNext(submitNumber);
        }
        
    }
}

