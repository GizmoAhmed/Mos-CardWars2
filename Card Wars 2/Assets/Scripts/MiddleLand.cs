using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class MiddleLand : NetworkBehaviour
{
    public GameObject creature;
    public GameObject building;

    [Header("Neighbors")]
    public GameObject across;
    public GameObject adjacentLeft;
    public GameObject adjacentRight;
    public GameObject diagonalLeft;
    public GameObject diagonalRight;

    void Start()
    {
        SetupNeighbors();
    }

    private void SetupNeighbors()
    {
        string myName = gameObject.name; // e.g. "L2"

        if (myName.StartsWith("L") && int.TryParse(myName.Substring(1), out int num))
        {
            // Determine row and column (1-based index)
            // Bottom row = 1–4, Top row = 5–8
            int col = (num - 1) % 4;   // 0..3
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

    public bool ValidPlace(CardMovement card)
    {
        CardDataSO cardData = card.gameObject.GetComponent<CardDisplay>().cardData;

        if (cardData.cardType == CardDataSO.CardType.Building && building == null)
        {
            return true;
        }
        if (cardData.cardType == CardDataSO.CardType.Creature && creature == null)
        {
            return true;
        }
        
        return false;
    }

    public void AttachCard(GameObject card)
    {
        card.transform.SetParent(transform, true);

        card.GetComponent<CardMovement>().onLand = true;
        
        CardDataSO cardData = card.GetComponent<CardDisplay>().cardData;
        
        if (cardData.cardType == CardDataSO.CardType.Creature)
        {
            creature = card;
            card.transform.localPosition = Vector2.zero;
        }
        else if (cardData.cardType == CardDataSO.CardType.Building)
        {
            building = card;
            card.transform.localPosition = new Vector3(-40, -35, 0);
        }
        
        // flip card back over
        card.GetComponent<CardDisplay>().FlipCard(true);
    }
}