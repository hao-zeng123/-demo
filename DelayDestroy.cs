using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DelayDestroy : MonoBehaviour
{
    public float delayTime = 0.6f;

    void Start()
    {
        Destroy(gameObject, delayTime);
    }
}
