using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using System;
using Firebase;
using Firebase.Database;

public class DataSetTest : MonoBehaviour
{
    DatabaseReference databaseReference;
    void Start()
    {
        // Get the root reference location of the database.
        databaseReference = FirebaseDatabase.DefaultInstance.RootReference;

        //WriteNewUser("chcosar_test", "katayama", "hoge@gmail.com");
        //WriteNewUser("lit_test", "reo", "unity@gmail.com");

        WriteNewScore("test1", 100);
        WriteNewScore("test2", 200);

    }

    private void WriteNewUser(string userId, string name, string email)
    {
        User user = new User(name, email);
        string json = JsonUtility.ToJson(user);

        databaseReference.Child("users").Child(userId).SetRawJsonValueAsync(json);
    }

    private void WriteNewScore(string userId, int score)
    {
        // Create new entry at /user-scores/$userid/$scoreid and at
        // /leaderboard/$scoreid simultaneously
        string key = databaseReference.Child("scores").Push().Key;
        LeaderBoardEntry entry = new LeaderBoardEntry(userId, score);
        Dictionary<string, System.Object> entryValues = entry.ToDictionary();

        Dictionary<string, System.Object> childUpdates = new Dictionary<string, System.Object>();
        childUpdates["/scores/" + key] = entryValues;
        childUpdates["/user-scores/" + userId + "/" + key] = entryValues;

        databaseReference.UpdateChildrenAsync(childUpdates);
    }


}
