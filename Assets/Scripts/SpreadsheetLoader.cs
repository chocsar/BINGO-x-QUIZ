using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;


namespace SpreadsheetSystems
{
    public class SpreadsheetLoader : MonoBehaviour
    {
        // URLは環境に応じて変更
        string requestURL = "https://script.google.com/macros/s/AKfycbyx8EIlFlR20QxbbkyMKKy1odFNsjOEKjIaoikXJ1q8wYEhRmRPt1D1/exec";
        UnityWebRequest www;
        [SerializeField] Text testText;

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

                // または、結果をバイナリデータとして取得します
                // byte[] results = www.downloadHandler.data;

            }
        }
    }
}