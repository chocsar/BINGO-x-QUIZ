using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChoiceView : MonoBehaviour
{
    [SerializeField] private Button choiceButton;
    [SerializeField] private Image choiceImage;
    [SerializeField] private Text choiceText;

    public void InitChoiceView()
    {
        //Buttonの入力を監視

    }


    public void SetImage(bool enabled)
    {
        choiceImage.enabled = enabled;
    }

    public void SetText(string text)
    {
        choiceText.text = text;
    }

}
