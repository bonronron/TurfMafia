using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateDecoy : MonoBehaviour
{
    private Transform symbol;
    private Transform cam;

    private void Start()
    {
        symbol = transform.GetChild(0);
        cam = GameObject.FindGameObjectWithTag("MainCamera").transform;
    }
    private void OnMouseDown()
    {
        transform.parent.Rotate(Vector3.up, 10);
    }
    private void Update()
    {
        symbol.LookAt(cam);
    }
}
