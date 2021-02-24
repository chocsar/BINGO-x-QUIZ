using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using System;

public class QuestionWindowView : MonoBehaviour
{
    // リクエストするスプシのWebアプリURL
    private const string requestURL = "https://script.google.com/macros/s/AKfycbyx8EIlFlR20QxbbkyMKKy1odFNsjOEKjIaoikXJ1q8wYEhRmRPt1D1/exec";
    UnityWebRequest www;

    [SerializeField] private Text questionNumberText;
    [SerializeField] private Text questionText;
    [SerializeField] private GameObject choices;
    [SerializeField] private Button[] choiceButtons;
    [SerializeField] private Slider timeSlider;
    [SerializeField] private Button enterButton;
    [SerializeField] private GameObject entered;
    [SerializeField] private GameObject rightObj;
    [SerializeField] private GameObject wrongObj;

    [SerializeField] private float answerTimerMaxValue = 15;
    [SerializeField] private float timeToReadQuestion = 9;

    private bool isPlaying;
    private bool isAnswerSetted;
    private float answerTimer;

    private int questionNum;
    private int answerNum;
    private int playerAnswerNum;

    private void Start()
    {
        choiceButtons[0].GetComponent<Button>().onClick.AddListener(ChoseButton0);
        choiceButtons[1].GetComponent<Button>().onClick.AddListener(ChoseButton1);
        choiceButtons[2].GetComponent<Button>().onClick.AddListener(ChoseButton2);
        enterButton.GetComponent<Button>().onClick.AddListener(SetAnswer);
    }

    private void Update()
    {
        if (isPlaying)
        {
            answerTimer -= Time.deltaTime;

            if (answerTimer <= answerTimerMaxValue)
            {
                choices.SetActive(true);
                timeSlider.value = answerTimer;
                if (isAnswerSetted) entered.SetActive(true);
            }
            else
            {
                isAnswerSetted = false;
            }

            if (answerTimer <= 0)
            {
                isPlaying = false;
                Invoke("CollectChecker", 2);
            }
        }
    }

    private void OnEnable()
    {
        //テスト用
        SetQuestionNumber(1);

        StartCoroutine(GetText());

        choices.SetActive(false);
        choiceButtons[0].GetComponent<Image>().enabled = false;
        choiceButtons[1].GetComponent<Image>().enabled = false;
        choiceButtons[2].GetComponent<Image>().enabled = false;
        entered.SetActive(false);
        answerTimer = answerTimerMaxValue + timeToReadQuestion;
        timeSlider.value = answerTimerMaxValue;
        playerAnswerNum = 0;

        isPlaying = true;
    }

    public void SetQuestionNumber(int number)
    {
        questionNum = number;
    }

    public void OpenWindow()
    {
        gameObject.SetActive(true);
    }

    private void CloseWindow()
    {
        gameObject.SetActive(false);
    }

    private IEnumerator GetText()
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

    private void PrintQuestions()
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
                string question = note["question"].Get<string>();
                answerNum = int.Parse(note["answer"].Get<string>());
                string choicese1 = note["choicese1"].Get<string>();
                string choicese2 = note["choicese2"].Get<string>();
                string choicese3 = note["choicese3"].Get<string>();

                //Debug.Log("answerNum = " + answerNum);
                questionText.text = question;
                questionNumberText.text = String.Format("{0:00}", id);

                choiceButtons[0].GetComponentInChildren<Text>().text = choicese1;
                choiceButtons[1].GetComponentInChildren<Text>().text = choicese2;
                choiceButtons[2].GetComponentInChildren<Text>().text = choicese3;
                break;
            }
        }
    }

    private void ChoseButton0()
    {
        if (isPlaying && !isAnswerSetted)
        {
            playerAnswerNum = 1;
            choiceButtons[0].GetComponent<Image>().enabled = true;
            choiceButtons[1].GetComponent<Image>().enabled = false;
            choiceButtons[2].GetComponent<Image>().enabled = false;
        }
    }

    private void ChoseButton1()
    {
        if (isPlaying && !isAnswerSetted)
        {
            playerAnswerNum = 2;
            choiceButtons[0].GetComponent<Image>().enabled = false;
            choiceButtons[1].GetComponent<Image>().enabled = true;
            choiceButtons[2].GetComponent<Image>().enabled = false;
        }
    }

    private void ChoseButton2()
    {
        if (isPlaying && !isAnswerSetted)
        {
            playerAnswerNum = 3;
            choiceButtons[0].GetComponent<Image>().enabled = false;
            choiceButtons[1].GetComponent<Image>().enabled = false;
            choiceButtons[2].GetComponent<Image>().enabled = true;
        }
    }

    private void SetAnswer()
    {
        isAnswerSetted = true;
    }

    //メモ：正解を出すタイミングはホスト側から提示する
    private void CollectChecker()
    {
        if (answerNum == playerAnswerNum)
        {
            //Debug.Log("seikai");
            rightObj.SetActive(true);
        }
        else
        {
            //Debug.Log("huseikai");
            wrongObj.SetActive(true);
        }
    }

}
