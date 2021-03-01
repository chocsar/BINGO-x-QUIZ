using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ReachCellView : MonoBehaviour
{
    [SerializeField] private Image cellImage;

    public void SetCellImage(bool isReach)
    {
        if (isReach)
        {
            cellImage.sprite = Resources.Load<Sprite>(ResourcesPath.ReachOn);
        }
        else
        {
            cellImage.sprite = Resources.Load<Sprite>(ResourcesPath.ReachOff);
        }
    }
}
