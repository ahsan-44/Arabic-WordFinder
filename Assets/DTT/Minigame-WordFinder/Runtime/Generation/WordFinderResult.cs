using System.Text;

namespace DTT.MiniGame.WordFinder
{
    /// <summary>
    /// Class containing result data of the word finder game.
    /// </summary>
    public class WordFinderResult
    {
        private readonly WordFinderResultData _resultData;
        /// <summary>
        /// Time it took to finish the game.
        /// </summary>
        public float TimeTaken => _resultData.timeTaken;

        /// <summary>
        /// Amount of times the user selected wrong letter combinations.
        /// </summary>
        public int WrongSelections => _resultData.wrongSelections;

        /// <summary>
        /// Amount of times the user selected correct letter combinations.
        /// </summary>
        public int CorrectSelections => _resultData.correctSelections;

        /// <summary>
        /// Sets the result data.
        /// </summary>
        /// <param name="result">The result data.</param>
        internal WordFinderResult(WordFinderResultData result)=>_resultData = result;

        /// <summary>
        /// Returns the result info in string format.
        /// </summary>
        /// <returns>Result in string format.</returns>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("Time Taken: " + TimeTaken);
            sb.Append("\nWrong Selections: " + WrongSelections);
            sb.Append("\nCorrect Selections: " + WrongSelections);
            return sb.ToString();
        }
    }
}