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

    [SerializeField] private Button cellButton;
    [SerializeField] private Image cellImage;

    private int number;
    private bool canOpen = false;


    public void InitCellView()
    {
        //セルへのボタン入力を監視
        cellButton.OnClickAsObservable().Subscribe(_ => OnClickCell()).AddTo(gameObject);
    }

    public void SetCellImage(int number, string status)
    {
        this.number = number;

        switch (status)
        {
            case BingoCellStatus.Default:
                cellImage.sprite = Resources.Load<Sprite>(ResourcesPath.NormalCell + number.ToString());
                break;
            case BingoCellStatus.Hit:
                cellImage.sprite = Resources.Load<Sprite>(ResourcesPath.HitCell + number.ToString());
                break;
            case BingoCellStatus.Open:
                cellImage.sprite = Resources.Load<Sprite>(ResourcesPath.OpenCell + number.ToString());
                break;
            case BingoCellStatus.Dead:
                cellImage.sprite = Resources.Load<Sprite>(ResourcesPath.DeadCell + number.ToString());
                break;
        }
    }

    private void OnClickCell()
    {
        if (canOpen)
        {
            canOpen = false;
            SetCellImage(number, BingoCellStatus.Open);
            //openCellSubject.OnNext(Unit.Default);
        }
    }
}
