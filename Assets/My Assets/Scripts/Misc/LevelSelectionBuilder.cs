using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelSelectionBuilder : MonoBehaviour
{
    [SerializeField]
    private GameObject levelSelectionPrefab, levelsHolder;
    [SerializeField]
    private int numberOfStages, numberOfLevels;

    [SerializeField]
    // Each level is repeated 5 times.
    // Levels 0-3: difficultyLevel: 1; grid size 3x3, 4x4, 5x5; timerBonus: 3 seconds.
    // LeveLs 4-7: difficultyLevel: 2; grid size 6x6, 7x7, 8x8; timerBonus: 5 seconds.
    // Levels 8-10: difficultyLevel: 3; grid size 9x9, 10x10, 11x11; timerBonus: 7 seconds.

    void Start()
    {
        int levelNum = 0;
        for (int i = 0; i < numberOfStages; i++)
        {
            for (int j = 0; j < numberOfLevels; j++)
            {
                GameObject obj = Instantiate(levelSelectionPrefab, levelsHolder.transform);
                obj.GetComponent<LevelObject>().SetLevel(levelNum, i + 1); //i + 1 to start from stage 1
                levelNum++;
            }
        }
    }
}
