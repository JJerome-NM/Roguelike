using DefaultNamespace;
using FirstLevelScene;
using FirstLevelScene.Player;
using Player;
using TMPro;
using UnityEngine;

namespace UIController.Inventory
{
    public class UiInventoryController : MonoBehaviour
    {
        private static readonly string RunesCountText = "Amount of runes found - ";

        public static UiInventoryController Instance { get; private set; }
        public static bool IsInventoryOpen { get; private set; }
        
        [SerializeField] private TextMeshProUGUI runesCount;
        
        private PlayerInventory _playerInventory;

        private void Awake()
        {
            Instance = this;
            GlobalEventManager.OnGameStarted.AddListener((_) => Hide());
            GlobalEventManager.OnGameStopped.AddListener((_) => Hide());
        }

        private void Start()
        {
            _playerInventory = PlayerInventory.Instance;
            
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
            _playerInventory ??= PlayerInventory.Instance;
            runesCount.SetText(RunesCountText + _playerInventory.collectedRunes);
        }
    }
}