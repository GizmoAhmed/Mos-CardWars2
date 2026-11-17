using CardScripts.CardData;
using CardScripts.CardStats_Folder;
using UnityEngine;

namespace CardScripts.CardDisplays
{
    public class SpellDisplay : CardDisplay
    {
        private GameObject _usesUIObject;
        private GameObject _xUIObject;

        protected override void FindDisplayParts()
        {
            base.FindDisplayParts();
            
            _usesUIObject = FindPart("Uses");
            _xUIObject = FindPart("x");

            if (_usesUIObject == null)
            {
                Debug.LogError($"The Uses UI object is null on {gameObject.name}. Make sure to set it in the <color=blue>inspector</color>");
            }

            if (_xUIObject == null)
            {
                Debug.LogError($"The little x object is null on {gameObject.name}. Make sure to set it in the <color=blue>inspector</color>");
            }
        }

        public override void SetDisplayElements_UsingData(CardStats s)
        {
            base.SetDisplayElements_UsingData(s);
            
            // spell specific stuff: 
            
            SpellDataSO spellData = s.CardData as SpellDataSO;

            if (spellData == null)
            {
                Debug.LogError($"SpellData so is null on {gameObject.name}");
                return;
            }

            UpdateUIUses(spellData.uses);
        }

        protected override void ShowCardFlip(bool up)
        {
            base.ShowCardFlip(up);
            
            _usesUIObject.SetActive(up);
            _xUIObject.SetActive(up);
        }

        public void UpdateUIUses(int newUses)
        {
            SetText(_usesUIObject, newUses.ToString());
        }
    }
}