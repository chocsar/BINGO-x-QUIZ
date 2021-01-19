﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace SpreadsheetSystems
{
    public class SpreadsheetLoader : MonoBehaviour
    {
        // URLは環境に応じて変更
        string requestURL = "https://script.google.com/macros/s/AKfycbyx8EIlFlR20QxbbkyMKKy1odFNsjOEKjIaoikXJ1q8wYEhRmRPt1D1/exec";

        void Start()
        {
            StartCoroutine(GetText());
        }

        // テキストファイルとして読み込む
        IEnumerator GetText()
        {

            UnityWebRequest www = UnityWebRequest.Get(requestURL);
            yield return www.SendWebRequest();

            if (www.isNetworkError || www.isHttpError)
            {
                Debug.Log(www.error);
            }
            else
            {
                // 結果をテキストとして表示します
                Debug.Log(www.downloadHandler.text);

                // または、結果をバイナリデータとして取得します
                // byte[] results = www.downloadHandler.data;
            }
        }
    }
}