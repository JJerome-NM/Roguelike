using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Obstacle
{
    public static class TileMapUtils
    {
        /// <param name="doSomeWithMap">
        /// arg1 = Tilemap for filling in
        /// arg2 = height index
        /// arg3 = width index
        /// </param>
        public static void DoSome(this Tilemap map, int h, int w, Action<Tilemap, int, int> doSomeWithMap)
        {
            for (int iH = 0; iH < h; ++iH)
            {
                for (int iW = 0; iW < w; ++iW)
                {
                    doSomeWithMap(map, iH, iW);
                }
            }
        }
        
        public static void Fill<T>(this Tilemap map, int h, int w, Func<T> getTileBase) where T : TileBase
        {
            map.DoSome(h, w, (m, iH, iW) => m.SetTile(new Vector3Int(iH, iW), getTileBase()));
        }

        public static void Fill<T>(this Tilemap map, int h, int w, Func<bool> predicate,  Func<T> getTileBase) where T : TileBase
        {
            map.DoSome(h, w, (m, iH, iW) =>
            {
                if (predicate()) m.SetTile(new Vector3Int(iW, iH), getTileBase());
            });
        }

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