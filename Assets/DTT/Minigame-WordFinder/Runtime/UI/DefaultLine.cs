using System.Collections;
using UnityEngine;

namespace DTT.MiniGame.WordFinder
{
    /// <summary>
    /// Behaviour for the line that gets drawn on the letter grid.
    /// Handles the success and failure animations.
    /// Derives from <see cref="WordFinderLine">, so it can implement the needed behaviour.
    /// </summary>
    public class DefaultLine : WordFinderLine
    {
        /// <summary>
        /// Speed at which the line snaps to the new position.
        /// </summary>
        [SerializeField]
        [Tooltip("Speed at which the line snaps to the new position")]
        private float _snapSpeed = 50;

        /// <summary>
        /// Time the fade out animation takes.
        /// </summary>
        [SerializeField]
        [Tooltip("Time the fade out animation takes")]
        private float _fadeTime = .1f;

        /// <summary>
        /// Goal position the line has to snap to.
        /// </summary>
        private Vector3 _goalPos;

        /// <summary>
        /// Initial alpha of the line.
        /// </summary>
        protected float p_originalAlpha;

        /// <summary>
        /// Gets necessary components and sets original alpha value.
        /// </summary>
        protected override void Awake()
        {
            base.Awake();
            p_originalAlpha = LineRenderer.startColor.a;
        }

        /// <summary>
        /// Used to animate the line.
        /// </summary>
        protected virtual void FixedUpdate()
        {
            // Moves to the new given position when a new letter is hovered on.
            if (_goalPos != EndPosition)
                SetEndPosition(Vector3.MoveTowards(EndPosition, _goalPos, _snapSpeed));
        }

        /// <summary>
        /// Once the fade animation has finished yielding, the line will be deactivated and returned to the pool.
        /// </summary>
        public override IEnumerator OnFailure() => FadeLine();

        /// <summary>
        /// Sets the new goal pos to the position of the selected letter.
        /// </summary>
        /// <param name="position">New end position of the letter</param>
        public override void SnapToLetter(Vector2 position) => _goalPos = position;

        /// <summary>
        /// You can implement behaviour here for when a line finished a word successfully.
        /// </summary>
        public override void OnSuccess() { }

        /// <summary>
        /// Resets the alpha of the line back to the original.
        /// The wordfinder uses a pool system, so resetting values to their original states is necessary.
        /// </summary>
        public override void ResetObject() => SetLineAlpha(p_originalAlpha);

        /// <summary>
        /// Sets the alpha of the line to a given value.
        /// </summary>
        /// <param name="alpha">Sets the alpha to this value</param>
        private void SetLineAlpha(float alpha)
        {
            Color color = LineRenderer.startColor;
            color.a = alpha;
            LineRenderer.startColor = color;
            LineRenderer.endColor = color;
        }

        /// <summary>
        /// Fades out the line.
        /// </summary>
        private IEnumerator FadeLine() =>
            Animations.EaseIn(LineRenderer.startColor.a, 0, _fadeTime, (value) => SetLineAlpha(value));
    }
}
