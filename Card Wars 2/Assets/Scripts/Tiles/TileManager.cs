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
        
        private const int ColumnsPerSide = 4;   

        private static int OpponentOf(int side) => side == 0 ? 1 : 0;

        [Header("Tile References")] [Tooltip("All grid tiles in the scene (automatically found on Awake)")]
        public List<Tile> allTiles = new List<Tile>();

        [Header("Charm Tiles")] public CharmTile player1CharmTile;
        public CharmTile player2CharmTile;

        // Dictionary for fast O(1) lookups by logical position
        // Key: (playerSide, row, column)
        // Value: Tile reference
        private Dictionary<(int row, int col, int side), Tile> _midTileMap = new Dictionary<(int, int, int), Tile>();

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
            _midTileMap.Clear();

            foreach (Tile tile in foundTiles)
            {
                // Only index grid tiles (not charm tiles)
                if (tile.row >= 0 && tile.column >= 0)
                {
                    allTiles.Add(tile);

                    // Add to dictionary for fast lookup
                    var key = (tile.row, tile.column, playerSide: tile.serverPlayerSide);

                    if (_midTileMap.ContainsKey(key))
                    {
                        Debug.LogWarning($"Duplicate Tile found at position {key}! " +
                                         $"Existing: {_midTileMap[key].gameObject.name}, " +
                                         $"New: {tile.gameObject.name}");
                    }
                    else
                    {
                        _midTileMap[key] = tile;
                    }
                }
                // Handle charm tiles separately
                else if (tile is CharmTile charmTile)
                {
                    if (charmTile.serverPlayerSide == 0)
                    {
                        player1CharmTile = charmTile;
                    }
                    else if (charmTile.serverPlayerSide == 1)
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
            if (column == -1) // we are dealing with a charm here 
            {
                if (playerSide == 0)
                {
                    return player1CharmTile;
                }

                return player2CharmTile;
            }

            // Middle tile
            var key = (row, column, playerSide);

            if (_midTileMap.TryGetValue(key, out Tile tile))
            {
                return tile;
            }

            Debug.LogWarning($"No Tile found at position [Row:{row}][Col:{column}][Player Side:{playerSide}], returning null tile...");
            return null;
        }
        
        /// <summary>
        /// Get all middle tiles for a specific player
        /// </summary>
        public List<MiddleTile> GetMiddleTilesForSide(int playerSide)
        {
            var tiles = new List<MiddleTile>();

            for (int column = 0; column < ColumnsPerSide; column++)
            {
                MiddleTile tile = GetTile(playerSide, column, playerSide) as MiddleTile;   // row == side
                if (tile != null) tiles.Add(tile);
            }

            return tiles;
        }

        public List<MiddleTile> GetAllMiddleTiles()
        {
            var tiles = GetMiddleTilesForSide(0);
            tiles.AddRange(GetMiddleTilesForSide(1));
            return tiles;
        }
        
        public MiddleTile GetLeftmostTile(int playerSide) =>
            GetTile(playerSide, 0, playerSide) as MiddleTile;

        public MiddleTile GetRightmostTile(int playerSide) =>
            GetTile(playerSide, ColumnsPerSide - 1, playerSide) as MiddleTile;

        public MiddleTile GetOpponentLeftmostTile(int mySide) => GetLeftmostTile(OpponentOf(mySide));

        public MiddleTile GetOpponentRightmostTile(int mySide) => GetRightmostTile(OpponentOf(mySide));

        public MiddleTile GetLeftAdjacentTile(MiddleTile thisTile)
        {
            int thisRow = thisTile.row;
            int thisCol = thisTile.column;
            int thisSide = thisTile.serverPlayerSide;
            
            return GetTile(thisRow, thisCol - 1, thisSide) as MiddleTile;
        }
        
        public MiddleTile GetRightAdjacentTile(MiddleTile thisTile)
        {
            int thisRow = thisTile.row;
            int thisCol = thisTile.column;
            int thisSide = thisTile.serverPlayerSide;
            
            return GetTile(thisRow, thisCol + 1, thisSide) as MiddleTile;
        }

        /// <summary>
        /// Get adjacent tiles left and right
        /// </summary>
        public List<MiddleTile> GetAdjacentTiles(int row, int column, int playerSide)
        {
            List<MiddleTile> adjacent = new List<MiddleTile>();

            // Left
            MiddleTile left = GetTile(row, column - 1, playerSide) as MiddleTile;
            if (left != null) adjacent.Add(left);

            // Right
            MiddleTile right = GetTile(row, column + 1, playerSide) as MiddleTile;
            if (right != null) adjacent.Add(right);

            return adjacent;
        }
        
        /// <summary>
        /// Get the middleTile across from this position (opponent's mirror)
        /// </summary>
        public Tile GetAcrossTile(int row, int column, int playerSide)
        {
            // Mirror to opponent's side, same row/column
            int opponentSide = OpponentOf(playerSide);
            row = opponentSide;
            
            return GetTile(row, column, opponentSide);
        }
        
        /// <summary>
        /// Diagonals are the left/right neighbours of the tile directly across
        /// </summary>
        public List<MiddleTile> GetDiagonalTiles(int playerSide, int column)
        {
            int opponentSide = OpponentOf(playerSide) ;
            return GetAdjacentTiles(opponentSide, column, opponentSide);   // row == side, as in GetAcrossTile
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