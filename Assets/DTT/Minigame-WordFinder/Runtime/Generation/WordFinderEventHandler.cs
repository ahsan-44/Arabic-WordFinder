using System;

namespace DTT.MiniGame.WordFinder
{
    /// <summary>
    /// Handles the events from the word finder game.
    /// </summary>
    public class WordFinderEventHandler
    {
        /// <summary>
        /// Event invoked once the game is cleared.
        /// </summary>
        public event Action OnClear;

        /// <summary>
        /// Event invoked once a letter is selected.
        /// </summary>
        public event Action<WordFinderLetter> OnSelectLetter;

        /// <summary>
        /// Event invoked when a letter is deselected, which means a word has been completed.
        /// </summary>
        public event Action OnDeselectLetter;

        /// <summary>
        /// Event invoked when a word has been selected.
        /// </summary>
        public event Action<bool, string> OnWordCompleted;

        /// <summary>
        /// Called once a new letter is selected.
        /// </summary>
        /// <param name="letter">The selected letter</param>
        internal void SelectLetter(WordFinderLetter letter) => OnSelectLetter?.Invoke(letter);

        /// <summary>
        /// Called once the game has been cleared.
        /// </summary>
        internal void ClearGame() => OnClear?.Invoke();

        /// <summary>
        /// Called once a letter has been deselected.
        /// </summary>
        internal void DeselectLetter() => OnDeselectLetter?.Invoke();

        /// <summary>
        /// Called once a word has been selected.
        /// </summary>
        /// <param name="valid">Whether the word was on the search list</param>
        /// <param name="word">Selected word</param>
        internal void WordCompleted(bool valid, string word) => OnWordCompleted?.Invoke(valid, word);
    }
}
