using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using System;

public class BingoCellView : MonoBehaviour
{
    [SerializeField] private Text cellText;
    [SerializeField] private Button cellButton;

    private void Start()
    {
        cellButton.OnClickAsObservable().Subscribe(_ => OpenCell()); //条件付けどうするか
    }

    public void SetCellText(int number)
    {
        cellText.text = number.ToString();
    }

    public void OpenCell()
    {

    }
    public void MakeBeforeOpenCell()
    {

    }
    public void KillCell()
    {

    }
}
