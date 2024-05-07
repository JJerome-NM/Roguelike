using DefaultNamespace;
using FirstLevelScene;
using FirstLevelScene.Game;
using UnityEngine;

namespace Levels
{
    public class LevelsController : MonoBehaviour
    {
        [SerializeField] private float startLevelMultiplayer = 1.5f;
        [SerializeField] private float levelMultiplayer = 2f;
        [SerializeField] private int startLevel = 1;
        
        // private float _levelMultiplayer;

        private int _currentLevel = 1;
        private float _currentMultiplayer;
        
        private void Awake()
        {
            _currentLevel = startLevel;
            _currentMultiplayer = startLevelMultiplayer;
            
            LevelsEventManager.UpdateLevel(1);
            
            GlobalEventManager.OnGameStarted.AddListener(OnGameStarted);
            GlobalEventManager.OnGameStopped.AddListener(OnGameStopped);
        }

        private void OnGameStopped(GameEndState state)
        {
            if (state == GameEndState.AllRunesWasFound)
            {
                _currentMultiplayer += levelMultiplayer;
                ++_currentLevel;
                LevelsEventManager.UpdateLevel(_currentLevel);
                LevelsEventManager.UpdateLevelMultiplayer(_currentMultiplayer);
            }
        }

        private void OnGameStarted(GameStartState state)
        {
            // LevelsEventManager.UpdateLevel(_currentLevel);
            // LevelsEventManager.UpdateLevelMultiplayer(_currentMultiplayer);
        }
    }
}