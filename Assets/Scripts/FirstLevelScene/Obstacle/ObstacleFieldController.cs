using DefaultNamespace;
using FirstLevelScene;
using UnityEngine;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;

namespace Obstacle
{
    public class ObstacleFieldController : MonoBehaviour
    {
        [SerializeField] private Tilemap barrierTilemap;
        [SerializeField] private Tilemap grassTilemap;
        [SerializeField] private TileBase barrier;
        [SerializeField] private TileBase[] grassTiles;

        [SerializeField] private int barrierChance;
        
        [SerializeField] private int height;
        [SerializeField] private int width;
        
        private Vector3Int[] _fieldPosition;

        private void Start()
        {
            _fieldPosition = grassTilemap.GetTilePositionWithType<TileBase>(grassTiles[0].GetType());

            GenerateBarriers();
            RandomizeGrass();
            
            GlobalEventManager.UpdateObstacleFields();
        }

        private void GenerateBarriers()
        {
            foreach (Vector3Int pos in _fieldPosition)
            {
                if (Random.Range(0, 100) > barrierChance)
                {
                    barrierTilemap.SetTile(pos, barrier);
                }
            }
        }

        private void RandomizeGrass()
        {
            foreach (Vector3Int pos in _fieldPosition)
            {
                grassTilemap.SetTile(pos, grassTiles[Random.Range(0, grassTiles.Length)]);
            }
        } 
        
        public void DestroyTile(Vector3 position)
        {
            Vector3Int gridPosition = barrierTilemap.WorldToCell(position);

            barrierTilemap.SetTile(gridPosition, null);            
        }
    }
}