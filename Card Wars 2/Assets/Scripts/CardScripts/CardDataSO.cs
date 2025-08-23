using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

[CreateAssetMenu(fileName = "RENAME", menuName = "New CardDataSO")]
public class CardDataSO : ScriptableObject
{
    public string Name;

    public enum CardType
    {
        Creature,
        Building,
        Spell,
        Charm
    }
    public CardType cardType;

    public enum SpellType
    {
        Active,
        Cast,
        None
    }
    public SpellType spellType;

    public Sprite MainImage;
    public Sprite Element;

    public int Attack, Defense, Magic;

    [TextArea]
    public string description;

    // Learned something new
    private void OnValidate()
    {
        // if this card isn't a Spell, force SpellType to None
        if (cardType != CardType.Spell)
        {
            spellType = SpellType.None;
        }
        else if (cardType == CardType.Spell && spellType == SpellType.None)
        {
            Debug.LogError($"{this} is a spell, but not spell type was set");
        }
    }
    
    // public CardEffectSO cardEffect;
}

