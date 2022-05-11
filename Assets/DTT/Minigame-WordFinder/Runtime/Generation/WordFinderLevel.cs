using System.Collections.Generic;
using UnityEngine;

namespace DTT.MiniGame.WordFinder
{
    /// <summary>
    /// Defines the difficulty settings of the word finder game.
    /// </summary>
    [CreateAssetMenu(fileName = "WordFinderLevel", menuName = "DTT/Mini Game/WordFinder/Level")]
    public class WordFinderLevel : ScriptableObject
    {
        /// <summary>
        /// All settings of the word finder game.
        /// </summary>
        [SerializeField]
        [Tooltip("All settings of the word finder game")]
        private WordFinderConfig _settings = new WordFinderConfig
        { horizontal = true, gridSize = new Vector2Int(3, 3), wordCount = 3 };

        /// <summary>
        /// All settings of the word finder game.
        /// </summary>
        public WordFinderConfig Config => _settings;

        /// <summary>
        /// Gets all words that can fit in the grid size.
        /// </summary>
        /// <returns>List of possible words</returns>
        public List<string> GetPossibleWords()
        {
            List<string> fittingWords = new List<string>(_settings.possibleWords);
            fittingWords.RemoveAll((word) =>
            {
                if (word.Length > _settings.gridSize.x && word.Length > _settings.gridSize.y)
                {
                    Debug.LogWarning("WordFinder: The word " + word + " is too long to fit in the grid.");
                    return true;
                }
                return false;
            });
            return fittingWords;
        }

        //Change settings from an external config.
        public void ChangeLevelSettings(WordFinderConfig config)
        {
            _settings = config;
            SetSettings(config);
        }

        /// <summary>
        /// Sets the settings to a new configuration.
        /// </summary>
        /// <param name="config">New settings</param>
        public void SetSettings(WordFinderConfig config) => _settings = config;
    }
}