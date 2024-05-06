using DefaultNamespace;
using Player;
using TMPro;
using UnityEngine;

namespace UIController.Inventory
{
    public class UiInventoryController : MonoBehaviour
    {
        private static readonly string RunesCountText = "Amount of runes found - ";
        
        public static bool IsInventoryOpen { get; private set; }
        
        [SerializeField] private TextMeshProUGUI runesCount;
        [SerializeField] private PlayerInventory playerInventory;

        private void Awake()
        {
            GlobalEventManager.OnGameStarted.AddListener((_) => Hide());
            GlobalEventManager.OnGameStopped.AddListener((_) => Hide());
        }

        private void Start()
        {
            Hide();
        }

        public void Show()
        {
            if (!GlobalStateManager.IsGameRunning) return;
            
            LoadInventory();
            gameObject.SetActive(true);
            
            IsInventoryOpen = true;
        }

        public void Hide()
        {
            gameObject.SetActive(false);
            
            IsInventoryOpen = false;
        }

        private void LoadInventory()
        {
            runesCount.SetText(RunesCountText + playerInventory.collectedRunes);
        }
    }
}