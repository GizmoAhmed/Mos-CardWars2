using System.Collections.Generic;
using GameManagement;
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
    
        public void Unsubscribe(AbilityEventType eventType, System.Action<AbilityEventData> callback)
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
    }
}
