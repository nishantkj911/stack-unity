using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class BlockColour : MonoBehaviour // TODO: Rectify this whole thing. it's a mess
{
    [FormerlySerializedAs("Color")] public Color color = new Color(0f, 0f, 0f, 1f);
    
    Shader _shader;
    // Start is called before the first frame update

    private Color GetRandomColor()
    {
        return new Color(Random.Range(0, 256), Random.Range(0, 256), Random.Range(0, 256), Random.Range(0f, 1f));
    }

    private void Start()
    {
        // _shader = this.gameObject.GetComponent<Renderer>().material.shader;
        this.gameObject.GetComponent<Renderer>().material.color = GetRandomColor();
    }

    // Update is called once per frame
    void Update()
    {
        this.gameObject.GetComponent<Renderer>().material.color = GetRandomColor();
    }
}
