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
        //セルを画面反映
        UpdateAllViewCells();

        //Modelのイベントを監視
        bingoModel.ChangeUserBingoPhaseEvent.Subscribe(bingoView.OnChangeBingoPhase).AddTo(gameObject);
        bingoModel.ChangeUserBingoStatusEvent.Subscribe(bingoView.OnChangeBingoStatus).AddTo(gameObject);
        bingoModel.ChangeUserNameEvent.Subscribe(bingoView.SetUserName);
        //Viewのイベントを監視
        bingoView.OpenCellEvent.Subscribe(OpenCell);
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
                bingoView.SetQuestionNumber(bingoModel.GetCurrentNumber());//問題番号を渡す
                bingoModel.SetUserBingoPhase(UserBingoPhase.Answer);
                break;

            case HostPhase.PresentAnswer:
                //AfterAnswerフェーズでなければ何もしない
                if (bingoModel.GetUserBingoPhase() != UserBingoPhase.AfterAnswer) return;
                //Openフェーズへ遷移
                UpdateAllViewCells();//TODO:画面反映のタイミング制御を修正, 対象箇所だけにする
                bingoModel.SetUserBingoPhase(UserBingoPhase.Open);
                break;
        }
    }

    //メモ：クイズに答えた後に呼び出す想定
    public void SetUserBingoPhase(string phase)
    {
        bingoModel.SetUserBingoPhase(phase);
    }

    //Modelの全Cellの状態を画面に反映
    private void UpdateAllViewCells()
    {
        bingoView.SetAllBingoCellStatus(bingoModel.GetAllBingoCells());
    }

    //Modelの対象Cellの状態を画面に反映
    private void UpdateViewCell(int index)
    {
        bingoView.SetBingoCellStatus(index, bingoModel.GetBingoCell(index));
    }

    private void OpenCell(int index)
    {
        //対象セルをOpenにする
        bingoModel.SetCellStatus(index, BingoCellStatus.Open);
        //画面反映
        UpdateViewCell(index); //メモ：変更箇所だけ反映にする
        //Readyフェーズへ遷移
        bingoModel.SetUserBingoPhase(UserBingoPhase.Ready);
    }


}
