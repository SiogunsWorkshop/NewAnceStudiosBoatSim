using System.Collections.Generic;
using UnityEngine;

namespace NewAnceBoatSimulation.Runtime.Water
{
    public class WaterTileManager : MonoBehaviour
    {
        [SerializeField] private Transform playerTransform;

        [Space, SerializeField] private GameObject tilePrefab;
        [SerializeField] private float tileSize = 50f;
        [SerializeField] private int tilesAroundPlayer = 3;
        [SerializeField] private int maxDistanceInTilesFromPlayerToUnload = 6;

        private Vector2Int _playerTileCoord;

        private readonly Dictionary<Vector2Int, GameObject> _tiles = new();
        private readonly List<GameObject> _unusedTiles = new();

        private void Awake()
        {
            if (!playerTransform)
            {
                Debug.LogError("Player Transform is not assigned in WaterTileManager.");
                enabled = false;
                return;
            }

            if (!tilePrefab)
            {
                Debug.LogError("Tile Prefab is not assigned in WaterTileManager.");
                enabled = false;
                return;
            }

            var childCount = transform.childCount;
            for (var i = childCount - 1; i >= 0; i--)
            {
                Destroy(transform.GetChild(i).gameObject);
            }

            _playerTileCoord = GetPlayerTileCoord(playerTransform.position);
            EnsureTilesAroundPlayer(playerTransform.position);
        }

        private void Update()
        {
            var currentPlayerTileCoord = GetPlayerTileCoord(playerTransform.position);
            if (currentPlayerTileCoord == _playerTileCoord) return;

            _playerTileCoord = currentPlayerTileCoord;
            EnsureTilesAroundPlayer(playerTransform.position);
            UnloadDistantTiles(playerTransform.position);
        }

        // ReSharper disable once MemberCanBePrivate.Global
        public void EnsureTilesAroundPlayer(Vector3 playerPosition)
        {
            var playerTileCoord = GetPlayerTileCoord(playerPosition);

            for (var x = -tilesAroundPlayer; x <= tilesAroundPlayer; x++)
            {
                for (var z = -tilesAroundPlayer; z <= tilesAroundPlayer; z++)
                {
                    Vector2Int tileCoord = new(playerTileCoord.x + x, playerTileCoord.y + z);
                    if (_tiles.ContainsKey(tileCoord)) continue;

                    var distance = Vector2Int.Distance(tileCoord, playerTileCoord);
                    if (distance > tilesAroundPlayer) continue; // Ensure circular-ish area around player

                    var tilePosition = GetTileWorldPosition(tileCoord);
                    var newTile = GetFreeTile();
                    newTile.transform.position = tilePosition;
                    newTile.SetActive(true);
                    _tiles.Add(tileCoord, newTile);
                }
            }
        }

        // ReSharper disable once MemberCanBePrivate.Global
        public void UnloadDistantTiles(Vector3 playerPosition)
        {
            var playerTileCoord = GetPlayerTileCoord(playerPosition);
            var tilesToRemove = new List<Vector2Int>();

            // ReSharper disable once ForeachCanBeConvertedToQueryUsingAnotherGetEnumerator
            foreach (var tile in _tiles)
            {
                var distance = Vector2Int.Distance(tile.Key, playerTileCoord);
                if (distance > maxDistanceInTilesFromPlayerToUnload)
                {
                    tilesToRemove.Add(tile.Key);
                }
            }

            foreach (var tileCoord in tilesToRemove)
            {
                var tile = _tiles[tileCoord];
                _unusedTiles.Add(tile);
                tile.SetActive(false);
                _tiles.Remove(tileCoord);
            }
        }

        private Vector2Int GetPlayerTileCoord(Vector3 playerPosition)
        {
            return GetTileCoord(playerPosition);
        }

        private Vector2Int GetTileCoord(Vector3 position)
        {
            return new Vector2Int(
                Mathf.FloorToInt((position.x + tileSize / 2) / tileSize),
                Mathf.FloorToInt((position.z + tileSize / 2) / tileSize)
            );
        }

        private Vector3 GetTileWorldPosition(Vector2Int tileCoord)
        {
            return new Vector3(tileCoord.x * tileSize, 0f, tileCoord.y * tileSize);
        }

        private GameObject GetFreeTile()
        {
            if (_unusedTiles.Count <= 0)
            {
                var newTile = Instantiate(tilePrefab, Vector3.zero, Quaternion.identity, transform);
                return newTile;
            }

            var tile = _unusedTiles[0];
            _unusedTiles.RemoveAt(0);
            return tile;
        }
    }
}