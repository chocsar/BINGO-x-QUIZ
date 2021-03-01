using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using System;
using UniRx;

public class QuestionWindowView : MonoBehaviour
{

    [SerializeField] private float answerTime = 15;
    [SerializeField] private float timeToReadQuestion = 2;

    public IObservable<int> SetAnswerEvent => answerSubject;
    private Subject<int> answerSubject = new Subject<int>();

    [SerializeField] private GameObject canvas;
    [SerializeField] private Text questionNumberText;
    [SerializeField] private Text questionText;
    [SerializeField] private ChoiceView[] choiceViews;
    [SerializeField] private GameObject choicesParent;
    [SerializeField] private Button enterButton;
    [SerializeField] private GameObject entered;
    [SerializeField] private Slider timeSlider;
    [SerializeField] private GameObject right;
    [SerializeField] private GameObject wrong;

    private bool isAnswering = false;
    private bool isAnswerSetted = false;
    private int userChoiceNum;

    private float answerTimer = 15;


    public void InitQuestionWindowView()
    {
        for (int index = 0; index < choiceViews.Length; index++)
        {
            //各選択肢ボタンへの入力を監視
            choiceViews[index].OnClickEvent.Subscribe(OnClick).AddTo(gameObject);
            //indexをセット
            choiceViews[index].SetIndex(index);
            //初期化処理
            choiceViews[index].InitChoiceView();
        }

        //回答ボタンへの入力を監視
        enterButton.OnClickAsObservable().Subscribe(_ => SetAnswer()).AddTo(gameObject);
    }

    private void Update()
    {
        if (!isAnswering) return;

        answerTimer -= Time.deltaTime;

        if (answerTimer <= answerTime)
        {
            //回答の開始
            if (!choicesParent.activeSelf)
            {
                choicesParent.SetActive(true);
            }

            //スライダーの更新
            timeSlider.value = answerTimer;
        }

        //回答時間切れ
        if (answerTimer <= 0)
        {
            //回答をセット
            isAnswerSetted = true;
            SetAnswer();
        }
    }

    public void ResetQuestionWindowView()
    {
        //UIのリセット
        foreach (ChoiceView choiceView in choiceViews)
        {
            choiceView.SetImage(false);
        }
        choicesParent.SetActive(false);
        entered.SetActive(false);
        right.SetActive(false);
        wrong.SetActive(false);

        //パラメータのリセット
        userChoiceNum = 0;
        answerTimer = answerTime + timeToReadQuestion;
        timeSlider.value = answerTime;
        isAnswerSetted = false;
    }

    private void OnClick(int index)
    {
        if (!isAnswering) return;

        //ImageのON/OFF切り替え
        for (int i = 0; i < choiceViews.Length; i++)
        {

            if (i == index)
            {
                choiceViews[i].SetImage(true);
            }
            else
            {
                choiceViews[i].SetImage(false);
            }
        }

        //選択番号を保持
        userChoiceNum = index + 1;

        isAnswerSetted = true;
    }

    private void SetAnswer()
    {
        if (!isAnswerSetted) return;

        //問題を終了
        isAnswering = false;
        entered.SetActive(true);

        //イベント通知
        answerSubject.OnNext(userChoiceNum);
    }

    public void ShowAnswerResult(bool isRight)
    {
        if (isRight)
        {
            right.SetActive(true);
        }
        else
        {
            wrong.SetActive(true);
        }

        //3秒後にウィンドウを閉じる
        Invoke("CloseWindow", 3);
    }

    public void OpenWindow()
    {
        canvas.SetActive(true);
        isAnswering = true;
    }

    private void CloseWindow()
    {
        canvas.SetActive(false);
        right.SetActive(false);
        wrong.SetActive(false);
    }

    public void SetQuestionNumber(int number)
    {
        questionNumberText.text = String.Format("{0:00}", number);
    }

    public void SetQuestion(string text)
    {
        questionText.text = text;
    }

    public void SetChoiceTexts(string[] choices)
    {
        //メモ：なぜ最初にnullで実行される？
        if (choices == null) return;

        for (int index = 0; index < choices.Length; index++)
        {
            choiceViews[index].SetText(choices[index]);
        }
    }
}
