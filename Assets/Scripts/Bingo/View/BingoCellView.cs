using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using System;

public class BingoCellView : MonoBehaviour
{
    public IObservable<Unit> OpenCellEvent => openCellSubject;
    private Subject<Unit> openCellSubject = new Subject<Unit>();
    [SerializeField] private Text cellText;
    [SerializeField] private Button cellButton;

    private bool isBeforeOpen = false;

    private void Start()
    {
        cellButton.OnClickAsObservable().Subscribe(_ => OnClickCell()).AddTo(gameObject);
    }

    public void SetCellText(int number)
    {
        cellText.text = number.ToString();
    }

    public void OpenCell()
    {
        isBeforeOpen = false;
    }
    public void MakeBeforeOpenCell()
    {
        isBeforeOpen = true;

    }
    public void KillCell()
    {
        isBeforeOpen = false;
    }

    private void OnClickCell()
    {
        if (isBeforeOpen)
        {
            OpenCell();
            openCellSubject.OnNext(Unit.Default);

        }
    }
}
