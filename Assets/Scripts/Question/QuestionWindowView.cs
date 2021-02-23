using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using System;

public class QuestionWindowView : MonoBehaviour
{
    private int questionNumber;
    [SerializeField] int _questionId = 1;
    // リクエストするスプシのWebアプリURL
    string requestURL = "https://script.google.com/macros/s/AKfycbyx8EIlFlR20QxbbkyMKKy1odFNsjOEKjIaoikXJ1q8wYEhRmRPt1D1/exec";
    UnityWebRequest www;
    [SerializeField] Text questionNumberText;
    [SerializeField] Text questionText;
    [SerializeField] Text choiceseText1;
    [SerializeField] Text choiceseText2;
    [SerializeField] Text choiceseText3;
    int answerNum;

    private void OnEnable()
    {
        //テスト用
        questionNumber = _questionId;

        StartCoroutine(GetText());
    }

    public void SetQuestionNumber(int number)
    {
        questionNumber = number;
    }

    public void OpenWindow()
    {
        gameObject.SetActive(true);
    }

    private void CloseWindow()
    {
        gameObject.SetActive(false);
    }


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
            Debug.Log("LoadJson : " + www.downloadHandler.text);
            // または、結果をバイナリデータとして取得します
            // byte[] results = www.downloadHandler.data;
            PrintQuestions();
        }
    }

    void PrintQuestions()
    {
        string jsonText = www.downloadHandler.text;

        JsonNode json = JsonNode.Parse(jsonText);

        Debug.Log("requestID = " + questionNumber);
        foreach (var note in json["questions"])
        {
            int id = int.Parse(note["id"].Get<string>());
            //簡易的な指定問題表示方法（効率悪め）

            if (id == _questionId)
            {
                string question = note["question"].Get<string>();
                answerNum = int.Parse(note["answer"].Get<string>());
                string choicese1 = note["choicese1"].Get<string>();
                string choicese2 = note["choicese2"].Get<string>();
                string choicese3 = note["choicese3"].Get<string>();

                Debug.Log("answerNum = " + answerNum);
                questionText.text = question;
                questionNumberText.text = String.Format("{0:00}", id);

                choiceseText1.text = choicese1;
                choiceseText2.text = choicese2;
                choiceseText3.text = choicese3;
                break;
            }
        }
    }
}
