using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BingoSysytem : MonoBehaviour
{
    public int[,] bingoNum = new int[3,3];
    [SerializeField] Text[] bingoNumText;




     

    // Start is called before the first frame update
    void Start()
    {
        NumberLoader();
        for (int i = 0; i < 9; i++)
        {
            bingoNumText[i].text = bingoNum[i/3, i%3].ToString();
        }
    }

    void NumberLoader()
    {
        for(int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                bingoNum[i, j] = Random.RandomRange(0, 101);
                Debug.Log(bingoNum[i, j]);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {


        
    }










}
