using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using System;


public class BingoModel : MonoBehaviour
{
    public IObservable<string> ChangeUserNameEvent => userNameSubject;
    public IObservable<string> ChangeUserBingoStatusEvent => userBingoStatusSubject;
    public IObservable<string> ChangeUserBingoPhaseEvent => userBingoPhaseSubject;
    public IObservable<BingoCellModel[]> ChangeCellModelsEvent => bingoCellModelsSubject; //TODO:Firebaseのセーブをindexで指定できれば不要
    public IObservable<BingoCellModel> ChangeCellModelEvent => bingoCellModelSubject;

    private Subject<string> userNameSubject = new Subject<string>();
    private Subject<string> userBingoStatusSubject = new Subject<string>();
    private Subject<string> userBingoPhaseSubject = new Subject<string>();
    private Subject<BingoCellModel[]> bingoCellModelsSubject = new Subject<BingoCellModel[]>(); //TODO:Firebaseのセーブをindexで指定できれば不要
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

    /// <summary>
    /// ホストから提示された数字を持っているかどうか
    /// </summary>
    /// <param name="number">ホストが提示した数字</param>
    /// <returns></returns>
    public bool HasNumber(int number)
    {
        for (int index = 0; index < bingoCellModels.Length; index++)
        {
            //数字を持っていた場合
            if (bingoCellModels[index].GetNumber() == number)
            {
                //数字を保持しておく
                SetCurrentNumber(number);
                SetCurrentNumIndex(index);
                //Hit状態にする
                SetCellStatus(index, BingoCellStatus.Hit);

                return true;
            }
        }

        return false;
    }

    public void SetUserName(string name)
    {
        this.userName = name;
        userNameSubject.OnNext(name);
    }
    public void SetUserBingoStatus(string status)
    {
        this.userBingoStatus = status;
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
        for (int index = 0; index < bingoCellModels.Length; index++)
        {
            bingoCellModels[index] = new BingoCellModel();
            bingoCellModels[index].SetIndex(index);
            bingoCellModels[index].SetNumber(UnityEngine.Random.Range(1, 26));

            SetCellStatus(index, BingoCellStatus.Default);

            //TODO:セルは数字でソートするのが自然
        }
        bingoCellModelsSubject.OnNext(bingoCellModels); //TODO:Firebaseのセーブをindexで指定できれば不要
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
        bingoCellModelsSubject.OnNext(bingoCellModels); //TODO:Firebaseのセーブをindexで指定できれば不要
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
