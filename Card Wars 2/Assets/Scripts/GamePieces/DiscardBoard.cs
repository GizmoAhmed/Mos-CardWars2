using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class DiscardBoard : NetworkBehaviour
{
	public List<GameObject> MyDiscardPile;

	private void Start() { gameObject.SetActive(false); }

	public void AddtoDiscard(GameObject card, bool yourCard)
	{
		card.transform.SetParent(transform, true);

		if (yourCard)
		{
			MyDiscardPile.Add(card);
		}
		else 
		{
			card.SetActive(false);
		}
	}
}
