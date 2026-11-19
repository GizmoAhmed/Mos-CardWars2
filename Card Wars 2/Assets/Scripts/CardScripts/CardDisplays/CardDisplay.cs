using System.Text.RegularExpressions;
using CardScripts.Abilities;
using CardScripts.CardData;
using CardScripts.CardStats_Folder;
using Mirror;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Assigns all the visual stuff from CardDataSO to a card game object this script is attached to
namespace CardScripts.CardDisplays
{
    public class CardDisplay : NetworkBehaviour
    {
        public bool faceUp;

        public CardDataSO cardData;

        [Header("Main UI Object Refs")]
        public GameObject infoObj;
        
        public GameObject mainImageObj;
        public GameObject cardBackObj;
        public GameObject backDrop;
        public GameObject soulTextObj;
        
        [Header("Name Refs")]
        public GameObject nameBackDrop;
        public GameObject nameTopTextObj;
        
        [Header("Right Info Slide")]
        public GameObject infoRightObj; // ▼
        public GameObject abilityDescBackdrop;
        public GameObject abilityDescTextObj;

        [Header("Left Info Slide")]
        public GameObject infoLeftObj; // ▼
        public GameObject burnCostTextObj;
        public GameObject burnButtonObj;

        [Header("Info scaling")]
        public bool infoGrown;

        protected CardAnimator Animator; 
        
        [Header("Rarity Colors")]
        [SerializeField] private Color commonColor   = Color.white;
        [SerializeField] private Color uncommonColor = new Color(0.8f, 1f, 0.92f);
        [SerializeField] private Color rareColor     = new Color(0.4f, 0.7f, 1f);
        [SerializeField] private Color epicColor     = new Color(0.78f, 0.49f, 1f);
        
        private CardInfoHandler _cardInfoHandler;

        private void Awake()
        {
            Animator = GetComponent<CardAnimator>();

            if (Animator == null)
            {
                Debug.LogError($"{gameObject.name} Awake: No CardAnimator found for CardDisplay on");
            }
        }

        public virtual void SetDisplayElements_UsingData(CardStats s)
        {
            cardData = s.CardData;

            if (cardData == null)
            {
                Debug.LogError($"CardData is null on {gameObject.name}");
                return;
            }

            // Set images
            SetImage(mainImageObj, cardData.mainImage);

            // Set names
            SetText(nameTopTextObj, cardData.cardName, changeColorOfText_FromDefaultToBlack: true);
            
            string textUsedForDesc = InitializeAbilityDescription(abilityDescTextObj, cardData.ability);
            
            // set syncvar to update on both clients
            // abilityDescription = textUsedForDesc;
            
            if (cardData.ability == null)
            {
                Debug.LogError($"CardDisplay: cardData.ability is null on {gameObject.name}");
            }

            FlipCard(face: true);

            AddCardRarityColor();

            HideUIElement(infoObj); // initially hide the info card

            _cardInfoHandler = FindObjectOfType<CardInfoHandler>();
        }

        protected string InitializeAbilityDescription(GameObject textObj, CardAbilitySO ability)
        {
            string desc;
            
            if (string.IsNullOrEmpty(ability.abilityDescription)) // no description typed out in the SO
            {
                desc = ParseAbilityName_IntoDescription(ability.name); // pass name as the ability description
                
                // turn the name of the ability into an apt description
                SetText(
                    textObj,
                    desc,
                    changeColorOfText_FromDefaultToBlack: false); // leave as error red
            }
            else // there is a description there
            {
                desc = ability.abilityDescription;
                
                SetText(
                    textObj,
                    ability.SpecifyAbilityDescription(), // use typed out description, filled in with actual ability values
                    changeColorOfText_FromDefaultToBlack: true); // set to black to indicate the typed desc was used
            }
            
            return desc;
        }

        private string ParseAbilityName_IntoDescription(string rawName)
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
            mainImageObj.SetActive(up);
            nameTopTextObj.SetActive(up);

            if (soulTextObj != null) // runes and spells don't have this, so don't error if it's not there
            {
                soulTextObj.SetActive(up);
            }
            else
            {
                /*Debug.Log(
                    $"{gameObject.name} <color=green>successfully</color> avoid null ref on the magic object, because it shouldn't have a magic object, when this flipping card. Double check that this is a <color=cyan>SPELL or RUNE</color>");*/
            }

            cardBackObj.SetActive(!up);
        }

        private void AddCardRarityColor()
        {
            Image backDropImage = backDrop.GetComponent<Image>();

            Color color;

            switch (cardData.cardRarity)
            {
                case CardDataSO.Rarity.Common:
                    color = commonColor;
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

            // eventually calls ToggleInfoSlide, but needs to turn off any previous slides
            _cardInfoHandler.HandleCardClick(this); 
        }

        public virtual void ToggleInfoSlide(bool show)
        {
            if (!faceUp || infoObj == null)
                return;

            if (!show && infoGrown) // closing a grown slide: snap it back
            {
                Animator.ScaleInfoSlide(nameBackDrop,           grow: false, instant: true);
                Animator.ScaleInfoSlide(abilityDescBackdrop,    grow: false, instant: true);
                infoGrown = false;
            }

            infoObj.SetActive(show);

            // move card to front, so other stuff can't cover the info
            gameObject.GetComponent<Canvas>().overrideSorting =
                infoObj.activeInHierarchy;
        }

        public virtual void OnClick_ScaleInfoSlide()
        {
            infoGrown = !infoGrown;
            Animator.ScaleInfoSlide(nameBackDrop, grow: infoGrown);
            Animator.ScaleInfoSlide(abilityDescBackdrop, grow: infoGrown);
        }

        public void UpdateUI_Soul(int newMagic)
        {
            if (soulTextObj != null) // spells have null magic obj 
            {
                SetText(soulTextObj, newMagic.ToString());
            }
        }

        public void UpdateUI_Burn(int newCost, string prefix = "+")
        {
            SetText(burnCostTextObj, prefix + newCost);
        }

        public void FlipBurn_ToCostFromReturn(int burnCost)
        {
            UpdateUI_Burn(burnCost, "-"); // use minus when placing on baord, to indicate that you will lose shards now if you burn
            
            Button burnButton = burnButtonObj.GetComponent<Button>();
            
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

        public void UpdateUI_BurnButtonActivity(bool active)
        {
            burnButtonObj.GetComponent<Button>().interactable = active;
        }
    }
}