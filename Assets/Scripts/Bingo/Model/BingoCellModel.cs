using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BingoCellModel
{
    public int index;
    public int number;
    public string status;

    public void SetIndex(int index)
    {
        this.index = index;
    }

    public int GetIndex()
    {
        return index;
    }
    public void SetNumber(int number)
    {
        this.number = number;
    }

    public int GetNumber()
    {
        return number;
    }

    public void SetStatus(string status)
    {
        this.status = status;
    }

    public string GetStatus()
    {
        return status;
    }

}
