using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using RTLTMPro;
using TMPro;

public class LevelObject : MonoBehaviour
{
    public int levelNum, stageNum, levelTime, starsRequired, playerStars;
    private int gridSize;
    public int GridSize
    {
        get
        {
            return gridSize;
        }
        set //Min size is 3x3, max is 11x11.
        {
            if (value < 3)
            {
                gridSize = 3;
            } else if (value > 11) {
                gridSize = 11;
            } else {
                gridSize = value;
            }
        }
    }
    private int starsEarned;
    public int StarsEarned
    {
        get { return GetStars(); }
        set { 
                if (value >= 3)
                {
                    SaveStars(3);
                } else {
                    SaveStars(value);
                }
            }
    }
    [SerializeField]
    private Sprite lockedSprite, unlockedSprite, starSprite, emptyStarSprite;
    [SerializeField]
    private RTLTextMeshPro text;
    [SerializeField]
    private GameObject starsHolder;
    [SerializeField]
    private GameObject notEnoughStarsObj;

    public void SetLevel(int level, int stage) //Set values on instantiate
    {
        levelNum = level;
        stageNum = stage;
        text.text = (levelNum + 1).ToString(); //Sets the text to the level number (+1 to remove level 0)
        GridSize = stageNum; //Min size is 3x3, max size is 11x11 (set in constructor)
        starsRequired = 10 * (stageNum - 1); //Number of stars required for each stage (currently 15 * stageNumber)
        levelTime = gridSize * 10; //Seconds to find each word (gridSize = wordCount)
        //Set the UI and check star requirements
        GetStars();
        UpdateStarsUI();
        CheckIfPlayable();
    }

    public void StartGame() //Called when clicked on this button
    {
        GameManager.instance.ChangeGrid(gridSize, gridSize); //Sets the grid size for the manager
        GameManager.instance.StartGameClassic(levelTime, levelNum); //Starts the game
    }

    private void SaveStars(int numOfStars) //Saves stars earned for this level to Prefs
    {
        starsEarned = numOfStars;
        PlayerPrefs.SetInt("Level" + levelNum, starsEarned);
        // print("Level " + levelNum + "Saved Stars: " + starsEarned);
        UpdateStarsUI();
    }

    private int GetStars() //Gets stars earned for this level from Prefs
    {
        starsEarned = PlayerPrefs.GetInt("Level" + levelNum, 0);
        // print("Level " + levelNum + " Stars Got: " + starsEarned);
        return starsEarned;
    }

    private void UpdateStarsUI() //Update level holder to show stars earned
    {
        Image[] starsEarnedUI = starsHolder.GetComponentsInChildren<Image>();
        for (int i = 0; i < starsEarned; i++)
        {
            starsEarnedUI[i].sprite = starSprite;
        }
        for (int i = 0; i < 3 - starsEarned; i++)
        {
            starsEarnedUI[i + starsEarned].sprite = emptyStarSprite;
        }
    }

    private void HideStarsUI()
    {
        foreach (var starUI in starsHolder.GetComponentsInChildren<Image>())
        {
            starUI.color = new Vector4();
        }
    }

    private void ShowStarsUI()
    {
        foreach (var starUI in starsHolder.GetComponentsInChildren<Image>())
        {
            starUI.color = Color.white;
        }
    }

    private void CheckIfPlayable()
    {
        if (starsRequired <= playerStars) //If the player has earned enough stars to unlock this level
        {
            GetComponent<Image>().sprite = unlockedSprite; //Unlocks the level
            GetComponent<Button>().interactable = true;
            text.alpha = 1f;
            ShowStarsUI();
        } else { //If the player has not earned enough stars to unlock this level
            GetComponent<Image>().sprite = lockedSprite; //Locks the level
            text.alpha = 0f;
            GetComponent<Button>().interactable = false;
            HideStarsUI();
        }
    }

    void OnEnable()
    {
        playerStars = PlayerPrefs.GetInt("PlayerStars", 0); //Gets the number of stars the player has earned
        starsEarned = GetStars(); //Gets the number of stars earned on this level
        UpdateStarsUI();
        CheckIfPlayable();
    }

    void OnDisable()
    {
        //Save the number of stars earned on this level
        SaveStars(starsEarned);
    }
}
