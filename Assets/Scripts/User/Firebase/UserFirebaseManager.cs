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
    private DatabaseReference hostPhaseOnlyRef;
    private DatabaseReference hostNumsRef;
    private DatabaseReference bingoUserRef;
    private DatabaseReference reachUserRef;

    //ユーザーごとのKey
    private string userKey;

    private void Start()
    {
        //FirebaseDatabaseへの参照を保持
        firebaseDatabase = FirebaseDatabase.Instance;
        hostPhaseOnlyRef = firebaseDatabase.GetReference($"{FirebaseKeys.HostPhaseOnly}");
        hostNumsRef = firebaseDatabase.GetReference($"{FirebaseKeys.Host}/{FirebaseKeys.HostNumbers}");
        bingoUserRef = firebaseDatabase.GetReference($"{FirebaseKeys.BingoUser}");
        reachUserRef = firebaseDatabase.GetReference($"{FirebaseKeys.ReachUser}");

        //BingoPresenterのイベントを監視
        bingoPresenter.ChangeUserBingoPhaseEvent.Subscribe(SaveUserBingoPhase).AddTo(gameObject);
        bingoPresenter.ChangeUserBingoStatusEvent.Subscribe(SaveUserBingoStatus).AddTo(gameObject);
        bingoPresenter.ChangeCellModelEvent.Subscribe(SaveUserNumber).AddTo(gameObject);
        bingoPresenter.ChangeUserNameEvent.Subscribe(SaveUserName).AddTo(gameObject);
        bingoPresenter.BingoEvent.Subscribe(SaveUserAsBingo).AddTo(gameObject);
        bingoPresenter.ReachEvent.Subscribe(SaveUserAsReach).AddTo(gameObject);

        if (!PlayerPrefs.HasKey(PlayerPrefsKeys.UserKey))
        {
            //ユーザーの新規作成
            CreateUserData();
        }
        else
        {
            //ユーザーデータのロード
            LoadUserData();
        }

    }

    /// <summary>
    /// ユーザーデータの新規作成
    /// </summary>
    private void CreateUserData()
    {
        //キーの作成
        userKey = Utility.UtilityPass.GeneratePassword();
        //userKey = "reotest"; //デバッグ用

        //キーの保存
        PlayerPrefs.SetString(PlayerPrefsKeys.UserKey, userKey);
        PlayerPrefs.Save();

        //ビンゴの初期化
        bingoPresenter.InitBingoPresenter();

        //データベースの変更を監視
        AddEventHandler();
    }

    private void LoadUserData()
    {
        //キーのロード
        userKey = PlayerPrefs.GetString(PlayerPrefsKeys.UserKey);

        //bingoCellModelsにロードしたデータを格納
        DatabaseReference reference = firebaseDatabase.GetReference($"{FirebaseKeys.Users}/{userKey}/{FirebaseKeys.UserNumbers}");
        reference.GetValueAsync(10, (res) =>
        {
            if (res.success)
            {
                //Debug.Log("Success fetched data : " + res.data.GetRawJsonValue());

                var json = new ReactiveProperty<string>();
                json.Value = res.data.GetRawJsonValue();

                //ロードし終わった時の処理    
                json.Subscribe(data =>
                {
                    //データが入ってなかった場合はデータを新規作成する
                    if (data == null)
                    {
                        CreateUserData();
                        return;
                    }

                    BingoCellModel[] bingoCellModels = new BingoCellModel[9];
                    bingoCellModels = Utility.UtilityRestJson.JsonUserDataLoad(json.Value);

                    //Hit状態として保存されていたならDead状態としてロードする
                    foreach (BingoCellModel bingoCellModel in bingoCellModels)
                    {
                        if (bingoCellModel.GetStatus() == BingoCellStatus.Hit)
                        {
                            bingoCellModel.SetStatus(BingoCellStatus.Dead);
                        }
                    }

                    //ビンゴの初期化処理 
                    bingoPresenter.InitBingoPresenter(bingoCellModels);

                    //データベースの変更を監視
                    AddEventHandler();
                });
            }
            else
            {
                Debug.Log("Fetch data failed : " + res.message);
            }
        });
    }

    private void AddEventHandler()
    {
        hostPhaseOnlyRef.ValueChanged += OnChangeHostPhase;
        hostNumsRef.LimitToLast(1).ValueChanged += OnGivenNumber;
        bingoUserRef.LimitToLast(1).ValueChanged += ReportBingoUser;
        reachUserRef.LimitToLast(1).ValueChanged += ReportReachUser;
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
        //Debug.Log("phase:" + phase.Trim('"'));
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
        //複数箇所に同時に書き込む実装に変更
        Dictionary<string, System.Object> childUpdates = new Dictionary<string, System.Object>();
        childUpdates[$"/{FirebaseKeys.Users}/{userKey}/{FirebaseKeys.UserName}"] = name;
        childUpdates[$"/{FirebaseKeys.UserNameAndStatusOnly}/{userKey}/{FirebaseKeys.UserName}"] = name;
        firebaseDatabase.GetReference("/").UpdateChildAsync(childUpdates, 10, (res) => { });
    }
    private void SaveUserBingoStatus(string status)
    {
        //PreBingoならBingoとして保存する
        if (status == UserBingoStatus.PreBingo)
        {
            status = UserBingoStatus.Bingo;
        }
        //PreReachならReachとして保存する
        else if (status == UserBingoStatus.PreReach)
        {
            status = UserBingoStatus.Reach;
        }

        //複数箇所に同時に書き込む実装に変更
        Dictionary<string, System.Object> childUpdates = new Dictionary<string, System.Object>();
        childUpdates[$"/{FirebaseKeys.Users}/{userKey}/{FirebaseKeys.UserStatus}"] = status;
        childUpdates[$"/{FirebaseKeys.UserNameAndStatusOnly}/{userKey}/{FirebaseKeys.UserStatus}"] = status;
        childUpdates[$"/{FirebaseKeys.UserStatusOnly}/{userKey}"] = status;

        firebaseDatabase.GetReference("/").UpdateChildAsync(childUpdates, 10, (res) => { });
    }
    private void SaveUserBingoPhase(string phase)
    {
        //複数箇所に同時に書き込む実装に変更
        Dictionary<string, System.Object> childUpdates = new Dictionary<string, System.Object>();
        childUpdates[$"/{FirebaseKeys.Users}/{userKey}/{FirebaseKeys.UserPhase}"] = phase;
        childUpdates[$"/{FirebaseKeys.UserPhaseOnly}/{userKey}"] = phase;
        firebaseDatabase.GetReference("/").UpdateChildAsync(childUpdates, 10, (res) => { });
    }
    private void SaveUserNumber(BingoCellModel bingoCellModel)
    {
        BingoCellModel cell = new BingoCellModel();
        cell.SetIndex(bingoCellModel.GetIndex());
        cell.SetNumber(bingoCellModel.GetNumber());
        cell.SetStatus(bingoCellModel.GetStatus());

        //CanOpen状態ならOpenとして保存する
        if (cell.GetStatus() == BingoCellStatus.CanOpen)
        {
            cell.SetStatus(BingoCellStatus.Open);
        }

        string json = JsonUtility.ToJson(cell);
        DatabaseReference userNumberRef = firebaseDatabase.GetReference($"{FirebaseKeys.Users}/{userKey}/{FirebaseKeys.UserNumbers}/{FirebaseKeys.UserNumber}{bingoCellModel.GetIndex()}");
        userNumberRef.SetRawJsonValueAsync(json, 10, (res) => { });
    }

    private void SaveUserAsBingo(string userName)
    {
        DatabaseReference bingoUserRef = firebaseDatabase.GetReference($"{FirebaseKeys.BingoUser}");
        bingoUserRef.Push(userName, 10, (res) => { });
    }

    private void ReportBingoUser(object sender, ValueChangedEventArgs e)
    {
        string userName = e.Snapshot.GetRawJsonValue();

        if (userName == null) return;

        //Unicodeから変換
        userName = System.Text.RegularExpressions.Regex.Unescape(userName);

        //Debug.Log(userName);

        if (userName.Contains("{"))
        {
            var name = userName.TrimStart('{').TrimEnd('}');
            if (userName.Contains(","))
            {
                var names = name.Split(',');
                userName = names[names.Length - 1].Split(':')[1];
            }
            else userName = name.Split(':')[1];
        }

        bingoPresenter.ReportBingoUser(userName.Trim('"'), UserBingoStatus.Bingo);
    }

    private void SaveUserAsReach(string userName)
    {
        DatabaseReference bingoUserRef = firebaseDatabase.GetReference($"{FirebaseKeys.ReachUser}");
        bingoUserRef.Push(userName, 10, (res) => { });
    }

    private void ReportReachUser(object sender, ValueChangedEventArgs e)
    {
        string userName = e.Snapshot.GetRawJsonValue();

        if (userName == null) return;

        //Unicodeから変換
        userName = System.Text.RegularExpressions.Regex.Unescape(userName);

        //Debug.Log(userName);

        if (userName.Contains("{"))
        {
            var name = userName.TrimStart('{').TrimEnd('}');
            if (userName.Contains(","))
            {
                var names = name.Split(',');
                userName = names[names.Length - 1].Split(':')[1];
            }
            else userName = name.Split(':')[1];
        }

        bingoPresenter.ReportBingoUser(userName.Trim('"'), UserBingoStatus.Reach);
    }
}
