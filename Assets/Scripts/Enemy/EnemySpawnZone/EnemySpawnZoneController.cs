using System.Collections.Generic;
using Enemy;
using Levels;
using UnityEngine;

namespace EnemySpawnZone
{
    public class EnemySpawnZoneController : MonoBehaviour
    {
        [SerializeField] private GameObject player;
        [SerializeField] private GameObject enemyPrefab;
        [SerializeField] private int maxEnemies = 1;
        
        private SpriteRenderer _spriteRenderer;
        private Bounds _bounds;

        public List<EnemyController> _spawnedEnemies = new();
        
        private void Awake()
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
            _bounds = _spriteRenderer.bounds;
            SpawnEnemies();
            
            LevelsEventManager.OnLevelMultiplayerUpdated.AddListener(OnLevelMultiplayerUpdated);
        }

        private void OnLevelMultiplayerUpdated(float newMultiplayer)
        {
            maxEnemies = (int)newMultiplayer / 2;
            _spawnedEnemies.ForEach(enemy =>
            {
                if (enemy != null)
                {
                    enemy.Damage(999999);
                }
            });
            _spawnedEnemies.Clear();
            
            SpawnEnemies();
        }
        
        private void SpawnEnemies()
        {
            for (int i = _spawnedEnemies.Count; i < maxEnemies; ++i)
            {
                float xPos = Random.Range(_bounds.min.x, _bounds.max.x);
                float yPos = Random.Range(_bounds.min.y, _bounds.max.y);
            
                var enemy = Instantiate(enemyPrefab, new Vector3(xPos, yPos, 0), Quaternion.identity);
                var enemyAI = enemy.GetComponentInChildren<EnemyAi>();
                enemyAI.InitPlayer(player);
                _spawnedEnemies.Add(enemy.GetComponentInChildren<EnemyController>());
            }
        }
    }
}
