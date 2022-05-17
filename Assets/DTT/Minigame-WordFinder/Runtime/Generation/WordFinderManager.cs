using DTT.MinigameBase;
using System;
using System.Collections;
using UnityEngine;

namespace DTT.MiniGame.WordFinder
{
    /// <summary>
    /// Manages the games elements, and handles the start, stop and pause events.
    /// </summary>
    [RequireComponent(typeof(WordFinderLineDrawer), typeof(WordFinderUI))]
    public class WordFinderManager : MonoBehaviour, IMinigame<WordFinderLevel, WordFinderResult>
    {
        /// <summary>
        /// The current level of the word finder.
        /// </summary>
        [SerializeField]
        [Tooltip("The current level of the word finder.")]
        private WordFinderLevel _level;

        /// <summary>
        /// Whether the game will start once the manager is set active.
        /// </summary>
        [SerializeField]
        [Tooltip("Whether the game will start once the manager is set active.")]
        private bool _startOnAwake = false;

        /// <summary>
        /// The script responsible for the UI of the game.
        /// </summary>
        private WordFinderUI _uiScript;

        /// <summary>
        /// The script responsible for the line drawing.
        /// </summary>
        private WordFinderLineDrawer _lineDrawer;

        /// <summary>
        /// The generator that supplies a generated grid with words.
        /// </summary>
        private WordFinderGenerator _generator = new WordFinderGenerator();

        /// <summary>
        /// The event handler that handles all possible events in the game.
        /// </summary>
        private WordFinderEventHandler _eventHandler = new WordFinderEventHandler();

        /// <summary>
        /// The Random object. Will be given a specific seed.
        /// Use this to generate your numbers.
        /// </summary>
        private System.Random _random;

        /// <summary>
        /// Holds the current data that will be passed to the <see cref="Finish"/> event.
        /// </summary>
        private WordFinderResultData _currentResultData;

        /// <summary>
        /// Invoked when the game is over or when <see cref="ForceFinish"/> has been called.
        /// </summary>
        public event Action<WordFinderResult> Finish;
        
        // Invoked when the level has been completed succesfully.
        public event Action<WordFinderResult> NextLevel;

        /// <summary>
        /// Invoked at the start of the game.
        /// </summary>
        public event Action Started;

        /// <summary>
        /// The event handler that handles all possible events in the game.
        /// </summary>
        public WordFinderEventHandler EventHandler { get => _eventHandler; }

        /// <summary>
        /// Whether the game is currently paused.
        /// </summary>
        public bool IsPaused { get; private set; }

        /// <summary>
        /// Whether the game is playing or not.
        /// </summary>
        public bool IsGameActive { get; private set; }

        /// <summary>
        /// Total time the user is playing the game.
        /// Resets when the user restarts and pauses when the user pauses.
        /// </summary>
        public float PlayTime { get; private set; }

        /// <summary>
        /// Whether the game is currently being generated.
        /// You can't start a new level while it's generating.
        /// </summary>
        public bool IsGenerating { get; private set; }

        /// <summary>
        /// Gets and initializes all necessary components.
        /// </summary>
        private void Awake()
        {
            _lineDrawer = GetComponent<WordFinderLineDrawer>();
            _lineDrawer.Init(_eventHandler);
            _uiScript = GetComponent<WordFinderUI>();
            _uiScript.Init(_eventHandler);
            _random = new System.Random();

            if (_startOnAwake)
                StartGame();
        }

        /// <summary>
        /// Keeps track of the playtime of the user.
        /// </summary>
        private void Update()
        {
            if (!IsGameActive || IsPaused)
                return;

            PlayTime -= Time.deltaTime;
            _currentResultData.timeTaken += Time.deltaTime;
            if (PlayTime <= 0)
                ForceFinish();
        }

        /// <summary>
        /// Adds the necessary listeners to events.
        /// </summary>
        private void OnEnable()
        {
            _lineDrawer.CompletedWord += CheckWordCompletion;
            Started += OnStart;
            Finish += OnFinish;
        }

        /// <summary>
        /// Removes listeners from events.
        /// </summary>
        private void OnDisable()
        {
            _lineDrawer.CompletedWord -= CheckWordCompletion;
            Started -= OnStart;
            Finish -= OnFinish;
        }

        /// <summary>
        /// Sets the board as intractable on start.
        /// </summary>
        private void OnStart() => _lineDrawer.interactable = true;

