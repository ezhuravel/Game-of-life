﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public float ZoomSensitivity = 1;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        var camSize = Camera.main.orthographicSize;

        camSize += Input.GetAxis("Mouse ScrollWheel") * ZoomSensitivity;
     
        Camera.main.orthographicSize = camSize;
    }
}
