using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using System;
using System.IO;



namespace SpreadsheetSystems
{
    public class SpreadsheetLoader : MonoBehaviour
    {

        // URLは環境に応じて変更
        string requestURL = "https://script.google.com/macros/s/AKfycbyx8EIlFlR20QxbbkyMKKy1odFNsjOEKjIaoikXJ1q8wYEhRmRPt1D1/exec";
        UnityWebRequest www;
        [SerializeField] Text testText;
        [SerializeField] int _questionId = 1;

        void Start()
        {
            StartCoroutine(GetText());
            //LoadJsonFile();





        }
        // テキストファイルとして読み込む
        IEnumerator GetText()
        {
            www = UnityWebRequest.Get(requestURL);
            yield return www.SendWebRequest();
            if (www.isNetworkError || www.isHttpError)
            {
                Debug.Log(www.error);
            }
            else
            {
                // 結果をテキストとして表示します
                Debug.Log(www.downloadHandler.text);
                testText.text = www.downloadHandler.text;
                Debug.Log("LoadDone");
                // または、結果をバイナリデータとして取得します
                // byte[] results = www.downloadHandler.data;
                PrintQuestions();
            }
        }

        void PrintQuestions()
        {
            string jsonText = www.downloadHandler.text;

            JsonNode json = JsonNode.Parse(jsonText);

            Debug.Log("requestID = " + _questionId);
            foreach (var note in json["questions"])
            {
                int id = int.Parse(note["id"].Get<string>());
                string question = note["question"].Get<string>();
                string answer = note["answer"].Get<string>();
                //簡易的な指定問題表示方法（効率悪め）

                if (id == _questionId)
                {
                    Debug.Log(question);
                    Debug.Log(answer);
                }
            }
        }
    }
}