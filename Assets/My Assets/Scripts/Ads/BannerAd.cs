using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Advertisements;

public class BannerAd : MonoBehaviour
{
    [SerializeField] string _androidAdUnitId = "Banner_Android";
    [SerializeField] string _iOsAdUnitId = "Banner_iOS";
    [SerializeField] string _androidGameId = "4742523";
    [SerializeField] string _iOSGameId = "4742522";
    [SerializeField] bool _testMode = true;
    private string _gameId;
    string _adUnitId;

    IEnumerator Start()
    {
        // Get the Ad Game ID for the current platform:
        _gameId = (Application.platform == RuntimePlatform.IPhonePlayer)
        ? _iOSGameId
        : _androidGameId;
        Advertisement.Initialize(_gameId, _testMode); //initialize ad

        // Get the Ad Unit ID for the current platform:
        _adUnitId = (Application.platform == RuntimePlatform.IPhonePlayer)
        ? _iOsAdUnitId
        : _androidAdUnitId;

        //waits till the ad is ready
        while(!Advertisement.IsReady(_adUnitId)) 
        {
            yield return null;
        }

        //show ad
        Advertisement.Banner.SetPosition(BannerPosition.BOTTOM_CENTER);
        Advertisement.Banner.Show(_adUnitId);
    }
}
