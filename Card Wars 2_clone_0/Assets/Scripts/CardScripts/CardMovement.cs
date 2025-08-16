using Mirror;
using UnityEngine;

public class CardMovement : NetworkBehaviour
{
    private void Start()
    {
        Debug.Log("Card Movement Start for " + gameObject.name);
    }

    public void OnPointerEnter()
    {
        Debug.Log("Pointer Entered: " + gameObject.name);
    }

    public void OnPointerExit()
    {
        Debug.Log("OnPointerExit: " +  gameObject.name);
    }

    public void BeginDrag()
    {
        Debug.Log("BeginDrag: " + gameObject.name);
    }

    public void EndDrag()
    {
        Debug.Log("EndDrag: " + gameObject.name);
    }
}
