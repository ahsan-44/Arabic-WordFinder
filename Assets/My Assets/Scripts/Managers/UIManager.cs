using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RTLTMPro;

/// <summary>
/// Handles all the script controlled UI in the game.
/// </summary>
public class UIManager : MonoBehaviour
{
    public static UIManager instance;
    [SerializeField]
    private RTLTextMeshPro starsText, coinsText;
    [SerializeField]
    private GameObject mainMenu, classicMode, endlessMode, levelSelection, settings, shop;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        } else {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        //Display the main menu and disable everything else
        mainMenu.SetActive(true);
        classicMode.SetActive(false);
        endlessMode.SetActive(false);
        levelSelection.SetActive(false);
        settings.SetActive(false);
        shop.SetActive(false);
    }

    public void StartClassicGame()
    {
        mainMenu.SetActive(false);
        levelSelection.SetActive(false);
        classicMode.SetActive(true);
    }

    public void StartEndlessGame()
    {
        mainMenu.SetActive(false);
        endlessMode.SetActive(true);
    }

    public void UpdateStarsText()
    {
        starsText.text = PlayerPrefs.GetInt("PlayerStars", 0).ToString();
    }

    public void UpdateCoinsText()
    {
        coinsText.text = PlayerPurchases.instance.CurrentCurrency.ToString();
    }
}
