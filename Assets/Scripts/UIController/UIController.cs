using DefaultNamespace;
using UIController.Inventory;
using UnityEngine;

namespace UIController
{
    public class UIController : MonoBehaviour
    {
        [Header("Keys")]
        [SerializeField] private KeyCode pause = KeyCode.Escape;
        [SerializeField] private KeyCode openInventory = KeyCode.Tab;

        [SerializeField] private MainPanelController mainPanelController;
        [SerializeField] private UiInventoryController inventoryController;

        private void Start()
        {
            GlobalEventManager.StartGame(GameStartState.Start);
        }

        private void Awake()
        {
            GlobalEventManager.OnGameStarted.AddListener(OnGameStart);
            GlobalEventManager.OnGameStopped.AddListener(OnGameStopped);
        }

        private void Update()
        {
            if (Input.GetKey(pause))
            {
                GlobalEventManager.StopGame(GameEndState.Pause);
            }
            
            if (Input.GetKey(openInventory) && !UiInventoryController.IsInventoryOpen && !MainPanelController.IsMainPanelOpen)
            {
                inventoryController.Show();
            } 
            else if (!Input.GetKey(openInventory) && UiInventoryController.IsInventoryOpen)
            {
                inventoryController.Hide();
            }
        }

        private void OnGameStart(GameStartState state)
        {
            mainPanelController.Hide(state);
            UiInventoryEventManager.HideInventory();
        }

        private void OnGameStopped(GameEndState state)
        {
            mainPanelController.Show(state);
            UiInventoryEventManager.HideInventory();
        }
    }
}