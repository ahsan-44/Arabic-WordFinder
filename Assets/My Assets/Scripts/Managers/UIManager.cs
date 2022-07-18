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

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        } else {
            Destroy(gameObject);
        }
    }

    public void UpdateStarsText()
    {
        //starsText.text = PlayerPrefs.GetInt("PlayerStars", 0).ToString();
    }

    public void UpdateCoinsText()
    {
        coinsText.text = PlayerPurchases.instance.CurrentCurrency.ToString();
    }
}
