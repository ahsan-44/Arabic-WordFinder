using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace DTT.MiniGame.WordFinder
{
    /// <summary>
    /// Generates the letter grid containing a set of words.
    /// </summary>
    internal class WordFinderGenerator
    {
        #region Variables
        #region Consts
        /// <summary>
        /// The char that represents a cell being empty.
        /// </summary>
        private const char CHAR_EMPTY_CELL = '-';

        /// <summary>
        /// The amount of times we try to fit a word into the grid until we take a new word.
        /// </summary>
        private const int MAX_ATTEMPTS_UNTIL_RETRY = 30;

        /// <summary>
        /// The amount of words we try to fit in until we give up.
        /// </summary>
        private const int MAX_WORDS_UNTIL_GIVEUP = 15;

        /// <summary>
        /// All possible filler letters to be places in the grid.
        /// </summary>
        private const string POSSIBLE_CHARS_STRING = "ابتثجحخدذرزسشصضطظعغفقكلمنهوي";
        #endregion
        #region Editor
        #endregion
        #region Public
        /// <summary>
        /// The words to put in the game.
        /// </summary>
        internal WordFinderWordData[] Words { get; private set; }
        #endregion
        #region private
        /// <summary>
        /// The grid of letters that represent the WordFinder game.
        /// </summary>
        private Dictionary<Vector2Int, char> _grid;

        /// <summary>
        /// Object holding all the current difficulty settings.
        /// </summary>
        private WordFinderLevel _currentLevel;

        /// <summary>
        /// Array of all possible words for this grid generation.
        /// </summary>
        private string[] _possibleWords;

        /// <summary>
        /// The random object to generate random values in the generation.
        /// </summary>
        private System.Random _random;
        #endregion
        #endregion
        #region Methods
        #region Public
        /// <summary>
        /// Generates a WordFinder game based on the level and seed.
        /// </summary>
        /// <param name="seed">The generation seed</param>
        /// <param name="level">The level to play</param>
        /// <param name="onComplete">The callback that passes the generated game successfully</param>
        internal IEnumerator Generate(int seed, WordFinderLevel level, Action<char[,]> onComplete)
        {
            if (level == null)
            {
                Debug.LogError("WordFinder error: No level object found");
                yield break;
            }

            _currentLevel = level;
            _possibleWords = GetWords();

            if (_possibleWords == null)
            {
                Debug.LogError("WordFinder error: No possible words found for the given difficulty");
                yield break;
            }

            if (level.Config.gridSize.x <= 0 || level.Config.gridSize.y <= 0)
            {
                Debug.LogError("WordFinder error: Invalid grid size: " + level.Config.gridSize);
                yield break;
            }

            _random = new System.Random(seed);

            _grid = new Dictionary<Vector2Int, char>(level.Config.gridSize.x * level.Config.gridSize.y);

            List<Vector2> allowedDirections = GetPossibleDirections();

            // Make sure the wordcount in the settings isn't more than the amount of possible words
            CreateWords(level.Config.wordCount <= _possibleWords.Length ? level.Config.wordCount : _possibleWords.Length);

            // Start inserting the words and replacing those that don't fit
            for (int wordIndex = 0; wordIndex < Words.Length; ++wordIndex)
            {
                int tries = 0; // Counter to keep track of how many positions have been tried
                int retries = 0; // Every time we try a new word this goes up
                bool fits = false;
                // Determine the position of the word's letters
                while (!fits)
                {
                    // The index position in the array
                    int gridIndex = _random.Next(_currentLevel.Config.gridSize.x * _currentLevel.Config.gridSize.y);

                    // Randomize the order of directions to go through
                    List<Vector2> directionsInRandomOrder = new List<Vector2>(allowedDirections).
                        OrderBy(x => _random.Next(allowedDirections.Count)).ToList();

                    // Check whether the word fits in any of the directions
                    foreach (Vector2 direction in directionsInRandomOrder)
                    {
                        // Try to insert the word and set fits to true if it succeeds
                        if (fits = InsertWord(gridIndex, Words[wordIndex], direction))
                            break;
                    }

                    if (!fits)
                    {
                        // If we reach a certain amount of tries, we should stop this and try again with a different word
                        tries++;
                        if (tries >= MAX_ATTEMPTS_UNTIL_RETRY && retries < MAX_WORDS_UNTIL_GIVEUP)
                        {
                            retries++;

                            string newWord = CreateWord();
                            if (newWord != null && Words.Where(x => x.Word == newWord.ToLower()).Count() < 1)
                            {
                                WordFinderWordData word = new WordFinderWordData();
                                word.Init(newWord);
                                Words[wordIndex] = word;
                                tries = 0;
                            }
                        }
                        // If we can't fit it, we just give in and stop trying
                        if (retries >= MAX_WORDS_UNTIL_GIVEUP)
                        {
                            for (int i = wordIndex; i < Words.Length; i++)
                                Words[i].Init(string.Empty); // 'Remove' the word we were trying to fit

                            Words = Words.Where(x => !string.IsNullOrWhiteSpace(x.Word)).ToArray();
                            break;
                        }
                    }
                }
                // Every loop we wait a frame to let other actions complete
                yield return null;
            }
            FillEmpty();

            // Put our grid in an array, because our caller doesn't need any more information than what letter and on what position
            char[,] returnData = new char[_currentLevel.Config.gridSize.x, _currentLevel.Config.gridSize.y];
            foreach (KeyValuePair<Vector2Int, char> item in _grid)
                returnData[item.Key.x, item.Key.y] = item.Value;

            onComplete(returnData);
        }

        /// <summary>
        /// Clears everything to make a new generation possible.
        /// </summary>
        internal void Clear()
        {
            Words = null;
            if (_grid != null) _grid.Clear();
            _random = null;
        }
        #endregion
        #region Private
        /// <summary>
        /// Gets all possible directions the words can be placed in.
        /// </summary>
        /// <returns>List of all possible directions</returns>
        private List<Vector2> GetPossibleDirections()
        {
            List<Vector2> allowedDirections = new List<Vector2>();
            if (_currentLevel.Config.horizontal)
            {
                allowedDirections.Add(Vector2.right);
                if (_currentLevel.Config.backwardsHorizontal)
                    allowedDirections.Add(Vector2.left);
            }
            if (_currentLevel.Config.vertical)
            {
                // We don't consider neither south nor north as backwards
                allowedDirections.Add(Vector2.up);
                if (_currentLevel.Config.backwardsVertical)
                    allowedDirections.Add(Vector2.down);
            }
            if (_currentLevel.Config.diagonal)
            {
                allowedDirections.Add(new Vector2(1, 1));
                allowedDirections.Add(new Vector2(1, -1));
                if (_currentLevel.Config.backwardsDiagonal)
                {
                    allowedDirections.Add(new Vector2(-1, 1));
                    allowedDirections.Add(new Vector2(-1, -1));
                }
            }

            return allowedDirections;
        }

        /// <summary>
        /// Creates <paramref name="wordCount"/> amount of words.
        /// </summary>
        /// <param name="wordCount">The amount of words to create</param>
        private void CreateWords(int wordCount)
        {
            Words = new WordFinderWordData[wordCount];
            // A reference to make checking for double values easy
            string[] determinedWords = new string[wordCount];

            int wordsIndex = 0;

            if (Words.Length <= 0)
                return;

            while (Words[wordCount - 1] == null)
            {
                string wordToInsert = CreateWord();

                if (!determinedWords.Contains(wordToInsert))
                {
                    determinedWords[wordsIndex] = wordToInsert;
                    WordFinderWordData word = new WordFinderWordData();
                    word.Init(wordToInsert);
                    Words[wordsIndex] = word;
                    wordsIndex++;
                }
            }
        }

        /// <summary>
        /// Get a single word.
        /// </summary>
        private string CreateWord() => _possibleWords[_random.Next(_possibleWords.Length)].ToLower();

        /// <summary>
        /// Receives the possible words from the game settings.
        /// </summary>
        /// <returns>Array of all possible words</returns>
        private string[] GetWords() => _currentLevel.GetPossibleWords().ToArray();

        /// <summary>
        /// Tries to insert a word and returns whether it succeeded or not.
        /// </summary>
        /// <param name="startingIndex">The index in the grid the word starts on</param>
        /// <param name="word">The word to insert</param>
        /// <param name="direction">The direction the word goes</param>
        /// <returns>Whether the inserting was successful or not</returns>
        private bool InsertWord(int startingIndex, WordFinderWordData word, Vector2 direction)
        {
            // Check if it fits
            for (int letterIndex = 0; letterIndex < word.Word.Length; ++letterIndex)
            {
                char? cellValue = GetCellOnCoordinate(startingIndex, direction * letterIndex);
                //Can't have it on a taken position
                if (cellValue != CHAR_EMPTY_CELL && (cellValue != word.Word[letterIndex] || cellValue == null))
                    return false;
            }
            // Insert the values
            for (int letterIndex = 0; letterIndex < word.Word.Length; ++letterIndex)
            {
                if (letterIndex == 0)
                {
                    // Set a reference to the first letter's position in the grid for the UI
                    word.startingCoordinates = GetCoordinateByIndex(startingIndex);
                }
                else if (letterIndex == word.Word.Length - 1)
                {
                    // Set a reference to the last letter's position in the grid for the UI
                    word.endingCoordinates = new Vector2(word.startingCoordinates.x + (direction.x * letterIndex),
                        word.startingCoordinates.y + (direction.y * letterIndex));
                }
                SetCellOnCoordinate(startingIndex, direction * letterIndex, word.Word[letterIndex]);
            }
            return true;
        }

        /// <summary>
        /// Fills the cells that don't represent letters for the words with random letters.
        /// </summary>
        private void FillEmpty()
        {
            for (int x = 0; x < _currentLevel.Config.gridSize.x; x++)
            {
                for (int y = 0; y < _currentLevel.Config.gridSize.y; y++)
                {
                    Vector2Int index = new Vector2Int(x, y);
                    // Set a random letter to empty cells
                    if (!_grid.ContainsKey(index))
                        _grid.Add(index, POSSIBLE_CHARS_STRING[_random.Next(POSSIBLE_CHARS_STRING.Length - 1)]);
                }

            }
        }

        /// <summary>
        /// Returns the cell's value on a given coordinate.
        /// </summary>
        /// <param name="x">The x coordinate of the cell</param>
        /// <param name="y">The y coordinate of the cell</param>
        /// <returns>Returns null if the cell is out of bounds, '-' if the cell has no value or the value if there is one</returns>
        private char? GetCellOnCoordinate(int x, int y)
        {
            if (x < _currentLevel.Config.gridSize.x && y < _currentLevel.Config.gridSize.y && x >= 0 && y >= 0)
            {
                Vector2Int index = new Vector2Int(x, y);
                if (_grid.ContainsKey(index))
                    return _grid[index];
                else
                    return CHAR_EMPTY_CELL;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Returns the neighbouring cell's value of the coordinates given the direction.
        /// </summary>
        /// <param name="x">The x coordinate of the starting cell</param>
        /// <param name="y">The y coordinate</param>
        /// <param name="direction">The direction to move to</param>
        private char? GetCellOnCoordinate(int x, int y, Vector2 direction) =>
            GetCellOnCoordinate(x + (int)direction.x, y + (int)direction.y);

        /// <summary>
        /// Returns the neighbouring cell value given the direction.
        /// </summary>
        /// <param name="index">The index of the starting cell</param>
        /// <param name="direction">The direction to move to</param>
        private char? GetCellOnCoordinate(int index, Vector2 direction)
        {
            Vector2 coordinates = GetCoordinateByIndex(index);
            return GetCellOnCoordinate((int)coordinates.x, (int)coordinates.y, direction);
        }

        /// <summary>
        /// Sets the value of the cell's value on the given coordinates.
        /// </summary>
        /// <param name="x">The x coordinate of the cell</param>
        /// <param name="y">The y coordinate of the cell</param>
        /// <param name="letter">The letter to set</param>
        private void SetCellOnCoordinate(int x, int y, char letter)
        {
            if (x < _currentLevel.Config.gridSize.x && y < _currentLevel.Config.gridSize.y && x >= 0 && y >= 0)
            {
                Vector2Int index = new Vector2Int(x, y);
                if (_grid.ContainsKey(index))
                    _grid.Remove(index);
                _grid.Add(index, letter);
            }
        }

        /// <summary>
        /// Sets the value on the neighbouring cell given the direction.
        /// </summary>
        /// <param name="x">The x coordinate of the starting cell</param>
        /// <param name="y">The y coordinate of the starting cell</param>
        /// <param name="direction">The direction to move to</param>
        /// <param name="letter">The letter to set</param>
        private void SetCellOnCoordinate(int x, int y, Vector2 direction, char letter) =>
            SetCellOnCoordinate(x + (int)direction.x, y + (int)direction.y, letter);

        /// <summary>
        /// Sets the value on the neighbouring cell given the direction.
        /// </summary>
        /// <param name="index">The index of the starting cell</param>
        /// <param name="direction">The direction to move to</param>
        /// <param name="letter">The letter to set</param>
        private void SetCellOnCoordinate(int index, Vector2 direction, char letter)
        {
            Vector2 coordinates = GetCoordinateByIndex(index);
            SetCellOnCoordinate((int)coordinates.x, (int)coordinates.y, direction, letter);
        }

        /// <summary>
        /// Gets the coordinates of the given grid index.
        /// </summary>
        /// <param name="index">The index in the grid</param>
        /// <returns>The x and y coordinates</returns>
        private Vector2 GetCoordinateByIndex(int index) =>
            new Vector2(index % _currentLevel.Config.gridSize.x, Mathf.FloorToInt(index / _currentLevel.Config.gridSize.y));
        #endregion
        #endregion
    }
}
