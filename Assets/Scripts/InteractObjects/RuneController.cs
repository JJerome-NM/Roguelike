using UnityEngine;

namespace InteractObjects
{
    public class RuneController : MonoBehaviour
    {
        [SerializeField] private GameObject glow;

        private bool _isCollected = false;

        public void Interact()
        {
            glow.SetActive(true);
            _isCollected = true;
        }

        public bool IsCollected() => _isCollected;
    }
}