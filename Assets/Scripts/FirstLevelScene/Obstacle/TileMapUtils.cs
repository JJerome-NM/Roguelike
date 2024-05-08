using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Obstacle
{
    public static class TileMapUtils
    {
        public static Vector3Int[] GetTilePositionWithType<T>(this Tilemap tilemap, Type type) where T : TileBase
        {
            return GetTilePositionsWhere<T>(tilemap, b => b.GetType() == type);
        }
        
        public static Vector3Int[] GetTilePositionsWhere<T>(this Tilemap tilemap, Predicate<T> predicate) where T : TileBase
        {
            List<Vector3Int> positions = new List<Vector3Int>();

            for (int y = tilemap.origin.y; y < tilemap.origin.y + tilemap.size.y; y++)
            {
                for (int x = tilemap.origin.x; x < tilemap.origin.x + tilemap.size.x; x++)
                {
                    var position = new Vector3Int(x, y, 0);
                    T tile = tilemap.GetTile<T>(position);
                    
                    if (tile != null && predicate(tile))
                    {
                        positions.Add(position);
                    }
                }
            }

            return positions.ToArray();
        }
    }
}