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
    
    public static class AbilityEventTypeExtensions
    {
        /// <summary>
        /// Determines scope from the event type name convention
        /// Global events start with "Any", Tile events end with "OnTile"
        /// </summary>
        public static bool IsGlobalEvent(this AbilityEventType eventType)
        {
            string name = eventType.ToString();
            return name.StartsWith("Any") || 
                   (!name.EndsWith("OnTile") && !name.EndsWith("Tile"));
        }
        
        public static bool IsTileEvent(this AbilityEventType eventType)
        {
            return !eventType.IsGlobalEvent();
        }
    }
}