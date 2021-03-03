using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UserNameInput : MonoBehaviour
{
    [SerializeField] private InputField inputField;
    [SerializeField] private GameObject nameInputObject;
    [SerializeField] private GameObject bingoObject;
    [SerializeField] private BingoPresenter bingoPresenter;
    [SerializeField] private BingoView bingoView;
    [SerializeField] private BingoModel bingoModel;

    public void OnClickStartButton()
    {
        if (!string.IsNullOrEmpty(inputField.text))
        {
            PlayerPrefs.SetString(PlayerPrefsKeys.UserName, inputField.text);
            PlayerPrefs.Save();
            //SceneManager.LoadScene(SceneNames.Bingo);
            //SceneManager.LoadSceneAsync(SceneNames.Bingo);
            //Debug.Log(inputField.text);
            nameInputObject.SetActive(false);
            bingoObject.SetActive(true);
            bingoPresenter.isPlaying = true;
            bingoView.isPlaying = true;
            bingoModel.DetermineBingoStatus();

        }
    }

}
