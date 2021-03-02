using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using System;


public class BingoModel : MonoBehaviour
{
    [SerializeField] private bool isCenterOpen = true; //初期状態で真ん中を上げるかどうか

    public IObservable<string> ChangeUserNameEvent => userNameSubject;
    public IObservable<string> ChangeUserBingoStatusEvent => userBingoStatusSubject;
    public IObservable<string> ChangeUserBingoPhaseEvent => userBingoPhaseSubject;
    public IObservable<BingoCellModel> ChangeCellModelEvent => bingoCellModelSubject;

    private Subject<string> userNameSubject = new Subject<string>();
    private Subject<string> userBingoStatusSubject = new Subject<string>();
    private Subject<string> userBingoPhaseSubject = new Subject<string>();
    private Subject<BingoCellModel> bingoCellModelSubject = new Subject<BingoCellModel>();

    //ユーザーのデータ
    private string userName;
    private string userBingoStatus;
    private string userBingoPhase;
    private BingoCellModel[] bingoCellModels = new BingoCellModel[9];

    //ホストから提示された数字を保持
    private int currentNumber;
    private int currentNumIndex; //現状は不要

    public void InitBingoModel()
    {
        //ユーザーデータの初期化
        SetUserName(PlayerPrefs.GetString(PlayerPrefsKeys.UserName));
        SetUserBingoStatus(UserBingoStatus.Default);
        SetUserBingoPhase(UserBingoPhase.Ready);
        SetRadomBingoNumbers();
    }

    public void InitBingoModel(BingoCellModel[] bingoCellModels)
    {
        SetUserName(PlayerPrefs.GetString(PlayerPrefsKeys.UserName));
        SetUserBingoPhase(UserBingoPhase.Ready);

        this.bingoCellModels = bingoCellModels;

        //イベント通知
        for (int index = 0; index < bingoCellModels.Length; index++)
        {
            bingoCellModelSubject.OnNext(bingoCellModels[index]);
        }

        //Userのビンゴ状態を判定
        DetermineBingoStatus();

    }

    /// <summary>
    /// ホストから提示された数字を持っているかどうか
    /// </summary>
    /// <param name="number">ホストが提示した数字</param>
    /// <returns></returns>
    public bool HasNumber(int number)
    {
        bool hasNumber = false;

        for (int index = 0; index < bingoCellModels.Length; index++)
        {
            //もしHit状態だった場合はDefaultに戻す
            if (bingoCellModels[index].GetStatus() == BingoCellStatus.Hit)
            {
                SetCellStatus(index, BingoCellStatus.Default);
            }
            //数字を持っていた場合
            if (bingoCellModels[index].GetNumber() == number)
            {
                //OpenかDead状態なら処理しない
                if (bingoCellModels[index].GetStatus() == BingoCellStatus.Open ||
                    bingoCellModels[index].GetStatus() == BingoCellStatus.Dead)
                {
                    break;
                }

                //数字を保持しておく
                SetCurrentNumber(number);
                SetCurrentNumIndex(index);
                //Hit状態にする
                SetCellStatus(index, BingoCellStatus.Hit);

                hasNumber = true;
            }
        }

        return hasNumber;
    }

    /// <summary>
    /// ユーザーのビンゴ状態（Default, Reach, Bingo）を判定するメソッド
    /// </summary>
    public void DetermineBingoStatus()
    {
        string[,] bingoLine = new string[8, 3];

        bingoLine[0, 0] = bingoCellModels[0].GetStatus();
        bingoLine[0, 1] = bingoCellModels[1].GetStatus();
        bingoLine[0, 2] = bingoCellModels[2].GetStatus();
        bingoLine[1, 0] = bingoCellModels[3].GetStatus();
        bingoLine[1, 1] = bingoCellModels[4].GetStatus();
        bingoLine[1, 2] = bingoCellModels[5].GetStatus();
        bingoLine[2, 0] = bingoCellModels[6].GetStatus();
        bingoLine[2, 1] = bingoCellModels[7].GetStatus();
        bingoLine[2, 2] = bingoCellModels[8].GetStatus();

        bingoLine[3, 0] = bingoCellModels[0].GetStatus();
        bingoLine[4, 0] = bingoCellModels[1].GetStatus();
        bingoLine[5, 0] = bingoCellModels[2].GetStatus();
        bingoLine[3, 1] = bingoCellModels[3].GetStatus();
        bingoLine[4, 1] = bingoCellModels[4].GetStatus();
        bingoLine[5, 1] = bingoCellModels[5].GetStatus();
        bingoLine[3, 2] = bingoCellModels[6].GetStatus();
        bingoLine[4, 2] = bingoCellModels[7].GetStatus();
        bingoLine[5, 2] = bingoCellModels[8].GetStatus();

        bingoLine[6, 0] = bingoCellModels[0].GetStatus();
        bingoLine[6, 1] = bingoCellModels[4].GetStatus();
        bingoLine[6, 2] = bingoCellModels[8].GetStatus();
        bingoLine[7, 0] = bingoCellModels[2].GetStatus();
        bingoLine[7, 1] = bingoCellModels[4].GetStatus();
        bingoLine[7, 2] = bingoCellModels[6].GetStatus();

        int openCountMax = 0;
        int openCountMaxContainingCanOpenCell = 0;

        for (int i = 0; i < 8; i++)
        {
            int openCount = 0;
            int openCountContainingCanOpenCell = 0;

            //Openセルのカウント
            for (int j = 0; j < 3; j++)
            {
                if (bingoLine[i, j] == BingoCellStatus.Open)
                {
                    openCount++;
                    openCountContainingCanOpenCell++;
                }
                if (bingoLine[i, j] == BingoCellStatus.CanOpen)
                {
                    openCountContainingCanOpenCell++;
                }
            }

            //カウントの最大値を更新
            if (openCountMax < openCount)
            {
                openCountMax = openCount;
            }
            if (openCountMaxContainingCanOpenCell < openCountContainingCanOpenCell)
            {
                openCountMaxContainingCanOpenCell = openCountContainingCanOpenCell;
            }

            //ビンゴの場合はカウント終了
            if (openCountMax == 3) break;
        }

        //ユーザーの状態を判定
        if (openCountMaxContainingCanOpenCell <= 1)
        {
            SetUserBingoStatus(UserBingoStatus.Default);
        }
        else if (openCountMaxContainingCanOpenCell == 2)
        {
            if (openCountMax < 2)
            {
                SetUserBingoStatus(UserBingoStatus.PreReach);
            }
            else if (openCountMax == 2)
            {
                SetUserBingoStatus(UserBingoStatus.Reach);
            }
        }
        else if (openCountMaxContainingCanOpenCell == 3)
        {
            if (openCountMax < 3)
            {
                SetUserBingoStatus(UserBingoStatus.PreBingo);
            }
            else if (openCountMax == 3)
            {
                SetUserBingoStatus(UserBingoStatus.Bingo);
            }
        }

    }

