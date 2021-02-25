using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BingoAnimationView : MonoBehaviour
{
    public void OpenWindow()
    {
        gameObject.SetActive(true);
    }

    public void CloseWindow()
    {
        gameObject.SetActive(false);
    }
}
