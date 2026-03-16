using System.Collections.Generic;
using GameManagement;
using Mirror;
using UnityEngine;

namespace AbilityEvents
{
    public class GlobalAbilityEventManager : MonoBehaviour
    {
        public static GlobalAbilityEventManager GlobalAbilityManagerInstance { get; private set; }
        
        private Dictionary<AbilityEventType, List<System.Action<AbilityEventData>>> _globalListeners;
    
        void Awake()
        {
            if (GlobalAbilityManagerInstance == null)
            {
                GlobalAbilityManagerInstance = this;
                _globalListeners = new Dictionary<AbilityEventType, List<System.Action<AbilityEventData>>>();
            }
            else
            {
                Destroy(gameObject);
            }
        }
        
        public void Subscribe(AbilityEventType eventType, System.Action<AbilityEventData> callback)
        {
            if (!_globalListeners.ContainsKey(eventType))
            {
                _globalListeners[eventType] = new List<System.Action<AbilityEventData>>();
            }
        
            _globalListeners[eventType].Add(callback);
        }
    
        public void GlobalUnsubscribe(AbilityEventType eventType, System.Action<AbilityEventData> callback)
        {
            if (_globalListeners.ContainsKey(eventType))
            {
                _globalListeners[eventType].Remove(callback);
            }
        }
    
        public void TriggerEvents_ForAllSubscribersOfType(AbilityEventData eventData)
        {
            if (_globalListeners.TryGetValue(eventData.EventType, out var listener))
            {
                List<System.Action<AbilityEventData>> callbacks = new List<System.Action<AbilityEventData>>(listener);
            
                foreach (var callback in callbacks)
                {
                    callback?.Invoke(eventData); // invoke that callback, aka, run execute on all cards that are listening to this particular event
                }
            }
        }

        // tell everyone who cares about Buff/Debuff Strength about how this creature got there stats changed
        // the people who care are those who subscribed through AbilityManager.Subscribed(AbilityEventType, ExecutionCallback),
        // see Register passive in CardStats.cs
        [Server]
        public void OnAnyCreatureStrengthBuffed(GameObject creature, int amount)
        {
            AbilityEventData statData = new AbilityEventData(
                AbilityEventType.BuffCreatureStrengthOnTile,
                creature,
                amount);
            
            TriggerEvents_ForAllSubscribersOfType(statData);
        }

        [Server]
        public void OnAnyCreatureStrengthNerfed(GameObject creature, int amount)
        {
            AbilityEventData statData = new AbilityEventData(
                AbilityEventType.DebuffCreatureStrengthOnTile,
                creature,
                amount);
            
            TriggerEvents_ForAllSubscribersOfType(statData);
        }

        [Server]
        public void OnAnyCreatureDefenseBuffed(GameObject creature, int amount)
        {
            AbilityEventData statData = new AbilityEventData(
                AbilityEventType.BuffCreatureDefenseOnTile,
                creature,
                amount);
            
            TriggerEvents_ForAllSubscribersOfType(statData);
        }

        [Server]
        public void OnAnyCreatureDefenseNerfed(GameObject creature, int amount)
        {
            AbilityEventData statData = new AbilityEventData(
                AbilityEventType.DebuffCreatureDefenseOnTile,
                creature,
                amount);
            
            TriggerEvents_ForAllSubscribersOfType(statData);
        }
    }
}
