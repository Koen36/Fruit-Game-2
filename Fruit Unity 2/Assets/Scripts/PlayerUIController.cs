using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerUIController : MonoBehaviour
{
    public static PlayerUIController Instance;

    public GameObject CanvasPrefab;
    public GameObject PauseMenuPrefab;
    public GameObject UIPrefab;
    GameObject Canvas;
    GameObject PauseMenu;
    GameObject UI;
    bool Pause = false;

    public void Start()
    {
        if (Instance == null)
        {
            Instance = this;
        }

        LoadLevelUI();
        DisableGameplayUI();
    }

    public void LoadLevelUI()
    {
        Canvas = Instantiate(CanvasPrefab);
        PauseMenu = Instantiate(PauseMenuPrefab);
        UI = Instantiate(UIPrefab);

        PauseMenu.transform.SetParent(Canvas.transform);
        UI.transform.SetParent(Canvas.transform);
        PauseMenu.transform.localPosition = Vector3.zero;
        PauseMenu.transform.localScale = Vector3.one;
        UI.transform.localPosition = Vector3.zero;

        DisablePauseUI();
        EnableGameplayUI();
    } //Load necessery UI (Pause Menu and Status things)

    public bool TogglePauseUI() 
    {
        Pause = !Pause;

        PauseMenu.SetActive(Pause);

        if (Pause)
        {
            Cursor.lockState = CursorLockMode.None; //Disable lock cursor inside window
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked; //Lock cursor inside window
        }

        return Pause;
    } //Toggle pause menu

    public void DisablePauseUI()
    {
        PauseMenu.SetActive(false);
        Cursor.lockState = CursorLockMode.None;
    } //Disable pause menu

    public void EnableGameplayUI()
    {
        UI.SetActive(true);
    } //Enable UI

    public void DisableGameplayUI()
    {
        //GEEN IDEE WELK MENU NOG
        UI.SetActive(false);
    } //Disable UI
}