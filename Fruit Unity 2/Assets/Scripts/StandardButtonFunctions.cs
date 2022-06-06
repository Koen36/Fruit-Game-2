using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class StandardButtonFunctions : MonoBehaviour
{
    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("Game is exiting");
    }
}
