using DefaultNamespace;
using FirstLevelScene;
using Game;
using Photon.Pun;
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
        [SerializeField] private Button exitButton;
        [SerializeField] private KeyCode hideKey = KeyCode.Escape;
        
        private TextMeshProUGUI _buttonText;

        private GameEndState _endState;

        private void Awake()
        {
            _buttonText = startButton.GetComponentInChildren<TextMeshProUGUI>();
            
            startButton.onClick.AddListener(OnStartButtonClick);
            exitButton.onClick.AddListener(Application.Quit);
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
            if (GlobalStateManager.IsFirstStart && PhotonNetwork.IsMasterClient)
            {
                PhotonNetwork.CurrentRoom.SetCustomProperties(new()
                {
                    { RoguelikeGame.GameIsRunning, true }
                });

                PhotonNetwork.CurrentRoom.IsOpen = false;
                PhotonNetwork.CurrentRoom.IsVisible = false;
            }
            else
            {
                GlobalEventManager.StartGame(_endState.GetEndStateInversion());
            }
        }
    }
}