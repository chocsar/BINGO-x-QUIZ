using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using System;

public class BingoCellView : MonoBehaviour
{
    public IObservable<int> OpenCellEvent => openCellSubject;
    private Subject<int> openCellSubject = new Subject<int>();

    [SerializeField] private Button cellButton;
    [SerializeField] private Image cellImage;

    private int index;
    private bool canOpen = false;


    public void InitCellView()
    {
        //セルへのボタン入力を監視
        cellButton.OnClickAsObservable().Subscribe(_ => OnClickCell()).AddTo(gameObject);
    }

    public void SetCellImage(int number, string status)
    {
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

    public void SetIndex(int index)
    {
        this.index = index;
    }

    private void OnClickCell()
    {
        if (canOpen)
        {
            canOpen = false;
            openCellSubject.OnNext(index);
        }
    }
}
