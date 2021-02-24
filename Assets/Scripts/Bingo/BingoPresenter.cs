using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using System;

public class BingoPresenter : MonoBehaviour
{
    public IObservable<string> ChangeUserNameEvent => bingoModel.ChangeUserNameEvent;
    public IObservable<string> ChangeUserBingoStatusEvent => bingoModel.ChangeUserBingoStatusEvent;
    public IObservable<string> ChangeUserBingoPhaseEvent => bingoModel.ChangeUserBingoPhaseEvent;
    public IObservable<BingoCellModel> ChangeCellModelEvent => bingoModel.ChangeCellModelEvent;

    [SerializeField] private BingoModel bingoModel;
    [SerializeField] private BingoView bingoView;
    [SerializeField] private QuestionWindowView questionWindowView;
    [SerializeField] private LoadingWindowView loadingWindowView;
    [SerializeField] private BingoAnimationView bingoAnimationView;

    private bool canUpdateCell = true;

    //デバッグ用
    // private void Start()
    // {
    //     InitBingoPresenter();
    // }

    /// <summary>
    /// BingoPresenterの初期化処理（ここからModelとViewも初期化）
    /// </summary>
    public void InitBingoPresenter()
    {
        //Modelのイベントを監視
        bingoModel.ChangeUserBingoPhaseEvent.Subscribe(bingoView.OnChangeBingoPhase).AddTo(gameObject);
        bingoModel.ChangeUserBingoStatusEvent.Subscribe(OnChangeBingoStatus).AddTo(gameObject);
        bingoModel.ChangeUserNameEvent.Subscribe(bingoView.SetUserName).AddTo(gameObject);
        bingoModel.ChangeCellModelEvent.Subscribe(UpdateCellView).AddTo(gameObject);

        //Viewのイベントを監視
        bingoView.OpenCellEvent.Subscribe(OpenCell);
        questionWindowView.SetAnswerEvent.Subscribe(SetAnswerResult);

        //ModelとViewの初期化処理
        bingoView.InitBingoView();
        bingoModel.InitBingoModel();
    }

    public void OnGivenNumber(int number)
    {
        //数字を持ってない場合は何も処理しない
        if (!bingoModel.HasNumber(number)) return;
        //ビンゴしてる場合は何も処理しない
        if (bingoModel.GetUserBingoStatus() == UserBingoStatus.Bingo) return;

        //ウィンドウへの処理
        OpenLoadingWindow();

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

                //問題を答えても画面に反映しない処理
                canUpdateCell = false;

                //ウィンドウへの処理
                CloseLoadingWindow();
                OpenQuestionWindow(bingoModel.GetCurrentNumber());

                //Answerフェーズへ遷移
                bingoModel.SetUserBingoPhase(UserBingoPhase.Answer);
                break;

            case HostPhase.PresentAnswer:
                //AfterAnswerフェーズでなければ何もしない
                if (bingoModel.GetUserBingoPhase() != UserBingoPhase.AfterAnswer) return;

                //ウィンドウへの処理
                CloseLoadingWindow();
                questionWindowView.ShowAnswerResult();

                //答えが出たら画面を反映
                canUpdateCell = true;
                UpdateCellView(bingoModel.GetBingoCell(bingoModel.GetCurrentNumIndex()));
                OnChangeBingoStatus(bingoModel.GetUserBingoStatus());

                //Openフェーズへ遷移
                bingoModel.SetUserBingoPhase(UserBingoPhase.Open);
                break;
        }
    }

    private void OpenLoadingWindow()
    {
        loadingWindowView.OpenWindow();
    }
    private void CloseLoadingWindow()
    {
        loadingWindowView.CloseWindow();
    }

    private void OpenQuestionWindow(int questionNumber)
    {
        questionWindowView.SetQuestionNumber(questionNumber);
        questionWindowView.OpenWindow();
    }

    private void OpenCell(int index)
    {
        //対象セルをOpenにする
        bingoModel.SetCellStatus(index, BingoCellStatus.Open);
        //Readyフェーズへ遷移
        bingoModel.SetUserBingoPhase(UserBingoPhase.Ready);
    }

    private void UpdateCellView(BingoCellModel bingoCellModel)
    {
        if (canUpdateCell)
        {
            bingoView.SetBingoCellStatus(bingoCellModel);
        }
    }

    private void OnChangeBingoStatus(string status)
    {
        if (canUpdateCell)
        {
            //viewの反映
            bingoView.OnChangeBingoStatus(status);

            //ビンゴ時の処理
            if (status == UserBingoStatus.Bingo)
            {
                //TODO
                bingoAnimationView.OpenWindow();
            }
        }
    }

    private void SetAnswerResult(bool isRight)
    {
        //問題の結果を保持
        int index = bingoModel.GetCurrentNumIndex();
        if (isRight)
        {
            bingoModel.SetCellStatus(index, BingoCellStatus.CanOpen);
        }
        else
        {
            bingoModel.SetCellStatus(index, BingoCellStatus.Dead);
        }

        //ウィンドウへの処理
        OpenLoadingWindow();

        //AfterAnswerフェーズへ遷移
        bingoModel.SetUserBingoPhase(UserBingoPhase.AfterAnswer);
    }


}
