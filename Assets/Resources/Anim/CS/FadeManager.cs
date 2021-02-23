using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FadeManager : MonoBehaviour
{
    [SerializeField] float fadeRate = 0.01f;
    [SerializeField] bool isFadein;
    [SerializeField] bool isFadeout; //playOnawake
    [SerializeField] bool thisIsImage;
    SpriteRenderer spriteRenderer;
    Image image;
    float alpha = 1;

    // Start is called before the first frame update
    void Start()
    {
        if (thisIsImage)
        {
            image = GetComponent<Image>();
        }
        else
        {
            spriteRenderer = this.gameObject.GetComponent<SpriteRenderer>();
        }
        if (isFadein)alpha = 0;
        if (isFadeout) alpha = 1;

    }
    void Update()
    {
        if (isFadein)
        {
            alpha  += fadeRate;
            if (alpha > 1)isFadein = false;
        }
        if (isFadeout)
        {
            alpha -= fadeRate;
          
            if (alpha > 1) isFadein = false;
        }

        if (thisIsImage)
        {
            var color = image.color;
            color.a = alpha;
            image.color = color;
        }
        else if(spriteRenderer != null)
        {
            var color = spriteRenderer.color;
            color.a = alpha;
            spriteRenderer.color = color;
        }
    }
}