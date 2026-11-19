using CardScripts.CardData;
using CardScripts.CardMovements;
using CardScripts.CardStats_Folder;
using CardScripts.CardStatss;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;

namespace CardScripts.CardDisplays
{
    public class CreatureDisplay : CardDisplay
    {
        [Header("Stats UI Objects")]
        public GameObject strengthObj;
        public GameObject defenseObj;
        public GameObject scoreObj;
        public GameObject elementObj;

        [Header("Ability UI Objects")]
        public GameObject activateButton;
        public GameObject abilityCost;
        public GameObject abilityUses;

        [Header("Rune UI Objects")]
        public GameObject runeTab1;
        public GameObject runeText1;
        public GameObject runeName1;
        public GameObject runeInfoCardIcon1;
        
        public GameObject runeTab2;
        public GameObject runeText2;
        public GameObject runeName2;
        public GameObject runeInfoCardIcon2;
        
        public GameObject runeIconFace1;
        public GameObject runeIconFace2;
        
        public override void SetDisplayElements_UsingData(CardStats s)
        {
            // Runs all the shared logic: image, name, description, flip, rarity, info card
            base.SetDisplayElements_UsingData(s);

            // If the base bailed out on a null check, don't continue
            if (cardData == null) return;

            // ---- Creature-only work ----
            CreatureDataSO creatureData = s.CardData as CreatureDataSO;
            if (creatureData == null)
            {
                Debug.LogError($"CardData is not a CreatureDataSO on {gameObject.name}");
                return;
            }
            
            SetImage(elementObj, creatureData.elementSprite);

            SetText(abilityCost, creatureData.abilityCost.ToString());
            SetText(abilityUses, 1.ToString()); // start as one
        }

        protected override void ShowCardFlip(bool up)
        {
            base.ShowCardFlip(up);

            elementObj.SetActive(up);

            strengthObj.SetActive(up);
            defenseObj.SetActive(up);
            scoreObj.SetActive(up);

            soulTextObj.SetActive(up);

            cardBackObj.SetActive(!up);
        }

        public override void ToggleInfoSlide(bool toggle = true)
        {
            base.ToggleInfoSlide(toggle);
            
            // name and info right are already adjusted in the base, now do creature specific stuff 
            Animator.ScaleInfoSlide(runeTab1, grow: false, instant: true);
            
            // ↓ todo we want rune tab 2 to scale as well, but it would overlap with the first, since they start off close together
            // Animator.ScaleInfoSlide(runeTab2, grow: false, instant: true);
            // mabye transform pos x to the side or something?
            
            if (infoObj.activeInHierarchy &&
                GetComponentInParent<CardMovement>().cardState == CardMovement.CardState.Field)
            {
                gameObject.transform.SetAsLastSibling();
            }

            if (!toggle) return; // if toggle info on/true, wait I just want to check sum first...

            RuneSlots runeSlots = GetComponentInChildren<RuneSlots>();
            
            if (runeSlots.currentRune1 == null) // then not runed up
            {
                runeTab1.SetActive(false);
                runeTab2.SetActive(false);
            }
            else if (runeSlots.currentRune1 != null &&
                     runeSlots.currentRune2 == null) // only first rune
            {
                runeTab1.SetActive(true);
                runeTab2.SetActive(false);
            }
            else // assume two runes on this creature
            {
                runeTab1.SetActive(true);
                runeTab2.SetActive(true);
            }
        }

        public override void OnClick_ScaleInfoSlide()
        {
            base.OnClick_ScaleInfoSlide();
            Animator.ScaleInfoSlide(runeTab1, grow: infoGrown);
        }

        public void UpdateUIStrength(int newAttack)
        {
            SetText(strengthObj, newAttack.ToString());
            // Anim.TextPop(_strengthObj);
        }

        public void UpdateCardUIDefense(int newDefense)
        {
            SetText(defenseObj, newDefense.ToString());
            // Anim.TextPop(_defenseObj);
        }

        public override void PopTextAnimate(CardStats.AnimTarget target, bool isBuff = true)
        {
            base.PopTextAnimate(target, isBuff); // strength and defense
            
            Transform toAnimate = target switch
            {
                CardStats.AnimTarget.Strength => strengthObj.transform,
                CardStats.AnimTarget.Defense  => defenseObj.transform,
                _ => null
            };
    
            if (toAnimate == null)
            {
                Debug.LogWarning($"No transform found for {target}");
            }
    
            Animator.PopTextAnimate(toAnimate, isBuff);
        }
        
        public void UpdateUI_AbilityCost(int newCost)
        {
            SetText(abilityCost, newCost.ToString(), true);
        }

        public void UpdateScoreUI(int newScore)
        {
            SetText(scoreObj, newScore.ToString());
        }

        public void DisplayRune(GameObject runeToDisplay = null) // optional, if passing null, remove runes
        {
            if (runeToDisplay == null) // removing all runes
            {
                // set both rune icons to nothing
                SetImage(runeIconFace1, null); 
                SetImage(runeIconFace2, null);
                
                SetImage(runeInfoCardIcon1, null);
                SetImage(runeInfoCardIcon2, null);
                
                SetText(runeText1, null);
                SetText(runeText2, null);
                
                SetText(runeName1, null);
                SetText(runeName2, null);
                
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

                InitializeAbilityDescription(runeText2, runeData.ability);

                SetText(runeName2, runeData.cardName, changeColorOfText_FromDefaultToBlack: true);
                
                SetImage(runeInfoCardIcon2, runeData.mainImage);

                return;
            }

            if (runeSlots.currentRune1 != null)
            {
                SetImage(runeIconFace1, runeData.mainImage); // current rune icon on face
                
                runeIconFace1.SetActive(true);
                
                InitializeAbilityDescription(runeText1, runeData.ability);
                
                SetText(runeName1, runeData.cardName, changeColorOfText_FromDefaultToBlack: true);
                
                SetImage(runeInfoCardIcon1, runeData.mainImage);
            }
        }

        public void UpdateUI_AbilityUsesLeft(int usesLeft)
        {
            SetText(abilityUses, usesLeft.ToString(), changeColorOfText_FromDefaultToBlack: true);
        }
    }
}