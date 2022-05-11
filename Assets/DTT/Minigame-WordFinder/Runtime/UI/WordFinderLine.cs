using System;
using System.Collections;
using UnityEngine;

namespace DTT.MiniGame.WordFinder
{
    /// <summary>
    /// Base line that handles the placement of the word lines.
    /// </summary>
    [RequireComponent(typeof(LineRenderer))]
    public abstract class WordFinderLine : MonoBehaviour, IPoolObject
    {
        #region Variables
        #region Private
        /// <summary>
        /// The linerenderer of this line.
        /// </summary>
        private LineRenderer _lineRenderer;

        public event Action Reset;
        #endregion
        #region Public
        /// <summary>
        /// The linerenderer of this line.
        /// </summary>
        public LineRenderer LineRenderer { get => _lineRenderer; }

        /// <summary>
        /// End position of the line.
        /// This is equal to the position of the last letter in the selected word.
        /// </summary>
        public Vector3 EndPosition { get => _lineRenderer.GetPosition(1); }

        /// <summary>
        /// Start position of the line.
        /// This is equal to the position of the first letter in the selected word.
        /// </summary>
        public Vector3 StartPosition { get => _lineRenderer.GetPosition(0); }
        #endregion
        #endregion
        #region Methods
        #region Unity
        /// <summary>
        /// Gets the line renderer component
        /// </summary>
        protected virtual void Awake() => _lineRenderer = GetComponent<LineRenderer>();
        #endregion
        #region Public 
        /// <summary>
        /// Called when the line has successfully completed a word.
        /// Use override to define OnSuccess animation.
        /// </summary>
        public abstract void OnSuccess();

        /// <summary>
        /// Called when the line failed to complete a word.
        /// Once the method is over, the line gets removed.
        /// Use override to define OnFailure animation.
        /// </summary>
        public abstract IEnumerator OnFailure();

        /// <summary>
        /// Resets the line when its reused.
        /// Override to implement your own reset behaviour.
        /// </summary>
        public virtual void ResetObject() => Reset?.Invoke();

        /// <summary>
        /// Handles setting the position of the end of the line using the given letter's potition.
        /// Use <see cref="SetEndPosition(Vector2)"> to set the line end position without visual bugs.
        /// Override to add your own snap animation.
        /// </summary>
        /// <param name="position">Position the line has to go to</param>
        public virtual void SnapToLetter(Vector2 position) => SetEndPosition(position);
        #endregion
        #region Protected
        /// <summary>
        /// Sets the position of the end point of the line to the given position.
        /// </summary>
        /// <param name="position">Position the line will be set to</param>
        protected void SetEndPosition(Vector2 position)
        {
            Vector3 pos = position;
            pos.z = LineRenderer.GetPosition(0).z;
            LineRenderer.SetPosition(1, pos);
        }
        #endregion
        #endregion
    }
}
