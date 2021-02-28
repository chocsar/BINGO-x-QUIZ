using System.Collections.Generic;
using FirebaseREST;
using UnityEngine;
using System;
using UniRx;

public class HostHidddenModel : MonoBehaviour
{
    public IObservable<string> SubmitCommandResultEvent => commandResultSubject;
    public IObservable<Color> SubmitCommandResultColorEvent => commandColorSubject;

    private Subject<string> commandResultSubject = new Subject<string>();
    private Subject<Color> commandColorSubject = new Subject<Color>();

    public void InitModel()
    {
        DeleteAllNumbers();
    }

    public void Command(string _command)
    {
        if (_command == CommandList.deleteAllNum)
        {
            DeleteAllNumbers();
        }
        else if(_command == CommandList.changeAllClientReady)
        {
            UpdateAllClientStatus(UserBingoPhase.Ready);
        }
        else
        {
            commandResultSubject.OnNext("DO NOT RUN COMMAND");
            commandColorSubject.OnNext(Color.red);
        }
    }

    private void UpdateAllClientStatus(string _status)
    {
        var data = new ReactiveProperty<string>();
        DatabaseReference reference = FirebaseDatabase.Instance.GetReference(FirebaseKeys.UserPhaseOnly);
        reference.GetValueAsync(10, (res) =>
        {
            if (res.success)
            {
                data.Value = res.data.GetRawJsonValue();
                data.Subscribe(x =>
                {
                    Dictionary<string, string> phaseDic = new Dictionary<string, string>();
                    phaseDic = Utility.UtilityRestJson.JsonPhaseLoad(x);
                    foreach (var clientphase in phaseDic)
                    {
                        if (clientphase.Value != _status)
                        {
                            UpdatePhaseDatabase(clientphase, _status);
                        }
                    }
                    commandResultSubject.OnNext("ALL CLIENT PHASE IS READY");
                    commandColorSubject.OnNext(Color.white);
                });
            }
            else
            {
                Debug.Log("Fetch data failed : " + res.message);
            }
        });
    }

    private void UpdatePhaseDatabase(KeyValuePair<string, string> phaseDic, string _updateValueData)
    {
        Dictionary<string, System.Object> childUpdates = new Dictionary<string, System.Object>();
        childUpdates[$"{FirebaseKeys.UserPhaseOnly}/{phaseDic.Key}"] = _updateValueData;
        childUpdates[$"{FirebaseKeys.Users}/{phaseDic.Key}/{FirebaseKeys.UserPhase}"] = _updateValueData;
        DatabaseReference reference = FirebaseDatabase.Instance.GetReference("/");
        reference.UpdateChildAsync(childUpdates, 10, (res) => { });
    }

    // ホストのデータベースに登録されている数字を全削除
    private void DeleteAllNumbers()
    {
        DatabaseReference reference = FirebaseDatabase.Instance.GetReference($"{FirebaseKeys.Host}/{FirebaseKeys.HostNumbers}");
        reference.RemoveValueAsync(10, (e) =>
        {
            if (e.success)
            {
                Debug.Log("Delete data success");
                commandResultSubject.OnNext("DELETE ALL NUMBERS");
                commandColorSubject.OnNext(Color.white);
            }
            else
            {
                Debug.Log("Delete data failed : " + e.message);
            }
        });
    }
}
