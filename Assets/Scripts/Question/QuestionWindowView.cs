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
    [SerializeField] GameObject answerBox;
    [SerializeField] Button choiceseButton1;
    [SerializeField] Button choiceseButton2;
    [SerializeField] Button choiceseButton3;
    int answerNum;
    [SerializeField] int _playerAnswer;
    [SerializeField] Slider timeSlider;
    bool isPlaying;
    float timer;

    private void Start()
    {
        choiceseButton1.GetComponent<Button>().onClick.AddListener(ChoseButton1);
        choiceseButton2.GetComponent<Button>().onClick.AddListener(ChoseButton2);
        choiceseButton3.GetComponent<Button>().onClick.AddListener(ChoseButton3);
    }

    private void OnEnable()
    {
        //テスト用
        questionNumber = _questionId;

        StartCoroutine(GetText());
        _playerAnswer = 0;
        timer = 25;
        timeSlider.value = 15;
        answerBox.SetActive(false);
        isPlaying = true;
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

    private void Update()
    {
        if (isPlaying)
        {
            timer -= Time.deltaTime;
            if (timer <= 15)
            {
                answerBox.SetActive(true);
                timeSlider.value = timer;
            }
            if (timer <= 0)
            {
                CollectChecker();
                isPlaying = false;
            }
        }
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
            //Debug.Log("LoadJson : " + www.downloadHandler.text);
            // または、結果をバイナリデータとして取得します
            // byte[] results = www.downloadHandler.data;
            PrintQuestions();
        }
    }

    void PrintQuestions()
    {
        string jsonText = www.downloadHandler.text;

        JsonNode json = JsonNode.Parse(jsonText);

        //Debug.Log("requestID = " + questionNumber);
        foreach (var note in json["questions"])
        {
            int id = int.Parse(note["id"].Get<string>());
            //簡易的な指定問題表示方法（効率悪め）

            if (id == questionNumber)
            {
                string question = note["question"].Get<string>();
                answerNum = int.Parse(note["answer"].Get<string>());
                string choicese1 = note["choicese1"].Get<string>();
                string choicese2 = note["choicese2"].Get<string>();
                string choicese3 = note["choicese3"].Get<string>();

                Debug.Log("answerNum = " + answerNum);
                questionText.text = question;
                questionNumberText.text = String.Format("{0:00}", id);

                choiceseButton1.GetComponentInChildren<Text>().text = choicese1;
                choiceseButton2.GetComponentInChildren<Text>().text = choicese2;
                choiceseButton3.GetComponentInChildren<Text>().text = choicese3;
                break;
            }
        }
    }

    void ChoseButton1()
    {
        _playerAnswer = 1;
    }
    void ChoseButton2()
    {
        _playerAnswer = 2;
    }
    void ChoseButton3()
    {
        _playerAnswer = 3;
    }

    void CollectChecker()
    {
        if(answerNum == _playerAnswer)
        {
            Debug.Log("seikai");
        }
        else
        {
            Debug.Log("huseikai");
        }
    }



}
