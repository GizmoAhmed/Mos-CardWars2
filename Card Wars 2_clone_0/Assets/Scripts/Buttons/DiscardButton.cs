using Mirror;
using UnityEngine;

public class DiscardButton : NetworkBehaviour
{
    public GameObject DiscardBoard;

    public void ShowBoard() { DiscardBoard.SetActive(true); }
    public void HideBoard() { DiscardBoard.SetActive(false); }
}
