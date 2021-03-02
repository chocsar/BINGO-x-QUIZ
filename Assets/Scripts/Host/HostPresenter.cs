using System.Collections.Generic;
using UnityEngine;
using UniRx;
using System;
using System.Collections;

namespace Host
{
    public class HostPresenter : MonoBehaviour
    {
        [SerializeField] HostModel hostModel;
        [SerializeField] HostView hostView;

        // Model側のイベント通知
        public IObservable<string> SubmitHostPhaseEvent => hostModel.SubmitHostPhaseEvent;
        public IObservable<int> SubmitHostNumberEvent => hostModel.SubmitHostNumberEvent;
        public IObservable<Dictionary<string, int>> LoadHostPhaseEvent => hostModel.ClientPhaseEvent;
        public IObservable<List<ClientStatus>> LoadHostStatusEvent => hostModel.ClientStatusEvent;
        public IObservable<string> SubmitAlertTextEvent => hostModel.AlertEvent;

        // View側のイベント通知
        public IObservable<Unit> ChangeHostPhaseEvent => hostView.ChangeHostPhaseEvent;
        public IObservable<int> ChangeHostBingoNumEvent => hostView.ChangeHostBingoNumEvent;
        public IObservable<Unit> LoadClientPhaseEvent => hostView.LoadClientPhaseEvent;
        public IObservable<Unit> LoadClientStatusEvent => hostView.LoadClientStatusEvent;

        // Start is called before the first frame update
        void Start()
        {
            hostView.ChangeHostPhaseEvent.Subscribe(hostModel.OnChangeHostPhase).AddTo(gameObject);
            hostView.ChangeHostBingoNumEvent.Subscribe(hostModel.OnChangeHostBingoNum).AddTo(gameObject);
            hostView.LoadClientPhaseEvent.Subscribe(hostModel.LoadClientPhase).AddTo(gameObject);
            hostView.LoadClientStatusEvent.Subscribe(hostModel.LoadClientStatus).AddTo(gameObject);
            hostModel.SubmitHostPhaseEvent.Subscribe(hostView.OnChangeHostPhase).AddTo(gameObject);
            hostModel.SubmitHostNumberEvent.Subscribe(hostView.RecieveHostNum).AddTo(gameObject);
            hostModel.ClientPhaseEvent.Subscribe(hostView.OnLoadClientPhase).AddTo(gameObject);
            hostModel.ClientStatusEvent.Subscribe(hostView.OnLoadClientStatus).AddTo(gameObject);
            hostModel.AlertEvent.Subscribe(hostView.AlertDisplay).AddTo(gameObject);

            hostModel.InitModel();
            hostView.InitView();

            StartCoroutine(CycleClientLoad());
        }

        private IEnumerator CycleClientLoad()
        {
            yield return new WaitForSeconds(3f);
            hostModel.LoadClientPhase(Unit.Default);
            StartCoroutine(CycleClientLoad());
        }
    }
}
