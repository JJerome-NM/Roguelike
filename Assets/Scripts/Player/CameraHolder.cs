using UnityEngine;

namespace Player
{
    public class CameraHolder : MonoBehaviour
    {
        [SerializeField] private GameObject cameraBounds;
        [SerializeField] private Transform player;
        [SerializeField] private float lerpSpeed = 1f;

        private Camera _mainCamera;
        private BoxCollider2D _boundsCollider;
        private Vector3 _offset;
        private Bounds _bounds;
        
        private void Awake()
        {
            _mainCamera = Camera.main;
        }

        private void Start()
        {
            _offset = transform.position - player.position;
            _boundsCollider = cameraBounds.GetComponent<BoxCollider2D>();

            InitBounds(_boundsCollider);
        }

        private void InitBounds(BoxCollider2D collider)
        {
            var bounds = collider.bounds;
            
            var height = _mainCamera.orthographicSize;
            var width = height * _mainCamera.aspect;

            var minX = bounds.min.x + width;
            var maxX = bounds.extents.x - width;

            var minY = bounds.min.y + height;
            var maxY = bounds.extents.y - height;

            _bounds = new Bounds();
            _bounds.SetMinMax(
                new Vector3(minX, minY, 0f),
                new Vector3(maxX, maxY, 0f)
                );
        }
        
        private void Update()
        {
            Vector3 lerpPos = Vector3.Lerp(transform.position, player.position + _offset, lerpSpeed * Time.deltaTime);
            transform.position = GetCameraBounds(lerpPos);
        }

        private Vector3 GetCameraBounds(Vector3 targetPosition)
        {
            return new Vector3(
                Mathf.Clamp(targetPosition.x, _bounds.min.x, _bounds.max.x),
                Mathf.Clamp(targetPosition.y, _bounds.min.y, _bounds.max.y),
                transform.position.z
            );
        }
    }
}