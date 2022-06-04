using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Purchasing;


public class IAPManager : MonoBehaviour, IStoreListener
{
    public static IAPManager instance;

    private static IStoreController m_StoreController;
    private static IExtensionProvider m_StoreExtensionProvider;

    //List all product IDs for initilization
    public IAP_Product[] allProducts;

    //Initilize all products
    public void InitializePurchasing() //Add and initialize all products
    {
        if (IsInitialized()) { return; }
        var builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());

        foreach (IAP_Product product in allProducts) //Add products to builder with purchasing type (all currently are non consumable)
        {
            if (!product.inGameProduct)
                builder.AddProduct(product.ID, ProductType.NonConsumable);
        }

        UnityPurchasing.Initialize(this, builder);
    }

    private bool IsInitialized()
    {
        return m_StoreController != null && m_StoreExtensionProvider != null;
    }

    public void BuyProduct(string productID) //Start buying the product with ID
    {
        BuyProductID(productID);
    }

    //Purchasing methods
    public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs args)
    {
        if (String.Equals(args.purchasedProduct.definition.id, args.purchasedProduct.definition.id, StringComparison.Ordinal))
        {
            // PlayerPurchases.instance.addToPurchases(args.purchasedProduct.definition.id); //Add to player purchase data
            PlayerPurchases.instance.ConfirmCurrencyPurchase(args.purchasedProduct.definition.id); //Send purchase confirmation
            NotificationsManager.instance.ShowMessage("Purchase success!");
            PlayerPurchases.instance.UpdateCurrency();
            print("confirming purchase id: " + args.purchasedProduct.definition.id);
        } else {
            Debug.Log("Purchase Failed");
            NotificationsManager.instance.ShowMessage("Purchase failed!");
        }
        return PurchaseProcessingResult.Complete;
    }







    //Initialization methods
    private void Awake()
    {
        TestSingleton();
    }

    void Start()
    {
        if (m_StoreController == null) { InitializePurchasing(); }
    }

    private void TestSingleton()
    {
        if (instance != null) { Destroy(gameObject); return; }
        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void BuyProductID(string productId)
    {
        if (IsInitialized())
        {
            Product product = m_StoreController.products.WithID(productId);
            if (product != null && product.availableToPurchase)
            {
                Debug.Log(string.Format("Purchasing product asychronously: '{0}'", product.definition.id));
                m_StoreController.InitiatePurchase(product);
            } else {
                Debug.LogError(productId + " is not found or is not available for purchase");
            }
        } else {
            Debug.Log(productId + " is not initialized.");
        }
    }

    public void RestorePurchases() //Only for IOS
    {
        if (!IsInitialized())
        {
            Debug.Log("RestorePurchases FAIL. Not initialized.");
            return;
        }

        if (Application.platform == RuntimePlatform.IPhonePlayer ||
            Application.platform == RuntimePlatform.OSXPlayer)
        {
            Debug.Log("RestorePurchases started ...");

            var apple = m_StoreExtensionProvider.GetExtension<IAppleExtensions>();
            apple.RestoreTransactions((result) => {
                Debug.Log("RestorePurchases continuing: " + result + ". If no further messages, no purchases available to restore.");
            });
            PlayerPurchases.instance.RestorePurchases();
        }
        else
        {
            Debug.Log("RestorePurchases FAIL. Not supported on this platform. Current = " + Application.platform);
        }
    }

    public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
    {
        Debug.Log("OnInitialized: PASS");
        m_StoreController = controller;
        m_StoreExtensionProvider = extensions;
    }


    public void OnInitializeFailed(InitializationFailureReason error)
    {
        Debug.Log("OnInitializeFailed InitializationFailureReason:" + error);
    }

    public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason)
    {
        Debug.Log(string.Format("OnPurchaseFailed: FAIL. Product: '{0}', PurchaseFailureReason: {1}", product.definition.storeSpecificId, failureReason));
        NotificationsManager.instance.ShowMessage("Purchase failed!");
    }
}