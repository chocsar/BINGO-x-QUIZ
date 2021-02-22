using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using System;

public class BingoView : MonoBehaviour
{
    public IObservable<Unit> OpenCellEvent => openCellSubject;

    private Subject<Unit> openCellSubject = new Subject<Unit>();
    [SerializeField] private BingoCellView[] bingoCellViews;
    [SerializeField] private GameObject questionWindow;



    void Start()
    {
        //Startではなく初期化処理にする？
        for (int index = 0; index < bingoCellViews.Length; index++)
        {
            bingoCellViews[index].OpenCellEvent.Subscribe(_ => OpenCell());
        }

    }

    void Update()
    {

    }

    public void OnChangeBingoPhase(string phase)
    {
        switch (phase)
        {
            case UserBingoPhase.Ready:
                break;

            case UserBingoPhase.BeforeAnswer:
                OpenBeforeAnswerWindow();
                break;

            case UserBingoPhase.Answer:
                OpenQuestionWindow();
                break;

            case UserBingoPhase.AfterAnswer:
                OpenAfterAnswerWindow();
                break;

            case UserBingoPhase.Open:
                OpenAnswerWindow();

                break;
        }
    }

    public void OnChangeBingoStatus(string status)
    {

    }

    public void SetQuestion(int number)
    {

    }

    public void SetBingoCellStates(BingoCellModel[] bingoCellModels)
    {
        for (int index = 0; index < bingoCellModels.Length; index++)
        {
            string status = bingoCellModels[index].GetStatus();

            switch (status)
            {
                case BingoCellStatus.BeforeOpen:
                    bingoCellViews[index].MakeBeforeOpenCell();
                    break;
                case BingoCellStatus.Open:
                    bingoCellViews[index].OpenCell();
                    break;
                case BingoCellStatus.Dead:
                    bingoCellViews[index].KillCell();
                    break;

            }
        }
    }

    private void OpenBeforeAnswerWindow()
    {
        //TODO
    }

    private void OpenQuestionWindow()
    {
        //TODO
    }

    private void OpenAfterAnswerWindow()
    {
        //TODO
    }

    private void OpenAnswerWindow()
    {
        //TODO
    }

    private void OpenCell()
    {
        openCellSubject.OnNext(Unit.Default);
    }
}
