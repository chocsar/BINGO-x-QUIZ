using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InputTextManager : MonoBehaviour
{
    [SerializeField] GameObject mainInputField;
    bool firstInput = true;

    // Start is called before the first frame update
    void Start()
    {
        //mainInputField.onEndEdit.AddListener(delegate { LockInput(mainInputField); });
    }

    private void Update()
    {
        //Debug.Log(textLength);
        //textLength = mainInputField.GetComponent<InputField>().text.Length;
        /*if (mainInputField.GetComponent<InputField>().text.Length == 0 && !firstInput)
        {
            mainInputField.GetComponent<keyboardClass>().CheckOnSelect();
        }*/
    }

    // Update is called once per frame
    public void OnEndText()
    {
        //mainInputField.text = "Enter Your Name...";
        firstInput = false;
        mainInputField.GetComponent<keyboardClass>().CheckOnSelect();
    }

    public void TextLenChecker()
    {
        if (mainInputField.GetComponent<InputField>().text.Length == 0)
        {
            mainInputField.GetComponent<keyboardClass>().CheckOnSelect();
        }
    }
}
