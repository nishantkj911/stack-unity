using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class BlockColour : MonoBehaviour
{
    [FormerlySerializedAs("Color")] public Color color = new Color(0f, 0f, 0f, 1f);
    
    Shader _shader;
    // Start is called before the first frame update

    private void Awake()
    {
        // _shader = this.gameObject.GetComponent<Renderer>().material.shader;
        this.gameObject.GetComponent<Renderer>().material.color = color;
    }

    // Update is called once per frame
    void Update()
    {
        this.gameObject.GetComponent<Renderer>().material.color = this.color;
    }
}
