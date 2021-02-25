using UnityEngine;
using UnityEngine.UI;
using UniRx;
using System;
using System.Collections.Generic;

namespace Host
{
    public class HostView : MonoBehaviour
    {
        [SerializeField] private Button nextPhaseButton;
        [SerializeField] private Text phaseText;
        [SerializeField] private Button generateNumButton;
        [SerializeField] private Text bingoNumText;
        [SerializeField] private InputField sendNumInputField;
        [SerializeField] private Button loadPhaseData;
        [SerializeField] private Text clientPhaseDataText;

        public IObservable<Unit> ChangeHostPhaseEvent => changeHostPhaseSubject;
        public IObservable<int> ChangeHostBingoNumEvent => changeHostBingoSubject;
        public IObservable<Unit> LoadClientPhaseEvent => LoadPhaseClientSubject;

        private Subject<Unit> changeHostPhaseSubject = new Subject<Unit>();
        private Subject<int> changeHostBingoSubject = new Subject<int>();
        private Subject<Unit> LoadPhaseClientSubject = new Subject<Unit>();

        private string nowPhase;

        public void InitView()
        {
            nextPhaseButton.onClick.AddListener(() => NextPhase());
            generateNumButton.onClick.AddListener(() => SubmitNumber());
            loadPhaseData.onClick.AddListener(() => LoadAllClientPhase());
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

        public void LoadAllClientPhase()
        {
            LoadPhaseClientSubject.OnNext(Unit.Default);
        }

        public void OnLoadClientPhase(Dictionary<string, int> _recieveDatas)
        {
            string displayText = "";
            foreach (var item in _recieveDatas)
            {
                displayText += $"{item.Key} : {item.Value}\n"; 
            }
            clientPhaseDataText.text = displayText;
        }
        
    }
}