    public void SetUserName(string name)
    {
        this.userName = name;
        userNameSubject.OnNext(name);
    }
    public void SetUserBingoStatus(string status)
    {
        this.userBingoStatus = status;
        Debug.Log("SetUserBingoStatus: " + status);
        userBingoStatusSubject.OnNext(status);
    }
    public string GetUserBingoStatus()
    {
        return userBingoStatus;
    }
    public void SetUserBingoPhase(string phase)
    {
        this.userBingoPhase = phase;
        userBingoPhaseSubject.OnNext(phase);
    }
    public string GetUserBingoPhase()
    {
        return userBingoPhase;
    }
    public BingoCellModel GetBingoCell(int index)
    {
        return bingoCellModels[index];
    }
    public BingoCellModel[] GetAllBingoCells()
    {
        return bingoCellModels;
    }

    /// <summary>
    /// ビンゴの数字をランダムでセット
    /// </summary>
    private void SetRadomBingoNumbers()
    {
        int minNum = 1;
        int maxNum = 25;

        List<int> bingoNumbers = new List<int>();
        for (int number = minNum; number <= maxNum; number++)
        {
            bingoNumbers.Add(number);
        }

        //ソートされた数字のリストを作成
        List<int> selectedNumbers = new List<int>();
        for (int index = 0; index < bingoCellModels.Length; index++)
        {
            int listIndex = UnityEngine.Random.Range(0, bingoNumbers.Count);
            int number = bingoNumbers[listIndex];
            bingoNumbers.RemoveAt(listIndex);

            selectedNumbers.Add(number);
        }
        selectedNumbers.Sort();

        //BingoCellModelへセット
        for (int index = 0; index < bingoCellModels.Length; index++)
        {
            bingoCellModels[index] = new BingoCellModel();
            bingoCellModels[index].SetIndex(index);
            bingoCellModels[index].SetNumber(selectedNumbers[index]);
            bingoCellModels[index].SetStatus(BingoCellStatus.Default);
            //bingoCellModels[index].SetStatus(BingoCellStatus.CanOpen); //デバッグ用
        }

        //真ん中のセルをOpenにする場合
        if (isCenterOpen)
        {
            bingoCellModels[4].SetNumber(-1);
            bingoCellModels[4].SetStatus(BingoCellStatus.Open);
        }

        //イベント通知
        for (int index = 0; index < bingoCellModels.Length; index++)
        {
            bingoCellModelSubject.OnNext(bingoCellModels[index]);
        }
    }

    /// <summary>
    /// 対象マスの状態を変更する（OpenとかDeadとか）
    /// </summary>
    /// <param name="index">位置</param>
    /// <param name="status">状態</param>
    public void SetCellStatus(int index, string status)
    {
        bingoCellModels[index].SetStatus(status);
        bingoCellModelSubject.OnNext(bingoCellModels[index]);

        //ユーザーの状態を判定
        DetermineBingoStatus();
    }

    private void SetCurrentNumber(int number)
    {
        this.currentNumber = number;
    }
    public int GetCurrentNumber()
    {
        return this.currentNumber;
    }
    private void SetCurrentNumIndex(int index)
    {
        this.currentNumIndex = index;
    }
    public int GetCurrentNumIndex()
    {
        return this.currentNumIndex;
    }

}
