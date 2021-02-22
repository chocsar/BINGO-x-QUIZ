using UnityEngine;
using UniRx;
using System;

namespace Host
{
    public class HostPresenter : MonoBehaviour
    {
        [SerializeField] HostModel hostModel;
        [SerializeField] HostView hostView;

        public IObservable<string> SubmitHostPhaseEvent => hostModel.SubmitHostPhaseEvent;
        public IObservable<int> SubmitHostNumberEvent => hostModel.SubmitHostNumberEvent;

        public IObservable<Unit> ChangeHostPhaseEvent => hostView.ChangeHostPhaseEvent;

        // Start is called before the first frame update
        void Start()
        {
            hostView.ChangeHostPhaseEvent.Subscribe(hostModel.OnChangeHostPhase).AddTo(gameObject);
            hostModel.SubmitHostPhaseEvent.Subscribe(hostView.OnChangeHostPhase).AddTo(gameObject);
            hostModel.SubmitHostNumberEvent.Subscribe(hostView.RecieveHostNum).AddTo(gameObject);

            hostModel.InitModel();
            hostView.InitView();
        }
    }
}
