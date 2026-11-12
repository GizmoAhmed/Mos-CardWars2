using System;
using System.Text.RegularExpressions;
using CardScripts.Abilities;
using CardScripts.CardData;
using CardScripts.CardStats_Folder;
using CardScripts.CardStatss;
using Mirror;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using ColorUtility = UnityEngine.ColorUtility;

// Assigns all the visual stuff from CardDataSO to a card game object this script is attached to
namespace CardScripts.CardDisplays
{
    public class CardDisplay : NetworkBehaviour
    {
        public bool faceUp;

        public CardDataSO cardData;

        // ~~~ Main Stuff shown on backdrop ~~~
        protected GameObject MainImageObj;
        protected GameObject CardBackObj;

        protected GameObject NameTop;

        [HideInInspector] public GameObject magicObj;

        private GameObject _backDrop;
        
        [Header("Rarity Colors")]
        [SerializeField] private Color commonColor   = Color.white;
        [SerializeField] private Color uncommonColor = new Color(0.8f, 1f, 0.92f);
        [SerializeField] private Color rareColor     = new Color(0.4f, 0.7f, 1f);
        [SerializeField] private Color epicColor     = new Color(0.78f, 0.49f, 1f);

        // ~~~ Stuff shown on left and right info cards ~~~
        protected GameObject InfoObj; // parent of next line ▼

        protected GameObject InfoRight; // ▼
        protected GameObject AbilityDesc;

        protected GameObject InfoLeft; // ▼
        private GameObject _burnObj;
        private GameObject _burnButtonObj;

        private CardInfoHandler _cardInfoHandler;

        private void Awake()
        {
            // find all object variables above
            FindDisplayParts();
        }

        public virtual void SetDisplayElements_UsingData(CardStats s)
        {
            // Debug.Log($"<color=yellow>Rendering:</color> Setting Display for {s} on this client");

            cardData = s.CardData;

            if (cardData == null)
            {
                Debug.LogError($"CardData is null on {gameObject.name}");
                return;
            }

            // Set images
            SetImage(MainImageObj, cardData.mainImage);

            // Set names
            SetText(NameTop, cardData.cardName, changeColorOfText_FromDefaultToBlack: true);

            // set description
            GameObject descTextChild = AbilityDesc.transform.GetChild(0).gameObject; // <-- child of AbilityDesc
            
            SetAbilityDescription(descTextChild, cardData.ability);
            
            if (cardData.ability == null)
            {
                Debug.LogError($"CardDisplay: cardData.ability is null on {gameObject.name}");
            }

            FlipCard(face: true);

            AddCardRarityColor();

            HideUIElement(InfoObj); // initially hide the info card

            _cardInfoHandler = FindObjectOfType<CardInfoHandler>();
        }

        protected void SetAbilityDescription(GameObject textObj, CardAbilitySO ability)
        {
            if (string.IsNullOrEmpty(ability.abilityDescription)) // no description typed out in the SO
            {
                // turn the name of the ability into an apt description
                SetText(
                    textObj,
                    ParseAbilityName_IntoDescription(ability.name), // pass name as the ability description
                    changeColorOfText_FromDefaultToBlack: false); // leave as error red
            }
            else // there is a description there
            {
                SetText(
                    textObj,
                    ability.abilityDescription, // use the typed out description
                    changeColorOfText_FromDefaultToBlack: true); // set to black to indicate the typed desc was used
            }
        }

        protected string ParseAbilityName_IntoDescription(string rawName)
        {
            if (string.IsNullOrEmpty(rawName)) return "";

            // Drop everything after (and including) the last underscore
            int lastUnderscore = rawName.LastIndexOf('_');
            if (lastUnderscore >= 0)
                rawName = rawName.Substring(0, lastUnderscore);

            // Replace underscores with spaces
            rawName = rawName.Replace("_", " ");

            // Add space before capital letters (camelCase/PascalCase splitting)
            rawName = Regex.Replace(rawName, "(?<!^)(?<! )(?=[A-Z])", " ");

            return rawName.Trim();
        }

        private void HideUIElement(GameObject obj)
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

        protected GameObject FindPart(string childName, Transform parent = null)
        {
            var searchRoot = parent ?? transform;

            var obj = searchRoot.Find(childName)?.gameObject;

            if (obj == null)
            {
                //Debug.LogWarning(
                //$"Missing {childName} UI Obj on {gameObject.name}. This is a concern if it's a creature, charm, or building that triggered this");
            }

            return obj;
        }

