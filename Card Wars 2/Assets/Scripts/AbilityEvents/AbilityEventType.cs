namespace AbilityEvents
{
    public enum AbilityEventType
    {
        // Unity serializes enums as integers (their index position), not their names
        // So when you add a new one, they all shift, which messes up every event as each one shifts in the inspector
        // To alleviate this, set there indexes like below
        
        // === GLOBAL EVENTS (happen anywhere, broadcast globally) ===
        AnyTurnStart = 0,
        AnyTurnEnd = 1,
        AnyAddCardToHand = 2,
        AnyShardsGained = 3,
        AnyCreatureKilled = 4,         
        AnyCardDrawn = 5,            
        AnySpellCasted = 6,
        AnyFieldCardPlaced = 7,
        AnyCreaturePlaced = 8,
        AnyCardPlaced = 9,
        AnyCardBurned = 10,
        AnyCreatureBurned = 11,
        AnySoulUpgrade = 12, // add below ↓, BUT above the Tile events
        
        // === TILE EVENTS (happen on specific tiles, broadcast locally) ===
        CardPlacedOnTile = 100,       
        CreatureBurnedOnTile = 101,   
        CardDiscardedFromTile = 102,
        CreatureAbilityOnTile = 103,
        
        BuffCreatureStrengthOnTile = 104,
        BuffCreatureDefenseOnTile = 105,
        DebuffCreatureStrengthOnTile = 106,
        DebuffCreatureDefenseOnTile = 107, // add new ones below↓
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