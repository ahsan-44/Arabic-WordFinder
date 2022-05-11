using System;
using UnityEngine;

namespace DTT.MiniGame.WordFinder
{
    ///<summary>
    /// Class holding the data of a word.
    ///</summary>
    internal class WordFinderWordData
    {
        #region Variables
        #region Public
        /// <summary>
        /// The word to find.
        /// </summary>
        internal string Word { get; private set; }

        /// <summary>
        /// The word's first letter's coordinates on the grid.
        /// </summary>
        internal Vector2 startingCoordinates;

        /// <summary>
        /// The word's last letter's coordinates on the grid.
        /// </summary>
        internal Vector2 endingCoordinates;

        /// <summary>
        /// Whether the word has been found by the player.
        /// </summary>
        internal bool Completed { get; private set; }

        /// <summary>
        /// Event fired when the word has been completed.
        /// </summary>
        internal event Action OnComplete;
        #endregion
        #endregion
        #region Methods
        #region Public
        /// <summary>
        /// Initialiser method for a Word.
        /// </summary>
        /// <param name="word">the word we have to find</param>
        internal void Init(string word) => Word = word.ToLower();

        /// <summary>
        /// Method called when the word has been found.
        /// </summary>
        internal void Complete()
        {
            Completed = true;
            OnComplete?.Invoke();
        }
        #endregion
        #endregion
    }
}
