using CardScripts.CardData;
using CardScripts.CardMovements;
using CardScripts.CardStatss;
using CardScripts.CardStatss.Runes;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

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

        private GameObject _runeTab1;
        private GameObject _runeText1;
        private GameObject _runeName1;
        private GameObject _runeIcon1;
        
        private GameObject _runeTab2;
        private GameObject _runeText2;
        private GameObject _runeName2;
        private GameObject _runeIcon2;
        
        private GameObject _currentRuneIcon1;
        private GameObject _currentRuneIcon2;

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
            _attackObj = FindPart("Attack");
            _defenseObj = FindPart("Defense");
            _scoreObj = FindPart("Score");

            GameObject CurrentRunes = FindPart("CurrentRunes");

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
                    _runeIcon1 = FindPart("RuneIcon1", _runeTab1.transform);
                }

                if (_runeTab2 != null)
                {
                    _runeText2 = FindPart("RuneDescText2", _runeTab2.transform);
                    _runeName2 = FindPart("RuneName2", _runeTab2.transform);
                    _runeIcon2 = FindPart("RuneIcon2", _runeTab2.transform);
                }
            }

            if (CurrentRunes != null)
            {
                _currentRuneIcon1 = FindPart("CurrentRune1", CurrentRunes.transform);
                _currentRuneIcon2 = FindPart("CurrentRune2", CurrentRunes.transform);
            }
        }

        protected override void ShowCardFlip(bool up)
        {
            base.ShowCardFlip(up);

            _elementObj.SetActive(up);

            _attackObj.SetActive(up);
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

            if (_creatureStats.currentRune1 == null) // then not runed up
            {
                _runeTab1.SetActive(false);
                _runeTab2.SetActive(false);
            }
            else if (_creatureStats.currentRune1 != null &&
                     _creatureStats.currentRune2 == null) // only first rune
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

        public void DisplayRune(RuneBase newRune)
        {
            Debug.Log($"Displaying rune: {newRune.gameObject.name} on {gameObject.name}");

            CardDataSO runeData = newRune.gameObject.GetComponent<CardStats>().cardData;

            if (_creatureStats.currentRune2 != null)
            {
                SetImage(_currentRuneIcon2, runeData.mainImage); // current rune icon on face
                // _currentRuneIcon2.GetComponent<Image>().sprite = runeData.mainImage;
                
                _currentRuneIcon2.SetActive(true);
                
                // the info text
                SetText(_runeText2, runeData.abilityDescription);
                // _runeText2.GetComponent<TextMeshProUGUI>().text = runeData.abilityDescription.ToUpper();
                
                SetText(_runeName2, runeData.cardName);
                
                SetImage(_runeIcon2, runeData.mainImage);

                return;
            }

            if (_creatureStats.currentRune1 != null)
            {
                SetImage(_currentRuneIcon1, runeData.mainImage); // current rune icon on face
                
                _currentRuneIcon1.SetActive(true);
                
                // the info text
                SetText(_runeText1, runeData.abilityDescription);
                
                SetText(_runeName1, runeData.cardName);
                
                SetImage(_runeIcon1, runeData.mainImage);
            }
        }
    }
}