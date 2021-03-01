using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using System;
using UniRx;

public class QuestionWindowView : MonoBehaviour
{
    public IObservable<bool> SetAnswerEvent => answerSubject;
    private Subject<bool> answerSubject = new Subject<bool>();


    // リクエストするスプシのWebアプリURL
    private const string requestURL = "https://script.google.com/macros/s/AKfycbyx8EIlFlR20QxbbkyMKKy1odFNsjOEKjIaoikXJ1q8wYEhRmRPt1D1/exec";
    UnityWebRequest www;

    [SerializeField] private Text questionNumberText;
    [SerializeField] private Text questionText;
    [SerializeField] private GameObject choices;
    [SerializeField] private Button[] choiceButtons;
    [SerializeField] private Text[] choiceTexts;
    [SerializeField] private Image[] choiceImages;
    [SerializeField] private Slider timeSlider;
    [SerializeField] private Button enterButton;
    [SerializeField] private GameObject entered;
    [SerializeField] private GameObject rightObj;
    [SerializeField] private GameObject wrongObj;

    [SerializeField] private float answerTimerMaxValue = 15;
    [SerializeField] private float timeToReadQuestion = 9;

    private bool isPlaying = true;
    private bool isAnswerSetted = false;
    private float answerTimer;
    private int questionNum;
    private int answerNum;
    private int playerAnswerNum;
    private bool isRight = false;

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
                //回答の開始
                if (!choices.activeSelf)
                {
                    choices.SetActive(true);
                }

                //スライダーの更新
                timeSlider.value = answerTimer;
            }

            if (answerTimer <= 0)
            {
                //回答をセット
                isAnswerSetted = true;
                SetAnswer();

            }
        }
    }

    private void OnEnable()
    {
        //デバッグ用
        //SetQuestionNumber(UnityEngine.Random.Range(0, 26));

        //問題のロード
        StartCoroutine(GetText()); //TODO:処理が終わるまで待機したい

        //初期化処理
        choices.SetActive(false);
        choiceImages[0].enabled = false;
        choiceImages[1].enabled = false;
        choiceImages[2].enabled = false;
        entered.SetActive(false);
        rightObj.SetActive(false);
        wrongObj.SetActive(false);
        answerTimer = answerTimerMaxValue + timeToReadQuestion;
        timeSlider.value = answerTimerMaxValue;
        playerAnswerNum = 0;
        isAnswerSetted = false;
        isRight = false;

        //問題の開始
        isPlaying = true;
    }

    private void OnDisable()
    {
        //テキストのリセット
        questionText.text = string.Empty;
        questionNumberText.text = string.Empty;
        choiceTexts[0].text = string.Empty;
        choiceTexts[1].text = string.Empty;
        choiceTexts[2].text = string.Empty;
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

                choiceTexts[0].text = choicese1;
                choiceTexts[1].text = choicese2;
                choiceTexts[2].text = choicese3;
                break;
            }
        }
    }

    private void ChoseButton0()
    {
        if (isPlaying)
        {
            playerAnswerNum = 1;
            choiceImages[0].enabled = true;
            choiceImages[1].enabled = false;
            choiceImages[2].enabled = false;
            isAnswerSetted = true;
        }
    }

    private void ChoseButton1()
    {
        if (isPlaying)
        {
            playerAnswerNum = 2;
            choiceImages[0].enabled = false;
            choiceImages[1].enabled = true;
            choiceImages[2].enabled = false;
            isAnswerSetted = true;
        }
    }

    private void ChoseButton2()
    {
        if (isPlaying)
        {
            playerAnswerNum = 3;
            choiceImages[0].enabled = false;
            choiceImages[1].enabled = false;
            choiceImages[2].enabled = true;
            isAnswerSetted = true;
        }
    }

    private void SetAnswer()
    {
        if (!isAnswerSetted) return;

        //問題を終了
        isPlaying = false;
        entered.SetActive(true);

        //正解チェック
        isRight = CheckAnswer();

        //イベント通知
        answerSubject.OnNext(isRight);
    }

    private bool CheckAnswer()
    {
        return (answerNum == playerAnswerNum);
    }

    public void ShowAnswerResult()
    {
        if (isRight)
        {
            rightObj.SetActive(true);
        }
        else
        {
            wrongObj.SetActive(true);
        }

        //TODO:いつ閉じるか
        Invoke("CloseWindow", 3);
    }

}
