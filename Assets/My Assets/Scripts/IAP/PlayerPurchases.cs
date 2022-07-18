using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


public class PlayerPurchases : MonoBehaviour
{
    public static PlayerPurchases instance;

    public int CurrentCurrency;
    [SerializeField]
    private List<string> playerPurchases, defaultPurchases; //Add items owned by default here
    [Tooltip("Add all in-game product details here")]
    private readonly Dictionary<string, int> allPurchasesDict = new Dictionary<string, int>(); //All products and their prices, used by the game's currency system (key = product ID, value = product in game price)
    private readonly string currency1000id = "com.kabakeb.coins1000", currency2500id = "com.kabakeb.coins2500", currency5000id = "com.kabakeb.coins5000", currency10000id = "com.kabakeb.coins10000";

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        } else {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        //Add all products to the dictionary from the IAP manager
        foreach (IAP_Product product in IAPManager.instance.allProducts)
        {
            allPurchasesDict.Add(product.ID, product.price);
        }
    }

    public void GetProductPrice(IAP_Product product) //Get the saved price of the product by product ID
    {
        if (!allPurchasesDict.TryGetValue(product.ID, out int price)) //Product not found
        {
            print("product not found, id: " + product.ID);
            price = 0;
        }
        print(price);
        //return price;
    }

    public void AddToPurchases(string purchaseName)
    {
        playerPurchases.Add(purchaseName);
    }

    public bool CheckIfPurchased(string productID)
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
        //TODO: Show confirmation
        playerPurchases.Clear(); //Delete saved purchases
        playerPurchases.AddRange(defaultPurchases); //Add default purchases back
        ResetCurrency();
    }

    //Add in game Currency
    public void AddCurrency(int amount)
    {
        CurrentCurrency += amount;
        UpdateCurrency();
    }

    public void BuyProductInGame(IAP_Product product) //Buy product with in game currency from this script
    {
        if (product.price <= CurrentCurrency) //If player can afford the cost
        {
            //Buy success
            AddToPurchases(product.ID); //Add to purchased products
            CurrentCurrency -= product.price; //Deduct cost from currency owned
            UpdateCurrency(); //Update currency UI
            if(product.ID.Equals("hint_powerup"))
            {
                GameManager.instance.ShowHint();
            }
            else if(product.ID.Equals("time_powerup"))
            {
                GameManager.instance.AddTime();
            }
            //NotificationsManager.instance.ShowMessage("Purchase success!"); //Show confirmation popup
        } else { //Not enough coins
            NotificationsManager.instance.ShowMessage("Purchase failed! Not enough coins.");
        }
    }

    public void BuyProductReal(IAP_Product product) //Buy product with real money from IAP manager
    {
        IAPManager.instance.BuyProduct(product.ID);
    }

    public void ConfirmCurrencyPurchase(string currencyid)
    {
        if (string.Equals(currencyid, currency1000id))
        {
            AddCurrency(1000);
        } else if (string.Equals(currencyid, currency2500id))
        {
            AddCurrency(2500);
        } else if (string.Equals(currencyid, currency5000id))
        {
            AddCurrency(5000);
        } else if (string.Equals(currencyid, currency10000id))
        {
            AddCurrency(10000);
        }
    }

    private void ResetCurrency()
    {
        CurrentCurrency = 0;
    }

    public void UpdateCurrency()
    {
        UIManager.instance.UpdateCoinsText(); //Update UI
    }

    void OnEnable()
    {
        //Load saved purchases
        JsonUtility.FromJsonOverwrite(PlayerPrefs.GetString("SavedPlayerPurchases"), this);
        //If no saved purchases, add default purchases
        if (playerPurchases.Count == 0)
        {
            playerPurchases.AddRange(defaultPurchases);
        }
    }

    void OnDisable()
    {
        //Save player purchases offline
        string jsonData = JsonUtility.ToJson(this, false);
        PlayerPrefs.SetString("SavedPlayerPurchases", jsonData);
        PlayerPrefs.Save();
    }
}
