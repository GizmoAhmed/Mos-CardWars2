using System.Collections.Generic;
using UnityEngine;

namespace AbilityEvents
{
    public class AbilityEventData
    {
        public readonly AbilityEventType EventType;
        public GameObject CardOfOrigin;
    
        public AbilityEventData(AbilityEventType type, GameObject cardOfOrigin)
        {
            EventType = type;
            CardOfOrigin = cardOfOrigin;
        }
    }
}