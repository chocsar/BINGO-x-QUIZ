using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Runtime.InteropServices;
using System;

public class keyboardClass : MonoBehaviour, ISelectHandler {

	BaseEventData baseData;
	[SerializeField] InputTextManager manager;

	[DllImport("__Internal")]
	private static extern void focusHandleAction (string _name, string _str);

	public void ReceiveInputData(string value) {
		gameObject.GetComponent<InputField> ().text = value;
	}

	public void OnSelect(BaseEventData data) {
		//Debug.Log(data);
		baseData = data;
		CheckOnSelect();
	}
	public void CheckOnSelect()
    {
		#if UNITY_WEBGL
		try
		{
			focusHandleAction(gameObject.name, gameObject.GetComponent<InputField>().text);
			Debug.Log("call");
			manager.TextLenChecker();
		}
		catch (Exception error)
		{
			Debug.Log("call2");
		}
		#endif
	}
}
