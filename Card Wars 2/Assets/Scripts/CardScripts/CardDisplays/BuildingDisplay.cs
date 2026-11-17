using CardScripts.CardMovements;

namespace CardScripts.CardDisplays
{
    public class BuildingDisplay : CardDisplay
    {
        public override void ToggleInfoSlide(bool toggle = true)
        {
            base.ToggleInfoSlide(toggle);
            
            // name and info right are shrunk in the base
            Animator.ScaleInfoSlide(InfoLeft, grow: false, instant: true);

            if (InfoObj.activeInHierarchy && GetComponentInParent<CardMovement>().cardState == CardMovement.CardState.Field) 
            {
                gameObject.transform.SetAsLastSibling();
            }
        }
    }
}