using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Advertisements;

public class AdManager : MonoBehaviour, IUnityAdsInitializationListener
{
    public static AdManager instance;
    [SerializeField] string _androidGameId;
    [SerializeField] string _iOSGameId;
    [SerializeField] bool _testMode = true;
    private string _gameId;
    public bool loadedInterAd = false;
    private bool isInitialized = false;
 
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        } else {
            Destroy(this);
        }
    }

    void Start()
    {
        InvokeRepeating("InitializeAds", 1f, 1f);
    }

    public void InitializeAds()
    {
        if (isInitialized == false)
        {
            _gameId = (Application.platform == RuntimePlatform.IPhonePlayer)
            ? _iOSGameId
            : _androidGameId;
            Advertisement.Initialize(_gameId, _testMode, this);
            Debug.Log("Attempting to initialize Unity Ads.");
        } else {
            isInitialized = true;
            CancelInvoke();
        }
    }
 
    public void OnInitializationComplete()
    {
        Debug.Log("Unity Ads initialization complete.");
        CancelInvoke("InitializeAds");
        //Load interstatial ad after init
        InterstatialAd.instance.LoadAd();
    }
 
    public void OnInitializationFailed(UnityAdsInitializationError error, string message)
    {
        Debug.Log($"Unity Ads Initialization Failed: {error.ToString()} - {message}");
    }

    public void ShowInterstatialAd()
    {
        if (loadedInterAd)
            InterstatialAd.instance.ShowAd();
        else
            Debug.Log("Interstatial Ad not loaded");
    }
}
