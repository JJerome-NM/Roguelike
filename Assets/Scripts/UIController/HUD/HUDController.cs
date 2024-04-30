using DefaultNamespace;
using Levels;
using Player;
using TMPro;
using UnityEngine;

namespace UIController.HUD
{
    public class HUDController : MonoBehaviour
    {
        private static string HealsBeginText = "Heals - ";
        private static string CurrentLevelText = " - level";
        
        [SerializeField] private TextMeshProUGUI healsText;
        [SerializeField] private TextMeshProUGUI currentLevelText;

        private void Awake()
        {
            HealsBeginText = healsText.text;
            
            PlayerEventManager.OnPlayerHealsUpdate.AddListener((newHeals) =>
            {
                healsText.SetText(HealsBeginText + newHeals.ToString("F0"));
            });
            LevelsEventManager.OnLevelUpdated.AddListener((level) =>
            {
                currentLevelText.SetText(level + CurrentLevelText); 
            });
            
            GlobalEventManager.OnGameStarted.AddListener((_) =>
            {
                healsText.gameObject.SetActive(true);
            });
            GlobalEventManager.OnGameStopped.AddListener((_) =>
            {
                healsText.gameObject.SetActive(false);
            });
        }
    }
}