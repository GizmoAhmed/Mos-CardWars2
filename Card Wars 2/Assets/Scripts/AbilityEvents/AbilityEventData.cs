using System.Collections.Generic;
using UnityEngine;

namespace AbilityEvents
{
    public class AbilityEventData
    {
        public readonly AbilityEventType EventType; // ie AddCardToHand, AnyFieldCardPlaced
        public GameObject CardToBeAffected;
        public int Value;                           // keep track of things like buffs
        
        public Dictionary<string, object> CustomData; 
        
        public AbilityEventData(AbilityEventType type, GameObject card = null, int v = 0, Dictionary<string, object> customData = null)
        {
            EventType = type;
            CardToBeAffected = card;    // optional, some spells ie don't affect cards
            Value = v;                  // optional, pass param for things like buffs
            CustomData = customData;         // also optional, pass things like tiles, etc.
        }
    }
}