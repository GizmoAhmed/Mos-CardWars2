using System.Collections.Generic;
using UnityEngine;

namespace AbilityEvents
{
    public class AbilityEventData
    {
        public readonly AbilityEventType EventType;
        public GameObject sourceCard;
    
        public AbilityEventData(AbilityEventType type, GameObject card)
        {
            EventType = type;
            sourceCard = card;
        }
    }
}