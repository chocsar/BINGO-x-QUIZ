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
    [SerializeField] Button[] choiceseButton;
    [SerializeField] Button collectButton;
    int answerNum;
    Color color = new Color(231, 197, 74, 255);
    [SerializeField] int _playerAnswer;
    [SerializeField] Slider timeSlider;
    [SerializeField] GameObject ansObj;
    [SerializeField] GameObject noansObj;
    [SerializeField] GameObject buttonInput;
    bool isPlaying;
    bool isAnswerSet;
    float timer;

    private void Start()
    {
        choiceseButton[0].GetComponent<Button>().onClick.AddListener(ChoseButton0);
        choiceseButton[1].GetComponent<Button>().onClick.AddListener(ChoseButton1);
        choiceseButton[2].GetComponent<Button>().onClick.AddListener(ChoseButton2);
        collectButton.GetComponent<Button>().onClick.AddListener(AnswerSet);
    }

    private void OnEnable()
    {
        //テスト用
        questionNumber = _questionId;

        StartCoroutine(GetText());
        _playerAnswer = 0;
        timer = 26;
        timeSlider.value = 15;
        answerBox.SetActive(false);
        buttonInput.SetActive(false);
        isPlaying = true;
        choiceseButton[0].GetComponent<Image>().enabled = false;
        choiceseButton[1].GetComponent<Image>().enabled = false;
        choiceseButton[2].GetComponent<Image>().enabled = false;
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
                if(isAnswerSet) buttonInput.SetActive(true);
            }
            else
            {
                isAnswerSet = false;
            }
            if (timer <= 0)
            {
                isPlaying = false;
                //Debug.Log("false");
                Invoke("CollectChecker", 2);
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

                choiceseButton[0].GetComponentInChildren<Text>().text = choicese1;
                choiceseButton[1].GetComponentInChildren<Text>().text = choicese2;
                choiceseButton[2].GetComponentInChildren<Text>().text = choicese3;
                break;
            }
        }
    }

    void ChoseButton0()
    {
        if (isPlaying && !isAnswerSet)
        {
            _playerAnswer = 1;
            choiceseButton[0].GetComponent<Image>().enabled = true;
            choiceseButton[1].GetComponent<Image>().enabled = false;
            choiceseButton[2].GetComponent<Image>().enabled = false;
        }
    }
    void ChoseButton1()
    {
        if (isPlaying && !isAnswerSet)
        {
            _playerAnswer = 2;
            choiceseButton[0].GetComponent<Image>().enabled = false;
            choiceseButton[1].GetComponent<Image>().enabled = true;
            choiceseButton[2].GetComponent<Image>().enabled = false;
        }
    }
    void ChoseButton2()
    {
        if (isPlaying && !isAnswerSet)
        {
            _playerAnswer = 3;
            choiceseButton[0].GetComponent<Image>().enabled = false;
            choiceseButton[1].GetComponent<Image>().enabled = false;
            choiceseButton[2].GetComponent<Image>().enabled = true;
        }
    }

    void CollectChecker()
    {
        if(answerNum == _playerAnswer)
        {
            Debug.Log("seikai");
            ansObj.SetActive(true);
        }
        else
        {
            Debug.Log("huseikai");
            noansObj.SetActive(true);
        }
    }

    void AnswerSet()
    {
        isAnswerSet = true;
    }

}
