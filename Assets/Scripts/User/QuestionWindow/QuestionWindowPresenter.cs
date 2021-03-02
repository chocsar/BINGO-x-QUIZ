using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using System;

public class QuestionWindowPresenter : MonoBehaviour
{
    public IObservable<bool> SetAnswerEvent => answerSubject;
    private Subject<bool> answerSubject = new Subject<bool>();
    [SerializeField] private QuestionWindowModel questionWindowModel;
    [SerializeField] private QuestionWindowView questionWindowView;

    public void InitQuestionWindowPresenter()
    {
        //ModelとViewのイベントを監視
        questionWindowModel.ChangeQuestionNumEvent.Subscribe(questionWindowView.SetQuestionNumber).AddTo(gameObject);
        questionWindowModel.ChangeQuestionEvent.Subscribe(questionWindowView.SetQuestion).AddTo(gameObject);
        questionWindowModel.ChangeChoicesEvent.Subscribe(questionWindowView.SetChoiceTexts).AddTo(gameObject);
        questionWindowView.SetAnswerEvent.Subscribe(SetAnswerResult).AddTo(gameObject);

        //初期化処理
        questionWindowView.InitQuestionWindowView();

        //問題のロード
        StartCoroutine(questionWindowModel.GetQuestions());
    }

    public void OpenQuestionWindow(int questionNum)
    {
        //リセット処理
        questionWindowModel.ResetQuestionWindowModel();
        questionWindowView.ResetQuestionWindowView();

        //問題のセット
        questionWindowModel.GetQuestion(questionNum);

        //ウィンドウを表示
        questionWindowView.OpenWindow();
    }

    private void SetAnswerResult(int userAnswer)
    {
        bool isRight = questionWindowModel.CheckAnswer(userAnswer);
        answerSubject.OnNext(isRight);
    }

    public void ShowAnswerResult()
    {
        questionWindowView.ShowAnswerResult(questionWindowModel.IsRight());
    }
}
