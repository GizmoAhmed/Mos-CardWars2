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
        protected GameObject MainImageObj;
        protected GameObject CardBackObj;

        protected GameObject NameTop;

        [HideInInspector] public GameObject magicObj;

        // ~~~ Stuff shown on left and right info cards ~~~
        protected GameObject InfoObj; // parent of next line ▼

        protected GameObject InfoRight; // ▼
        protected GameObject AbilityDesc;

        protected GameObject InfoLeft; // ▼
        private GameObject _burnObj;

        protected CardInfoHandler CardInfoHandler;

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
            SetImage(MainImageObj, cardData.mainImage);

            // Set names
            SetText(NameTop, cardData.cardName);

            // set description
            GameObject descTextChild = AbilityDesc.transform.GetChild(0).gameObject; // <-- child of AbilityDesc
            SetText(descTextChild, cardData.abilityDescription);

            FlipCard(face: true);

            Hide(InfoObj); // initially hide the info card

            CardInfoHandler = FindObjectOfType<CardInfoHandler>();
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
            MainImageObj = FindPart("MainImage");
            CardBackObj = FindPart("CardBack");
            
            magicObj = FindPart("Magic");

            // info right + left
            InfoObj = FindPart("Info");

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
                GameObject burnButton = FindPart("BurnButton", InfoLeft.transform);
                _burnObj = FindPart("BurnCostText", burnButton.transform);
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
            MainImageObj.SetActive(up);
            NameTop.SetActive(up);
            magicObj.SetActive(up);
            CardBackObj.SetActive(!up);
        }

        public void OnCardClicked()
        {
            if (!faceUp)
                return;

            CardInfoHandler.HandleCardClick(this);
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
        
        public void UpdateUIMagic(int newMagic)
        {
            SetText(magicObj, newMagic.ToString(), true);
        }

        public void UpdateUI_BurnCost(int newCost)
        {
            SetText(_burnObj, newCost.ToString(), true);
        }
    }
}