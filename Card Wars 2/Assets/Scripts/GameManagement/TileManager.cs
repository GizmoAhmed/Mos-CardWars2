using System.Collections.Generic;
using UnityEngine;
using Tiles;
using Mirror;

namespace GameManagement
{
    /// <summary>
    /// Manages all tiles in the game and provides easy lookup by logical position
    /// </summary>
    public class TileManager : MonoBehaviour
    {
        public static TileManager Instance { get; private set; }
        
        [Header("Tile References")]
        [Tooltip("All grid tiles in the scene (automatically found on Awake)")]
        public List<Tile> allTiles = new List<Tile>();
        
        [Header("Charm Tiles")]
        public CharmTile player1CharmTile;
        public CharmTile player2CharmTile;
        
        // Dictionary for fast O(1) lookups by logical position
        // Key: (playerSide, row, column)
        // Value: Tile reference
        private Dictionary<(int row, int col, int side), Tile> _tileMap = new Dictionary<(int, int, int), Tile>();
        
        void Awake()
        {
            // Singleton pattern
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject); // Optional: persist across scenes
            }
            else
            {
                Destroy(gameObject);
            }
        }
        
        /// <summary>
        /// Find all tiles and index in a dictionary for fast lookup via GetTile
        /// </summary>
        public void MemoizeTiles()
        {
            // Find all Tile components in the scene
            Tile[] foundTiles = FindObjectsOfType<Tile>();
            
            allTiles.Clear();
            _tileMap.Clear();
            
            foreach (Tile tile in foundTiles)
            {
                // Only index grid tiles (not charm tiles)
                if (tile.row >= 0 && tile.column >= 0)
                {
                    allTiles.Add(tile);
                    
                    // Add to dictionary for fast lookup
                    var key = (tile.row, tile.column, tile.playerSide);
                    
                    if (_tileMap.ContainsKey(key))
                    {
                        Debug.LogWarning($"Duplicate Tile found at position {key}! " +
                                       $"Existing: {_tileMap[key].gameObject.name}, " +
                                       $"New: {tile.gameObject.name}");
                    }
                    else
                    {
                        _tileMap[key] = tile;
                    }
                }
                // Handle charm tiles separately
                else if (tile is CharmTile charmTile)
                {
                    if (charmTile.playerSide == 0)
                    {
                        player1CharmTile = charmTile;
                    }
                    else if (charmTile.playerSide == 1)
                    {
                        player2CharmTile = charmTile;
                    }
                }
            }
        }
        
        /// <summary>
        /// Get a tile by its logical position
        /// </summary>
        public Tile GetTile(int row, int column, int playerSide)
        {
            var key = (row, column, playerSide);
            
            if (_tileMap.TryGetValue(key, out Tile tile))
            {
                return tile;
            }
            
            Debug.LogError($"No Tile found at position [Row:{row}][Col:{column}][Player Side:{playerSide}]");
            return null;
        }
        
        /*/// <summary>
        /// Get the creature at a specific logical position
        /// </summary>
        public GameObject GetCreatureAtPosition(int playerSide, int row, int column)
        {
            Tile middleTile = GetTile(playerSide, row, column);
            return middleTile?.logicalCreature;
        }
        
        /// <summary>
        /// Get the building at a specific logical position
        /// </summary>
        public GameObject GetBuildingAtPosition(int playerSide, int row, int column)
        {
            MiddleTile middleTile = GetTile(playerSide, row, column);
            return middleTile?.logicalBuilding;
        }
        
        /// <summary>
        /// Check if a middleTile is occupied (has creature or building)
        /// </summary>
        public bool IsTileOccupied(int playerSide, int row, int column)
        {
            MiddleTile middleTile = GetTile(playerSide, row, column);
            return middleTile != null && middleTile.IsOccupied;
        }
        
        /// <summary>
        /// Get all tiles for a specific player
        /// </summary>
        public List<MiddleTile> GetTilesForPlayer(int playerSide)
        {
            List<MiddleTile> playerTiles = new List<MiddleTile>();
            
            foreach (MiddleTile tile in allTiles)
            {
                if (tile.playerSide == playerSide)
                {
                    playerTiles.Add(tile);
                }
            }
            
            return playerTiles;
        }
        
        /// <summary>
        /// Get adjacent tiles (left, right, up, down)
        /// </summary>
        public List<MiddleTile> GetAdjacentTiles(int playerSide, int row, int column)
        {
            List<MiddleTile> adjacent = new List<MiddleTile>();
            
            // Left
            MiddleTile left = GetTile(playerSide, row, column - 1);
            if (left != null) adjacent.Add(left);
            
            // Right
            MiddleTile right = GetTile(playerSide, row, column + 1);
            if (right != null) adjacent.Add(right);
            
            // Up (higher row)
            MiddleTile up = GetTile(playerSide, row + 1, column);
            if (up != null) adjacent.Add(up);
            
            // Down (lower row)
            MiddleTile down = GetTile(playerSide, row - 1, column);
            if (down != null) adjacent.Add(down);
            
            return adjacent;
        }
        
        /// <summary>
        /// Get diagonal tiles
        /// </summary>
        public List<MiddleTile> GetDiagonalTiles(int playerSide, int row, int column)
        {
            List<MiddleTile> diagonal = new List<MiddleTile>();
            
            // Top-left
            MiddleTile topLeft = GetTile(playerSide, row + 1, column - 1);
            if (topLeft != null) diagonal.Add(topLeft);
            
            // Top-right
            MiddleTile topRight = GetTile(playerSide, row + 1, column + 1);
            if (topRight != null) diagonal.Add(topRight);
            
            // Bottom-left
            MiddleTile bottomLeft = GetTile(playerSide, row - 1, column - 1);
            if (bottomLeft != null) diagonal.Add(bottomLeft);
            
            // Bottom-right
            MiddleTile bottomRight = GetTile(playerSide, row - 1, column + 1);
            if (bottomRight != null) diagonal.Add(bottomRight);
            
            return diagonal;
        }
        
        /// <summary>
        /// Get all tiles surrounding a position (adjacent + diagonal)
        /// </summary>
        public List<MiddleTile> GetSurroundingTiles(int playerSide, int row, int column)
        {
            List<MiddleTile> surrounding = new List<MiddleTile>();
            surrounding.AddRange(GetAdjacentTiles(playerSide, row, column));
            surrounding.AddRange(GetDiagonalTiles(playerSide, row, column));
            return surrounding;
        }
        
        /// <summary>
        /// Get the middleTile across from this position (opponent's mirror)
        /// </summary>
        public MiddleTile GetAcrossTile(int playerSide, int row, int column)
        {
            // Mirror to opponent's side, same row/column
            int opponentSide = playerSide == 0 ? 1 : 0;
            return GetTile(opponentSide, row, column);
        }
        
        /// <summary>
        /// Get all creatures on the board
        /// </summary>
        public List<GameObject> GetAllCreatures()
        {
            List<GameObject> creatures = new List<GameObject>();
            
            foreach (MiddleTile tile in allTiles)
            {
                if (tile.logicalCreature != null)
                {
                    creatures.Add(tile.logicalCreature);
                }
            }
            
            return creatures;
        }
        
        /// <summary>
        /// Get all buildings on the board
        /// </summary>
        public List<GameObject> GetAllBuildings()
        {
            List<GameObject> buildings = new List<GameObject>();
            
            foreach (MiddleTile tile in allTiles)
            {
                if (tile.logicalBuilding != null)
                {
                    buildings.Add(tile.logicalBuilding);
                }
            }
            
            return buildings;
        }
        
        /// <summary>
        /// Get all creatures for a specific player
        /// </summary>
        public List<GameObject> GetCreaturesForPlayer(int playerSide)
        {
            List<GameObject> creatures = new List<GameObject>();
            
            foreach (MiddleTile tile in allTiles)
            {
                if (tile.playerSide == playerSide && tile.logicalCreature != null)
                {
                    creatures.Add(tile.logicalCreature);
                }
            }
            
            return creatures;
        }
        
        /// <summary>
        /// Get charm middleTile for a player
        /// </summary>
        public CharmTile GetCharmTile(int playerSide)
        {
            return playerSide == 0 ? player1CharmMiddleTile : player2CharmMiddleTile;
        }*/
    }
}