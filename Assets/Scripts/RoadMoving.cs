using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoadMoving : MonoBehaviour
{
    public float speed;
    
    private void Update()
    {
        transform.position += Vector3.left * (Time.deltaTime * speed);
    }
}
