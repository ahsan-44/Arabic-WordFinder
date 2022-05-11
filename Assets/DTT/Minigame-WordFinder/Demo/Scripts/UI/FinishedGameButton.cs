using UnityEngine;
using UnityEngine.UI;

namespace DTT.MiniGame.WordFinder.Demo
{
    /// <summary>
    /// Handles the button that comes in when a game is finished.
    /// </summary>
    [RequireComponent(typeof(Button), typeof(CanvasGroup))]
    public class FinishedGameButton : MonoBehaviour
    {
        /// <summary>
        /// Reference to the word finder manager of this scene.
        /// </summary>
        [SerializeField]
        [Tooltip("Reference to the word finder manager of this scene")]
        private WordFinderManager _manager;

        /// <summary>
        /// Canvas group of the button.
        /// </summary>
        private CanvasGroup _canvasGroup;

        /// <summary>
        /// Gets necessary components.
        /// </summary>
        void Awake()
        {
            _canvasGroup = GetComponent<CanvasGroup>();
            TurnOffButton();
        }

        /// <summary>
        /// Adds listeners.
        /// </summary>
        private void OnEnable()
        {
            // _manager.Finish += FadeIn;
            _manager.Started += TurnOffButton;
        }

        /// <summary>
        /// Removes listeners.
        /// </summary>
        private void OnDisable()
        {
            // _manager.Finish -= FadeIn;
            _manager.Started -= TurnOffButton;
        }

        /// <summary>
        /// Fades in the button.
        /// </summary>
        private void FadeIn(WordFinderResult result)
        {
            _canvasGroup.alpha = 0;
            _canvasGroup.blocksRaycasts = true;
            _canvasGroup.interactable = true;
            StartCoroutine(Animations.Value(0, 1, .3f, (value) => _canvasGroup.alpha = value));
        }

        /// <summary>
        /// Resets the button to be invisible.
        /// </summary>
        private void TurnOffButton()
        {
            _canvasGroup.blocksRaycasts = false;
            _canvasGroup.interactable = false;
            _canvasGroup.alpha = 0;
        }
    }
}