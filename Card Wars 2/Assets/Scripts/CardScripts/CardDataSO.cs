using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

[CreateAssetMenu(fileName = "~~ RENAME ME ~~", menuName = "New CardDataSO")]
public class CardDataSO : ScriptableObject
{
    [Header("General")]
    public string cardName;

    public Sprite mainImage;
    
    public enum CardType
    {
        Creature,
        Building,
        Spell,
        Charm
    }
    public CardType cardType;
    
    public int magic;
    
    [TextArea]
    // EVERY card in game has this
    public string abilityDescription;
    
    // public CardEffectSO cardEffect;

    [Header("Spell Specific")]
    public SpellType spellType;
    public enum SpellType
    {
        Active,
        Passive,
        None
    }
    
    [Header("Creature Specific")]
    public Element element;
    public enum Element
    {
        Forge,
        Spirit,
        Haunted,
        Crystal,
        Occult,
        None
    }
    
    public Sprite elementSprite;

    public int attack, defense;
    
    [Tooltip("if -1, then either building, spell, or passive-ability creatures")]
    public int abilityCost; // only some creatures have this
    
    /// Learned something new: OnValidate() is like start but for scriptable objects
    private void OnValidate()
    {
        // if this card isn't a Spell, force SpellType to None
        if (cardType != CardType.Spell)
        {
            spellType = SpellType.None;
        }
        else if (cardType == CardType.Spell && spellType == SpellType.None)
        {
            Debug.LogError($"{this} is a spell, but no spell type was set, Setting to passive by default");
            spellType = SpellType.Passive;
        }

        if (cardType != CardType.Creature)
        {
            element = Element.None;
        }

        // only creatures can have ability costs, all else get -1
        if (cardType != CardType.Creature)
        {
            abilityCost = -1;
        }
    }
}

