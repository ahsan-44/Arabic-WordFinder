using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace DTT.MiniGame.WordFinder
{
    ///<summary>
    /// Handles the UI for the WordFinder minigame.
    ///</summary>
    public class WordFinderUI : MonoBehaviour
    {
        #region Variables 
        #region Editor
        [Header("Prefabs")]
        /// <summary>
        /// Prefab of a single letter.
        /// </summary>
        [SerializeField]
        [Tooltip("Prefab of a single letter")]
        private WordFinderLetter _letterPrefab;

        /// <summary>
        /// Prefab of a single word.
        /// </summary>
        [SerializeField]
        [Tooltip("Prefab of a single word")]
        private WordFinderWord _wordPrefab;

        /// <summary>
        /// The Layout group of the letters playing field.
        /// </summary>
        [SerializeField]
        private GridLayoutGroup _lettersLayoutGroup {get; set;}

        /// <summary>
        /// The Layout group of the words area.
        /// </summary>
        [SerializeField]
        private GridLayoutGroup _wordsLayoutGroup {get; set;}
        #endregion
        #region Public
        /// <summary>
        /// References to all letters active in the playing field.
        /// </summary>
        public WordFinderLetter[,] letterGrid { get; private set; }
        #endregion
        #region Private
        /// <summary>
        /// The event handler that handles all possible events in the game.
        /// </summary>
        private WordFinderEventHandler _eventHandler;

        /// <summary>
        /// A pool containing all used words.
        /// </summary>
        private ObjectPool<WordFinderWord> _wordsPool = new ObjectPool<WordFinderWord>();

        /// <summary>
        /// A pool containing all used letters.
        /// </summary>
        private ObjectPool<WordFinderLetter> _lettersPool = new ObjectPool<WordFinderLetter>();

        /// <summary>
        /// List of all filler objects in the word panel
        /// </summary>
        private List<GameObject> _wordPanelFillers = new List<GameObject>();

        /// <summary>
        /// List of all possible directions the words could be placed in.
        /// </summary>
        private List<Vector2> _possibleDirections = new List<Vector2>()
        {
            Vector2.right,
            Vector2.up,
            Vector2.down,
            Vector2.left,
            new Vector2(1,1),
            new Vector2(-1, 1),
            new Vector2(1,-1),
            new Vector2(-1,-1)
        };
        #endregion
        #endregion
        #region Methods
        #region Unity
        /// <summary>
        /// Adds events from the event handler.
        /// </summary>
        private void OnEnable()
        {
            if (_eventHandler == null)
                return;

            _eventHandler.OnClear += Clear;
            foreach (WordFinderLetter letter in letterGrid)
                AddLetterListeners(letter);
        }

        /// <summary>
        /// Removes events from the event handler.
        /// </summary>
        private void OnDisable()
        {
            if (_eventHandler == null)
                return;

            _eventHandler.OnClear -= Clear;
            RemoveLetterListeners();
        }
        #endregion
        #region Public
        /// <summary>
        /// Sets all the necessary events.
        /// </summary>
        /// <param name="eventHandler"></param>
        internal void Init(WordFinderEventHandler eventHandler)
        {
            _eventHandler = eventHandler;
            eventHandler.OnClear += Clear;
        }

        /// <summary>
        /// Method that calculates the size of all Tiles (Letters & Words).
        /// </summary>
        /// <param name="grid">The generated letter field</param>
        /// <param name="eventHandler">The handler controlling the game events</param>>
        /// <param name="words">The generated words you have to find</param>
        internal WordFinderLetter[,] GenerateTiles(char[,] grid, WordFinderEventHandler eventHandler, WordFinderWordData[] words)
        {
            Vector2Int size = new Vector2Int(grid.GetUpperBound(0) + 1, grid.GetUpperBound(1) + 1);
            float gridSizeWidth = ((RectTransform)_lettersLayoutGroup.transform).rect.width - _lettersLayoutGroup.padding.left - _lettersLayoutGroup.padding.right;
            float gridSizeHeight = ((RectTransform)_lettersLayoutGroup.transform).rect.height - _lettersLayoutGroup.padding.top - _lettersLayoutGroup.padding.bottom;
            float cardSizeX = Mathf.FloorToInt(gridSizeWidth / size.x);
            float cardSizeY = Mathf.FloorToInt(gridSizeHeight / size.y);
            float smallestSize = Mathf.Min(cardSizeX, cardSizeY);

            _lettersLayoutGroup.cellSize = new Vector2(cardSizeX, cardSizeY) - _lettersLayoutGroup.spacing;
            _lettersLayoutGroup.constraint = GridLayoutGroup.Constraint.FixedRowCount;
            _lettersLayoutGroup.startAxis = GridLayoutGroup.Axis.Vertical;
            _lettersLayoutGroup.constraintCount = (int)size.y;

            // Generates the UI for the letters.
            letterGrid = new WordFinderLetter[size.x, size.y];
            for (int x = 0; x < size.x; x++)
            {
                for (int y = 0; y < size.y; y++)
                {
                    int currentX, currentY;
                    currentX = x;
                    currentY = y;

                    WordFinderLetter letterObj = _lettersPool.GetObject(_letterPrefab, _lettersLayoutGroup.transform,
                        (letter) => letter.Init(grid[x, y], new Vector2(x, y), eventHandler));
                    AddLetterListeners(letterObj);

                    string letterName = string.Format("Letter X{0} Y{1}", x, y);
                    letterObj.name = letterName;
                    letterGrid[x, y] = letterObj;
                }
            }

            GenerateWordUI(words);

            // Sets the neighbours of each letter.
            for (int x = 0; x < size.x; x++)
            {
                for (int y = 0; y < size.y; y++)
                {
                    Dictionary<Vector2, WordFinderLetter> neighbours = new Dictionary<Vector2, WordFinderLetter>();
                    foreach (Vector2 direction in _possibleDirections)
                    {
                        if (x + (int)direction.x >= 0 && x + (int)direction.x < size.x
                            && y + (int)direction.y >= 0 && y + (int)direction.y < size.y)
                            neighbours.Add(direction, letterGrid[x + (int)direction.x, y + (int)direction.y]);
                    }

                    letterGrid[x, y].SetNeighbours(neighbours);
                }
            }

            return letterGrid;
        }

        /// <summary>
        /// Calculates the size of every word displayed.
        /// </summary>
        /// <param name="words">The words to display</param>
        internal void GenerateWordUI(WordFinderWordData[] words)
        {
            RectTransform layoutRect = (RectTransform)_wordsLayoutGroup.transform;
            float gridSizeWidth = layoutRect.rect.width - _wordsLayoutGroup.padding.left - _wordsLayoutGroup.padding.right;
            float gridSizeHeight = layoutRect.rect.height - _wordsLayoutGroup.padding.top - _wordsLayoutGroup.padding.bottom;
            float maxHeight = 0;

            // Generates the UI for the words.
            for (int i = 0; i < words.Length; i++)
            {
                WordFinderWord word = _wordsPool.GetObject(_wordPrefab, _wordsLayoutGroup.transform,
                    (_word) => _word.Init(words[i]));

                RectTransform transform = (RectTransform)word.transform;

                if (transform.rect.height > maxHeight)
                    maxHeight = transform.rect.height;

                Vector2 destinationPos = word.Text.rectTransform.anchoredPosition;
            }
            _wordsLayoutGroup.enabled = true;
            _wordsLayoutGroup.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
            _wordsLayoutGroup.constraintCount =
                (int)Math.Ceiling((maxHeight + _wordsLayoutGroup.spacing.y) * _wordsPool.ActiveObjects.Count / gridSizeHeight);
            _wordsLayoutGroup.cellSize =
                new Vector2((gridSizeWidth / _wordsLayoutGroup.constraintCount) - _wordsLayoutGroup.spacing.x, maxHeight);

            // Calculates how many filler spaces are needed to have an even amount of cells in each column
            int wordsPerColumn = (int)Math.Floor(gridSizeHeight / (maxHeight + _wordsLayoutGroup.spacing.y));
            int neededSpaces = wordsPerColumn - (words.Length % wordsPerColumn);
            if (neededSpaces == wordsPerColumn)
                neededSpaces = 0;

            // Fills space in the grid so the height of the grid doesn't change
            for (int i = 0; i < neededSpaces; i++)
            {
                GameObject space = new GameObject();
                space.AddComponent<RectTransform>();
                space.transform.SetParent(_wordsLayoutGroup.transform);
                space.name = "fillerSpace";
                _wordPanelFillers.Add(space);
            }

            // Waits a frame until the auto font size is updated
            StartCoroutine(WaitForFrame(() =>
            {
                float minSize = float.MaxValue;
                if (_wordsPool.ActiveObjects.Count > 0)
                    minSize = _wordsPool.ActiveObjects.Min(word => word.Text.fontSize);

                for (int w = 0; w < _wordsPool.ActiveObjects.Count; w++)
                {
                    _wordsPool.ActiveObjects[w].Text.enableAutoSizing = false;
                    _wordsPool.ActiveObjects[w].Text.fontSize = minSize;
                    RectTransform transform = (RectTransform)_wordsPool.ActiveObjects[w].transform;
                    _wordsPool.ActiveObjects[w].Text.ForceMeshUpdate();
                }
            }));
        }

        /// <summary>
        /// Clears the game's words and letters.
        /// </summary>
        internal void Clear()
        {
            RemoveLetterListeners();
            for (int i = 0; i < _wordPanelFillers.Count; ++i)
                Destroy(_wordPanelFillers[i]);

            _wordPanelFillers.Clear();

            _wordsPool.ReturnAllObjects();
            _lettersPool.ReturnAllObjects(transform);
        }
        #endregion
        #region Private
        /// <summary>
        /// Unsibscribes the select and deselect listeners from the letters.
        /// </summary>
        private void RemoveLetterListeners()
        {
            if (letterGrid == null)
                return;

            foreach (WordFinderLetter letter in letterGrid)
            {
                if (letter == null)
                    continue;

                letter.Selected -= _eventHandler.SelectLetter;
                letter.Confirmed -= _eventHandler.DeselectLetter;
            }
        }

        /// <summary>
        /// Adds select listeners to the letter.
        /// </summary>
        /// <param name="letter">Letter from the grid</param>
        private void AddLetterListeners(WordFinderLetter letter)
        {
            letter.Selected += _eventHandler.SelectLetter;
            letter.Confirmed += _eventHandler.DeselectLetter;
        }

        /// <summary>
        /// Waits a frame until executing onWaited.
        /// </summary>
        /// <param name="onWaited">Called when 1 frame has passed</param>
        private IEnumerator WaitForFrame(Action onWaited)
        {
            yield return null;
            onWaited.Invoke();
        }
        #endregion
        #endregion
    }
}
