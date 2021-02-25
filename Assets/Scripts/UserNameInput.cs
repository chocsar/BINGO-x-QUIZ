using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UserNameInput : MonoBehaviour
{
    [SerializeField] private InputField inputField;

    public void OnClickStartButton()
    {
        if (!string.IsNullOrEmpty(inputField.text))
        {
            PlayerPrefs.SetString(PlayerPrefsKeys.UserName, inputField.text);
            PlayerPrefs.Save();
            SceneManager.LoadScene(SceneNames.Bingo);
            //SceneManager.LoadSceneAsync(SceneNames.Bingo);
            //Debug.Log(inputField.text);
        }
    }

}
