using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BingoChecker : MonoBehaviour
{
    [SerializeField] bool inputNum = false;
    [SerializeField] Text[] bingoNumText;
    [SerializeField] bool[] isChecker;
    bool[,] bingo = new bool[8, 3];
    int status = 0; //0=none 1=reach 2=bingo

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < 9; i++)
        {
            bingoNumText[i].text = "0";
            isChecker[i] = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (inputNum == true)
        {
            for (int i = 0; i < 9; i++)
            {
                if (isChecker[i] == true)
                {
                    bingoNumText[i].text = "1";
                }
            }
            inputNum = false;
            BoolSet();
        }
    }


    void BoolSet()
    {
        bingo[0, 0] = isChecker[0];
        bingo[0, 1] = isChecker[1];
        bingo[0, 2] = isChecker[2];
        bingo[1, 0] = isChecker[3];
        bingo[1, 1] = isChecker[4];
        bingo[1, 2] = isChecker[5];
        bingo[2, 0] = isChecker[6];
        bingo[2, 1] = isChecker[7];
        bingo[2, 2] = isChecker[8];

        bingo[3, 0] = isChecker[0];
        bingo[4, 0] = isChecker[1];
        bingo[5, 0] = isChecker[2];
        bingo[3, 1] = isChecker[3];
        bingo[4, 1] = isChecker[4];
        bingo[5, 1] = isChecker[5];
        bingo[3, 2] = isChecker[6];
        bingo[4, 2] = isChecker[7];
        bingo[5, 2] = isChecker[8];

        bingo[6, 0] = isChecker[0];
        bingo[6, 1] = isChecker[4];
        bingo[6, 2] = isChecker[8];
        bingo[7, 0] = isChecker[2];
        bingo[7, 1] = isChecker[4];
        bingo[7, 2] = isChecker[6];


        //Debug.Log(bingo[0, 0]);

        ClearChecker();

    }






    void ClearChecker()
    {
        for (int i = 0; i < 8; i++)
        {
            int clearNum = 0;
            for (int j = 0; j < 3; j++)
            {
                if (bingo[i, j] == true)
                {
                    clearNum++;
                }
                //Debug.Log(clearNum-1);
                if (clearNum - 1 >= status)
                {
                    status = clearNum - 1;

                }
            }
            if (clearNum == 3)
            {
                break;
            }
        }
        ClearPrinter();
    }





    void ClearPrinter()
    {
        switch (status)
        {
            case 0:
                Debug.Log("none");
                break;
            case 1:
                Debug.Log("reach");
                break;
            case 2:
                Debug.Log("bingo");
                break;
            default:
                Debug.Log("unexpected checker num");
                break;
        }
    }

}
