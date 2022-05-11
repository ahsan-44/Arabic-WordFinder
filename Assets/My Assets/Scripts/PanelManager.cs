using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanelManager : MonoBehaviour
{
    [SerializeField]
    private GameObject mainMenu, settings;

    void Start()
    {
        OpenMainMenu();
    }

    public void OpenMainMenu()
    {
        mainMenu.SetActive(true);
        settings.SetActive(false);
    }

    public void OpenSettings()
    {
        mainMenu.SetActive(false);
        settings.SetActive(true);
    }

    public void CloseAllPanels()
    {
        mainMenu.SetActive(false);
        settings.SetActive(false);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
