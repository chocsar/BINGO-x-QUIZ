using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using System;

public class BingoPresenter : MonoBehaviour
{
    public IObservable<string> ChangeUserBingoPhaseEvent => bingoModel.ChangeBingoPhaseEvent;

    [SerializeField] private BingoModel bingoModel;
    [SerializeField] private BingoView bingoView;

    private void Start()
    {
        bingoModel.ChangeBingoPhaseEvent.Subscribe(bingoView.OnChangeBingoPhase).AddTo(gameObject);
    }

    public void OnGivenNumber(int number)
    {
        //数字を持ってない場合は何も処理しない
        if (!bingoModel.HasNumber(number)) return;

        //BeforeAnswerフェーズへ遷移
        bingoModel.SetUserBingoPhase(UserBingoPhase.BeforeAnswer);
    }

    public void OnChangeHostPhase(string phase)
    {
        switch (phase)
        {
            case HostPhase.SelectNum:
                break;

            case HostPhase.PresentQuestion:
                //BeforeAnswerフェーズでなければ何もしない
                if (bingoModel.GetUserBingoPhase() != UserBingoPhase.BeforeAnswer) return;
                //Answerフェーズへ遷移
                bingoView.SetQuestion(bingoModel.GetCurrentNumber()); //indexも渡す？
                bingoModel.SetUserBingoPhase(UserBingoPhase.Answer);
                break;

            case HostPhase.PresentAnswer:
                //AfterAnswerフェーズでなければ何もしない
                if (bingoModel.GetUserBingoPhase() != UserBingoPhase.AfterAnswer) return;
                //Openフェーズへ遷移
                bingoView.SetBingoCellStates(bingoModel.GetBingoCells());
                bingoModel.SetUserBingoPhase(UserBingoPhase.Open);
                break;
        }
    }

    public void SetUserBingoPhase(string phase)
    {
        bingoModel.SetUserBingoPhase(phase);
    }


}
