using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HintPowerup : MonoBehaviour
{
    public static HintPowerup instance;
    private RectTransform rectTransform;
    [SerializeField]
    private Image hintEffect;
    private Button button;
    [SerializeField]
    private TextMeshProUGUI ownedCounter;
    [SerializeField]
    private IAP_Product hintPowerup;

    void Awake()
    {
        instance = this;
        rectTransform = hintEffect.GetComponent<RectTransform>();
        hintEffect.enabled = false;
        button = GetComponent<Button>();
    }

    public void ShowHint()
    {
        PlayerPurchases.instance.UsePowerup(hintPowerup); //Use hint powerup
        AddTimePowerup.UpdateCounter(ownedCounter, hintPowerup); //Update powerup counter
    }

    public void ResetHint()
    {
        hintEffect.enabled = false;
    }

    public void ShowHintLetter(Vector2 hintLocation) //Sets the hint UI as child to the letter, letter coords same as from WordFinderUI class
    {
        hintEffect.enabled = true;
        hintEffect.transform.SetParent(GameObject.Find(string.Format("Letter X{0} Y{1}", hintLocation.x, hintLocation.y)).transform); //Finds the letter in the hierarchy with the same name its assigned to, and sets it as parent.
        rectTransform.localPosition = Vector3.zero;
        rectTransform.localScale = Vector3.one;
    }

    void EnableButton(bool enableState)
    {
        button.interactable = enableState;
        if(enableState) //If hint button is available, hide the effect
        {
            ResetHint();
        }
    }

    void UpdateSelf()
    {
        AddTimePowerup.UpdateCounter(ownedCounter, hintPowerup);
    }

    void OnEnable()
    {
        UpdateSelf();
        GameManager.enableHint += EnableButton;
        PlayerPurchases.UpdateUI += UpdateSelf; //Called when a powerup is bought
    }

    void OnDisable()
    {
        hintEffect.enabled = false;
        GameManager.enableHint -= EnableButton;
    }
}