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
        else
        {
            commandResultSubject.OnNext("DO NOT RUN COMMAND");
            commandColorSubject.OnNext(Color.red);
        }
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
