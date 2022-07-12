using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddTimePowerup : MonoBehaviour
{
    //Add time already in GameManager
    public void AddTime(int timeAdded)
    {
        GameManager.instance.AddTime();
    }
}
