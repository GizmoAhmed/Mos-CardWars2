using CardScripts.CardMovements;

namespace CardScripts.CardDisplays
{
    public class BuildingDisplay : CardDisplay
    {
        public override void ToggleInfoSlide(bool toggle = true)
        {
            base.ToggleInfoSlide(toggle);

            if (infoObj.activeInHierarchy && GetComponentInParent<BaseMovement>().cardState == BaseMovement.CardState.Field) 
            {
                gameObject.transform.SetAsLastSibling();
            }
        }
    }
}