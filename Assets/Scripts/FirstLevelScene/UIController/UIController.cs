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

        private void Awake()
        {
            GlobalEventManager.OnGameStarted.AddListener(OnGameStart);
            GlobalEventManager.OnGameStopped.AddListener(OnGameStopped);
        }

        private void Update()
        {
            if (!GlobalStateManager.IsGameRunning) return;
            
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
            if (!GlobalStateManager.IsGameRunning) return;
            
            mainPanelController.Hide(state);
            UiInventoryEventManager.HideInventory();
        }

        private void OnGameStopped(GameEndState state)
        {
            if (!GlobalStateManager.IsGameRunning) return;
            
            mainPanelController.Show(state);
            UiInventoryEventManager.HideInventory();
        }
    }
}