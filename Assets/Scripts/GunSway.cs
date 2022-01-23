using System;
using UnityEngine;

public class GunSway : MonoBehaviour
{

    [Header("Sway Settings")]
    [SerializeField] private float smooth = 8f;
    [SerializeField] private float multiplier = 2f;

    private void Update()
    {
        // get mouse input
        float mouseX = Input.GetAxis("Mouse X") * multiplier;
        float mouseY = Input.GetAxis("Mouse Y") * multiplier;

        // calculate target rotation
        Quaternion rotationX = Quaternion.AngleAxis(-mouseY, Vector3.right);
        Quaternion rotationY = Quaternion.AngleAxis(mouseX, Vector3.up);

        Quaternion targetRotation = rotationX * rotationY;

        // rotate 
        transform.localRotation = Quaternion.Slerp(transform.localRotation, targetRotation, smooth * Time.deltaTime);
    }
}