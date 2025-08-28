using System.Collections;
using System.Collections.Generic;
using Mirror;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(CardDisplay))]
public class CardStats : NetworkBehaviour
{
    public CardDataSO cardData;

    [SyncVar] public PlayerStats thisCardOwner;
    
    private CardDisplay display;

    [SyncVar(hook = nameof(UpdateMagic))]   public int magic;
    [SyncVar(hook = nameof(UpdateAttack))]  public int attack;
    [SyncVar(hook = nameof(UpdateDefense))] public int defense;

    public override void OnStartClient()
    {
        base.OnStartClient();
    
        display = GetComponent<CardDisplay>();
        display.InitDisplay(this);
        
        InitCardStats();
        
        // these updates not called via hook (change in stat). 
        // that way, the stat can be zero if so desired from the CardDataSO
        display.UpdateUIMagic(magic);
        display.UpdateUIAttack(attack);
        display.UpdateUIDefense(defense);
    } 

    [Command]
    private void InitCardStats()
    {
        magic = cardData.Magic;
        
        if (cardData.cardType == CardDataSO.CardType.Creature)
        {
            attack = cardData.Attack;
            defense = cardData.Defense;
        }
        else if (cardData.cardType == CardDataSO.CardType.Building)
        {
            attack = -1;
            defense = cardData.Defense;
        }
        else
        {
            attack = -1;
            defense = -1;
        }
    }
    
    public void UpdateMagic(int oldMagic, int newMagic)
    {
        display.UpdateUIMagic(newMagic);
    }

    public void UpdateAttack(int oldAttack, int newAttack)
    {
        display.UpdateUIAttack(newAttack);
    }

    public void UpdateDefense(int oldDefense, int newDefense)
    {
        display.UpdateUIDefense(newDefense);
    }

}
