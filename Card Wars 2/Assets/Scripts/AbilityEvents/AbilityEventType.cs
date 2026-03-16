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
        AnyCardPlaced,
        
        // === TILE EVENTS (happen on specific tiles, broadcast locally) ===
        CardPlacedOnTile,       
        CreatureBurnedOnTile,   
        BuffCreatureStrengthOnTile,
        BuffCreatureDefenseOnTile,
        DebuffCreatureStrengthOnTile,
        DebuffCreatureDefenseOnTile,
    }
}