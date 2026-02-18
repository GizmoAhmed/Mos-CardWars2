using CardScripts.CardData;
using CardScripts.CardStatss;
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

        // ~~~ Main Stuff shown on backdrop ~~~
        protected GameObject _mainImageObj;
        protected GameObject _cardBackObj;

        protected GameObject _nameTop;

        [HideInInspector] public GameObject _magicObj;

        // ~~~ Stuff shown on left and right info cards ~~~
        protected GameObject infoObj; // parent of next line ▼

        protected GameObject infoRight; // ▼
        protected GameObject abilityDesc;

        protected GameObject infoLeft; // ▼
        private GameObject burnObj;

        protected CardInfoHandler cardInfoHandler;

        public virtual void InitDisplayWithData(CardStats s)
        {
            cardData = s.CardData;    
            
            if (cardData == null)
            {
                Debug.LogError($"CardData is null on {gameObject.name}");
                return;
            }

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
        }

        protected void Hide(GameObject obj)
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
                Debug.LogError($"Missing {childName} on {searchRoot.gameObject.name}");
            }

            return obj;
        }

        protected virtual void FindDisplayParts()
        {
            // main
            _mainImageObj = FindPart("MainImage");
            _cardBackObj = FindPart("CardBack");
            
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

        protected void SetImage(GameObject obj, Sprite sprite)
        {
            if (obj != null && obj.TryGetComponent(out Image img))
                img.sprite = sprite;
        }

        protected void SetText(GameObject obj, string text, bool isStatText = false)
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

        protected virtual void ShowCardFlip(bool up)
        {
            _mainImageObj.SetActive(up);
            _nameTop.SetActive(up);
            _magicObj.SetActive(up);
            _cardBackObj.SetActive(!up);
        }

        public void OnCardClicked()
        {
            if (!faceUp)
                return;

            cardInfoHandler.HandleCardClick(this);
        }
        
        public virtual void ToggleInfoSlide(bool show)
        {
            if (!faceUp || infoObj == null)
                return;

            infoObj.SetActive(show);
            
            // move card to front, so other stuff can't cover the info
            gameObject.GetComponent<Canvas>().overrideSorting =
                infoObj.activeInHierarchy;
        }
        
        public void UpdateUIMagic(int newMagic)
        {
            SetText(_magicObj, newMagic.ToString(), true);
        }

        public void UpdateUI_BurnCost(int newCost)
        {
            SetText(burnObj, newCost.ToString(), true);
        }
    }
}