using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationContainer : MonoBehaviour
{
    static public AnimationContainer i;
    public GameObject[] animations;
    // Start is called before the first frame update
    void Start()
    {
        i = this;
    }
}
