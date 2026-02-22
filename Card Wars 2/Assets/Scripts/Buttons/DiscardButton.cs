using Mirror;
using UnityEngine;

public class DiscardButton : NetworkBehaviour
{
    public GameObject discardBoard;

    public void ShowBoard()
    {
        discardBoard.SetActive(true);
    }
}