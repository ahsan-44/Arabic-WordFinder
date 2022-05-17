using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HintPowerup : MonoBehaviour
{
    public static HintPowerup instance;
    private RectTransform rectTransform;
    private Image image;
    private Button button;

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

    public void ShowHintLetter(Vector2 hintLocation)
    {
        image.enabled = true;
        transform.SetParent(GameObject.Find(string.Format("Letter X{0} Y{1}", hintLocation.x, hintLocation.y)).transform); //Finds the letter in the hierarchy with the same name its assigned to, and sets it as parent.
        rectTransform.localPosition = Vector3.zero;
        rectTransform.localScale = Vector3.one;
        print("Showing hint letter at: " + hintLocation);
    }

    public void ShowHintWord(Vector2 startPos, Vector2 endPos)
    {

    }

    void EnableButton(bool enableState)
    {
        button.interactable = enableState;
    }

    void OnEnable()
    {
        GameManager.enableHint += EnableButton;
    }

    void OnDisable()
    {
        image.enabled = false;
        GameManager.enableHint -= EnableButton;
    }

    //For codility
    public int solution(int[] A, int[] X) 
    {
        //A = gas in each city
        //X = distance of each city
        //Y = gas cost of each city
        //Z = net gas gain
        int citiesVisited = 0;
        int gas = 0;

        citiesVisited ++;
        if (gas <= 0 || citiesVisited == A.Length)
        {
            return citiesVisited;
        }
        
        return 0;
    }

    // public int RecursiveSolution(int[] A, int[] X, int citiesVisited = 0, int startingGas = 0, int city = 0)
    // {
    //     //A = gas in each city
    //     //X = distance of each city
    //     int citiesVisited = 0;
    //     int gas = 0;
    //     /*
    //     1: Compare distances between other cities, and their net gas
    //     2: Choose city with highest net gas, if more than one, call function over those cities.
    //     3: Repeat until gas = 0, or all cities have been reached.
    //     4: return number of trips
    //     */

    //     if (citiesVisited > RecursiveSolution())
    //     {
    //         citiesVisited = RecursiveSolution;
    //     }

    //     //Loop exit condition
    //     if (gas <= 0 || citiesVisited == A.Length)
    //     {
    //         //Exit
    //     }

    //     return citiesVisited;
    // }

    /* 
    Write a program in C# Sharp to print numbers from n to 1 using recursion. Go to the editor
    Test Data :
    How many numbers to print : 10
    Expected Output :
    10 9 8 7 6 5 4 3 2 1
    */
    void sol(int number)
    {
        print(number);
        if (number > 1)
        {
            sol(number - 1);
        }
    }
}