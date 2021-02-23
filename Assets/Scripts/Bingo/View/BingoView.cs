using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using System;
using UnityEngine.UI;

public class BingoView : MonoBehaviour
{
    public IObservable<Unit> OpenCellEvent => openCellSubject;

    private Subject<Unit> openCellSubject = new Subject<Unit>();

    //ユーザーの画面
    [SerializeField] private Text userNameText;
    [SerializeField] private BingoCellView[] bingoCellViews;

    //フェーズごとに出現させるウィンドウ
    [SerializeField] private GameObject questionWindow;

    private int currentQuestionNumber;


    public void InitBingoView()
    {
        //各BingoCellViewのイベントを監視
        for (int index = 0; index < bingoCellViews.Length; index++)
        {
            bingoCellViews[index].InitCellView();
            bingoCellViews[index].OpenCellEvent.Subscribe(_ => OpenCell());
        }
    }

    public void SetUserName(string name)
    {
        userNameText.text = name;
    }

    /// <summary>
    /// セルの状態を画面に反映する
    /// </summary>
    /// <param name="bingoCellModels">セルのデータ</param>
    public void SetBingoCellStatus(BingoCellModel[] bingoCellModels)
    {
        for (int index = 0; index < bingoCellModels.Length; index++)
        {
            int number = bingoCellModels[index].GetNumber();
            string status = bingoCellModels[index].GetStatus();

            bingoCellViews[index].SetCellImage(number, status);
        }
    }
    public void OnChangeBingoStatus(string status)
    {
        //TODO:リーチやビンゴの画面表示
    }

    private void OpenCell()
    {
        /*
        メモ：
        正解時にセルを押した時の処理だが不要かも
        セルを押せる状態もセルが開いてる状態もModelやFirebaseでは同じ扱いでいいのでは？
        モデルからの画面反映のタイミングを制御する
        */

        openCellSubject.OnNext(Unit.Default);
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

    private void OpenBeforeAnswerWindow()
    {
        //TODO:問題提示までホストを待ってることを示すウィンドウ（なしでもOK）
    }

    public void SetQuestionNumber(int number)
    {
        this.currentQuestionNumber = number;
    }

    private void OpenQuestionWindow()
    {
        //TODO:問題のウィンドウを表示する
        //TODO:この処理をPresenterに移行する
    }

    private void OpenAfterAnswerWindow()
    {
        //TODO:答え提示までホストを待ってることを示すウィンドウ（なしでもOK）
    }

    private void OpenAnswerWindow()
    {
        //TODO:正解不正解のウィンドウを表示する
    }


}
