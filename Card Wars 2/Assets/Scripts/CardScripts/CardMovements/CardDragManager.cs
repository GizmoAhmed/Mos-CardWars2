using CardScripts.CardMovements;
using UnityEngine;

public class CardDragManager : MonoBehaviour
{
    public static CardDragManager Instance { get; private set; }
    
    public CardMovement CurrentlyDraggedCard { get; private set; }
    
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    public void RegisterDraggedCard(CardMovement card)
    {
        CurrentlyDraggedCard = card;
    }
    
    public void UnregisterDraggedCard(CardMovement card)
    {
        if (CurrentlyDraggedCard == card)
        {
            CurrentlyDraggedCard = null;
        }
    }
}