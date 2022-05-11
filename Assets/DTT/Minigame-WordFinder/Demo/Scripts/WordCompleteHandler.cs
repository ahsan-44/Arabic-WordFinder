using UnityEngine;

namespace DTT.MiniGame.WordFinder.Demo
{
    /// <summary>
    /// Adds effects to the events of <see cref="WordFinderWord"/>.
    /// </summary>
    [RequireComponent(typeof(AudioSource))]
    public class WordCompleteHandler : MonoBehaviour
    {
        /// <summary>
        /// Word finder word component.
        /// </summary>
        [SerializeField]
        [Tooltip("Word finder word component")]
        private WordFinderWord _word;

        /// <summary>
        /// Audio source of this word.
        /// </summary>
        private AudioSource _audioSource;

        /// <summary>
        /// Gets the necessary components.
        /// </summary>
        private void Awake() => _audioSource = GetComponent<AudioSource>();

        /// <summary>
        /// Sets values to their initial and starts intro animation.
        /// Subscribes to necessary events.
        /// </summary>
        private void OnEnable()
        {
            _word.Text.alpha = 0;

            // Animates the word in.
            StartCoroutine(Animations.Value(0, 1, 0.3f, (value) => { _word.Text.alpha = value; }));

            _word.Reset += ResetWord;
            _word.Completed += OnComplete;
        }

        /// <summary>
        /// Unsubscribes to events.
        /// </summary>
        private void OnDisable()
        {
            _word.Reset -= ResetWord;
            _word.Completed -= OnComplete;
        }

        /// <summary>
        /// Sets the font style to strike through to indicate the word has been found.
        /// </summary>
        private void OnComplete()
        {
            _audioSource.Play();
            _word.Text.fontStyle = TMPro.FontStyles.Strikethrough;
        }

        /// <summary>
        /// Resets the word to its original state.
        /// </summary>
        private void ResetWord()
        {
            _word.Text.enableAutoSizing = true;
            _word.Text.fontStyle = TMPro.FontStyles.Normal;
        }
    }
}
