namespace AbilityEvents
{
    public enum AbilityEventType
    {
        // === GLOBAL EVENTS (happen anywhere, broadcast globally) ===
        AnyTurnStart,
        AnyTurnEnd,
        AnyAddCardToHand,
        AnyShardsGained,
        AnyCreatureKilled,         
        AnyCardDrawn,            
        AnySpellCasted,
        AnyFieldCardPlaced,
        AnyCreaturePlaced,
        AnyCardPlaced,
        AnyCardBurned,
        AnyCreatureBurned,
        
        // === TILE EVENTS (happen on specific tiles, broadcast locally) ===
        CardPlacedOnTile,       
        CreatureBurnedOnTile,   
        CardDiscardedFromTile,
        CreatureAbilityOnTile,
        
        BuffCreatureStrengthOnTile,
        BuffCreatureDefenseOnTile,
        DebuffCreatureStrengthOnTile,
        DebuffCreatureDefenseOnTile,
    }
}