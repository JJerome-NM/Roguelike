using UnityEngine;

namespace Player
{
    public class CameraHolder : MonoBehaviour
    {
        [SerializeField] private Transform player;
        [SerializeField] private float lerpSpeed;
        
        private Vector3 _offset;

        private void Start()
        {
            _offset = transform.position - player.position;
        }

        private void Update()
        {
            transform.position = Vector3.Lerp(transform.position, player.position + _offset, lerpSpeed * Time.deltaTime);
        }
    }
}