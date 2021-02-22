using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BingoCellModel : MonoBehaviour
{
    private int number;
    private string cellStatus;

    public void SetNumber(int number)
    {
        this.number = number;
    }

    public int GetNumber()
    {
        return number;
    }
    public void SetCellStatus(string status)
    {
        this.cellStatus = status;
    }

    public string GetCellStatus()
    {
        return cellStatus;
    }

}
