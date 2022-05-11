using UnityEngine;
using UnityEngine.UI;

namespace DTT.MiniGame.WordFinder.Demo
{
    /// <summary>
    /// Handles resuming and pausing the game.
    /// </summary>
    [RequireComponent(typeof(Button))]
    public class PauseButton : MonoBehaviour
    {
        /// <summary>
        /// Reference to the word finder manager of this scene.
        /// </summary>
        [SerializeField]
        [Tooltip("Reference to the word finder manager of this scene")]
        private WordFinderManager _manager;

        /// <summary>
        /// Icon of the button.
        /// </summary>
        [SerializeField]
        [Tooltip("Icon of the button")]
        private Image _icon;

        /// <summary>
        /// Pause image.
        /// </summary>
        [SerializeField]
        [Tooltip("Pause image")]
        private Sprite _pause;

        /// <summary>
        /// Resume image.
        /// </summary>
        [SerializeField]
        [Tooltip("Resume image")]
        private Sprite _resume;

        /// <summary>
        /// Button component.
        /// </summary>
        private Button _button;

        /// <summary>
        /// Gets necessary components.
        /// </summary>
        private void Awake() => _button = GetComponent<Button>();

        /// <summary>
        /// Adds listeners.
        /// </summary>
        private void OnEnable() => _button.onClick.AddListener(TogglePaused);

        /// <summary>
        /// Removes listeners.
        /// </summary>
        private void OnDisable() => _button.onClick.RemoveListener(TogglePaused);

        /// <summary>
        /// Toggles the pause state of the game.
        /// </summary>
        private void TogglePaused()
        {
            if (!_manager.IsGameActive)
                return;

            if (_manager.IsPaused)
            {
                _icon.sprite = _pause;
                _manager.Continue();
            }
            else
            {
                _icon.sprite = _resume;
                _manager.Pause();
            }
        }
    }
}