using DefaultNamespace;
using UnityEngine;

namespace InteractObjects
{
    public class RuneController : MonoBehaviour
    {
        [SerializeField] private GameObject glow;

        private bool _isCollected = false;

        private void Awake()
        {
            GlobalEventManager.OnGameStarted.AddListener((state) =>
            {
                if (state != GameStartState.Resume)
                {
                    glow.SetActive(false);
                    _isCollected = false;
                }
            });
        }

        public void Interact()
        {
            glow.SetActive(true);
            _isCollected = true;
        }

        public bool IsCollected() => _isCollected;
    }
}