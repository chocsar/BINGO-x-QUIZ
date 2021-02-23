using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using System;
using FirebaseREST;
using UniRx;


public class UserFirebaseManager : MonoBehaviour
{
    [SerializeField] private BingoPresenter bingoPresenter;

    //FirebaseDatabase
    private FirebaseDatabase firebaseDatabase;
    private DatabaseReference hostPhaseRef;
    private DatabaseReference hostNumsRef;
    private DatabaseReference usersRef;
    private DatabaseReference userNameRef;
    private DatabaseReference userStatusRef;
    private DatabaseReference userPhaseRef;
    private DatabaseReference userNumbersRef;

    //ユーザーごとのKey
    private string userKey;

    private void Start()
    {
        //FirebaseDatabaseへの参照を保持（usersとHost）
        firebaseDatabase = FirebaseDatabase.Instance;
        usersRef = firebaseDatabase.GetReference(FirebaseKeys.Users);
        hostPhaseRef = firebaseDatabase.GetReference($"{FirebaseKeys.Host}/{FirebaseKeys.HostPhase}");
        hostNumsRef = firebaseDatabase.GetReference($"{FirebaseKeys.Host}/{FirebaseKeys.HostNumbers}");

        if (!PlayerPrefs.HasKey(PlayerPrefsKeys.UserKey))
        {
            //ユーザーの新規作成
            CreateUser();
        }
        else
        {
            //ユーザーデータのロード
            //LoadUser();

            CreateUser(); //デバッグ用
        }

        //FirebaseDatabaseへの参照を保持（usersにある自分のデータ）
        userNameRef = usersRef.Child(userKey).Child(FirebaseKeys.UserName);
        userPhaseRef = usersRef.Child(userKey).Child(FirebaseKeys.UserPhase);
        userStatusRef = usersRef.Child(userKey).Child(FirebaseKeys.UserStatus);
        userNumbersRef = usersRef.Child(userKey).Child(FirebaseKeys.UserNumbers);

        //ホストの変更を監視
        hostPhaseRef.ValueChanged += OnChangeHostPhase;
        hostNumsRef.ValueChanged += OnGivenNumber;

        //BingoPresenterのイベントを監視
        bingoPresenter.ChangeUserBingoPhaseEvent.Subscribe(SaveUserBingoPhase);
        bingoPresenter.ChangeUserBingoStatusEvent.Subscribe(SaveUserBingoStatus);
        bingoPresenter.ChangeCellModelsEvent.Subscribe(SaveUserNumbers);
        bingoPresenter.ChangeUserNameEvent.Subscribe(SaveUserName);

    }

    /// <summary>
    /// ユーザーデータの新規作成
    /// </summary>
    private void CreateUser()
    {
        //キーの作成
        userKey = Utility.UtilityPass.GeneratePassword();
        PlayerPrefs.SetString(PlayerPrefsKeys.UserKey, userKey);
        //Presenterの初期化処理
        bingoPresenter.InitBingoPresenter();
    }

    /// <summary>
    /// ユーザーデータのロード
    /// </summary>
    private void LoadUser()
    {
        //キーの保持
        userKey = PlayerPrefs.GetString(PlayerPrefsKeys.UserKey);

        //TODO:データをロードしてPresenterに渡す処理
        //ロードだから非同期になる？
    }

    /// <summary>
    /// ホストからHostPhaseが変更された時の処理
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void OnChangeHostPhase(object sender, ValueChangedEventArgs e)
    {
        string phase;
        //ホストのフェーズを取得
        phase = e.Snapshot.GetRawJsonValue();
        Debug.Log("phase : " + phase.Trim('"'));
        bingoPresenter.OnChangeHostPhase(phase);
    }

    /// <summary>
    /// ホストから数字が提示された時の処理
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void OnGivenNumber(object sender, ValueChangedEventArgs e)
    {
        //ホストが出した数字を取得
        string number = e.Snapshot.GetRawJsonValue();
        var nums = number.TrimStart('{').TrimEnd('}').Split(',');
        number = nums[nums.Length - 1].Split(':')[1];
        Debug.Log("num:" + number);

        bingoPresenter.OnGivenNumber(Int32.Parse(number));
    }

    //Firebaseへのデータのセーブ処理
    private void SaveUserName(string name)
    {
        userNameRef.SetValueAsync(name, 10, (res) => { });
    }
    private void SaveUserBingoStatus(string status)
    {
        userStatusRef.SetValueAsync(status, 10, (res) => { });
    }
    private void SaveUserBingoPhase(string phase)
    {
        userPhaseRef.SetValueAsync(phase, 10, (res) => { });
    }
    private void SaveUserNumbers(BingoCellModel[] bingoCellModels)
    {
        for (int index = 0; index < bingoCellModels.Length; index++)
        {
            string json = JsonUtility.ToJson(bingoCellModels[index]);
            userNumbersRef.Child(FirebaseKeys.UserNumber + index).SetRawJsonValueAsync(json, 10, (res) => { });
        }
    }
}
