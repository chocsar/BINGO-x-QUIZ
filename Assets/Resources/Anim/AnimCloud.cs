using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Anim
{
    public class AnimCloud : MonoBehaviour
    {
        [SerializeField] float nextTime;
        [SerializeField] GameObject CloudSet;

        // Start is called before the first frame update
        void Start()
        {
            GenerateCloud();
        }


        void GenerateCloud()
        {
            Instantiate(CloudSet, this.transform.position, Quaternion.identity, this.gameObject.transform);
            Invoke("GenerateCloud", nextTime);
        }


    }
}
