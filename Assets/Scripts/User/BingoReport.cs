using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BingoReport : MonoBehaviour
{
    [SerializeField] private Text userNameText;
    [SerializeField] private float moveTime = 2;

    [SerializeField] private Vector3 start;
    [SerializeField] private Vector3 end;
    private float x = 0;

    private void Update()
    {
        if (x <= 1)
        {
            x += (1 / moveTime) * Time.deltaTime;
            transform.localPosition = GetPostion(x);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private Vector3 GetPostion(float x)
    {
        return start + (end - start) * x;
    }

    public void SetText(string userName)
    {
        userNameText.text = userName;
    }
}
