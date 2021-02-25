﻿using System.Collections.Generic;
using UnityEngine;
using UniRx;
using System;

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

        // View側のイベント通知
        public IObservable<Unit> ChangeHostPhaseEvent => hostView.ChangeHostPhaseEvent;
        public IObservable<int> ChangeHostBingoNumEvent => hostView.ChangeHostBingoNumEvent;
        public IObservable<Unit> LoadClientPhaseEvent => hostView.LoadClientPhaseEvent;


        // Start is called before the first frame update
        void Start()
        {
            hostView.ChangeHostPhaseEvent.Subscribe(hostModel.OnChangeHostPhase).AddTo(gameObject);
            hostView.ChangeHostBingoNumEvent.Subscribe(hostModel.OnChangeHostBingoNum).AddTo(gameObject);
            hostView.LoadClientPhaseEvent.Subscribe(hostModel.LoadClientPhase).AddTo(gameObject);
            hostModel.SubmitHostPhaseEvent.Subscribe(hostView.OnChangeHostPhase).AddTo(gameObject);
            hostModel.SubmitHostNumberEvent.Subscribe(hostView.RecieveHostNum).AddTo(gameObject);
            hostModel.ClientPhaseEvent.Subscribe(hostView.OnLoadClientPhase).AddTo(gameObject);

            hostModel.InitModel();
            hostView.InitView();
        }
    }
}
