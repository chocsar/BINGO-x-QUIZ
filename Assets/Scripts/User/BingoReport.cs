using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BingoReport : MonoBehaviour
{
    [SerializeField] private Text statusText;
    [SerializeField] private Text userNameText;
    [SerializeField] private float moveTime = 2;

    private Vector3 start = new Vector3(-600, -600, 0);
    private Vector3 end = new Vector3(600, -600, 0);
    private float x = 0;

    private void Start()
    {
        //上下にランダム性を持たせる
        float random = Random.Range(0f, 150f);
        start.y -= random;
        end.y -= random;
    }

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

    public void SetUserName(string userName)
    {
        userNameText.text = userName;
    }

    public void SetStatus(string status)
    {
        if (status == UserBingoStatus.Bingo)
        {
            statusText.text = "BINGO!";
        }
        else if (status == UserBingoStatus.Reach)
        {
            statusText.text = "REACH!";
        }
    }
}
