using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class DecorativeBox : MonoBehaviour
{
    // Start is called before the first frame update
    private bool _init;
    void Start()
    {
        
    }

    private void FixedUpdate()
    {
        if (Physics.Raycast(transform.position, Vector3.down, 0.5f) && !_init)
        {
            _init = true;
            transform.DOMoveZ(-4, 2).SetRelative().OnComplete(() =>
            {
                DOTween.Kill(transform);
                DOTween.instance.DOKill(this);
                Destroy(this.gameObject);
            });
        }
    }

    // Update is called once per frame
    void Update()
    {
     Debug.DrawRay(transform.position, Vector3.down * 2);   
    }
}
