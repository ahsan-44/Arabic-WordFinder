using DTT.Utils.Extensions;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace DTT.MiniGame.WordFinder
{
    ///<summary>
    /// This class handles all line drawing done in the WordFinder game.
    ///</summary>
    public class WordFinderLineDrawer : MonoBehaviour
    {
        #region Variables
        #region Editor
        /// <summary>
        /// Prefab used as the word selector in the game.
        /// </summary>
        [SerializeField]
        [Tooltip("Prefab used as the word selector in the game.")]
        private WordFinderLine _linePrefab;
        #endregion
        #region Private
        /// <summary>
        /// The event handler that handles all possible events in the game.
        /// </summary>
        private WordFinderEventHandler _eventHandler;

        /// <summary>
        /// The lineRenderer reference.
        /// </summary>
        private WordFinderLine _currentLineRenderer;

        /// <summary>
        /// Save the gridPosition and world position of all points in our lineRenderer.
        /// </summary>
        private Dictionary<Vector2, Vector3> _linePositions = new Dictionary<Vector2, Vector3>();

        /// <summary>
        /// The current direction we are swiping.
        /// </summary>
        private Vector2 _currentDirection;

        /// <summary>
        /// The <see cref="WordFinderLetter"/> that we started our swipe on.
        /// </summary>
        private WordFinderLetter _startPos;

        /// <summary>
        /// The position from where to start line drawing animation.
        /// </summary>
        private Vector3 _animationPos;

        /// <summary>
        /// A list of all selected <see cref="WordFinderLetter"/> representing our Word.
        /// </summary>
        private List<WordFinderLetter> _selectedWord = new List<WordFinderLetter>();

        /// <summary>
        /// The reference to the current size of ui.
        /// </summary>
        private RectTransform _textSizeReference;

        /// <summary>
        /// Object pool containing all available and active lines.
        /// </summary>
        private ObjectPool<WordFinderLine> _linesPool = new ObjectPool<WordFinderLine>();
        #endregion
        #region Public
        /// <summary>
        /// Delegate for the event fired when user has released their swipe to check for swiped word.
        /// </summary>
        /// <param name="word">The word that was completed</param>
        internal delegate void WordCompleted(string word);

        /// <summary>
        /// Event fired when user has released their swipe to check for swiped word.
        /// </summary>
        internal event WordCompleted CompletedWord;

        /// <summary>
        /// Whether the LineDrawer is currently interactable.
        /// </summary>
        public bool interactable = false;
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

            _eventHandler.OnSelectLetter += DrawLine;
            _eventHandler.OnDeselectLetter += Completed;
            _eventHandler.OnClear += Clear;
        }

        /// <summary>
        /// Removes events from the event handler.
        /// </summary>
        private void OnDisable()
        {
            if (_eventHandler == null)
                return;

            _eventHandler.OnSelectLetter -= DrawLine;
            _eventHandler.OnDeselectLetter -= Completed;
            _eventHandler.OnClear -= Clear;
        }
        #endregion
        #region Public
        /// <summary>
        /// Initialises the events.
        /// </summary>
        /// <param name="eventHandler"></param>
        internal void Init(WordFinderEventHandler eventHandler)
        {
            _eventHandler = eventHandler;
            eventHandler.OnSelectLetter += DrawLine;
            eventHandler.OnDeselectLetter += Completed;
            eventHandler.OnClear += Clear;
        }

        /// <summary>
        /// Method to set the text size reference of the line.
        /// </summary>
        /// <param name="letter">The current letter used in the letters area</param>
        internal void SetLineSizeReference(RectTransform letter) => _textSizeReference = letter;

        /// <summary>
        /// Returns the currently selected word we swiped
        /// </summary>
        /// <returns>The currently selected word</returns>
        internal string GetWord()
        {
            StringBuilder sb = new StringBuilder();
            foreach (WordFinderLetter letter in _selectedWord)
                sb.Append(letter.Data);

            return sb.ToString();
        }

        /// <summary>
        /// Draw a line to given WordFinderLetter.
        /// </summary>
        /// <param name="target">The WordFinderLetter we swiped</param>
        internal void DrawLine(WordFinderLetter target)
        {
            if (!interactable)
                return;

            // Creates new line if there is no displayed line yet.
            if (_currentLineRenderer == null)
            {
                _currentLineRenderer = _linesPool.GetObject(_linePrefab, transform);
                _currentLineRenderer.LineRenderer.positionCount = 2;
                _currentLineRenderer.LineRenderer.SetPosition(0, Vector3.zero);
                _currentLineRenderer.LineRenderer.SetPosition(1, Vector3.zero);
            }

            // Gets the width of the letter in world space.
            RectTransform textRext = (RectTransform)_textSizeReference.transform;
            Rect worldRect = textRext.GetWorldRect();
            float lineSize = worldRect.width < worldRect.height ?
                worldRect.height - 0.05f : worldRect.width - 0.05f;
            SetLineSize(lineSize);

            Vector2 gridPos = target.GridPosition;

            if (!_linePositions.ContainsKey(gridPos))
            {
                if (_linePositions.Count > 0)
                {
                    Vector2 directionFromStart = gridPos - _startPos.GridPosition;
                    Vector2 lineDirection = gridPos - _linePositions.Last().Key;

                    if (_currentDirection.Equals(Vector2.zero) || _linePositions.Count >= 1)
                    {

                        float absoluteX = (float)Mathf.Abs(_startPos.GridPosition.x - gridPos.x);
                        float absoluteY = (float)Mathf.Abs(_startPos.GridPosition.y - gridPos.y);

                        if (absoluteX != 0 && absoluteX != absoluteY && absoluteY != 0)
                            return;

                        // New word selected, so call onSelected.
                        target.OnSelected();

                        if (new Vector2(Mathf.Clamp(_currentDirection.x, -1, 1), Mathf.Clamp(_currentDirection.y, -1, 1)) != -lineDirection)
                        {
                            // If we changed direction reset the line.
                            _currentDirection = -directionFromStart;
                            if (_linePositions.Count >= 2)
                            {
                                DrawMissingLine(target, gridPos);
                                return;
                            }
                        }

                        // If we swiped out of reachable distance draw all in between.
                        if (Mathf.Abs(lineDirection.x) > 1 || Mathf.Abs(lineDirection.y) > 1)
                        {
                            if (!_startPos)
                                return;

                            int gapX = (int)(_startPos.GridPosition.x - gridPos.x);
                            int gapY = (int)(_startPos.GridPosition.y - gridPos.y);

                            WordFinderLetter currentLetter = _startPos;

                            _linePositions.Clear();
                            _currentLineRenderer.LineRenderer.positionCount = 0;
                            _selectedWord.Clear();

                            ReDrawLine(_currentLineRenderer.LineRenderer);

                            int totalSteps = Mathf.Max(Mathf.Abs(gapX), Mathf.Abs(gapY));
                            int directionX = (int)_currentDirection.x == 0 ? 0 : ((int)_currentDirection.x / totalSteps);
                            int directionY = (int)_currentDirection.y == 0 ? 0 : ((int)_currentDirection.y / totalSteps);
                            Vector2 direction = new Vector2(-directionY, -directionX);

                            if (direction.Equals(Vector2.zero))
                                return;

                            for (int i = 0; i < totalSteps; i++)
                            {
                                AddPosition(currentLetter);
                                if (currentLetter.Neighbours.ContainsKey(direction))
                                    currentLetter = currentLetter.Neighbours[direction];
                            }

                            AddPosition(target);
                            return;
                        }
                    }
                    else if (!_currentDirection.Equals(_linePositions.Last().Key - gridPos))
                        return;
                }
                else
                    _startPos = target;

                AddPosition(target);
            }
            else
            {
                // If we shortend our line dont draw further points.
                DrawMissingLine(target, gridPos);
            }
        }

        /// <summary>
        /// Method when we have completed a word (released input).
        /// </summary>
        internal void Completed()
        {
            if (!interactable || _selectedWord.Count <= 0)
                return;

            CompletedWord?.Invoke(GetWord());
        }

        /// <summary>
        /// Method called when we have a result for our previously made line.
        /// </summary>
        /// <param name="success">Whether this word was actually something we had to find.</param>
        internal void FinishLine(bool success)
        {
            ReDrawLine(_currentLineRenderer.LineRenderer);

            if (success)
            {
                _currentLineRenderer.OnSuccess();

                for (int i = 0; i < _selectedWord.Count; i++)
                    _selectedWord[i].OnSuccess();
            }
            else
            {
                for (int i = 0; i < _selectedWord.Count; i++)
                    _selectedWord[i].OnFailure();

                StartCoroutine(OnLineFailure(_currentLineRenderer));
            }

            // Reset the values of the line drawer
            _currentLineRenderer = null;
            _linePositions.Clear();
            _currentDirection = Vector2.zero;
            _selectedWord.Clear();
        }

        /// <summary>
        /// Clears all lines.
        /// </summary>
        internal void Clear()
        {
            interactable = false;
            ClearLinePositions();
            _linesPool.ReturnAllObjects();
        }

        /// <summary>
        /// Clears the line positions for redrawing.
        /// </summary>
        internal void ClearLinePositions() => _linePositions.Clear();
        #endregion
        #region Private
        /// <summary>
        /// Called when the game started.
        /// </summary>
        private void OnStart() => interactable = true;

        /// <summary>
        /// Called when the game finished.
        /// </summary>
        private void OnFinish() => interactable = false;

        /// <summary>
        /// Method that handles the distance between startPosition and given target Letter for linedrawing.
        /// </summary>
        /// <param name="target">The letter we swiped</param>
        /// <param name="gridPos">The grid position of the given letter</param>
        private void DrawMissingLine(WordFinderLetter target, Vector2 gridPos)
        {
            int gapX = (int)(_startPos.GridPosition.x - gridPos.x);
            int gapY = (int)(_startPos.GridPosition.y - gridPos.y);
            int totalSteps = Mathf.Max(Mathf.Abs(gapX), Mathf.Abs(gapY));
            if (totalSteps < 1)
                return;

            _linePositions.Clear();
            _currentLineRenderer.LineRenderer.positionCount = 2;
            _selectedWord.Clear();

            ReDrawLine(_currentLineRenderer.LineRenderer);

            Vector2 direction = new Vector2(Mathf.Clamp(-_currentDirection.y, -1, 1), Mathf.Clamp(-_currentDirection.x, -1, 1));

            if (direction.Equals(Vector2.zero))
                return;

            WordFinderLetter currentLetter = _startPos;
            for (int i = 0; i < totalSteps; i++)
            {
                AddPosition(currentLetter);
                if (currentLetter.Neighbours.ContainsKey(direction))
                    currentLetter = currentLetter.Neighbours[direction];
            }

            AddPosition(target);
        }

        /// <summary>
        /// Add a given letter to the lineDrawer.
        /// </summary>
        /// <param name="letter">The letter we want to draw a line towards</param>
        private void AddPosition(WordFinderLetter letter)
        {
            if (_linePositions.ContainsKey(letter.GridPosition))
                return;

            _linePositions.Add(letter.GridPosition, letter.transform.position);
            _selectedWord.Add(letter);

            ReDrawLine(_currentLineRenderer.LineRenderer);
        }

        /// <summary>
        /// Waits until the failure animation is done and then removes the failed line.
        /// </summary>
        /// <param name="line">To be removed line</param>
        private IEnumerator OnLineFailure(WordFinderLine line)
        {
            yield return line.OnFailure();
            _linesPool.ReturnObject(line);
        }

        /// <summary>
        /// Sets the line size of the currently displayed line.
        /// </summary>
        /// <param name="size">Size of the line</param>
        private void SetLineSize(float size) => _currentLineRenderer.LineRenderer.widthMultiplier = size;

        /// <summary>
        /// Redraws the line to tracked positions on given LineRenderer.
        /// </summary>
        /// <param name="renderer">The LineRenderer to draw positions on</param>
        private void ReDrawLine(LineRenderer renderer)
        {
            List<Vector3> newPositions = new List<Vector3>();
            foreach (KeyValuePair<Vector2, Vector3> linePoint in _linePositions)
                newPositions.Add(linePoint.Value);

            renderer.positionCount = 2;
            if (newPositions.Count == 0)
                return;

            if (renderer.GetPosition(0).Equals(Vector3.zero)) // Vector3.zero is the initial value of a newly added vertex
            {
                _animationPos = newPositions[0];
                renderer.SetPosition(0, newPositions[0]);
                renderer.SetPosition(1, newPositions[0]);
            }

            Vector3 currentPos = _animationPos;

            _currentLineRenderer.SnapToLetter(currentPos + (newPositions.Last() - currentPos));
        }
        #endregion
        #endregion
    }
}
