using System.Collections;
using System.Collections.Generic;
using CardScripts;
using Mirror;
using PlayerStuff;
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
                cardStats.thisCardOwner.UseMagic(cardStats.magic);
            }
        }
        
        cardDisplay.FlipCard(true);
        cardMovement.currentLand = this;
    }

    public virtual void DetachCard(GameObject card)
    {
        CardDisplay cardDisplay = card.GetComponent<CardDisplay>();
        CardDataSO cardData = cardDisplay.cardData;

        if ((cardData.cardType == CardDataSO.CardType.Creature)&& (card == creature)) // if the detaching card is the attached card, then we are in the right place
        {
            // so subsequent creatures can be placed here
            creature = null;
        }
        else if (cardData.cardType == CardDataSO.CardType.Building && card == building)
        {
            building = null;
        }

    }

    /// <summary>
    /// If this land has an open creature slot, allow the creature to be placed. Same for building
    /// </summary>
    /// <param name="cardMove"></param>
    /// <returns></returns>
    public virtual bool ValidPlace(CardMovement cardMove)
    {
        CardStats cardStats = cardMove.GetComponent<CardStats>();

        Player cardOwner = cardStats.thisCardOwner.gameObject.GetComponent<Player>();

        // if not your turn, you can't place a card anywhere
        if (cardOwner != null &&
            cardOwner.GetComponent<Player>().myTurn == false)
        {
            return false;
        }

        if (cardStats.magic == 0) // you should still be able to place stuff that cost zero 
        {
            return true;
        }

        if (cardOwner.GetComponent<PlayerStats>().currentMagic <= 0) // if magic is negative, can't place
        {
            return false;
        }

        CardDataSO cardData = cardMove.GetComponent<CardDisplay>().cardData;
        
        // need to have enough magic for active spells
        if (cardData.cardType == CardDataSO.CardType.Spell && cardStats.magic > cardOwner.GetComponent<PlayerStats>().currentMagic) 
        {
            return false;
        }
        
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

        if (cardData.cardType == CardDataSO.CardType.Spell) return true; // Spells can be placed anywhere
        
        return false;
    }
}