using System.Collections;
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
        hostPhaseRef = firebaseDatabase.GetReference("Host").Child("phase");
        hostNumsRef = firebaseDatabase.GetReference("Host").Child("numbers");
        userPhaseRef = firebaseDatabase.GetReference("users").Child(userKey).Child("phase");
        userStatusRef = firebaseDatabase.GetReference("users").Child(userKey).Child("status");
        userNumbersRef = firebaseDatabase.GetReference("users").Child(userKey).Child("numbers");

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
        string number = e.Snapshot.Child("number").GetRawJsonValue();
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
            userNumbersRef.Child("num" + index).SetRawJsonValueAsync(json);
        }
    }
}
