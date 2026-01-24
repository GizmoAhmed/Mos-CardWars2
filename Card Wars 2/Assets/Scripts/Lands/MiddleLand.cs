using CardScripts.CardData;
using CardScripts.CardDisplays;
using CardScripts.CardMovements;
using CardScripts.CardStatss;
using Mirror;
using UnityEngine;

namespace Lands
{
    public class MiddleLand : NetworkBehaviour
    {
        [Tooltip("True, if this players, false if the other side")]
        public bool tileOwner;

        public GameObject creature;
        public GameObject building;

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
                across = FindLand("L" + acrossNum);

                // Adjacent left
                if (col > 0)
                    adjacentLeft = FindLand("L" + (num - 1));

                // Adjacent right
                if (col < 3)
                    adjacentRight = FindLand("L" + (num + 1));

                // Diagonal left
                if (col > 0)
                    diagonalLeft = FindLand("L" + (acrossNum - 1));

                // Diagonal right
                if (col < 3)
                    diagonalRight = FindLand("L" + (acrossNum + 1));
            }
        }

        private GameObject FindLand(string name)
        {
            var found = GameObject.Find(name);
            if (found == null)
                Debug.LogWarning($"Could not find {name} for {gameObject.name}");
            return found;
        }
    }
}