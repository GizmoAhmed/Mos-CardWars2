using AbilityEvents;
using CardScripts.Abilities;
using CardScripts.CardStatss;
using Extensions;
using CardScripts.Abilities.AbilityClasses;
using CardScripts.CardStats_Folder;
using UnityEngine;
using Tiles;
using Extensions;

[CreateAssetMenu(
    fileName = "DrawRandomCharm_FromCharmGroup_DrawnWithExtraSoul_Spell", 
    menuName = "Abilities/Spell/DrawRandomCharm_FromCharmGroup_DrawnWithExtraSoul_Spell")]
public class DrawRandomCharm_FromCharmGroup_DrawnWithExtraSoul_Spell : CastAbilitySO
{
    [Tooltip("The Additional soul cost placed on drawn building")]
    public int extraSoul;
        
    public override void ExecuteAbility(GameObject thisCard, AbilityEventData eventData)
    {
        Debug.Log($"Executing on {this} on {thisCard.name}");
        CharmTile charmTile = eventData.target.GetComponent<CharmTile>();

        if (charmTile.charms.Count <= 0) return; // empty

        GameObject charmPicked = charmTile.charms.GetRandomFromList(); // <-- errors here, can't resolve symbol

        GameObject dupeCharm = RedrawCard
        (
            redrawMe: charmPicked,
            drawToThisPlayer: thisCard.Ext_GetOwningPlayerStats()
        );
        
        CardStats stats = dupeCharm.GetComponent<CardStats>();
        stats.UpdateSyncSoulToPlayer(extraSoul);
    }
}