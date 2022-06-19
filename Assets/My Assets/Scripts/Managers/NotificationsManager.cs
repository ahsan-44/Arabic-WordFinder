using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class NotificationsManager : MonoBehaviour
{
    public static NotificationsManager instance;
    [SerializeField]
    private GameObject messagesHolder, storeHolder, gameoverHolder;
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

    void Start()
    {
        for (int i = 0; i < transform.childCount; i++) //Deactivate all tier 1 children on start
        {
            transform.GetChild(i).gameObject.SetActive(false);
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

    public void ShowStore()
    {
        storeHolder.SetActive(true);
    }

    public void ShowGameOver()
    {
        gameoverHolder.SetActive(true);
    }
}
