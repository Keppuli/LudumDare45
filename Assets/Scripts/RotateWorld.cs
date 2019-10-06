using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateWorld : MonoBehaviour
{

    void Update()
    {
        transform.Rotate(Vector2.up, 1f * Time.deltaTime, Space.World);
    }
}
