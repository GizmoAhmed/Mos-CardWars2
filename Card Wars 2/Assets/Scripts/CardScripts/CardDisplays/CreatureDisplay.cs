using CardScripts.CardData;
using CardScripts.CardMovements;
using CardScripts.CardStats_Folder;
using CardScripts.CardStatss;
using UnityEngine;

namespace CardScripts.CardDisplays
{
    public class CreatureDisplay : CardDisplay
    {
        private GameObject _element;

        private GameObject _strengthObj;
        private GameObject _defenseObj;
        private GameObject _scoreObj;

        private GameObject _elementObj;

        private GameObject activateButton;
        private GameObject abilityCost;

        private GameObject _runeTab1;
        private GameObject _runeText1;
        private GameObject _runeName1;
        private GameObject _runeInfoCardIcon1;
        
        private GameObject _runeTab2;
        private GameObject _runeText2;
        private GameObject _runeName2;
        private GameObject _runeInfoCardIcon2;
        
        private GameObject runeIconFace1;
        private GameObject runeIconFace2;

        private CreatureStats _creatureStats;

        public override void InitDisplayWithData(CardStats s)
        {
            cardData = s.CardData as CreatureDataSO;

            if (cardData == null)
            {
                Debug.LogError($"creatureData is null on {gameObject.name}");
                return;
            }

            _creatureStats = GetComponent<CreatureStats>();

            // ------------- to get around null check in base class ---------------------
            // find all object variables above
            FindDisplayParts();

            // Set images
            SetImage(MainImageObj, cardData.mainImage);

            // Set names
            SetText(NameTop, cardData.cardName);

            // set description
            GameObject descTextChild = AbilityDesc.transform.GetChild(0).gameObject; // <-- child of AbilityDesc
            SetText(descTextChild, cardData.abilityDescription);

            FlipCard(face: true);

            Hide(InfoObj); // initially hide the info card

            CardInfoHandler = FindObjectOfType<CardInfoHandler>();
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
            _strengthObj = FindPart("Attack");
            _defenseObj = FindPart("Defense");
            _scoreObj = FindPart("Score");

            GameObject currentRunes = FindPart("CurrentRuneIcons");

            if (InfoObj != null)
            {
                activateButton = FindPart("ActivateButton", InfoRight.transform);
                abilityCost = FindPart("AbilityCostText", activateButton.transform);

                _runeTab1 = FindPart("RuneTab1", InfoLeft.transform);
                _runeTab2 = FindPart("RuneTab2", InfoLeft.transform);

                if (_runeTab1 != null)
                {
                    _runeText1 = FindPart("RuneDescText1", _runeTab1.transform);
                    _runeName1 = FindPart("RuneName1", _runeTab1.transform);
                    _runeInfoCardIcon1 = FindPart("RuneIcon1", _runeTab1.transform);
                }

                if (_runeTab2 != null)
                {
                    _runeText2 = FindPart("RuneDescText2", _runeTab2.transform);
                    _runeName2 = FindPart("RuneName2", _runeTab2.transform);
                    _runeInfoCardIcon2 = FindPart("RuneIcon2", _runeTab2.transform);
                }
            }

            if (currentRunes != null)
            {
                runeIconFace1 = FindPart("CurrentRuneIcon1", currentRunes.transform);
                runeIconFace2 = FindPart("CurrentRuneIcon2", currentRunes.transform);
            }
        }

        protected override void ShowCardFlip(bool up)
        {
            base.ShowCardFlip(up);

            _elementObj.SetActive(up);

            _strengthObj.SetActive(up);
            _defenseObj.SetActive(up);
            _scoreObj.SetActive(up);

            magicObj.SetActive(up);

            CardBackObj.SetActive(!up);
        }

        public override void ToggleInfoSlide(bool toggle = true)
        {
            base.ToggleInfoSlide(toggle);

            if (InfoObj.activeInHierarchy &&
                GetComponentInParent<CardMovement>().cardState == CardMovement.CardState.Field)
            {
                gameObject.transform.SetAsLastSibling();
            }

            if (!toggle) return; // if toggle info on/true, wait i just wanna check sum first...

            RuneSlots runeSlots = GetComponentInChildren<RuneSlots>();
            
            if (runeSlots.currentRune1 == null) // then not runed up
            {
                _runeTab1.SetActive(false);
                _runeTab2.SetActive(false);
            }
            else if (runeSlots.currentRune1 != null &&
                     runeSlots.currentRune2 == null) // only first rune
            {
                _runeTab1.SetActive(true);
                _runeTab2.SetActive(false);
            }
            else // assume two runes on this creature
            {
                _runeTab1.SetActive(true);
                _runeTab2.SetActive(true);
            }
        }

        public void UpdateUIStrength(int newAttack)
        {
            SetText(_strengthObj, newAttack.ToString(), true);
        }

        public void UpdateCardUIDefense(int newDefense)
        {
            SetText(_defenseObj, newDefense.ToString(), true);
        }

        public void UpdateUI_AbilityCost(int newCost)
        {
            SetText(abilityCost, newCost.ToString(), true);
        }

        public void UpdateCardUI_Score(int newScore)
        {
            SetText(_scoreObj, newScore.ToString(), true);
        }

        public void DisplayRune(GameObject runeToDisplay = null) // optional, if passing null, remove runes
        {
            if (runeToDisplay == null) // removing all runes
            {
                // set both rune icons to nothing
                SetImage(runeIconFace1, null); 
                SetImage(runeIconFace2, null);
                
                SetImage(_runeInfoCardIcon1, null);
                SetImage(_runeInfoCardIcon2, null);
                
                SetText(_runeText1, null);
                SetText(_runeText2, null);
                
                SetText(_runeName1, null);
                SetText(_runeName2, null);
                
                runeIconFace1.SetActive(false);
                runeIconFace2.SetActive(false);
                return;
            }

            CardDataSO runeData = runeToDisplay.GetComponent<CardStats>().cardData;
            
            RuneSlots runeSlots = GetComponentInChildren<RuneSlots>();

            if (runeSlots.currentRune2 != null)
            {
                SetImage(runeIconFace2, runeData.mainImage); // current rune icon on face
                
                runeIconFace2.SetActive(true);
                
                // the info text
                SetText(_runeText2, runeData.abilityDescription);
                
                SetText(_runeName2, runeData.cardName);
                
                SetImage(_runeInfoCardIcon2, runeData.mainImage);

                return;
            }

            if (runeSlots.currentRune1 != null)
            {
                SetImage(runeIconFace1, runeData.mainImage); // current rune icon on face
                
                runeIconFace1.SetActive(true);
                
                // the info text
                SetText(_runeText1, runeData.abilityDescription);
                
                SetText(_runeName1, runeData.cardName);
                
                SetImage(_runeInfoCardIcon1, runeData.mainImage);
            }
        }
    }
}