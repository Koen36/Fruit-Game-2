using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeMonitor : MonoBehaviour
{
    public int Monitor = 0;
    public int ScreenWidth = 1920;
    public int ScreenHeight = 1080;
    public bool FullScreen = true;

    void Start()
    {
        Display.displays[Monitor].Activate();

        Screen.SetResolution(ScreenWidth, ScreenHeight, FullScreen);
    }  
}
