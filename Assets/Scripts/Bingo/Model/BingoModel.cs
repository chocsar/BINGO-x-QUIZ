using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using System;


public class BingoModel : MonoBehaviour
{
    public IObservable<string> ChangeBingoPhaseEvent => userBingoPhaseSubject;

    private Subject<string> userBingoPhaseSubject = new Subject<string>();
    private string userName;
    private string userBingoPhase;
    private string userBingoStatus;
    private BingoCellModel[] bingoCellModels = new BingoCellModel[9];

    private int currentNumber;
    private int currentNumIndex;


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
    }

    public string GetUserBingoStatus()
    {
        //Bingoフェースかどうかだけわかればいいかも
        return userBingoStatus;
    }

    public void SetUserName(string name)
    {
        this.userName = name;

        //イベントを発行
    }


    public int GetCurrentNumber()
    {
        return currentNumber;
    }

    private void SetCurrentNumber(int number)
    {
        this.currentNumber = number;
    }

    public int GetCurrentNumIndex()
    {
        return currentNumIndex;
    }

    private void SetCurrentNumIndex(int index)
    {
        this.currentNumIndex = index;
    }

    private void InitBingoNumbers()
    {
        foreach (BingoCellModel bingoCell in bingoCellModels)
        {
            bingoCell.SetNumber(UnityEngine.Random.Range(1, 26)); //数字の範囲は？
            bingoCell.SetCellStatus(BingoCellStatus.Default); //真ん中は開ける？
        }

        //ここでデータの復元？
    }

    public BingoCellModel[] GetBingoCells()
    {
        return bingoCellModels;
    }






}
