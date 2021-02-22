using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BingoView : MonoBehaviour
{
    [SerializeField] private BingoCellView[] bingoCellViews;
    [SerializeField] private GameObject questionWindow;

    void Start()
    {

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

    public void SetQuestion(int number)
    {

    }

    public void SetBingoCellStates(BingoCellModel[] bingoCellModels)
    {
        for (int index = 0; index < bingoCellModels.Length; index++)
        {
            string status = bingoCellModels[index].GetCellStatus();

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
}
