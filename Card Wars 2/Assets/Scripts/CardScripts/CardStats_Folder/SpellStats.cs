using CardScripts.CardData;
using CardScripts.CardDisplays;
using Mirror;
using UnityEngine;

namespace CardScripts.CardStats_Folder
{
    public class SpellStats : CardStats
    {
        private SpellDisplay _spellDisplay;

        [Header("Consumable Specific Stats")] [SyncVar(hook = nameof(Hook_UpdateUsesUI))]
        public int uses;

        protected override void Awake()
        {
            base.Awake();

            _spellDisplay = Display as SpellDisplay;

            if (_spellDisplay == null)
            {
                Debug.LogError($"Spell Stats on {gameObject.name} has no spell display");
            }
        }

        public override void SetStats_FromData()
        {
            base.SetStats_FromData();

            SpellDataSO data = cardData as SpellDataSO;

            if (data != null)
            {
                if (data.uses <= 0)
                {
                    Debug.LogError($"Spell data on {gameObject.name} has <color=red>uses set to 0</color>");
                }

                uses = data.uses;
            }
            else
            {
                Debug.LogError($"{gameObject.name}: card data was null when retrieved here");
            }
        }

        public override void SetAndApplyCardData(CardDataSO data, bool serverCall)
        {
            base.SetAndApplyCardData(data, serverCall);

            if (!serverCall)
            {
                _spellDisplay.UpdateUIUses(uses);
            }
        }

        /// <summary>
        /// called from server when the uses variable changes, then runs this on both clients
        /// </summary>
        /// <returns></returns>
        public void Hook_UpdateUsesUI(int old, int updated)
        {
            _spellDisplay.UpdateUIUses(updated);
        }
    }
}