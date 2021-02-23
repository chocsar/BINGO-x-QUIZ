using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimDestroy : MonoBehaviour
{
    [SerializeField] float destroyTime = 5;
    // Start is called before the first frame update
    void Start()
    {
        Invoke("DestroyObj", destroyTime);
    }

    // Update is called once per frame
    void DestroyObj()
    {
        Destroy(this.gameObject);
    }
}
