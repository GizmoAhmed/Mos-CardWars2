using Mirror;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

// Assigns all the visual stuff from CardDataSO to a card game object this script is attached to
namespace CardScripts
{
    [RequireComponent(typeof(CardStats))]
    public class CardDisplay : NetworkBehaviour
    {
        public bool faceUp;

        public CardDataSO cardData;

        [HideInInspector] public CardStats stats;

        // ~~~ Main Stuff shown on backdrop ~~~
        private GameObject _mainImageObj;

        private GameObject _cardBackObj;

        private GameObject _elementLeft;
        private GameObject _elementRight;

        private GameObject _nameTop;

        private GameObject _attackObj;
        private GameObject _defenseObj;
        [HideInInspector] public GameObject _magicObj;

        // ~~~ Stuff shown on left and right info cards ~~~
        public GameObject infoObj; // parent of next line ▼

        public GameObject infoRight; // ▼
        public GameObject abilityDesc;
        public GameObject activateButton;
        public GameObject abilityCost;

        public GameObject infoLeft; // ▼
        public GameObject burnObj;

        private CardInfoHandler cardInfoHandler;

        public void InitDisplay(CardStats s)
        {
            stats = s;

            if (cardData == null)
            {
                Debug.LogError($"CardData is null on {gameObject.name}");
                return;
            }

            // find all object variables above
            FindDisplayParts();

            // Set images
            SetImage(_mainImageObj, cardData.mainImage);
            SetImage(_elementLeft, cardData.elementSprite);
            SetImage(_elementRight, cardData.elementSprite);

            // Set names
            SetText(_nameTop, cardData.cardName);

            // set description
            GameObject descTextChild = abilityDesc.transform.GetChild(0).gameObject; // <-- child of AbilityDesc
            SetText(descTextChild, cardData.abilityDescription);

            if (cardData.cardType !=
                CardDataSO.CardType.Creature) // if not a creature, it doens't have an ability button
            {
                Destroy(activateButton); // don't need it
            }
            else
            {
                GameObject activateButtonText = activateButton.transform.GetChild(0).gameObject;
                SetText(activateButtonText, cardData.abilityCost.ToString());
            }

            FlipCard(face: true);

            Hide(infoObj); // initially hide the info card

            cardInfoHandler = FindObjectOfType<CardInfoHandler>();
        }

        private void Hide(GameObject obj)
        {
            if (obj != null)
            {
                obj.SetActive(false);
            }
            else
            {
                Debug.LogError($"Can't hide null on {gameObject.name}");
            }
        }

        private GameObject FindPart(string childName, Transform parent = null)
        {
            var searchRoot = parent ?? transform;

            var obj = searchRoot.Find(childName)?.gameObject;
            if (obj == null)
            {
                Debug.LogError($"Missing {childName} on {searchRoot.gameObject.name}");
            }

            return obj;
        }

        private void FindDisplayParts()
        {
            // main
            _mainImageObj = FindPart("MainImage");
            _cardBackObj = FindPart("CardBack");

            _elementLeft = FindPart("ElementLeft");
            _elementRight = FindPart("ElementRight");

            _attackObj = FindPart("Attack");
            _defenseObj = FindPart("Defense");
            _magicObj = FindPart("Magic");

            // info right + left
            infoObj = FindPart("Info");

            // some cascading logic here, if infoObj is find, you can find the rest and so on
            if (infoObj != null)
            {
                GameObject nameBackDrop = FindPart("NameBackDrop", infoObj.transform);
                _nameTop = FindPart("NameTop", nameBackDrop.transform);

                // right >
                infoRight = FindPart("InfoRight", infoObj.transform);
                abilityDesc = FindPart("AbilityDesc", infoRight.transform);
                activateButton = FindPart("ActivateButton", infoRight.transform);

                abilityCost = FindPart("AbilityCostText", activateButton.transform);

                // left <
                infoLeft = FindPart("InfoLeft", infoObj.transform);
                GameObject burnButton = FindPart("BurnButton", infoLeft.transform);
                burnObj = FindPart("BurnCostText", burnButton.transform);
            }
            else
            {
                Debug.LogWarning($"Missing {infoObj.name}, can't find the rest of the info");
            }
        }

