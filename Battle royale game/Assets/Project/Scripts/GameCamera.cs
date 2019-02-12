﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameCamera : MonoBehaviour
{
    [SerializeField] private GameObject target;
    [SerializeField] private Vector3 translationOffset;
    [SerializeField] private Vector3 followOffset;

    private float verticalRotationAngle;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
     
    }

    private void FixedUpdate()
    {
        // Make the camera look at the target.
        float yAngle = target.transform.eulerAngles.y;
        Quaternion rotation = Quaternion.Euler(0, yAngle, 0);

        transform.position = target.transform.position - (rotation * followOffset);
        transform.LookAt(target.transform.position + translationOffset);

        // Make the camera look up or down.
        verticalRotationAngle += Input.GetAxis("Mouse Y");
        transform.RotateAround(target.transform.position, target.transform.right, -verticalRotationAngle);
    }
}