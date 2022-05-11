using System;
using UnityEngine;
using RTLTMPro;

namespace DTT.MiniGame.WordFinder
{
    /// <summary>
    /// Base class for a word UI element.
    /// </summary>
    public class WordFinderWord : MonoBehaviour, IPoolObject
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
        /// The text mesh component of the word object.
        /// </summary>
        public RTLTextMeshPro Text { get => _text; protected set => _text = value; }

        /// <summary>
        /// The displayed word.
        /// </summary>
        public string Word { get => _data.Word; }
        #endregion
        #region Private
        /// <summary>
        /// Holds the word's data.
        /// </summary>
        private WordFinderWordData _data;

        /// <summary>
        /// Invoked when the word is reset.
        /// </summary>
        public event Action Reset;

        /// <summary>
        /// Invoked when the word has been found.
        /// </summary>
        public event Action Completed;
        #endregion
        #endregion
        #region Methods
        #region Unity
        /// <summary>
        /// Removes listeners from events.
        /// </summary>
        protected virtual void OnDisable()
        {
            if (_data != null)
                _data.OnComplete -= OnComplete;
        }
        #endregion
        #region Public
        /// <summary>
        /// Resets the data of the word.
        /// </summary>
        public void ResetObject() => Reset?.Invoke();

        /// <summary>
        /// Initializes the component by settings the text and start listening to
        /// the OnComplete event of the word.
        /// </summary>
        /// <param name="word"></param>
        internal void Init(WordFinderWordData word)
        {
            _data = word;
            word.OnComplete += OnComplete;
            _text.text = word.Word;
            _text.ForceMeshUpdate();
        }
        #endregion
        #region Protected
        /// <summary>
        /// Method called when the word has been found.
        /// </summary>
        protected void OnComplete() => Completed?.Invoke();
        #endregion
        #endregion
    }
}
