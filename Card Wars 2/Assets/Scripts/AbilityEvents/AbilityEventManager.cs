using System.Collections.Generic;
using GameManagement;
using UnityEngine;

namespace AbilityEvents
{
    public class AbilityEventManager : MonoBehaviour
    {
        public static AbilityEventManager AbilityManagerInstance { get; private set; }
        
        private Dictionary<AbilityEventType, List<System.Action<AbilityEventData>>> _listeners;
    
        void Awake()
        {
            if (AbilityManagerInstance == null)
            {
                AbilityManagerInstance = this;
                _listeners = new Dictionary<AbilityEventType, List<System.Action<AbilityEventData>>>();
            }
            else
            {
                Destroy(gameObject);
            }
        }
        
        public void Subscribe(AbilityEventType eventType, System.Action<AbilityEventData> callback)
        {
            if (!_listeners.ContainsKey(eventType))
            {
                _listeners[eventType] = new List<System.Action<AbilityEventData>>();
            }
        
            _listeners[eventType].Add(callback);
        }
    
        public void Unsubscribe(AbilityEventType eventType, System.Action<AbilityEventData> callback)
        {
            if (_listeners.ContainsKey(eventType))
            {
                _listeners[eventType].Remove(callback);
            }
        }
    
        public void TriggerEvent(AbilityEventData eventData)
        {
            if (_listeners.TryGetValue(eventData.EventType, out var listener))
            {
                List<System.Action<AbilityEventData>> callbacks = new List<System.Action<AbilityEventData>>(listener);
            
                foreach (var callback in callbacks)
                {
                    callback?.Invoke(eventData); // invoke that callback, aka, run execute on all cards that are listening to this particular event
                }
            }
        }
    }
}
