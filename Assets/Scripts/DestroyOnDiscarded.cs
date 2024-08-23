using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyOnDiscarded : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        if (gameObject.transform.position.y < -10)
        {
            Destroy(gameObject);
        }   
    }
}
