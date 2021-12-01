using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BgController : MonoBehaviour
{
    private Material _material;

    public float speed;

    void Start()
    {
        _material= GetComponent<Renderer>().material; 
    }

    void Update()
    {
        _material.mainTextureOffset += new Vector2(0, .5f) * speed * Time.deltaTime;   
    }
}
