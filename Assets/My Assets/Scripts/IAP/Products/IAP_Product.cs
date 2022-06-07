using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A scriptable object class contains all the NON CONSUMABLE products of the game. (Can be expanded on later if we need to add other types of products)
/// </summary>
[CreateAssetMenu(fileName = "New Product", menuName = "IAP Product")]
public class IAP_Product : ScriptableObject
{
    /// <summary>
    /// The Product ID used in the app stores and IAP manager.
    /// </summary>
    public string ID;
    /// <summary>
    /// A Product name shown to the user.
    /// </summary>
    public string productName;
    /// <summary>
    /// The in game price of the product.
    /// </summary>
    public int price;
    /// <summary>
    /// Is this product bought in game or with real money?
    /// </summary>
    public bool inGameProduct;
}
