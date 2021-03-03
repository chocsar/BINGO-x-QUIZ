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
    public IObservable<string> BingoEvent => bingoModel.BingoEvent;
    public IObservable<string> ReachEvent => bingoModel.ReachEvent;

    [SerializeField] private BingoModel bingoModel;
    [SerializeField] private BingoView bingoView;
    [SerializeField] private QuestionWindowPresenter questionWindowPresenter;
    [SerializeField] private LoadingWindowView loadingWindowView;
    [SerializeField] private BingoAnimationWindowView bingoAnimationView;

    private bool canUpdateCell = true;
    public bool isPlaying = false;

    //デバッグ用
    // private void Start()
    // {
    //     InitBingoPresenter();
    // }
    // private void Update()
    // {
    //     if (Input.GetKeyDown(KeyCode.Space))
    //     {
    //         OpenQuestionWindow(1);
    //     }
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
        questionWindowPresenter.SetAnswerEvent.Subscribe(SetAnswerResult);

        //ModelとViewの初期化処理
        bingoView.InitBingoView();
        bingoModel.InitBingoModel();
        questionWindowPresenter.InitQuestionWindowPresenter();
    }

    public void InitBingoPresenter(BingoCellModel[] bingoCellModels)
    {
        //Modelのイベントを監視
        bingoModel.ChangeUserBingoPhaseEvent.Subscribe(bingoView.OnChangeBingoPhase).AddTo(gameObject);
        bingoModel.ChangeUserBingoStatusEvent.Subscribe(OnChangeBingoStatus).AddTo(gameObject);
        bingoModel.ChangeUserNameEvent.Subscribe(bingoView.SetUserName).AddTo(gameObject);
        bingoModel.ChangeCellModelEvent.Subscribe(UpdateCellView).AddTo(gameObject);

        //Viewのイベントを監視
        bingoView.OpenCellEvent.Subscribe(OpenCell);
        questionWindowPresenter.SetAnswerEvent.Subscribe(SetAnswerResult);

        //ModelとViewの初期化処理
        bingoView.InitBingoView();
        bingoModel.InitBingoModel(bingoCellModels);
        questionWindowPresenter.InitQuestionWindowPresenter();
    }

    public void OnGivenNumber(int number)
    {
        //名前入力ウィンドウがある場合は弾く
        if (!isPlaying) return;

        //ビンゴしてる場合は何も処理しない
        if (bingoModel.GetUserBingoStatus() == UserBingoStatus.Bingo) return;
        //数字を持ってない場合は何も処理しない
        if (!bingoModel.HasNumber(number)) return;

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
                //何かの理由で正規のフェーズ遷移が行われなかった場合の対策
                if (bingoModel.GetUserBingoPhase() == UserBingoPhase.BeforeAnswer ||
                    bingoModel.GetUserBingoPhase() == UserBingoPhase.Answer ||
                    bingoModel.GetUserBingoPhase() == UserBingoPhase.AfterAnswer)
                {
                    CloseLoadingWindow();
                    bingoModel.SetUserBingoPhase(UserBingoPhase.Ready);
                }
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
                questionWindowPresenter.ShowAnswerResult();

                //答えが出たら画面を反映
                canUpdateCell = true;
                UpdateCellView(bingoModel.GetBingoCell(bingoModel.GetCurrentNumIndex()));
                OnChangeBingoStatus(bingoModel.GetUserBingoStatus());

                //正解ならOpen，不正解ならReadyフェーズへ遷移
                if (bingoModel.GetBingoCell(bingoModel.GetCurrentNumIndex()).GetStatus() == BingoCellStatus.CanOpen)
                {
                    bingoModel.SetUserBingoPhase(UserBingoPhase.Open);
                }
                else
                {
                    bingoModel.SetUserBingoPhase(UserBingoPhase.Ready);
                }

                //一定時間でOpenからReadyに遷移する
                Invoke("OpenCellByTime", 10);
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

    private void OpenQuestionWindow(int questionNum)
    {
        StartCoroutine(questionWindowPresenter.OpenQuestionWindow(questionNum));
    }

    private void OpenCell(int index)
    {
        //対象セルをOpenにする
        bingoModel.SetCellStatus(index, BingoCellStatus.Open);
        //Readyフェーズへ遷移
        bingoModel.SetUserBingoPhase(UserBingoPhase.Ready);
    }

    private void OpenCellByTime()
    {
        if (bingoModel.GetUserBingoPhase() == UserBingoPhase.Open)
        {
            OpenCell(bingoModel.GetCurrentNumIndex());
        }
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
                bingoAnimationView.OpenWindow();
            }
        }
    }

    private void SetAnswerResult(bool isRight)
    {
        //問題の結果を保存
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

    public void ReportBingoUser(string userName, string userStatus)
    {
        //Debug.Log(userName + " is " + userStatus);
        bingoView.StackBingoUser(userName, userStatus);
    }

}
