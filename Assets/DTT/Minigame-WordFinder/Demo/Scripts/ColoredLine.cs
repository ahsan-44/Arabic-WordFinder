using System.Collections;
using UnityEngine;

namespace DTT.MiniGame.WordFinder.Demo
{
    /// <summary>
    /// Handles the line component with color and sound effects.
    /// </summary>
    [RequireComponent(typeof(AudioSource))]
    public class ColoredLine : DefaultLine
    {
        /// <summary>
        /// A random color will be chosen out of these colors.
        /// </summary>
        [SerializeField]
        [Tooltip("A random color will be chosen out of these colors")]
        private Color[] _lineColors;

        /// <summary>
        /// On fail clip.
        /// </summary>
        [SerializeField]
        [Tooltip("On fail clip")]
        private AudioClip _onFail;

        /// <summary>
        /// On succeed clip.
        /// </summary>
        [SerializeField]
        [Tooltip("On succeed clip")]
        private AudioClip _onSucceed;

        /// <summary>
        /// Audio source of this line.
        /// </summary>
        private AudioSource _audioSource;

        /// <summary>
        /// Gets necessary components.
        /// </summary>
        protected override void Awake()
        {
            base.Awake();
            _audioSource = GetComponent<AudioSource>();
        }

        /// <summary>
        /// Adds a failure sound effect to the line.
        /// </summary>
        public override IEnumerator OnFailure()
        {
            _audioSource.PlayOneShot(_onFail);
            yield return base.OnFailure();
            yield return new WaitUntil(() => !_audioSource.isPlaying);
        }

        /// <summary>
        /// Adds sound effect to line completion.
        /// </summary>
        public override void OnSuccess() => _audioSource.PlayOneShot(_onSucceed);

        /// <summary>
        /// Randomizes the color.
        /// </summary>
        public override void ResetObject()
        {
            SetRandomColor();
            base.ResetObject();
        }

        /// <summary>
        /// Sets the line to a random color.
        /// </summary>
        private void SetRandomColor()
        {
            Color color = _lineColors[Random.Range(0, _lineColors.Length - 1)];
            color.a = p_originalAlpha;
            LineRenderer.startColor = color;
            LineRenderer.endColor = color;
        }
    }
}
