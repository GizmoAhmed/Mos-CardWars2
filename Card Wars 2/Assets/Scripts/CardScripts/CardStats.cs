using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

[RequireComponent(typeof(CardDisplay))]
public class CardStats : NetworkBehaviour
{
    public CardDataSO cardData;

    private CardDisplay display;

    [SyncVar(hook = nameof(UpdateMagic))]   public int magic;
    [SyncVar(hook = nameof(UpdateAttack))]  public int attack;
    [SyncVar(hook = nameof(UpdateDefense))] public int defense;

    public override void OnStartClient()
    {
        display = GetComponent<CardDisplay>();
        display.Init(this);
        
        magic = cardData.Magic;
        
        if (cardData.cardType == CardDataSO.CardType.Creature)
        { 
            attack = cardData.Attack;
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
