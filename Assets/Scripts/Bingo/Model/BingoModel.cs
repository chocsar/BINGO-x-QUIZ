using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using System;


public class BingoModel : MonoBehaviour
{
    public IObservable<string> ChangeUserBingoPhaseEvent => userBingoPhaseSubject;
    public IObservable<string> ChangeUserBingoStatusEvent => userBingoStatusSubject;
    public IObservable<BingoCellModel[]> ChangeCellModelsEvent => bingoCellModelsSubject;
    public IObservable<string> ChangeUserNameEvent => userNameSubject;

    private Subject<string> userBingoPhaseSubject = new Subject<string>();
    private Subject<string> userBingoStatusSubject = new Subject<string>();
    private Subject<BingoCellModel[]> bingoCellModelsSubject = new Subject<BingoCellModel[]>();
    private Subject<string> userNameSubject = new Subject<string>();

    private string userName;
    private string userBingoPhase;
    private string userBingoStatus;
    private BingoCellModel[] bingoCellModels = new BingoCellModel[9];
    private int currentNumber;
    private int currentNumIndex;


    public void InitBingoModel()
    {
        InitBingoNumbers();
        SetUserName(PlayerPrefs.GetString(PlayerPrefsKeys.UserName));
        SetUserBingoPhase(UserBingoPhase.Ready);
        SetUserBingoStatus(UserBingoStatus.Default);
    }

    public bool HasNumber(int number)
    {
        for (int index = 0; index < bingoCellModels.Length; index++)
        {
            if (bingoCellModels[index].GetNumber() == number)
            {
                SetCurrentNumber(number);
                SetCurrentNumIndex(index);
                return true;
            }
        }
        return false;
    }

    public void SetUserBingoPhase(string phase)
    {
        this.userBingoPhase = phase;

        //イベントを発行
        userBingoPhaseSubject.OnNext(phase);
    }

    public string GetUserBingoPhase()
    {
        return userBingoPhase;
    }

    public void SetUserBingoStatus(string status)
    {
        this.userBingoStatus = status;

        //イベントを発行
        userBingoStatusSubject.OnNext(status);
    }

    public string GetUserBingoStatus()
    {
        //Bingoフェースかどうかだけわかればいいかも
        return userBingoStatus;
    }

    public void SetUserName(string name)
    {
        this.userName = name;

        userNameSubject.OnNext(name);
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

    private int GetCurrentNumIndex()
    {
        return this.currentNumIndex;
    }

    private void InitBingoNumbers()
    {
        //ここでデータの復元？

        for (int index = 0; index < bingoCellModels.Length; index++)
        {
            bingoCellModels[index] = new BingoCellModel();
            bingoCellModels[index].SetNumber(UnityEngine.Random.Range(1, 26)); //数字の範囲は？
            bingoCellModels[index].SetStatus(BingoCellStatus.Default); //真ん中は開ける？

            //TODO:セルは数字でソートするのが自然
        }

        //イベントの発行
        bingoCellModelsSubject.OnNext(bingoCellModels);
    }

    public BingoCellModel[] GetBingoCells()
    {
        return bingoCellModels;
    }

    public void SetCurrentCellStatus(string status)
    {
        bingoCellModels[currentNumIndex].SetStatus(status);

        //イベントを発行
        bingoCellModelsSubject.OnNext(bingoCellModels);
    }






}
