using Mirror;
using UnityEngine;

namespace Tiles
{
    public class Tile : NetworkBehaviour
    {
        [Tooltip("True, if this players, false if the other side")]
        public bool tileOwner;

        public GameObject creature;
        public GameObject building;
        
        public virtual bool IsOccupied => creature != null; 

        // todo current active card, the one on top

        [Header("Neighbors")] public GameObject across;
        public GameObject adjacentLeft;
        public GameObject adjacentRight;
        public GameObject diagonalLeft;
        public GameObject diagonalRight;

        void Start()
        {
            SetupNeighbors();
        }

        public virtual void SetupNeighbors()
        {
            string myName = gameObject.name; // e.g. "L2"

            if (myName.StartsWith("L") && int.TryParse(myName.Substring(1), out int num))
            {
                // Determine row and column (1-based index)
                // Bottom row = 1–4, Top row = 5–8
                int col = (num - 1) % 4; // 0..3
                bool isBottom = num <= 4;

                // Across (same column, other row)
                int acrossNum = isBottom ? num + 4 : num - 4;
                across = FindTileByName("L" + acrossNum);

                // Adjacent left
                if (col > 0)
                    adjacentLeft = FindTileByName("L" + (num - 1));

                // Adjacent right
                if (col < 3)
                    adjacentRight = FindTileByName("L" + (num + 1));

                // Diagonal left
                if (col > 0)
                    diagonalLeft = FindTileByName("L" + (acrossNum - 1));

                // Diagonal right
                if (col < 3)
                    diagonalRight = FindTileByName("L" + (acrossNum + 1));
            }
        }

        private GameObject FindTileByName(string name)
        {
            var found = GameObject.Find(name);
            if (found == null)
                Debug.LogWarning($"Could not find {name} for {gameObject.name}");
            return found;
        }
    }
}