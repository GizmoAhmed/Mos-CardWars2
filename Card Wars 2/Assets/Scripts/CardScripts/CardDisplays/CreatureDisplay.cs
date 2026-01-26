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
        
        private GameObject _currentRune1;
        private GameObject _currentRune2;

        private CreatureStats _creatureStats;

        public override void InitDisplay(CardStats s)
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

            GameObject CurrentRunes = FindPart("CurrentRunes");

            if (infoObj != null)
            {
                activateButton = FindPart("ActivateButton", infoRight.transform);
                abilityCost = FindPart("AbilityCostText", activateButton.transform);

                _runeTab1 = FindPart("RuneTab1", infoLeft.transform);
                _runeTab2 = FindPart("RuneTab2", infoLeft.transform);

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
                _currentRune1 = FindPart("CurrentRune1", CurrentRunes.transform);
                _currentRune2 = FindPart("CurrentRune2", CurrentRunes.transform);
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

            if (infoObj.activeInHierarchy &&
                GetComponentInParent<BaseMovement>().cardState == BaseMovement.CardState.Field)
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
                _currentRune2.GetComponent<Image>().sprite = // current rune icon on face
                    runeData.mainImage;
                
                _currentRune2.SetActive(true);
                
                // the info text
                _runeText2.GetComponent<TextMeshProUGUI>().text = runeData.abilityDescription.ToUpper();
                _runeText2.GetComponent<TextMeshProUGUI>().color = Color.black; // remove error color
                
                _runeName2.GetComponent<TextMeshProUGUI>().text = runeData.cardName.ToUpper();
                _runeName2.GetComponent<TextMeshProUGUI>().color = Color.black; // remove error color
                
                _runeIcon2.GetComponent<Image>().sprite = runeData.mainImage;

                return;
            }

            if (_creatureStats.currentRune1 != null) // if rune one is bound
            {
                _currentRune1.GetComponent<Image>().sprite =
                    runeData.mainImage;
                
                _currentRune1.SetActive(true);
                
                _runeText1.GetComponent<TextMeshProUGUI>().text = runeData.abilityDescription.ToUpper();
                _runeText1.GetComponent<TextMeshProUGUI>().color = Color.black; // remove error color

                _runeName1.GetComponent<TextMeshProUGUI>().text = runeData.cardName.ToUpper();
                _runeName1.GetComponent<TextMeshProUGUI>().color = Color.black; // remove error color

                _runeIcon1.GetComponent<Image>().sprite = runeData.mainImage;
            }
        }
    }
}