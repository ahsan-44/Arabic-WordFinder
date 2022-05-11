using System;
using RTLTMPro;
using TMPro;
using UnityEngine;

namespace DTT.MiniGame.WordFinder.Demo
{
    /// <summary>
    /// Displays the duration of the game.
    /// </summary>
    public class WordGameTimer : MonoBehaviour
    {
        /// <summary>
        /// Reference to the current word finder manager.
        /// </summary>
        [SerializeField]
        [Tooltip("Reference to the current word finder manager")]
        private WordFinderManager _wordFinderManager;

        /// <summary>
        /// Text component of the timer.
        /// </summary>
        private TextMeshProUGUI _text;

        /// <summary>
        /// Gets the necessary components.
        /// </summary>
        void Start() => _text = GetComponent<TextMeshProUGUI>();

        /// <summary>
        /// Updates the timer.
        /// </summary>
        void Update()
        {
            TimeSpan time = TimeSpan.FromSeconds(_wordFinderManager.PlayTime);
            _text.text = time.ToString("mm':'ss");
        }
    }
}