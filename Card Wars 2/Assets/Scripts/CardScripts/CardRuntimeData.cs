using Mirror;
using UnityEngine;

namespace CardScripts
{
    public class CardRuntimeData : NetworkBehaviour
    {
        [Header("Turn Tracking")]
        [SyncVar] public int turnsOnField = 0;
        
        [Header("General Purpose")]
        // For anything that doesn't fit above
        // Key: variable name, Value: current value
        public readonly SyncDictionary<string, int> customInts 
            = new SyncDictionary<string, int>();

        /// <summary>
        /// Called by PlayerCardTracker.cs at the end of each turn
        /// Increment variables in this runtime data all at once
        /// </summary>
        [Server]
        public void EndOfTurn_RunTimeDataIncrement()
        {
            turnsOnField++;
        }

        /// <summary>
        /// When a card is discarded reset its data
        /// Anything you want rest on discard goes here
        /// </summary>
        [Server]
        public void ResetCardRunTimeData()
        {
            turnsOnField = 0;
            
            customInts.Clear();
        }
    }
}
