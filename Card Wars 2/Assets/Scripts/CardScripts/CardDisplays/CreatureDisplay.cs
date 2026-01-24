using CardScripts.CardData;
using CardScripts.CardMovements;
using CardScripts.CardStatss;
using UnityEngine;

namespace CardScripts.CardDisplays
{
    public class CreatureDisplay : CardDisplay
    {
        private GameObject _element;

        private GameObject _attackObj;
        private GameObject _defenseObj;
        private GameObject _scoreObj;

        private GameObject _elementObj;

        private GameObject activateButton;
        private GameObject abilityCost;

        public override void InitDisplay(CardStats s)
        {
            cardData = s.CardData as CreatureDataSO;
            
            if (cardData == null)
            {
                Debug.LogError($"creatureData is null on {gameObject.name}");
                return;
            }
            
            // ------------- to get around null check in base class ---------------------
            // find all object variables above
            FindDisplayParts();

            // Set images
            SetImage(_mainImageObj, cardData.mainImage);

            // Set names
            SetText(_nameTop, cardData.cardName);

            // set description
            GameObject descTextChild = abilityDesc.transform.GetChild(0).gameObject; // <-- child of AbilityDesc
            SetText(descTextChild, cardData.abilityDescription);

            FlipCard(face: true);

            Hide(infoObj); // initially hide the info card

            cardInfoHandler = FindObjectOfType<CardInfoHandler>();
            // -----------------------------------------------------
            
            CreatureDataSO creatureData = s.CardData as CreatureDataSO;
            
            SetImage(_elementObj, creatureData.elementSprite);

            GameObject activateButtonText = activateButton.transform.GetChild(0).gameObject;
            SetText(activateButtonText, creatureData.abilityCost.ToString());
        }

        protected override void FindDisplayParts()
        {
            base.FindDisplayParts();

            _elementObj = FindPart("Element");
            _attackObj = FindPart("Attack");
            _defenseObj = FindPart("Defense");
            _scoreObj = FindPart("Score");

            if (infoObj != null)
            {
                activateButton = FindPart("ActivateButton", infoRight.transform);
                abilityCost = FindPart("AbilityCostText", activateButton.transform);
            }
            else
            {
                Debug.LogWarning($"Missing {infoObj.name}, can't find the rest of the info");
            }
        }

        protected override void ShowCardFlip(bool up)
        {
            base.ShowCardFlip(up);

            _elementObj.SetActive(up);

            _attackObj.SetActive(up);
            _defenseObj.SetActive(up);
            _scoreObj.SetActive(up);

            _magicObj.SetActive(up);

            _cardBackObj.SetActive(!up);
        }

        public override void ToggleInfoSlide(bool toggle = true)
        {
            base.ToggleInfoSlide(toggle);

            if (infoObj.activeInHierarchy && GetComponentInParent<BaseMovement>().cardState == BaseMovement.CardState.Field) 
            {
                gameObject.transform.SetAsLastSibling();
            }
        }
        
        public void UpdateUIAttack(int newAttack)
        {
            SetText(_attackObj, newAttack.ToString(), true);
        }

        public void UpdateUIDefense(int newDefense)
        {
            SetText(_defenseObj, newDefense.ToString(), true);
        }

        public void UpdateUI_AbilityCost(int newCost)
        {
            SetText(abilityCost, newCost.ToString(), true);
        }
        
        public void UpdateUI_Score(int newScore)
        {
            SetText(_scoreObj, newScore.ToString(), true);
        }
    }
}