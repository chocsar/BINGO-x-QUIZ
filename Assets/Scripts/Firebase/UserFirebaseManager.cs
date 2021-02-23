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
    private DatabaseReference usersRef;
    private DatabaseReference userNameRef;
    private DatabaseReference userPhaseRef;
    private DatabaseReference userStatusRef;
    private DatabaseReference userNumbersRef;
    private string userKey;


    private void Start()
    {
        firebaseDatabase = FirebaseDatabase.DefaultInstance;
        usersRef = firebaseDatabase.GetReference(FirebaseKeys.Users);
        hostPhaseRef = firebaseDatabase.GetReference(FirebaseKeys.Host).Child(FirebaseKeys.HostPhase);
        hostNumsRef = firebaseDatabase.GetReference(FirebaseKeys.Host).Child(FirebaseKeys.HostNumbers);

        if (!PlayerPrefs.HasKey(PlayerPrefsKeys.UserKey))
        {
            CreateUser();
        }
        else
        {
            //LoadUser();
            CreateUser(); //デバッグ用
        }

        userNameRef = usersRef.Child(userKey).Child(FirebaseKeys.UserName);
        userPhaseRef = usersRef.Child(userKey).Child(FirebaseKeys.UserPhase);
        userStatusRef = usersRef.Child(userKey).Child(FirebaseKeys.UserStatus);
        userNumbersRef = usersRef.Child(userKey).Child(FirebaseKeys.UserNumbers);

        //ホストの変更を監視
        hostPhaseRef.ValueChanged += OnChangeHostPhase;
        hostNumsRef.ChildAdded += OnGivenNumber;
        //BingoPresenterのイベントを監視
        bingoPresenter.ChangeUserBingoPhaseEvent.Subscribe(SaveUserBingoPhase);
        bingoPresenter.ChangeUserBingoStatusEvent.Subscribe(SaveUserBingoStatus);
        bingoPresenter.ChangeCellModelsEvent.Subscribe(SaveUserNumbers);
        bingoPresenter.ChangeUserNameEvent.Subscribe(SaveUserName);

    }

    private void CreateUser()
    {
        //キーの作成
        userKey = usersRef.Push().Key;
        PlayerPrefs.SetString(PlayerPrefsKeys.UserKey, userKey);
        //Presenterの初期化処理
        bingoPresenter.InitBingoPresenter();
    }

    private void LoadUser()
    {
        userKey = PlayerPrefs.GetString(PlayerPrefsKeys.UserKey);
        //TODO
        //ロードだから非同期になる？
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

    private void SaveUserName(string name)
    {
        userNameRef.SetValueAsync(name);
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
