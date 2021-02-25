using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.UI;

public class MovieController : MonoBehaviour
{
    [SerializeField] GameObject B1image;
    [SerializeField] GameObject B2image;
    [SerializeField] GameObject bingoAnim;

    // void Start()
    // {
    //     Invoke("VideoPause", 0.2f);
    //     B2image.SetActive(true);
    //     B1image.SetActive(false);
    // }

    private void OnEnable()
    {
        Invoke("VideoPause", 0.2f);

    }
    void VideoPlay()
    {
        bingoAnim.SetActive(true);
    }

    //void TexOn()
    //{
    //    B1image.SetActive(false);
    //    var color = B1image.GetComponent<Image>().color;
    //    color.a = alpha;
    //    B1image.GetComponent<Image>().color = color;
    //    B2image.GetComponent<Image>().color = color;
    //}
}
