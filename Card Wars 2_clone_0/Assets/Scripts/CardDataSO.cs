using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

[CreateAssetMenu(fileName = "NewCard", menuName = "Cards")]
public class CardDataSO : ScriptableObject
{
    public String Name;
    public enum CardType
    {
        Creature,
        Building,
        Spell,
        Charm
    }

    public CardType cardType;

    public Sprite MainImage;
    public Sprite Element;

    public int Attack, Defense, Magic;

    public String description;
    
    // public CardEffectSO effect; 
}
