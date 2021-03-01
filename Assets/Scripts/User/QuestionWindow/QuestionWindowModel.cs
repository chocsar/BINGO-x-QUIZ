using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UniRx;
using System;

public class QuestionWindowModel : MonoBehaviour
{
    // リクエストするスプシのWebアプリURL
    private const string requestURL = "https://script.google.com/macros/s/AKfycbyx8EIlFlR20QxbbkyMKKy1odFNsjOEKjIaoikXJ1q8wYEhRmRPt1D1/exec";
    UnityWebRequest www;

    public IObservable<string> ChangeQuestionEvent => question;
    public IObservable<int> ChangeQuestionNumEvent => questionNumber;
    public IObservable<string[]> ChangeChoicesEvent => choices;

    private ReactiveProperty<int> questionNumber = new ReactiveProperty<int>();
    private ReactiveProperty<string> question = new ReactiveProperty<string>();
    private ReactiveProperty<string[]> choices = new ReactiveProperty<string[]>();


    [SerializeField] private float answerTimerMaxValue = 15;
    [SerializeField] private float timeToReadQuestion = 9;

    private bool isPlaying = true;
    private bool isAnswerSetted = false;
    private float answerTimer;
    private int answerNum;
    private int playerAnswerNum;
    private bool isRight = false;


    public IEnumerator InitQuestionWindowModel(int questionNum)
    {
        //問題番号のセット
        questionNumber.Value = questionNum;

        yield return StartCoroutine(GetQuestions());
    }

    private IEnumerator GetQuestions()
    {
        www = UnityWebRequest.Get(requestURL);

        yield return www.SendWebRequest();

        if (www.isNetworkError || www.isHttpError)
        {
            Debug.Log(www.error);
        }
        else
        {
            GetQuestion(questionNumber.Value);
        }
    }

    private void GetQuestion(int questionNum)
    {
        string jsonText = www.downloadHandler.text;

        JsonNode json = JsonNode.Parse(jsonText);

        //Debug.Log("requestID = " + questionNumber);
        foreach (var note in json["questions"])
        {
            int id = int.Parse(note["id"].Get<string>());
            //簡易的な指定問題表示方法（効率悪め）

            if (id == questionNum)
            {
                //問題文のセット
                question.Value = note["question"].Get<string>();

                //選択肢のセット
                string[] choices = new string[3];
                choices[0] = note["choicese1"].Get<string>();
                choices[1] = note["choicese2"].Get<string>();
                choices[2] = note["choicese3"].Get<string>();
                this.choices.Value = choices;

                //正解番号のセット
                this.answerNum = int.Parse(note["answer"].Get<string>());

                break;
            }
        }
    }
}
