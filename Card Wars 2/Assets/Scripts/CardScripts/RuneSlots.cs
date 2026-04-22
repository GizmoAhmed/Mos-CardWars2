using CardScripts.CardData;
using CardScripts.CardDisplays;
using CardScripts.CardMovements;
using CardScripts.CardStats_Folder;
using Mirror;
using UnityEngine;

namespace CardScripts
{
    /// <summary>
    /// This class is a component of a child in base creature card
    /// Keeps track of a creatures rune(s)
    /// Holds them as children in order for them to subscribe listen
    /// </summary>
    public class RuneSlots : NetworkBehaviour
    {
        [HideInInspector] public CreatureDisplay creatureDisplay;
        
        [Header("Rune Stuff")] [SyncVar(hook = nameof(UIRuneChange))]
        public GameObject currentRune1;

        public bool overRuneable;

        [SyncVar(hook = nameof(UIRuneChange))] public GameObject currentRune2;

        public bool CanBeRuned => (currentRune1 == null) || (overRuneable && currentRune2 == null);
        
        // Start is called before the first frame update
        void Start()
        {
            creatureDisplay = GetComponentInParent<CreatureDisplay>();

            if (creatureDisplay == null)
            {
                Debug.LogWarning($"RuneSlots on {transform.parent.gameObject.name} requires a CreatureDisplay");
            }
        }
        
        public void BindRune(GameObject runeCard)
        {
            // register rune's passive ability
            PassiveListenerCard listener = runeCard.GetComponent<PassiveListenerCard>();
            
            if (listener != null)
                listener.RegisterPassiveAbility();
            else
                Debug.LogError($"No listener found on {runeCard.name}");
            
            // set variables in here
            if (currentRune1 == null) 
            {
                currentRune1 = runeCard; // goes to RuneChange
            }
            else if (currentRune2 == null && overRuneable)
            {
                currentRune2 = runeCard; // goes to RuneChange
            }
            
            // add rune as child (slot) then hide
            RpcSlotChildRuneCard(runeCard);
        }

        [ClientRpc]
        private void RpcSlotChildRuneCard(GameObject rune)
        {
            // visual things like 'parenting' and 'SetActive' need to be done for both clients
            rune.transform.SetParent(transform);
            rune.SetActive(false);
        }

        public void UIRuneChange(GameObject oldRune, GameObject newRune)
        {
            creatureDisplay.DisplayRune(newRune);
        }

        public void UnbindAllRunes()
        {
            currentRune1 = null;
            currentRune2 = null;
            
            // discard each slotted rune
            foreach (Transform rune in transform)
            {
                RuneMovement runeMove = rune.GetComponent<RuneMovement>();
                runeMove.ServerDiscard();
                
                // they are still hidden from the initial bind, activate them to show on discard board
                RpcShowRune(rune.gameObject);
            }
        }

        [ClientRpc]
        private void RpcShowRune(GameObject rune)
        {
            rune.gameObject.SetActive(true);
        }
    }
}