        private void SetImage(GameObject obj, Sprite sprite)
        {
            if (obj != null && obj.TryGetComponent(out Image img))
                img.sprite = sprite;
        }

        private void SetText(GameObject obj, string text, bool isStatText = false)
        {
            if (obj != null && obj.TryGetComponent(out TextMeshProUGUI tmp))
            {
                tmp.text = text.ToUpper();

                if (!isStatText) // stat text just stays whatever color is on the card, things like description would remain black
                    tmp.color = Color.black; // red text means error
            }
            else
            {
                Debug.LogError($"Missing {text} on {gameObject.name} or missing obj {obj.name} altogether");
            }
        }

        public void FlipCard(bool face)
        {
            ShowCardFlip(face);
            faceUp = face;
        }

        private void ShowCardFlip(bool f)
        {
            if (f)
            {
                _mainImageObj.SetActive(true);
                _elementLeft.SetActive(true);
                _elementRight.SetActive(true);
                _nameTop.SetActive(true);

                if (cardData.cardType == CardDataSO.CardType.Creature)
                {
                    _attackObj.SetActive(true);
                    _defenseObj.SetActive(true);
                    _magicObj.SetActive(true);
                }
                else if (cardData.cardType == CardDataSO.CardType.Rune)
                {
                    _attackObj.SetActive(false);
                    _defenseObj.SetActive(false);
                    _magicObj.SetActive(false);
                }
                else // building, spell, and charm
                {
                    _attackObj.SetActive(false);
                    _defenseObj.SetActive(false);
                    _magicObj.SetActive(true);
                }
                
                _cardBackObj.SetActive(false);
            }
            else
            {
                _mainImageObj.SetActive(false);
                _elementLeft.SetActive(false);
                _elementRight.SetActive(false);
                _nameTop.SetActive(false);
                _attackObj.SetActive(false);
                _defenseObj.SetActive(false);
                _magicObj.SetActive(false);

                // Show card back
                _cardBackObj.SetActive(true);
            }
        }

        /// <summary>
        /// Show this cards info slide
        /// TODO this is where you would include things like showing the charm and/or the ability button
        /// 
        /// </summary>
        public void ToggleInfoSlide(bool toggle = true)
        {
            if (!faceUp) // if faced down
                return;

            if (infoObj == null)
            {
                Debug.LogError($"The info card for this card is null ({gameObject.name})");
                return;
            }

            if (toggle) // just switch
                infoObj.SetActive(!infoObj.activeInHierarchy);
            else // explicitly set 
            {
                infoObj.SetActive(false);
            }

            if (infoObj.activeInHierarchy) // toggle on
            {
                cardInfoHandler.SaveInfo(gameObject);

                // place buildings and creatures in front of each other
                if (GetComponentInParent<CardMovement>().cardState == CardMovement.CardState.Field
                    && (GetComponentInParent<CardStats>().cardData.cardType == CardDataSO.CardType.Creature
                        || GetComponentInParent<CardStats>().cardData.cardType ==
                        CardDataSO.CardType.Building)) // on a land and a creature/building
                {
                    gameObject.transform.SetAsLastSibling();
                }
            }
            else
            {
                cardInfoHandler.ClearSavedCard();
            }

            gameObject.GetComponent<Canvas>().overrideSorting =
                infoObj.activeInHierarchy; // visually move card to front if info is on
        }

        public void UpdateUIMagic(int newMagic)
        {
            SetText(_magicObj, newMagic.ToString(), true);
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

        public void UpdateUI_BurnCost(int newCost)
        {
            SetText(burnObj, newCost.ToString(), true);
        }
    }
}