using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flashlight : MonoBehaviour
{
    public GameObject light;

    private bool lastKeyState = false;
    
    // Update is called once per frame
    void Update()
    {
        bool input = false;
        if (input != lastKeyState)
        {
            if (input)
            {
                // The key has been pressed

                light.SetActive(!light.active);
            }

            lastKeyState = input;
        }
    }
}
