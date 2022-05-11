using System;
using System.Collections.Generic;
using RTLTMPro;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace DTT.MiniGame.WordFinder
{
    ///<summary>
    ///A letter in the wordfinder game.
    ///</summary>
    public class WordFinderLetter : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerEnterHandler, IPoolObject
    {
        #region Variables
        #region Editor
        /// <summary>
        /// The visual text of the letter.
        /// </summary>
        [SerializeField]
        [Tooltip("The visual text")]
        private RTLTextMeshPro _text;
        #endregion
        #region Public
        /// <summary>
        /// References to the text that belongs with this .
        /// </summary>
        public RTLTextMeshPro Text => _text;

        /// <summary>
        /// The data of the WordFinderLetter.
        /// </summary>
        public char Data { get; private set; }

        /// <summary>
        /// The neighbours of this letter.
        /// </summary>
        public Dictionary<Vector2, WordFinderLetter> Neighbours { get; private set; }

        /// <summary>
        /// The grid position of this letter.
        /// </summary>
        public Vector2 GridPosition { get; private set; }

        /// <summary>
        /// Whether this letter belongs to a completed word.
        /// </summary>
        public bool Completed { get; private set; }

        /// <summary>
        /// Invoked once the object is reused.
        /// </summary>
        public event Action Reset;

        /// <summary>
        /// Invoked once the letter has been dragged, clicked or let go off.
        /// </summary>
        public event Action<WordFinderLetter> Selected;

        /// <summary>
        /// Invoked when the letter was part of a selected string of letters.
        /// </summary>
        public event Action Confirmed;

        /// <summary>
        /// Invoked when this letter is the last letter of a new selected word.
        /// </summary>
        public event Action LastSelected;

        /// <summary>
        /// Invoked when the letter was a part of a correct word.
        /// </summary>
        public event Action Success;

        /// <summary>
        /// Invoked when the letter was a part of an incorrect word.
        /// </summary>
        public event Action Failure;
        #endregion
        #region Private
        /// <summary>
        /// Whether a letter is being selected.
        /// </summary>
        private static bool _selecting;
        #endregion
        #endregion
        #region Methods
        #region Public
        /// <summary>
        /// Resets the letter when its reused in the next game.
        /// Override to implement your own reset behaviour.
        /// </summary>
        public void ResetObject()
        {
            Reset?.Invoke();
            Completed = false;
        }

        /// <summary>
        /// Called when the letter is part of a correct word.
        /// </summary>
        public void OnSuccess()
        {
            Success?.Invoke();
            Completed = true;
        }

        /// <summary>
        /// Called when the letter is part of an incorrect word.
        /// </summary>
        public void OnFailure() => Failure?.Invoke();

        /// <summary>
        /// Called when this letter is the last letter of a new selected word.
        /// </summary>
        public void OnSelected() => LastSelected?.Invoke();

        /// <summary>
        /// Initialiser method of a letter.
        /// </summary>
        /// <param name="data">The WordFinderLetterData that belongs to this letter</param>
        /// <param name="gridPos">The grid position of this WordFinderLetter</param>
        /// <param name="eventHandler">Event handler of the game</param>
        internal void Init(char data, Vector2 gridPos, WordFinderEventHandler eventHandler)
        {
            Data = data;
            _text.text = data.ToString();
            GridPosition = gridPos;

            Text.ForceMeshUpdate();
        }

        /// <summary>
        /// Handles the click on a letter.
        /// </summary>
        /// <param name="eventData">The PointerEventData</param>
        public void OnPointerDown(PointerEventData eventData)
        {
            if (_selecting)
                return;

            _selecting = true;
            Selected?.Invoke(this);
        }

        /// <summary>
        /// Handles the release of an active swipe.
        /// </summary>
        /// <param name="eventData">The PointerEventData</param>
        public void OnPointerUp(PointerEventData eventData)
        {
            if (!_selecting)
                return;

            _selecting = false;
            Selected?.Invoke(this);
            Confirmed?.Invoke();
        }

        /// <summary>
        /// Handles the continious dragging of the letters.
        /// </summary>
        /// <param name="eventData">The PointerEventData</param>
        public void OnPointerEnter(PointerEventData eventData)
        {
            if (!_selecting)
                return;

            Selected?.Invoke(this);
        }

        /// <summary>
        /// Set the neighbours of this letter.
        /// </summary>
        /// <param name="neighbours">The neighbours of this letter</param>
        internal void SetNeighbours(Dictionary<Vector2, WordFinderLetter> neighbours) => Neighbours = neighbours;
        #endregion
        #endregion
    }
}
