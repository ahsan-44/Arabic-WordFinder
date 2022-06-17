using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HintPowerup : MonoBehaviour
{
    public static HintPowerup instance;
    private RectTransform rectTransform;
    private Image image;
    private Button button;
    [SerializeField]
    private TextMeshProUGUI ownedCounter;
    [SerializeField]
    private IAP_Product hintPowerup;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        image = GetComponent<Image>();
        image.enabled = false;
    }

    public void ShowHint()
    {
        PlayerPurchases.instance.UsePowerup(hintPowerup);
        AddTimePowerup.UpdateCounter(ownedCounter, hintPowerup);
    }

    public void ShowHintLetter(Vector2 hintLocation) //Sets the hint UI as child to the letter, letter coords same as from WordFinderUI class
    {
        image.enabled = true;
        transform.SetParent(GameObject.Find(string.Format("Letter X{0} Y{1}", hintLocation.x, hintLocation.y)).transform); //Finds the letter in the hierarchy with the same name its assigned to, and sets it as parent.
        rectTransform.localPosition = Vector3.zero;
        rectTransform.localScale = Vector3.one;
        print("Showing hint letter at: " + hintLocation);
    }

    void EnableButton(bool enableState)
    {
        button.interactable = enableState;
    }

    void OnEnable()
    {
        GameManager.enableHint += EnableButton;
        AddTimePowerup.UpdateCounter(ownedCounter, hintPowerup);
    }

    void OnDisable()
    {
        image.enabled = false;
        GameManager.enableHint -= EnableButton;
    }
}