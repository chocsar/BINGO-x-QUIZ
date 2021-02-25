using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FirebaseREST;

public class HostFirebaseLoader : MonoBehaviour
{
    private Dictionary<string, string> phaseDic = new Dictionary<string, string>();

    public Dictionary<string, int> ClientDataLoad()
    {
        string data = ""; 
        DatabaseReference reference = FirebaseDatabase.Instance.GetReference("userphase");
        reference.GetValueAsync(10, (res) =>
        {
            if (res.success)
            {
                Debug.Log("Success fetched data : " + res.data.GetRawJsonValue());
                Debug.Log("res : " + res.data.GetRawJsonValue());
                //DisplayAllPhase();
                
            }
            else
            {
                Debug.Log("Fetch data failed : " + res.message);
            }
        });
        

        Debug.Log("data : " + data);
        //phaseDic = Utility.UtilityRestJson.JsonPhaseLoad(data);
        return PhaseCheck(phaseDic);
    }

    private Dictionary<string, int> PhaseCheck(Dictionary<string, string> dic)
    {
        Debug.Log("check");
        int[] phaseNum = new int[5]; // 1:ready, 2:beforeanswer, 3: answer, 4: afteranswer, 5: open
        foreach (var data in dic)
        {
            Debug.Log("phase : " + data.Value);
            switch (data.Value)
            {
                case UserBingoPhase.Ready:
                    phaseNum[0]++;
                    break;
                case UserBingoPhase.BeforeAnswer:
                    phaseNum[1]++;
                    break;
                case UserBingoPhase.Answer:
                    phaseNum[2]++;
                    break;
                case UserBingoPhase.AfterAnswer:
                    phaseNum[3]++;
                    break;
                case UserBingoPhase.Open:
                    phaseNum[4]++;
                    break;
                default:
                    phaseNum[4]++;
                    break;
            }
        }
        Dictionary<string, int> sendData = new Dictionary<string, int>()
            {
                { UserBingoPhase.Ready, phaseNum[0]},
                { UserBingoPhase.BeforeAnswer, phaseNum[1]},
                { UserBingoPhase.Answer, phaseNum[2]},
                { UserBingoPhase.AfterAnswer, phaseNum[3]},
                { UserBingoPhase.Open, phaseNum[4]},
            };

        return sendData;
    }

    private void DisplayAllPhase()
    {
        foreach (var item in phaseDic)
        {
            Debug.Log($"{item.Key} : {item.Value}");
        }
    }


}
