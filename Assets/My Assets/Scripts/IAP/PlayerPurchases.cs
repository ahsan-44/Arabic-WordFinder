using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


public class PlayerPurchases : MonoBehaviour
{
    public static PlayerPurchases instance;

    public int currentCurrency {get; set;} = 0;
    [SerializeField]
    private List<string> playerPurchases, defaultPurchases; //Add items owned by default here
    [SerializeField]
    private List<string> allPurchaseIDs;
    [SerializeField]
    private int[] allPurchasePrices;
    [SerializeField]
    private Dictionary<string, int> allPurchasesDict = new Dictionary<string, int>();
    public static Action confirmRestoreAction, updateProductStatus;
    public static Action<int> updateCurrencyAction;
    private string currency1000id = "com.ath.coins1000", currency2500id = "com.ath.coins2500", currency5000id = "com.ath.coins5000", currency10000id = "com.ath.coins10000";

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        } else {
            Destroy(gameObject);
        }
    }

    public int getProductPrice(string productID) //Get the saved price of the product by product ID
    {
        if (!allPurchasesDict.TryGetValue(productID, out int price)) //Product not found
        {
            print("product not found, id: " + productID);
            price = 0;
        }
        return price;
    }

    public void addToPurchases(string purchaseName)
    {
        playerPurchases.Add(purchaseName);
    }

    public bool checkIfPurchased(string productID)
    {
        //Return purchase status of an item
        if (playerPurchases.Contains(productID))
        {
            return true;
        } else {
            return false;
        }
    }

    public void RestorePurchases()
    {
        playerPurchases.Clear(); //Delete saved purchases
        playerPurchases.AddRange(defaultPurchases); //Add default purchases back
        resetCurrency();
        PlayerPrefs.SetInt("PlayerWeapon", 0);
        PlayerPrefs.SetInt("Theme", 0);
        PlayerPrefs.SetInt("WeaponSkin", 0);
        confirmRestoreAction();
    }

    //In game Currency
    public void addCurrency(int number)
    {
        currentCurrency += number;
    }

    public void buyProduct(string productID, int cost)
    {
        if (cost <= currentCurrency)
        {
            //Buy success
            addToPurchases(productID);
            currentCurrency -= cost;
            updateCurrency();
            if (updateProductStatus != null)
                {
                    updateProductStatus();
                }
        } else {
            //Fail
            print("Not enough currency");
        }
    }

    //Buying recurring currency (ID not saved offline)
    public void BuyCoins1000()
    {
        IAPManager.instance.BuyProduct(currency1000id);
    }
    public void BuyCoins2500()
    {
        IAPManager.instance.BuyProduct(currency2500id);
    }
    public void BuyCoins5000()
    {
        IAPManager.instance.BuyProduct(currency5000id);
    }
    public void BuyCoins10000()
    {
        IAPManager.instance.BuyProduct(currency10000id);
    }

    public void confirmCurrencyPurchase(string currencyid)
    {
        if (string.Equals(currencyid, currency1000id))
        {
            addCurrency(1000);
        } else if (string.Equals(currencyid, currency2500id))
        {
            addCurrency(2500);
        } else if (string.Equals(currencyid, currency5000id))
        {
            addCurrency(5000);
        } else if (string.Equals(currencyid, currency10000id))
        {
            addCurrency(10000);
        }
    }

    private void resetCurrency()
    {
        currentCurrency = 0;
    }

    public void updateCurrency()
    {
        PlayerPrefs.SetInt("currency", currentCurrency);
        if (updateCurrencyAction != null)
            updateCurrencyAction(currentCurrency);
    }

    void OnEnable()
    {
        updateCurrency();
        updateProductStatus += updateCurrency;
        //Load saved purchases
        JsonUtility.FromJsonOverwrite(PlayerPrefs.GetString("savedPlayerData"), this);
        //If no saved purchases, add default purchases
        if (playerPurchases.Count == 0)
        {
            playerPurchases.AddRange(defaultPurchases);
        }
    }

    void OnDisable()
    {
        updateProductStatus -= updateCurrency;
        //Save player purchases offline
        string jsonData = JsonUtility.ToJson(this, false);
        PlayerPrefs.SetString("savedPlayerData", jsonData);
        PlayerPrefs.Save();
    }
}
