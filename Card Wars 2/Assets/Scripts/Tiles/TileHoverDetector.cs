using CardScripts.CardMovements;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Tiles
{
    public class TileHoverDetector : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        private Tile _tile;
    
        void Awake()
        {
            _tile = GetComponent<Tile>();
        }
    
        public void OnPointerEnter(PointerEventData eventData)
        {
            // Only notify if a card is being dragged
            CardMovement draggedCard = CardDragManager.Instance?.CurrentlyDraggedCard;
        
            if (draggedCard != null)
            {
                draggedCard.OnTileHoverEnter(_tile);
            }
        }
    
        public void OnPointerExit(PointerEventData eventData)
        {
            CardMovement draggedCard = CardDragManager.Instance?.CurrentlyDraggedCard;
        
            if (draggedCard != null)
            {
                draggedCard.OnTileHoverExit(_tile);
            }
        }
    }
}