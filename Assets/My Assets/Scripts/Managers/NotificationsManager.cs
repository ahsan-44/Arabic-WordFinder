using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class NotificationsManager : MonoBehaviour
{
    public static NotificationsManager instance;
    [SerializeField]
    private GameObject messagesHolder, storePopupHolder;
    [SerializeField]
    private TextMeshProUGUI msgText;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        } else {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Sets and shows a popup message to the user
    /// </summary>
    public void ShowMessage(string msg)
    {
        msgText.text = msg;
        messagesHolder.SetActive(true);
    }

    public void ShowStorePopup(string msg)
    {
        msgText.text = msg;
        storePopupHolder.SetActive(true);
    }
}
