﻿using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using System;
using Firebase;
using Firebase.Database;
using UniRx;


public class UserFirebaseManager : MonoBehaviour
{
    [SerializeField] private BingoPresenter bingoPresenter;
    private FirebaseDatabase firebaseDatabase;
    private DatabaseReference hostPhaseRef;
    private DatabaseReference hostNumsRef;
    private DatabaseReference userPhaseRef;
    private DatabaseReference userStatusRef;
    private DatabaseReference userNumbersRef;
    private string userKey = "testuser";


    private void Start()
    {
        firebaseDatabase = FirebaseDatabase.DefaultInstance;
        hostPhaseRef = firebaseDatabase.GetReference(FirebaseKeys.Host).Child(FirebaseKeys.HostPhase);
        hostNumsRef = firebaseDatabase.GetReference(FirebaseKeys.Host).Child(FirebaseKeys.HostNumbers);
        userPhaseRef = firebaseDatabase.GetReference(FirebaseKeys.Users).Child(userKey).Child(FirebaseKeys.UserPhase);
        userStatusRef = firebaseDatabase.GetReference(FirebaseKeys.Users).Child(userKey).Child(FirebaseKeys.UserStatus);
        userNumbersRef = firebaseDatabase.GetReference(FirebaseKeys.Users).Child(userKey).Child(FirebaseKeys.UserNumbers);

        hostPhaseRef.ValueChanged += OnChangeHostPhase;
        hostNumsRef.ChildAdded += OnGivenNumber;

        bingoPresenter.ChangeUserBingoPhaseEvent.Subscribe(SaveUserBingoPhase);
        bingoPresenter.ChangeUserBingoStatusEvent.Subscribe(SaveUserBingoStatus);
        bingoPresenter.ChangeCellModelsEvent.Subscribe(SaveUserNumbers);


    }

    private void OnChangeHostPhase(object sender, ValueChangedEventArgs e)
    {
        //ホストのフェーズを取得
        string phase = e.Snapshot.GetRawJsonValue();
        //Debug.Log("phase:" + phase);

        bingoPresenter.OnChangeHostPhase(phase);
    }

    private void OnGivenNumber(object sender, ChildChangedEventArgs e)
    {
        //ホストが出した数字を取得
        string number = e.Snapshot.Child(FirebaseKeys.HostNumber).GetRawJsonValue();
        //Debug.Log("num:" + number);

        bingoPresenter.OnGivenNumber(int.Parse(number));
    }

    private void SaveUserBingoPhase(string phase)
    {
        userPhaseRef.SetValueAsync(phase);
    }

    private void SaveUserBingoStatus(string status)
    {
        userStatusRef.SetValueAsync(status);
    }

    private void SaveUserNumbers(BingoCellModel[] bingoCellModels)
    {
        for (int index = 0; index < bingoCellModels.Length; index++)
        {
            string json = JsonUtility.ToJson(bingoCellModels[index]);
            userNumbersRef.Child(FirebaseKeys.UserNumber + index).SetRawJsonValueAsync(json);
        }
    }
}
