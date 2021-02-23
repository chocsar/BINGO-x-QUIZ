using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using System;

public class BingoPresenter : MonoBehaviour
{
    public IObservable<string> ChangeUserBingoPhaseEvent => bingoModel.ChangeUserBingoPhaseEvent;
    public IObservable<string> ChangeUserBingoStatusEvent => bingoModel.ChangeUserBingoStatusEvent;
    public IObservable<BingoCellModel[]> ChangeCellModelsEvent => bingoModel.ChangeCellModelsEvent;
    public IObservable<string> ChangeUserNameEvent => bingoModel.ChangeUserNameEvent;

    [SerializeField] private BingoModel bingoModel;
    [SerializeField] private BingoView bingoView;


    /// <summary>
    /// BingoPresenterの初期化処理（ここからModelとViewも初期化）
    /// </summary>
    public void InitBingoPresenter()
    {
        //ModelとViewの初期化処理
        bingoModel.InitBingoModel();
        bingoView.InitBingoView();

        //Modelのイベントを監視
        bingoModel.ChangeUserBingoPhaseEvent.Subscribe(bingoView.OnChangeBingoPhase).AddTo(gameObject);
        bingoModel.ChangeUserBingoStatusEvent.Subscribe(bingoView.OnChangeBingoStatus).AddTo(gameObject);
        bingoModel.ChangeUserNameEvent.Subscribe(bingoView.SetUserName);
        //Viewのイベントを監視
        bingoView.OpenCellEvent.Subscribe(_ => bingoModel.SetCurrentCellStatus(BingoCellStatus.Open));
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
                bingoView.SetQuestion(bingoModel.GetCurrentNumber());
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

    //メモ：クイズに答えた後に呼び出す想定
    public void SetUserBingoPhase(string phase)
    {
        bingoModel.SetUserBingoPhase(phase);
    }


}
