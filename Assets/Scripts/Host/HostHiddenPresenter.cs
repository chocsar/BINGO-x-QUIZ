using System.Collections.Generic;
using UnityEngine;
using UniRx;
using System;

public class HostHiddenPresenter : MonoBehaviour
{
    [SerializeField] private HostHidddenModel hostHidddenModel;
    [SerializeField] private HostHiddenView hostHiddenView;

    // View側のイベント通知
    public IObservable<string> SubmitCommandEvent => hostHiddenView.SubmitCommandEvent;

    // Model側のイベント通知
    public IObservable<string> SubmitCommandResultEvent => hostHidddenModel.SubmitCommandResultEvent;
    public IObservable<Color> SubmitCommandResultColorEvent => hostHidddenModel.SubmitCommandResultColorEvent;

    private void Start()
    {
        hostHiddenView.SubmitCommandEvent.Subscribe(hostHidddenModel.Command).AddTo(gameObject);

        hostHidddenModel.SubmitCommandResultEvent.Subscribe(hostHiddenView.ReceiveResult).AddTo(gameObject);
        hostHidddenModel.SubmitCommandResultColorEvent.Subscribe(hostHiddenView.RecieveColor).AddTo(gameObject);

        hostHiddenView.InitView();
        hostHidddenModel.InitModel();

    }
}
