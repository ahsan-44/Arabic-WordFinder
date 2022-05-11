using UnityEngine;

namespace DTT.MiniGame.WordFinder.Demo
{
    /// <summary>
    /// Adds effects to the events of the <see cref="WordFinderLetter"/>.
    /// </summary>
    [RequireComponent(typeof(AudioSource))]
    public class LetterEffectHandler : MonoBehaviour
    {
        /// <summary>
        /// The letter component.
        /// </summary>
        [SerializeField]
        [Tooltip("The letter component")]
        private WordFinderLetter _letter;

        /// <summary>
        /// Audio source of the letter.
        /// </summary>
        private AudioSource _audioSource;

        /// <summary>
        /// Gets the necessary components.
        /// </summary>
        private void Awake() => _audioSource = GetComponent<AudioSource>();

        /// <summary>
        /// Starts fade-in animation and listens to the letter select event.
        /// </summary>
        private void OnEnable()
        {
            _letter.Text.alpha = 0;
            StartCoroutine(Animations.Value(0, 1, 0.2f, (value) => _letter.Text.alpha = value));

            _letter.LastSelected += OnSelected;
        }

        /// <summary>
        /// Unsubscribes from events.
        /// </summary>
        private void OnDisable() => _letter.LastSelected -= OnSelected;

        /// <summary>
        /// Plays audio when the letter is selected.
        /// </summary>
        private void OnSelected()
        {
            if (!_audioSource.isPlaying)
                _audioSource.Play();
        }
    }
}