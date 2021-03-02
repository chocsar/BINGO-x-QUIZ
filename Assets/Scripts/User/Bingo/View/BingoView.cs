﻿using System.Collections;
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

    //ビンゴ通知
    [SerializeField] private GameObject reportParent;
    [SerializeField] private BingoReport reportPrefab;
    private List<string> bingoUserList = new List<string>();
    [SerializeField] private float reportInterval = 1;
    private float timer = 0;


    private void Update()
    {
        //デバッグ用
        // if (Input.GetKeyDown(KeyCode.Space))
        // {
        //     StackBingoUser("testuser");
        // }

        //スタックされたビンゴユーザーを一定間隔で生成
        if (bingoUserList.Count != 0)
        {
            timer += Time.deltaTime;

            if (timer >= reportInterval)
            {
                timer = 0;
                ReportBingoUser(bingoUserList[0]);
                bingoUserList.RemoveAt(0);
            }
        }
    }


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
        //tapCellAudioSource.clip = Resources.Load<AudioClip>(ResourcesPath.TapCellAudioClip);
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

    public void StackBingoUser(string userName)
    {
        bingoUserList.Add(userName);
    }

    private void ReportBingoUser(string userName)
    {
        BingoReport report = Instantiate(reportPrefab, new Vector3(-600, -600, 0), Quaternion.identity);
        report.SetText(userName);
        report.transform.SetParent(reportParent.transform);
        report.transform.localScale = new Vector3(1, 1, 1);
    }

}
