using System;
using System.Collections.Generic;
using Mirror;
using Tiles;
using UnityEngine;

namespace AbilityEvents
{
    public class TileEventManager : NetworkBehaviour
    {
        private Tile _tile;

        public Dictionary<AbilityEventType, List<System.Action<AbilityEventData>>> _localListeners;

        public void InitTileEventManager(Tile t)
        {
            _tile = t;
            _localListeners = new Dictionary<AbilityEventType, List<System.Action<AbilityEventData>>>();
        }

        public void SubscribeToTileEvent(AbilityEventType eventType, Action<AbilityEventData> callback)
        {
            if (!_localListeners.ContainsKey(eventType))
            {
                _localListeners[eventType] = new List<Action<AbilityEventData>>();
            }
            
            _localListeners[eventType].Add(callback);
            // Debug.Log($"Tile [{_tile.playerSide}][{_tile.row},{_tile.column}]: Subscribed to {eventType}");
        }
        
        public void UnsubscribeFromTileEvent(AbilityEventType eventType, Action<AbilityEventData> callback)
        {
            if (_localListeners.ContainsKey(eventType))
            {
                _localListeners[eventType].Remove(callback);
            }
        }
        
        [Server]
        private void TriggerTileEvent(AbilityEventData eventData)
        {
            if (_localListeners.ContainsKey(eventData.EventType))
            {
                List<Action<AbilityEventData>> callbacks = 
                    new List<Action<AbilityEventData>>(_localListeners[eventData.EventType]);
                
                Debug.Log($"Tile [{_tile.playerSide}][{_tile.row},{_tile.column}]: Triggering {eventData.EventType} for {callbacks.Count} local listeners");
                
                foreach (var callback in callbacks)
                {
                    callback?.Invoke(eventData);
                }
            }
        }
        
        [Server]
        public void OnCardPlacedOnTile(GameObject placedCard)
        {
            AbilityEventData eventData = new AbilityEventData(
                AbilityEventType.CardPlacedOnTile,
                placedCard
            );
            
            TriggerTileEvent(eventData);
        }
        
        [Server]
        public void OnCreatureBurnedOnTile(GameObject burnedCreature)
        {
            AbilityEventData eventData = new AbilityEventData(
                AbilityEventType.CreatureBurnedOnTile,
                burnedCreature
            );
            
            TriggerTileEvent(eventData);
        }

        [Server]
        public void OnBuffCreatureStrengthOnTile(GameObject creature, int amount)
        {
            AbilityEventData statData = new AbilityEventData(
                AbilityEventType.BuffCreatureStrengthOnTile,
                creature,
                amount);
            
            TriggerTileEvent(statData);
        }

        [Server]
        public void OnNerfCreatureStrengthOnTile(GameObject nerfCreature, int amount)
        {
            AbilityEventData statData = new AbilityEventData(
                AbilityEventType.DebuffCreatureStrengthOnTile,
                nerfCreature,
                amount);
            
            TriggerTileEvent(statData);
        }
        
        [Server]
        public void OnBuffCreatureDefenseOnTile(GameObject creature, int amount)
        {
            AbilityEventData statData = new AbilityEventData(
                AbilityEventType.BuffCreatureDefenseOnTile,
                creature,
                amount);
            
            TriggerTileEvent(statData);
        }

        [Server]
        public void OnNerfCreatureDefenseOnTile(GameObject nerfCreature, int amount)
        {
            AbilityEventData statData = new AbilityEventData(
                AbilityEventType.DebuffCreatureDefenseOnTile,
                nerfCreature,
                amount);
            
            TriggerTileEvent(statData);
        }
    }
}