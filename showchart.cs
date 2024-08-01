using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vuforia;
using System;

public class showchart : MonoBehaviour
{
    public GameObject objectToControl; // This is the object that will show/hide
    public GameObject rotatingObject;  // This is the object that will rotate
    private VirtualButtonBehaviour virtualButton;
    private int buttonPressCount = 0;
    private bool wasPressed = false; // This is used to avoid counting multiple times in a single press

    public float rotationSpeed = 50f; // Speed of the rotation in degrees per second

    void Start()
    {
        virtualButton = GetComponentInChildren<VirtualButtonBehaviour>();
        objectToControl.SetActive(false); // Initially hide the object
    }

    void Update()
    {
        // Check the virtual button's press status
        HandleVirtualButton();

        // Rotate the object if it's active
        RotateObject();
    }

    void HandleVirtualButton()
    {
        if (virtualButton.Pressed && !wasPressed) // Check if button is pressed and was not already pressed in the last frame
        {
            wasPressed = true; // Mark that the button is pressed
            buttonPressCount++;

            if (buttonPressCount == 1) // First press
            {
                objectToControl.SetActive(true); // Show the object
            }
            else if (buttonPressCount == 2) // Second press
            {
                objectToControl.SetActive(false); // Hide the object
                buttonPressCount = 0; // Reset the counter
            }
        }
        else if (!virtualButton.Pressed)
        {
            wasPressed = false; // Reset when button is not pressed
        }
    }

    void RotateObject()
    {
        if (rotatingObject.activeInHierarchy) // If the rotating object is active
        {
            rotatingObject.transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime); // Rotate around its Y-axis
        }
    }
}