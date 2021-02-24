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
        hostPhaseRef = firebaseDatabase.GetReference($"{FirebaseKeys.Host}/{FirebaseKeys.HostPhase}");
        hostNumsRef = firebaseDatabase.GetReference($"{FirebaseKeys.Host}/{FirebaseKeys.HostNumbers}");

        if (!PlayerPrefs.HasKey(PlayerPrefsKeys.UserKey))
        {
            //ユーザーの新規作成
            CreateUserKey();
        }
        else
        {
            //ユーザーデータのロード
            //LoadUser();
            CreateUserKey(); //デバッグ用
        }

        //FirebaseDatabaseへの参照を保持（usersにある自分のデータ）
        userNameRef = firebaseDatabase.GetReference($"{FirebaseKeys.Users}/{userKey}/{FirebaseKeys.UserName}");
        userPhaseRef = firebaseDatabase.GetReference($"{FirebaseKeys.Users}/{userKey}/{FirebaseKeys.UserPhase}");
        userStatusRef = firebaseDatabase.GetReference($"{FirebaseKeys.Users}/{userKey}/{FirebaseKeys.UserStatus}");
        userNumbersRef = firebaseDatabase.GetReference($"{FirebaseKeys.Users}/{userKey}/{FirebaseKeys.UserNumbers}");

        //ホストの変更を監視
        hostPhaseRef.ValueChanged += OnChangeHostPhase;
        hostNumsRef.LimitToLast(1).ValueChanged += OnGivenNumber;

        //BingoPresenterのイベントを監視
        bingoPresenter.ChangeUserBingoPhaseEvent.Subscribe(SaveUserBingoPhase);
        bingoPresenter.ChangeUserBingoStatusEvent.Subscribe(SaveUserBingoStatus);
        bingoPresenter.ChangeCellModelEvent.Subscribe(SaveUserNumber);
        bingoPresenter.ChangeUserNameEvent.Subscribe(SaveUserName);

        //ビンゴの初期化
        bingoPresenter.InitBingoPresenter();

    }

    /// <summary>
    /// ユーザーデータの新規作成
    /// </summary>
    private void CreateUserKey()
    {
        //キーの作成
        //userKey = Utility.UtilityPass.GeneratePassword();
        userKey = " mittan";
        PlayerPrefs.SetString(PlayerPrefsKeys.UserKey, userKey);
        PlayerPrefs.Save();
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
        Debug.Log("phase:" + phase.Trim('"'));
        //bingoPresenter.OnChangeHostPhase(phase);
        bingoPresenter.OnChangeHostPhase(phase.Trim('"'));
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
        if (number == null) return;
        if (number.Contains("{"))
        {
            var num = number.TrimStart('{').TrimEnd('}');
            if (number.Contains(","))
            {
                var nums = num.Split(',');
                number = nums[nums.Length - 1].Split(':')[1];
            }
            else number = num.Split(':')[1];
        }

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
    private void SaveUserNumber(BingoCellModel bingoCellModel)
    {
        string json = JsonUtility.ToJson(bingoCellModel);
        userNumbersRef = firebaseDatabase.GetReference($"{FirebaseKeys.Users}/{userKey}/{FirebaseKeys.UserNumbers}/{FirebaseKeys.UserNumber}{bingoCellModel.GetIndex()}");
        userNumbersRef.SetRawJsonValueAsync(json, 10, (res) => { });
    }
}