        /// <summary>
        /// Turns off interaction with the board and sets the game inactive.
        /// </summary>
        /// <param name="result">Result of the game.</param>
        private void OnFinish(WordFinderResult result) //When game is finished
        {
            IsPaused = false;
            IsGameActive = false;
            _lineDrawer.interactable = false;
            //Excecute LevelComplete() method in game manager
        }

        /// <summary>
        /// Clear the level and all auto assigned values.
        /// </summary>
        public void ClearGame()
        {
            _generator.Clear();
            _currentResultData.timeTaken = 0; //Current level data not resetting, adding manually what I need to reset (currently time taken only)
            EventHandler.ClearGame();
        }

        /// <summary>
        /// Clears the last level and sets up a new level.
        /// </summary>
        /// <param name="level">New level that needs to be started.</param>
        public void StartGame(WordFinderLevel level)
        {
            if (IsGenerating)
                return;

            _currentResultData = new WordFinderResultData();
            _level = level;

            ClearGame();
            Continue();

            IsGameActive = true;
            //Sets the playtime to the level time.
            PlayTime = level.Config.maxTime;
            StartCoroutine(GenerateWordFinder());
        }

        /// <summary>
        /// Clears the last level and sets up a new level.
        /// Uses the last set <see cref="WordFinderLevel"/>.
        /// </summary>
        public void StartGame() => StartGame(_level);

        /// <summary>
        /// Pauses the game. Disables the line drawer.
        /// </summary>
        public void Pause()
        {
            _lineDrawer.interactable = false;
            IsPaused = true;
        }

        /// <summary>
        /// Continues the game.
        /// </summary>
        public void Continue()
        {
            _lineDrawer.interactable = true;
            IsPaused = false;
        }

        /// <summary>
        /// Immediately finishes the game.
        /// </summary>
        public void ForceFinish() => Finish.Invoke(new WordFinderResult(_currentResultData));

        /// <summary>
        /// Checks whether all words have been found and finishes the game.
        /// </summary>
        private void CheckCompletion()
        {
            for (int i = 0; i < _generator.Words.Length; ++i)
                if (!_generator.Words[i].Completed)
                    return;

            // If all words have been found
            NextLevel.Invoke(new WordFinderResult(_currentResultData)); //go to next level
        }

        //Add to timer
        public void AddTime(float time)
        {
            PlayTime += time;
            if (PlayTime < 0) //Player loses on reaching negative time
            {
                PlayTime = 0;
                ForceFinish();
            }
        }

        /// <summary>
        /// Generates the game and updates the UI when it completes.
        /// </summary>
        private IEnumerator GenerateWordFinder()
        {
            IsGenerating = true;
            WordFinderLetter[,] letters = null;
            yield return _generator.Generate(_random.Next(), _level, (grid) =>
            {
                // Send the grid and the words to the UI
                letters = _uiScript.GenerateTiles(grid, EventHandler, _generator.Words);
                IsPaused = false;
                Started.Invoke();
            });

            yield return new WaitForEndOfFrame();
            if (letters != null && letters.Length > 0)
                _lineDrawer.SetLineSizeReference((RectTransform)letters[0, 0].transform);

            IsGenerating = false;
        }

        /// <summary>
        /// Checks if the given word matches any of the words from the game.
        /// </summary>
        /// <param name="word">The word to check</param>
        private void CheckWordCompletion(string word)
        {
            if (string.IsNullOrWhiteSpace(word))
            {
                EventHandler.WordCompleted(false, word);
                _lineDrawer.FinishLine(false);
                return;
            }

            char[] reversed = word.ToCharArray();
            Array.Reverse(reversed);
            WordFinderWordData foundWord = Array.Find(_generator.Words, x => x.Word.ToUpper() == word.ToUpper() || x.Word.ToUpper() == new string(reversed).ToUpper());

            // Word has successfully been found, check for completion of the game.
            if (foundWord != null && !foundWord.Completed && !foundWord.Equals(default(WordFinderWordData)))
            {
                EventHandler.WordCompleted(true, word);
                _currentResultData.correctSelections++;
                _lineDrawer.FinishLine(true);
                foundWord.Complete();
                CheckCompletion();
                return;
            }
            EventHandler.WordCompleted(false, word);
            _currentResultData.wrongSelections++;
            _lineDrawer.FinishLine(false);
        }

        //Added by me
        public WordFinderWordData[] WordsInLevel() //Return the list of words in the level
        {
            return _generator.Words;
        }
    }
}