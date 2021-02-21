using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using System;
using Firebase;
using Firebase.Database;

public class GameManager : MonoBehaviour
{
    [SerializeField] private BingoPresenter bingoPresenter;
    private FirebaseDatabase firebaseDatabase;
    private DatabaseReference hostPhaseRef;
    private DatabaseReference hostNumsRef;

    private void Start()
    {
        firebaseDatabase = FirebaseDatabase.DefaultInstance;
        hostPhaseRef = firebaseDatabase.GetReference("Host").Child("phase");
        hostNumsRef = firebaseDatabase.GetReference("Host").Child("nums");

        hostPhaseRef.ValueChanged += OnChangeHostPhase;
        hostNumsRef.ChildAdded += OnGivenNumber;
    }

    private void OnChangeHostPhase(object sender, ValueChangedEventArgs e)
    {
        //ホストのフェーズを取得
        var phase = e.Snapshot.GetRawJsonValue();
        //Debug.Log("phase:" + phase);

        if (phase == HostPhase.SelectNum)
        {
            //ここで処理
        }
    }

    private void OnGivenNumber(object sender, ChildChangedEventArgs e)
    {
        //ホストが出した数字を取得
        var number = e.Snapshot.Child("num").GetRawJsonValue();
        //Debug.Log("num:" + number);

        //ここで処理
    }
}
