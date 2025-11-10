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
        private GameObject _nameBottom;
    
        private GameObject _attackObj;
        private GameObject _defenseObj;
        [HideInInspector] public GameObject magicObj;
    
        // ~~~ Stuff shown on left and right info cards ~~~
        public GameObject _infoObj; // parent of next line ▼
        public GameObject _infoRight; // ▼
        public GameObject _abilityDesc;
        public GameObject _activateButton;
    

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
            SetText(_nameBottom, cardData.cardName);
            
            // set description
            GameObject descTextChild = _abilityDesc.transform.GetChild(0).gameObject; // <-- child of AbilityDesc
            SetText(descTextChild, cardData.abilityDescription);
                          
            FlipCard(face : true);
        }

        private void Hide(GameObject obj)
        {
            if (obj != null)
                obj.SetActive(false);
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
            _cardBackObj  = FindPart("CardBack");
            _elementLeft  = FindPart("ElementLeft");
            _elementRight = FindPart("ElementRight");
            _nameTop     = FindPart("NameTop");
            _nameBottom    = FindPart("NameBottom");
            _attackObj    = FindPart("Attack");
            _defenseObj   = FindPart("Defense");
            magicObj      = FindPart("Magic");
            
            // info right + left
            _infoObj        = FindPart("Info");
            
            // some cascading logic here, if infoObj is find, you can find the rest and so on
            if (_infoObj != null)
            {
                _infoRight      = FindPart("InfoRight", _infoObj.transform);
                _abilityDesc    = FindPart("AbilityDesc", _infoRight.transform);
                _activateButton = FindPart("ActivateButton", _infoRight.transform);
            }
            else
            {
                Debug.LogWarning($"Missing {_infoObj.name}, can't find the rest of the info");
            }
        }

        private void SetImage(GameObject obj, Sprite sprite)
        {
            if (obj != null && obj.TryGetComponent(out Image img))
                img.sprite = sprite;
        }
        
        private void SetText(GameObject obj, string text, bool statText = false)
        {
            if (obj != null && obj.TryGetComponent(out TextMeshProUGUI tmp))
            {
                tmp.text = text.ToUpper();
                
                if (!statText) // stat text just stays whatever color is on the card, things like description would remain black
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
                _nameBottom.SetActive(true);

                // Only show stats relevant to the type
                switch (cardData.cardType)
                {
                    case CardDataSO.CardType.Creature:
                        _attackObj.SetActive(true);
                        _defenseObj.SetActive(true);
                        magicObj.SetActive(true);
                        break;
                    case CardDataSO.CardType.Building:
                        _attackObj.SetActive(false);
                        _defenseObj.SetActive(false);
                        magicObj.SetActive(true);
                        break;
                    case CardDataSO.CardType.Spell:
                        _attackObj.SetActive(false);
                        _defenseObj.SetActive(false);
                        magicObj.SetActive(true);
                        break;
                    case CardDataSO.CardType.Charm:
                        _attackObj.SetActive(false);
                        _defenseObj.SetActive(false);
                        magicObj.SetActive(false);
                        break;
                }

                _cardBackObj.SetActive(false);
            }
            else
            {
                _mainImageObj.SetActive(false);
                _elementLeft.SetActive(false);
                _elementRight.SetActive(false);
                _nameTop.SetActive(false);
                _nameBottom.SetActive(false);
                _attackObj.SetActive(false);
                _defenseObj.SetActive(false);
                magicObj.SetActive(false);

                // Show card back
                _cardBackObj.SetActive(true);
            }
        }

        public void UpdateUIMagic(int newMagic)
        {
            SetText(magicObj, newMagic.ToString(), true);
        }

        public void UpdateUIAttack(int newAttack)
        {
            if (newAttack < 0)
            {
                Hide(_attackObj);
            }
            else
            {
                SetText(_attackObj, newAttack.ToString(), true);
            }
        }
    
        public void UpdateUIDefense(int newDefense)
        {
            if (newDefense < 0)
            {
                Hide(_defenseObj);
            }
            else
            {
                SetText(_defenseObj, newDefense.ToString(), true);
            }
        }

        public void UpdateUICost(int newCost)
        {
            
        }
    }
}
