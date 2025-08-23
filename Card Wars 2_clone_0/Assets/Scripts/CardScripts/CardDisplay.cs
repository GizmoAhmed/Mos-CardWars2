using Mirror;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

// Assigns all the visual stuff from CardDataSO to a card game object this script is attached to
[RequireComponent(typeof(CardStats))]
public class CardDisplay : NetworkBehaviour
{
    public bool faceUp;
    
    public CardDataSO cardData;
    
    public CardStats stats;

    private GameObject mainImageObj;

    private GameObject cardBackObj;
    
    private GameObject elementLeft;
    private GameObject elementRight;
    
    private GameObject nameLeft;
    private GameObject nameRight;
    
    private GameObject attackObj;
    private GameObject defenseObj;
    private GameObject magicObj;

    public void Init(CardStats s)
    {
        Debug.Log($"Initializing Card Display for {gameObject.name}...");
        
        stats = s;
        
        if (cardData == null)
        {
            Debug.LogError($"CardData is null on {gameObject.name}");
            return;
        }
                  
        FindDisplayParts();
                  
        // Set images
        SetImage(mainImageObj, cardData.MainImage);
        SetImage(elementLeft, cardData.Element);
        SetImage(elementRight, cardData.Element);
                  
        // Set names
        SetText(nameLeft, cardData.Name);
        SetText(nameRight, cardData.Name);
                          
        FlipCard(face : true);
    }

    private void Hide(GameObject obj)
    {
        if (obj != null)
            obj.SetActive(false);
    }
    
    private GameObject FindPart(string childName)
    {
        var obj = transform.Find(childName)?.gameObject;
        if (obj == null)
        {
            Debug.LogError($"Missing {childName} on {gameObject.name}");
        }
        else
        {
            Debug.Log($"Found {childName} on {obj.name}");
        }

        return obj;
    }

    private void FindDisplayParts()
    {
        mainImageObj = FindPart("MainImage");
        cardBackObj  = FindPart("CardBack");
        elementLeft  = FindPart("ElementLeft");
        elementRight = FindPart("ElementRight");
        nameLeft     = FindPart("NameLeft");
        nameRight    = FindPart("NameRight");
        attackObj    = FindPart("Attack");
        defenseObj   = FindPart("Defense");
        magicObj     = FindPart("Magic");
    }

    private void SetImage(GameObject obj, Sprite sprite)
    {
        if (obj != null && obj.TryGetComponent(out Image img))
            img.sprite = sprite;
    }

    private void SetText(GameObject obj, string text)
    {
        if (obj != null && obj.TryGetComponent(out TextMeshProUGUI tmp))
            tmp.text = text;
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
            mainImageObj.SetActive(true);
            elementLeft.SetActive(true);
            elementRight.SetActive(true);
            nameLeft.SetActive(true);
            nameRight.SetActive(true);

            // Only show stats relevant to the type
            switch (cardData.cardType)
            {
                case CardDataSO.CardType.Creature:
                    attackObj.SetActive(true);
                    defenseObj.SetActive(true);
                    magicObj.SetActive(true);
                    break;
                case CardDataSO.CardType.Building:
                    attackObj.SetActive(false);
                    defenseObj.SetActive(true);
                    magicObj.SetActive(true);
                    break;
                case CardDataSO.CardType.Spell:
                    attackObj.SetActive(false);
                    defenseObj.SetActive(true);
                    magicObj.SetActive(true);
                    break;
                case CardDataSO.CardType.Charm:
                    attackObj.SetActive(false);
                    defenseObj.SetActive(false);
                    magicObj.SetActive(false);
                    break;
            }

            cardBackObj.SetActive(false);
        }
        else
        {
            mainImageObj.SetActive(false);
            elementLeft.SetActive(false);
            elementRight.SetActive(false);
            nameLeft.SetActive(false);
            nameRight.SetActive(false);
            attackObj.SetActive(false);
            defenseObj.SetActive(false);
            magicObj.SetActive(false);

            // Show card back
            cardBackObj.SetActive(true);
        }
    }

    public void UpdateUIMagic(int newMagic)
    {
        SetText(magicObj, newMagic.ToString());
    }

    public void UpdateUIAttack(int newAttack)
    {
        SetText(attackObj, newAttack.ToString());

        if (newAttack == -1)
        {
            Hide(attackObj);
        }
    }
    
    public void UpdateUIDefense(int newDefense)
    {
        SetText(attackObj, newDefense.ToString());

        if (newDefense == -1)
        {
            Hide(defenseObj);
        }
    }
}
