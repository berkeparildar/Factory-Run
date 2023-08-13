using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    private float _speed;
    void Start()
    {
        _speed = 3;
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(_speed * Vector3.forward * Time.deltaTime);
    }
}
