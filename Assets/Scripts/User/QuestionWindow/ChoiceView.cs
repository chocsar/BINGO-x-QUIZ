using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using System;

public class ChoiceView : MonoBehaviour
{
    public IObservable<int> OnClickEvent => clickSubject;
    private Subject<int> clickSubject = new Subject<int>();

    [SerializeField] private Button choiceButton;
    [SerializeField] private Image choiceImage;
    [SerializeField] private Text choiceText;

    private int index;

    public void InitChoiceView()
    {
        //Buttonの入力を監視
        choiceButton.OnClickAsObservable().Subscribe(_ => OnClick()).AddTo(gameObject);

    }

    public void SetIndex(int index)
    {
        this.index = index;
    }

    public int GetIndex()
    {
        return this.index;
    }


    public void SetImage(bool enabled)
    {
        choiceImage.enabled = enabled;
    }

    public void SetText(string text)
    {
        choiceText.text = text;
    }

    public void OnClick()
    {
        clickSubject.OnNext(index);
    }

}
