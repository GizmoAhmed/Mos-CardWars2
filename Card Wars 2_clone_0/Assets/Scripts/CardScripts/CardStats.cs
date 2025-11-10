using System.Collections;
using System.Collections.Generic;
using CardScripts;
using Mirror;
using PlayerStuff;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(CardDisplay))]
public class CardStats : NetworkBehaviour
{
    public CardDataSO cardData;

    [SyncVar] public PlayerStats thisCardOwner;
    
    private CardDisplay _display;

    [SyncVar(hook = nameof(UpdateMagic))]   public int magic;
    [SyncVar(hook = nameof(UpdateAttack))]  public int attack;
    [SyncVar(hook = nameof(UpdateDefense))] public int defense;
    
    [SyncVar(hook = nameof(UpdateCost))] public int abilityCost;
    
    // TODO public Charm ActiveCharm

    public override void OnStartClient()
    {
        base.OnStartClient();
    
        _display = GetComponent<CardDisplay>();
        _display.InitDisplay(this);
        
        RefreshCardStats();
        
        // these updates not called via hook (change in stat). 
        // that way, the stat can be zero if so desired from the CardDataSO
        _display.UpdateUIMagic(magic);
        _display.UpdateUIAttack(attack);
        _display.UpdateUIDefense(defense);
    } 

    /// <summary>
    /// Applies the stats from the CardDataSO
    /// </summary>
    [Command]
    private void RefreshCardStats()
    {
        magic = cardData.magic;
        
        if (cardData.cardType == CardDataSO.CardType.Creature)
        {
            attack = cardData.attack;
            defense = cardData.defense;
        }
        else if (cardData.cardType == CardDataSO.CardType.Building)
        {
            attack = -1;
            defense = cardData.defense;
        }
        else // spell or charm
        {
            attack = -1;
            defense = -1;
        }
    }
    
    public void UpdateMagic(int oldMagic, int newMagic)
    {
        _display.UpdateUIMagic(newMagic);
    }

    public void UpdateAttack(int oldAttack, int newAttack)
    {
        _display.UpdateUIAttack(newAttack);
    }

    public void UpdateDefense(int oldDefense, int newDefense)
    {
        _display.UpdateUIDefense(newDefense);
    }

    public void UpdateCost(int oldCost, int newCost)
    {
        _display.UpdateUICost(newCost);
    }
}
