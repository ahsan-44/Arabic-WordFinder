using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class AddTimePowerup : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI ownedCounter;
    [SerializeField]
    private IAP_Product timePowerup;

    public void AddTime()
    {
        PlayerPurchases.instance.UsePowerup(timePowerup);
        UpdateCounter(ownedCounter, timePowerup);
    }

    /// <summary>
    /// Updates the owned counter for the target powerup
    /// </summary>
    public static void UpdateCounter(TextMeshProUGUI targetText, IAP_Product product)
    {
        int ownedCount = PlayerPurchases.instance.GetOwnedCount(product); //Get the number from player purchases
        
        if (ownedCount > 9)
        {
            targetText.text = "9+"; //If more than 9, show 9+
        } else {
            targetText.text = ownedCount.ToString();
        }        
    }

    void OnEnable()
    {
        UpdateCounter(ownedCounter, timePowerup);
    }
}
