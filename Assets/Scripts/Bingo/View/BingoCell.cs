using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BingoCell : MonoBehaviour
{
    [SerializeField] private Text cellText;

    private void SetCellText(int number)
    {
        cellText.text = number.ToString();
    }

}
