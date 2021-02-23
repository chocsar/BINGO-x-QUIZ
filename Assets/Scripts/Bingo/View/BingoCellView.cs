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

    [SerializeField] private Text cellText;//Spriteに変更する
    [SerializeField] private Button cellButton;

    private bool canOpen = false;

    public void InitCellView()
    {
        //セルへのボタン入力を監視
        cellButton.OnClickAsObservable().Subscribe(_ => OnClickCell()).AddTo(gameObject);
    }

    public void SetCellText(int number) //TODO:SetCellImageに変更する
    {
        cellText.text = number.ToString();
    }

    //メモ：セルを見た目を変更する処理 → SetCellImage(string status)に組み込めそう
    public void OpenCell()
    {

    }
    public void KillCell()
    {

    }
    public void MakeBeforeOpenCell() //命名が微妙
    {

    }

    private void OnClickCell()
    {
        if (canOpen)
        {
            OpenCell();
            openCellSubject.OnNext(Unit.Default);
        }
    }
}
