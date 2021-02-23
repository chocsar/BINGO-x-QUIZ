using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestionWindowView : MonoBehaviour
{
    private int questionNumber;

    private void OnEnable()
    {

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

}
