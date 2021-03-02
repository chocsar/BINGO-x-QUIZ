using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using System;
using UnityEngine.UI;

public class BingoView : MonoBehaviour
{
    public IObservable<int> OpenCellEvent => openCellSubject;

    private Subject<int> openCellSubject = new Subject<int>();

    //ユーザーの画面
    [SerializeField] private Text userNameText;
    [SerializeField] private BingoCellView[] bingoCellViews;
    [SerializeField] private ReachCellView reachCellView;
    [SerializeField] private AudioSource tapCellAudioSource;

    private int currentQuestionNumber;

    public void InitBingoView()
    {
        for (int index = 0; index < bingoCellViews.Length; index++)
        {
            bingoCellViews[index].InitCellView();

            bingoCellViews[index].SetIndex(index);

            //各BingoCellViewのイベントを監視
            bingoCellViews[index].OpenCellEvent.Subscribe(OpenCell);
        }

        //サウンド
        tapCellAudioSource.clip = Resources.Load<AudioClip>(ResourcesPath.TapCellAudioClip);
    }

    public void SetUserName(string name)
    {
        userNameText.text = name;
    }

    /// <summary>
    /// セルの状態を画面に反映する
    /// </summary>
    /// <param name="bingoCellModel">セルのデータ</param>
    public void SetBingoCellStatus(BingoCellModel bingoCellModel)
    {
        int index = bingoCellModel.GetIndex();
        int number = bingoCellModel.GetNumber();
        string status = bingoCellModel.GetStatus();

        bingoCellViews[index].SetCellImage(number, status);
    }

    private void OpenCell(int index)
    {
        //サウンド再生
        tapCellAudioSource.Play();
        openCellSubject.OnNext(index);
    }

    public void OnChangeBingoStatus(string status)
    {

        switch (status)
        {
            case UserBingoStatus.Default:
                reachCellView.SetCellImage(false);
                break;
            case UserBingoStatus.Reach:
                reachCellView.SetCellImage(true);
                break;
            case UserBingoStatus.Bingo:
                break;
        }
    }

    public void OnChangeBingoPhase(string phase)
    {
        switch (phase)
        {
            case UserBingoPhase.Ready:
                break;

            case UserBingoPhase.BeforeAnswer:
                break;

            case UserBingoPhase.Answer:
                break;

            case UserBingoPhase.AfterAnswer:
                break;

            case UserBingoPhase.Open:
                break;
        }
    }

}
