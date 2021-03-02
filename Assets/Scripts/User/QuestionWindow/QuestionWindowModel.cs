using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UniRx;
using System;

public class QuestionWindowModel : MonoBehaviour
{
    /// <summary>
    /// デバッグ用にロードする問題を指定する（負の数なら動作しない）
    /// </summary>
    [SerializeField] private int debugQuestionNumber = -1;

    // リクエストするスプシのWebアプリURL
    private const string requestURL = "https://script.google.com/macros/s/AKfycbyx8EIlFlR20QxbbkyMKKy1odFNsjOEKjIaoikXJ1q8wYEhRmRPt1D1/exec";
    UnityWebRequest www;

    public IObservable<int> ChangeQuestionNumEvent => questionNumber;
    public IObservable<string> ChangeQuestionEvent => question;
    public IObservable<string[]> ChangeChoicesEvent => choices;
    private ReactiveProperty<int> questionNumber = new ReactiveProperty<int>();
    private ReactiveProperty<string> question = new ReactiveProperty<string>();
    private ReactiveProperty<string[]> choices = new ReactiveProperty<string[]>();

    private int answerNumber;
    private bool isRight;

    public IEnumerator GetQuestions()
    {
        //全ての問題をロードする
        www = UnityWebRequest.Get(requestURL);
        yield return www.SendWebRequest();

        //エラー処理
        if (www.isNetworkError || www.isHttpError)
        {
            Debug.Log(www.error);
        }
    }

    public void GetQuestion(int questionNum)
    {
        //デバッグ用
        if (debugQuestionNumber > 0)
        {
            questionNum = debugQuestionNumber;
        }

        string jsonText = www.downloadHandler.text;
        JsonNode json = JsonNode.Parse(jsonText); //TODO：OverFlowのエラー対応

        //問題のセット
        foreach (var note in json["questions"])
        {
            int id = int.Parse(note["id"].Get<string>());

            if (id == questionNum)
            {
                //問題番号のセット
                questionNumber.Value = questionNum;

                //問題文のセット
                question.Value = note["question"].Get<string>();

                //選択肢のセット
                string[] choices = new string[3];
                choices[0] = note["choicese1"].Get<string>();
                choices[1] = note["choicese2"].Get<string>();
                choices[2] = note["choicese3"].Get<string>();
                this.choices.Value = choices;
                //Debug.Log(choices[0]);

                //正解番号のセット
                answerNumber = int.Parse(note["answer"].Get<string>());

                break;
            }
        }
    }

    public void ResetQuestionWindowModel()
    {
        questionNumber.Value = 0;
        question.Value = string.Empty;

        string[] choices = new string[3] { string.Empty, string.Empty, string.Empty };
        this.choices.Value = choices;
    }

    public bool CheckAnswer(int userAnswer)
    {
        isRight = (answerNumber == userAnswer);

        return isRight;
    }

    public bool IsRight()
    {
        return isRight;
    }

}