        protected virtual void FindDisplayParts()
        {
            // main
            MainImageObj = FindPart("MainImage");
            CardBackObj = FindPart("CardBack");

            magicObj = FindPart("Magic");

            // info right + left
            InfoObj = FindPart("Info");
            
            _backDrop = FindPart("BackDrop");

            // some cascading logic here, if infoObj is find, you can find the rest and so on
            if (InfoObj != null)
            {
                GameObject nameBackDrop = FindPart("NameBackDrop", InfoObj.transform);
                NameTop = FindPart("NameTop", nameBackDrop.transform);

                // right >
                InfoRight = FindPart("InfoRight", InfoObj.transform);
                AbilityDesc = FindPart("AbilityDesc", InfoRight.transform);

                // left <
                InfoLeft = FindPart("InfoLeft", InfoObj.transform);
                _burnButtonObj = FindPart("BurnButton", InfoLeft.transform);
                _burnObj = FindPart("BurnCostText", _burnButtonObj.transform);
            }
            else
            {
                Debug.LogWarning($"Missing {InfoObj.name}, can't find the rest of the info");
            }
        }

        protected void SetImage(GameObject obj, Sprite sprite)
        {
            if (obj != null && obj.TryGetComponent(out Image img))
                img.sprite = sprite;
        }

        protected void SetText(GameObject obj, string text, bool changeColorOfText_FromDefaultToBlack = false)
        {
            if (obj != null && obj.TryGetComponent(out TextMeshProUGUI tmp))
            {
                if (text == null)
                {
                    Debug.LogWarning($"Can't set text on {gameObject.name} cause it was passed null");
                    tmp.text = string.Empty;
                }
                else
                {
                    tmp.text = text.ToUpper();
                }

                if (changeColorOfText_FromDefaultToBlack) // stat text just stays whatever color is on the card, things like description would remain black
                    tmp.color = Color.black; // the red text it starts with means error
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

        protected virtual void ShowCardFlip(bool up)
        {
            MainImageObj.SetActive(up);
            NameTop.SetActive(up);

            if (magicObj != null) // runes and spells don't have this, so don't error if it's not there
            {
                magicObj.SetActive(up);
            }
            else
            {
                /*Debug.Log(
                    $"{gameObject.name} <color=green>successfully</color> avoid null ref on the magic object, because it shouldn't have a magic object, when this flipping card. Double check that this is a <color=cyan>SPELL or RUNE</color>");*/
            }

            CardBackObj.SetActive(!up);
        }

        private void AddCardRarityColor()
        {
            Image backDropImage = _backDrop.GetComponent<Image>();

            Color color;

            switch (cardData.cardRarity)
            {
                case CardDataSO.Rarity.Common:
                    color = Color.white;
                    break;
                case CardDataSO.Rarity.Uncommon:
                    color = uncommonColor;
                    break;
                case CardDataSO.Rarity.Rare:
                    color = rareColor;
                    break;
                case CardDataSO.Rarity.Epic:
                    color = epicColor;
                    break;
                default:
                    color = Color.magenta; // obvious "something's wrong" color
                    break;
            }

            backDropImage.color = color;
        }

        public void OnCardClicked()
        {
            if (!faceUp)
                return;

            _cardInfoHandler.HandleCardClick(this);
        }

        public virtual void ToggleInfoSlide(bool show)
        {
            if (!faceUp || InfoObj == null)
                return;

            InfoObj.SetActive(show);

            // move card to front, so other stuff can't cover the info
            gameObject.GetComponent<Canvas>().overrideSorting =
                InfoObj.activeInHierarchy;
        }

        public void UpdateUI_Soul(int newMagic)
        {
            if (magicObj != null) // spells have null magic obj 
            {
                SetText(magicObj, newMagic.ToString());
            }
        }

        public void UpdateUI_Burn(int newCost)
        {
            SetText(_burnObj, newCost.ToString());
        }

        public void FlipBurn_ToCostFromReturn(int burnCost)
        {
            UpdateUI_Burn(burnCost);
            
            Button burnButton = _burnButtonObj.GetComponent<Button>();
            
            ChangeButtonColor(burnButton, Color.red);
        }

        private void ChangeButtonColor(Button button, Color color)
        {
            ColorBlock cb = button.colors;

            cb.normalColor = color;
            cb.highlightedColor = color;
            cb.pressedColor = color;
            
            button.colors = cb;
        }
    }
}