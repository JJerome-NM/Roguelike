using System;
using DefaultNamespace;
using Game;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UIController
{
    public class MainPanelController : MonoBehaviour
    {
        public static bool IsMainPanelOpen { get; private set; } = true;
        
        [SerializeField] private TextMeshProUGUI mainText;
        [SerializeField] private Button startButton;
        [SerializeField] private KeyCode hideKey = KeyCode.Escape;
        
        private TextMeshProUGUI _buttonText;

        private GameEndState _endState;

        private void Awake()
        {
            _buttonText = startButton.GetComponentInChildren<TextMeshProUGUI>();
            
            startButton.onClick.AddListener(OnStartButtonClick);
        }

        private void Start()
        {
            _buttonText = startButton.GetComponentInChildren<TextMeshProUGUI>();
            
            startButton.onClick.AddListener(OnStartButtonClick);
        }

        public void Hide(GameStartState state)
        {
            gameObject.SetActive(false);
            
            IsMainPanelOpen = false;
        }

        public void Show(GameEndState state)
        {
            _endState = state;
            _buttonText.SetText(state.ToStartButtonText());
            mainText.SetText(state.ToText());
            gameObject.SetActive(true);
            
            IsMainPanelOpen = true;
        }

        private void OnStartButtonClick()
        {
            GlobalEventManager.StartGame(_endState.GetEndStateInversion());
        }
    }
}