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
        [SerializeField] private Button checkStatusButton;
        [SerializeField] private Text clientStatusText;

        public IObservable<Unit> ChangeHostPhaseEvent => changeHostPhaseSubject;
        public IObservable<int> ChangeHostBingoNumEvent => changeHostBingoSubject;
        public IObservable<Unit> LoadClientPhaseEvent => LoadPhaseClientSubject;
        public IObservable<Unit> LoadClientStatusEvent => LoadStatusClientSubject;
        public IObservable<string> AlertEvent => alertSubject;

        private Subject<Unit> changeHostPhaseSubject = new Subject<Unit>();
        private Subject<int> changeHostBingoSubject = new Subject<int>();
        private Subject<Unit> LoadPhaseClientSubject = new Subject<Unit>();
        private Subject<Unit> LoadStatusClientSubject = new Subject<Unit>();
        private Subject<string> alertSubject = new Subject<string>();

        private string nowHostPhase;
        private bool canGenerateNumber;

        public void InitView()
        {
            nextPhaseButton.onClick.AddListener(() => NextPhase());
            generateNumButton.onClick.AddListener(() => SubmitNumber());
            loadPhaseData.onClick.AddListener(() => LoadAllClientPhase());
            checkStatusButton.onClick.AddListener(() => LoadAllClientStatus());
            alertText.color = Color.red;
        }

        // HostのPhaseが変わった時の見た目の処理
        public void OnChangeHostPhase(string _phase)
        {
            nowHostPhase = _phase;
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
            AlertReset();
            changeHostPhaseSubject.OnNext(Unit.Default);
        }

        // 数字生成のイベント通知
        private void SubmitNumber()
        {
            if (nowHostPhase != HostPhase.SelectNum)
            {
                alertText.text = "SelectNumPhaseではありません。";
                return;
            }
            if (string.IsNullOrEmpty(sendNumInputField.text)) {
                alertText.text = "数字が入力されていません。";
                return;
            }

            AlertReset();
            var submitNumber = Int32.Parse(sendNumInputField.text);

            changeHostBingoSubject.OnNext(submitNumber);
        }

        public void AlertDisplay(string _alert)
        {
            alertText.text = _alert; 
        }

        private void AlertReset()
        {
            alertText.text = "";
        }

        // Clientのフェーズデータ読み取るイベント通知
        public void LoadAllClientPhase()
        {
            AlertReset();
            LoadPhaseClientSubject.OnNext(Unit.Default);
        }

        public void OnLoadClientPhase(Dictionary<string, int> _recieveDatas)
        {
            string displayText = "";
            if (nowHostPhase == HostPhase.SelectNum && _recieveDatas[UserBingoPhase.BeforeAnswer] == 0 && _recieveDatas[UserBingoPhase.Answer] == 0 && _recieveDatas[UserBingoPhase.AfterAnswer] == 0 && _recieveDatas[UserBingoPhase.Open] == 0) canGenerateNumber = true;
            else canGenerateNumber = false;

            foreach (var item in _recieveDatas)
            {
                displayText += $"{item.Key} : {item.Value}\n";
            }

            
            clientPhaseDataText.text = displayText;
        }

        // Clientのフェーズデータ読み取るイベント通知
        public void LoadAllClientStatus()
        {
            AlertReset();
            LoadStatusClientSubject.OnNext(Unit.Default);
        }

        public void OnLoadClientStatus(List<ClientStatus> _status)
        {
            var reachMember = "reach : ";
            var bingoMember = "bingo : ";
            foreach (var item in _status)
            {
                switch (item.status)
                {
                    case UserBingoStatus.Reach:
                        reachMember += item.username + ",";
                        break;
                    case UserBingoStatus.Bingo:
                        bingoMember += item.username + ",";
                        break;
                    default:
                        break;
                }
            }

            clientStatusText.text = $"{reachMember}\n{bingoMember}";
        }

    }
}

