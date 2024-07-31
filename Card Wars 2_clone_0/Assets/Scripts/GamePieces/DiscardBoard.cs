using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class DiscardBoard : NetworkBehaviour
{
	public List<GameObject> MyDiscardPile;

	private void Start()
	{
		gameObject.SetActive(false);
	}

	public void AddtoDiscard(GameObject card, bool yourCard)
	{
		card.transform.SetParent(transform, true);

		if (yourCard)
		{
			MyDiscardPile.Add(card); // add this card to the viewable discard pil
		}
		else 
		{
			card.SetActive(false); // you won't be able to see the card
		}
	}
}
