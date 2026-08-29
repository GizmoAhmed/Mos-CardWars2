using CardScripts.CardData;
using Mirror;
using UnityEngine;

namespace CardScripts.CardStats_Folder
{
    public class SpellStats : CardStats
    {
        [Header("Consumable Specific Stats")] 
        [SyncVar] public int uses;

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
    }
}