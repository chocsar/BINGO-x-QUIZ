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
        [SerializeField] private Text alertText;

        public IObservable<Unit> ChangeHostPhaseEvent => changeHostPhaseSubject;
        public IObservable<int> ChangeHostBingoNumEvent => changeHostBingoSubject;
        public IObservable<Unit> LoadClientPhaseEvent => LoadPhaseClientSubject;

        private Subject<Unit> changeHostPhaseSubject = new Subject<Unit>();
        private Subject<int> changeHostBingoSubject = new Subject<int>();
        private Subject<Unit> LoadPhaseClientSubject = new Subject<Unit>();

        private string nowPhase;
        private bool canGenerateNumber;

        public void InitView()
        {
            nextPhaseButton.onClick.AddListener(() => NextPhase());
            generateNumButton.onClick.AddListener(() => SubmitNumber());
            loadPhaseData.onClick.AddListener(() => LoadAllClientPhase());
            alertText.color = Color.red;
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
            if (nowPhase != HostPhase.SelectNum)
            {
                alertText.text = "SelectNumPhaseではありません。";
                return;
            }
            if (string.IsNullOrEmpty(sendNumInputField.text)) {
                alertText.text = "数字が入力されていません。";
                return;
            }
            if (!canGenerateNumber)
            {
                alertText.text = "全員がready状態ではありません。";
                return;
            }

            var submitNumber = Int32.Parse(sendNumInputField.text);

            changeHostBingoSubject.OnNext(submitNumber);
        }

        // Clientのフェーズデータ読み取るイベント通知
        public void LoadAllClientPhase()
        {
            LoadPhaseClientSubject.OnNext(Unit.Default);
        }

        public void OnLoadClientPhase(Dictionary<string, int> _recieveDatas)
        {
            string displayText = "";
            if (nowPhase == HostPhase.SelectNum && _recieveDatas[UserBingoPhase.BeforeAnswer] == 0 && _recieveDatas[UserBingoPhase.Answer] == 0 && _recieveDatas[UserBingoPhase.AfterAnswer] == 0 && _recieveDatas[UserBingoPhase.Open] == 0) canGenerateNumber = true;
            else canGenerateNumber = false;

            foreach (var item in _recieveDatas)
            {
                displayText += $"{item.Key} : {item.Value}\n";
            }

            
            clientPhaseDataText.text = displayText;
        }
        
    }
}

