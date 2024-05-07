using System.Collections.Generic;
using Obstacle;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;

namespace FirstLevelScene.Obstacle
{
    [RequireComponent(typeof(PhotonView))]
    public class ObstacleFieldController : MonoBehaviour
    {
        [SerializeField] private Tilemap barrierTilemap;
        [SerializeField] private Tilemap grassTilemap;
        [SerializeField] private TileBase barrier;
        [SerializeField] private TileBase[] grassTiles;

        [SerializeField] private int barrierChance;
        
        private PhotonView _photonView;
        private Vector3Int[] _fieldPosition;
        
        private void Awake()
        {
            _fieldPosition = grassTilemap.GetTilePositionWithType<TileBase>(grassTiles[0].GetType());
            _photonView = GetComponent<PhotonView>();
            
            GlobalEventManager.OnGameStarted.AddListener((_) => GenerateBarriers());
        }

        private void GenerateBarriers()
        {
            if (!PhotonNetwork.IsMasterClient) return;

            List<Vector3> positions = new ();
            foreach (Vector3Int pos in _fieldPosition)
            {
                if (Random.Range(0, 100) > barrierChance)
                {
                    positions.Add(barrierTilemap.CellToWorld(pos));
                }
            }

            _photonView.RPC(nameof(SpawnBarriersOnOtherClients), RpcTarget.Others, positions.ToArray());

            AddBarriersToTilemap(positions.ToArray());
        }

        public void DestroyTile(Vector3 position)
        {
            Vector3Int gridPosition = barrierTilemap.WorldToCell(position);

            barrierTilemap.SetTile(gridPosition, null);
            
            _photonView.RPC(nameof(DestroyOnOther), RpcTarget.Others, position);

            GlobalEventManager.UpdateObstacleFields();
        }

        private void AddBarriersToTilemap(Vector3[] positions)
        {
            foreach (var position in positions)
            {
                var gridPosition = barrierTilemap.WorldToCell(position);
                barrierTilemap.SetTile(gridPosition, barrier);
            }
            
            GlobalEventManager.UpdateObstacleFields();
        }
        
        #region RPC

        [PunRPC]
        private void SpawnBarriersOnOtherClients(Vector3[] positions)
        {
            AddBarriersToTilemap(positions);
        }

        [PunRPC]
        private void DestroyOnOther(Vector3 position)
        {
            Vector3Int gridPosition = barrierTilemap.WorldToCell(position);
            barrierTilemap.SetTile(gridPosition, null);   
            
            GlobalEventManager.UpdateObstacleFields();
        }

        #endregion
    }
}