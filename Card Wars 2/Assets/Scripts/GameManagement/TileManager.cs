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
        private Dictionary<(int side, int row, int col), Tile> _tileMap = new Dictionary<(int, int, int), Tile>();
        
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
        /// Find all tiles and index them for fast lookup
        /// </summary>
        public void InitializeTiles()
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
                    var key = (tile.playerSide, tile.row, tile.column);
                    
                    if (_tileMap.ContainsKey(key))
                    {
                        Debug.LogWarning($"Duplicate tile found at position {key}! " +
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
            
            // Debug.Log($"TileManager initialized with {allTiles.Count} grid tiles");
            // Debug.Log($"Player 1 charm tile: {(player1CharmTile != null ? player1CharmTile.gameObject.name : "Not found")}");
            // Debug.Log($"Player 2 charm tile: {(player2CharmTile != null ? player2CharmTile.gameObject.name : "Not found")}");
        }
        
        /// <summary>
        /// Get a tile by its logical position
        /// </summary>
        public Tile GetTile(int playerSide, int row, int column)
        {
            var key = (playerSide, row, column);
            
            if (_tileMap.TryGetValue(key, out Tile tile))
            {
                return tile;
            }
            
            Debug.LogWarning($"No tile found at position [Side:{playerSide}][Row:{row}][Col:{column}]");
            return null;
        }
        
        /// <summary>
        /// Get the creature at a specific logical position
        /// </summary>
        public GameObject GetCreatureAtPosition(int playerSide, int row, int column)
        {
            Tile tile = GetTile(playerSide, row, column);
            return tile?.logicalCreature;
        }
        
        /// <summary>
        /// Get the building at a specific logical position
        /// </summary>
        public GameObject GetBuildingAtPosition(int playerSide, int row, int column)
        {
            Tile tile = GetTile(playerSide, row, column);
            return tile?.logicalBuilding;
        }
        
        /// <summary>
        /// Check if a tile is occupied (has creature or building)
        /// </summary>
        public bool IsTileOccupied(int playerSide, int row, int column)
        {
            Tile tile = GetTile(playerSide, row, column);
            return tile != null && tile.IsOccupied;
        }
        
        /// <summary>
        /// Get all tiles for a specific player
        /// </summary>
        public List<Tile> GetTilesForPlayer(int playerSide)
        {
            List<Tile> playerTiles = new List<Tile>();
            
            foreach (Tile tile in allTiles)
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
        public List<Tile> GetAdjacentTiles(int playerSide, int row, int column)
        {
            List<Tile> adjacent = new List<Tile>();
            
            // Left
            Tile left = GetTile(playerSide, row, column - 1);
            if (left != null) adjacent.Add(left);
            
            // Right
            Tile right = GetTile(playerSide, row, column + 1);
            if (right != null) adjacent.Add(right);
            
            // Up (higher row)
            Tile up = GetTile(playerSide, row + 1, column);
            if (up != null) adjacent.Add(up);
            
            // Down (lower row)
            Tile down = GetTile(playerSide, row - 1, column);
            if (down != null) adjacent.Add(down);
            
            return adjacent;
        }
        
        /// <summary>
        /// Get diagonal tiles
        /// </summary>
        public List<Tile> GetDiagonalTiles(int playerSide, int row, int column)
        {
            List<Tile> diagonal = new List<Tile>();
            
            // Top-left
            Tile topLeft = GetTile(playerSide, row + 1, column - 1);
            if (topLeft != null) diagonal.Add(topLeft);
            
            // Top-right
            Tile topRight = GetTile(playerSide, row + 1, column + 1);
            if (topRight != null) diagonal.Add(topRight);
            
            // Bottom-left
            Tile bottomLeft = GetTile(playerSide, row - 1, column - 1);
            if (bottomLeft != null) diagonal.Add(bottomLeft);
            
            // Bottom-right
            Tile bottomRight = GetTile(playerSide, row - 1, column + 1);
            if (bottomRight != null) diagonal.Add(bottomRight);
            
            return diagonal;
        }
        
        /// <summary>
        /// Get all tiles surrounding a position (adjacent + diagonal)
        /// </summary>
        public List<Tile> GetSurroundingTiles(int playerSide, int row, int column)
        {
            List<Tile> surrounding = new List<Tile>();
            surrounding.AddRange(GetAdjacentTiles(playerSide, row, column));
            surrounding.AddRange(GetDiagonalTiles(playerSide, row, column));
            return surrounding;
        }
        
        /// <summary>
        /// Get the tile across from this position (opponent's mirror)
        /// </summary>
        public Tile GetAcrossTile(int playerSide, int row, int column)
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
            
            foreach (Tile tile in allTiles)
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
            
            foreach (Tile tile in allTiles)
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
            
            foreach (Tile tile in allTiles)
            {
                if (tile.playerSide == playerSide && tile.logicalCreature != null)
                {
                    creatures.Add(tile.logicalCreature);
                }
            }
            
            return creatures;
        }
        
        /// <summary>
        /// Get charm tile for a player
        /// </summary>
        public CharmTile GetCharmTile(int playerSide)
        {
            return playerSide == 0 ? player1CharmTile : player2CharmTile;
        }
        
        /*/// <summary>
        /// Debug: Print board state
        /// </summary>
        [ContextMenu("Debug Print Board State")]
        public void DebugPrintBoardState()
        {
            Debug.Log("===== BOARD STATE =====");
            
            for (int side = 0; side <= 1; side++)
            {
                Debug.Log($"\n--- Player {side + 1} Side ---");
                
                // Assuming 2 rows, 4 columns
                for (int row = 1; row >= 0; row--)
                {
                    string rowString = "";
                    for (int col = 0; col < 4; col++)
                    {
                        Tile tile = GetTile(side, row, col);
                        
                        if (tile != null)
                        {
                            string occupant = "Empty";
                            
                            if (tile.logicalCreature != null)
                            {
                                CreatureStats creature = tile.logicalCreature.GetComponent<CreatureStats>();
                                occupant = creature != null ? $"C:{creature.cardData.cardName}" : "C:?";
                            }
                            else if (tile.logicalBuilding != null)
                            {
                                BuildingStats building = tile.logicalBuilding.GetComponent<BuildingStats>();
                                occupant = building != null ? $"B:{building.cardData.cardName}" : "B:?";
                            }
                            
                            rowString += $"[{tile.gameObject.name}:{occupant}] ";
                        }
                        else
                        {
                            rowString += "[NULL] ";
                        }
                    }
                    Debug.Log(rowString);
                }
            }
            
            Debug.Log("======================");
        }*/
    }
}