using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using UniRx;


public class HostHiddenView : MonoBehaviour
{
    [SerializeField] private InputField commandInputField;
    [SerializeField] private Button commandSubmitButton;

    [SerializeField] private Text commandResultText;

    public IObservable<string> SubmitCommandEvent => commandSubject;

    private Subject<string> commandSubject = new Subject<string>();

    public void InitView()
    {
        commandSubmitButton.onClick.AddListener(() => SubmitCommand());
    }

    private void SubmitCommand()
    {
        var commandString = commandInputField.text;
        commandSubject.OnNext(commandString);
    }

    public void ReceiveResult(string _resultData)
    {
        commandResultText.text = _resultData;
    }

    public void RecieveColor(Color _color)
    {
        commandResultText.color = _color;
    }
}
