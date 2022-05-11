namespace DTT.MiniGame.WordFinder
{
    /// <summary>
    /// Struct holding the result data of a wordfinder game.
    /// </summary>
    internal struct WordFinderResultData
    {
        /// <summary>
        /// Time it took to finish the game.
        /// </summary>
        public float timeTaken;

        /// <summary>
        /// Amount of times the user selected wrong letter combinations.
        /// </summary>
        public int wrongSelections;

        /// <summary>
        /// Amount of times the user selected correct letter combinations.
        /// </summary>
        public int correctSelections;
    }
}