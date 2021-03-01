using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using System;

public class QuestionWindowPresenter : MonoBehaviour
{
    [SerializeField] private QuestionWindowModel questionWindowModel;
    [SerializeField] private QuestionWindowView questionWindowView;

    public void InitQuestionWindowPresenter()
    {
        questionWindowModel.ChangeQuestionEvent.Subscribe(questionWindowView.SetQuestion).AddTo(gameObject);
        questionWindowModel.ChangeQuestionNumEvent.Subscribe(questionWindowView.SetQuestionNumber).AddTo(gameObject);
        questionWindowModel.ChangeChoicesEvent.Subscribe(questionWindowView.SetChoiceTexts).AddTo(gameObject);


    }
}
