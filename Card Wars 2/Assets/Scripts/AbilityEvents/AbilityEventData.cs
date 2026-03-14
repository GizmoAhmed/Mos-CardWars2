using System.Collections.Generic;
using UnityEngine;

namespace AbilityEvents
{
    public class AbilityEventData
    {
        public readonly AbilityEventType EventType; // ie AddCardToHand, FieldCardPlaced
        public GameObject cardToBeAffected;
        public int value;                           // keep track of things like buffs
        
        // this is also worth looking into
        // public Dictionary<string, object> customData; // Flexible extra data
        // eventData.customData["buff"] = int, string, or etc.;

        
        public AbilityEventData(AbilityEventType type, GameObject card = null, int v = 0)
        {
            EventType = type;
            cardToBeAffected = card; // optional, some spells ie don't affect cards
            value = v; // optional, pass param for things like buffs
        }
    }
}