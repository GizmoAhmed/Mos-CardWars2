using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class DiscardBoard : NetworkBehaviour
{
	public List<GameObject> MyDiscardPile;

	private void Start() { gameObject.SetActive(false); }

	public void AddtoDiscard(GameObject card)
	{
		MyDiscardPile.Add(card);
		card.transform.SetParent(transform, false);
	}
}
