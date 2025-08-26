using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class MiddleLand : NetworkBehaviour
{
    public GameObject creature;
    public GameObject building;
    
    // todo current active card, the one on top

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

    public virtual void SetupNeighbors()
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

    public virtual void AttachCard(GameObject card)
    {
        CardMovement cardMovement = card.GetComponent<CardMovement>();
        CardDisplay cardDisplay = card.GetComponent<CardDisplay>();
        CardDataSO cardData = cardDisplay.cardData;
        CardStats cardStats = card.GetComponent<CardStats>();

        card.transform.SetParent(transform, true);

        if (cardData.cardType == CardDataSO.CardType.Creature || 
            cardData.cardType == CardDataSO.CardType.Building)
        {
            if (cardData.cardType == CardDataSO.CardType.Creature)
            {
                creature = card;
                card.transform.localPosition = Vector2.zero;
                card.transform.SetAsLastSibling(); // above building
                
                if (cardStats?.thisCardOwner != null)
                {
                    int creatureScore = cardStats.attack + cardStats.defense;
                    cardStats.thisCardOwner.AddScore(creatureScore);
                } 
            }
            else 
            {
                building = card;
                card.transform.localPosition = new Vector3(-40, -35, 0);
                card.transform.SetAsFirstSibling(); // below creature
            }

            if (cardStats?.thisCardOwner != null)
            {
                cardStats.thisCardOwner.AddMagic(cardStats.magic);
            }
        }


        // playersStats.currentMagic += cardStats.magic;
        
        cardDisplay.FlipCard(true);
        cardMovement.onLand = true;
    }

    public virtual void DetachCard(GameObject card)
    {
        // TODO undo all the attach stuff, also make the AddMagic function a negative so it'll subtract
    }

    /// <summary>
    /// If this land has an open creature slot, allow the creature to be placed. Same for building
    /// </summary>
    /// <param name="card"></param>
    /// <returns></returns>
    public virtual bool ValidPlace(CardMovement card)
    {
        CardStats cardStats = card.GetComponent<CardStats>();

        // if not your turn, you can't place a card anywhere
        if (cardStats.thisCardOwner != null &&
            cardStats.thisCardOwner.GetComponent<Player>().myTurn == false)
        {
            return false;
        }

        CardDataSO cardData = card.GetComponent<CardDisplay>().cardData;
        
        if (cardData.cardType == CardDataSO.CardType.Building && building == null &&
            (gameObject.name.EndsWith("1") ||
             gameObject.name.EndsWith("2") ||
             gameObject.name.EndsWith("3") ||
             gameObject.name.EndsWith("4")) )
            return true;

        if (cardData.cardType == CardDataSO.CardType.Creature && creature == null &&
            (gameObject.name.EndsWith("1") ||
             gameObject.name.EndsWith("2") ||
             gameObject.name.EndsWith("3") ||
             gameObject.name.EndsWith("4")) )
            return true;

        // cast spells can be placed anywhere, where they immediately discarded upon cast
        if (cardData.spellType == CardDataSO.SpellType.Cast) return true;

        return false;
    }
}