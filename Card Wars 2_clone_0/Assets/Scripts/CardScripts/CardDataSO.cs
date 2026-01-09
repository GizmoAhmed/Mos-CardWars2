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
        Charm,
        Spell,
        Rune
    }
    public CardType cardType;
    
    public int magic;
    
    [TextArea]
    // EVERY card in game has this
    public string abilityDescription;
    
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
    
    [Tooltip("if -1, not a creature")]
    public int abilityCost; 

    public int burnCost = 2;
    
    /// Learned something new: OnValidate() is like start but for scriptable objects
    private void OnValidate()
    {
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

