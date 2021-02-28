using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Runtime.InteropServices;
using System;

public class InputTextManager : MonoBehaviour , ISelectHandler
{

    [DllImport("__Internal")]
    private static extern void focusHandleAction(string _name, string _str);

    public void ReceiveInputData(string value)
    {
        gameObject.GetComponent<InputField>().text = value;
    }



    public void TextLenChecker()
    {
        if (gameObject.GetComponent<InputField>().text.Length == 0)
        {
            OnChecker();
        }
    }

    public void OnSelect(BaseEventData eventData)
    {
        #if UNITY_WEBGL
        try
        {
            focusHandleAction(gameObject.name, gameObject.GetComponent<InputField>().text);
            Invoke("TextLenChecker", 0.3f);
        }
        catch (Exception error) { }
        #endif
    }

    void OnChecker()
    {
        #if UNITY_WEBGL
        try
        {
            focusHandleAction(gameObject.name, gameObject.GetComponent<InputField>().text);
            Invoke("TextLenChecker", 0.3f);
        }
        catch (Exception error) { }
        #endif
    }
}
