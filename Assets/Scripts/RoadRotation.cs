using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoadRotation : MonoBehaviour
{
    private void Start()
    {
        Input.gyro.enabled = true;
    }

    private void Update()
    {
        transform.eulerAngles = new Vector3(0.0f, 0.0f, transform.eulerAngles.z - Input.gyro.rotationRateUnbiased.z * Time.deltaTime * Mathf.Rad2Deg);
    }
}
