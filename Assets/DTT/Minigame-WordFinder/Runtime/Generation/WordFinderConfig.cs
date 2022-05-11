using System;
using System.Collections.Generic;
using UnityEngine;

namespace DTT.MiniGame.WordFinder
{
    /// <summary>
    /// Struct that defines the settings of the word finder game.
    /// </summary>
    [Serializable]
    public struct WordFinderConfig
    {
        [Header("Word Placement Settings")]
        /// <summary>
        /// Wheter words should be instantiated horizontally.
        /// </summary>
        [Tooltip("Whether words should be instantiated horizontally")]
        public bool horizontal;

        /// <summary>
        /// Whether words should be instantiated vertically.
        /// </summary>
        [Tooltip("Whether words should be instantiated vertically")]
        public bool vertical;

        /// <summary>
        /// Whether words should be instantiated diagonally.
        /// </summary>
        [Tooltip("Whether words should be instantiated diagonally")]
        public bool diagonal;

        /// <summary>
        /// Whether words should be instantiated backwards horizontally.
        /// </summary>
        [Tooltip("Whether words should be instantiated backwards horizontally")]
        public bool backwardsHorizontal;

        /// <summary>
        /// Whether words should be instantiated backwards vertically.
        /// </summary>
        [Tooltip("Whether words should be instantiated backwards vertically")]
        public bool backwardsVertical;

        /// <summary>
        /// Whether words should be instantiated backwards diagonally.
        /// </summary>
        [Tooltip("Whether words should be instantiated backwards diagonally")]
        public bool backwardsDiagonal;

        [Header("Grid Settings")]
        /// <summary>
        /// The size of the game's grid.
        /// </summary>
        [Tooltip("The size of the game's grid")]
        public Vector2Int gridSize;

        [Header("Word Settings")]
        /// <summary>
        /// The amount of words to find.
        /// </summary>
        [Tooltip("The amount of words to find")]
        public int wordCount;

        /// <summary>
        /// List containing all possible words.
        /// </summary>
        [Tooltip("List containing all possible words")]
        public List<string> possibleWords;
        [Header("Level Settings")]
        /// <summary>
        /// The countdown timer start value for this level.
        /// </summary>
        [Tooltip("The countdown timer start value for this level")]
        public float maxTime;
    }
}