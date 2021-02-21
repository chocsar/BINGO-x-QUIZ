using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.Networking;
using System.IO;

namespace SpreadsheetSystems
{
    public class SpreadsheetLoader : MonoBehaviour
    {
        // URLは環境に応じて変更
        string requestURL = "https://script.google.com/macros/s/AKfycbyx8EIlFlR20QxbbkyMKKy1odFNsjOEKjIaoikXJ1q8wYEhRmRPt1D1/exec";
        UnityWebRequest www;
        string questionData;
        string filePath = "assets/Resources/question.json";
        bool _isJsonWrite = false;
        [SerializeField]int _questionId = 1;

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
                //string json = JsonUtility.ToJson(www.downloadHandler.text);
                questionData = www.downloadHandler.text;

                if (_isJsonWrite == true)
                {
                    StreamWriter streamWriter = new StreamWriter(filePath);
                    streamWriter.Write(questionData);
                    streamWriter.Flush();
                    streamWriter.Close();
                }
                // または、結果をバイナリデータとして取得します
                // byte[] results = www.downloadHandler.data;

            }
        }
    }
}