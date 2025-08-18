using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class MiddleLand : NetworkBehaviour
{
    public GameObject creature;
    public GameObject building;

    public GameObject across;

    void Start()
    {
        // thanks chatgpt
        // Example: If this object is "L1", we want to find "L5"
        string myName = gameObject.name; // e.g. "L1"
        
        if (myName.StartsWith("L"))
        {
            // Parse the number after 'L'
            if (int.TryParse(myName.Substring(1), out int num))
            {
                int acrossNum = -1;

                // Map local lands 1-4 to 5-8, and vice versa
                if (num <= 4) // 1-4
                {
                    acrossNum = num + 4;
                }
                else // 5-8
                {
                    acrossNum = num - 4;
                }

                // Assign across if valid
                if (acrossNum != -1)
                {
                    string acrossName = "L" + acrossNum;
                    GameObject found = GameObject.Find(acrossName);
                    if (found != null)
                    {
                        across = found;
                    }
                    else
                    {
                        Debug.LogWarning($"Could not find across land {acrossName} for {myName}");
                    }
                }
            }
        }
    }

    public void AttachCard(GameObject card)
    {
        CardDataSO cardData = card.GetComponent<CardDisplay>().cardData;
        
        card.transform.SetParent(transform, true);
        card.transform.localPosition = Vector2.zero;

        if (cardData.cardType == CardDataSO.CardType.Creature)
        {
            creature = card;
        }
        else if (cardData.cardType == CardDataSO.CardType.Building)
        {
            building = card;
        }
    }
}