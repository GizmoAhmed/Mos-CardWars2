using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.UI;

public class CreatureLand : NetworkBehaviour
{
	[Header("Image")]
	public Image image;

	public enum Element 
	{
		Null,
		Forge,
		Sky,
		Crystal,
		Tomb,
		School
	}

	[Header("Elemental")]
	public Element currentElement = Element.Null;

	[Header("Occupying Creature")]
	public bool Taken;
	public GameObject CurrentCard = null;

	[Header("Neighbors")]
	public GameObject across;

	public GameObject _Across
	{
		get { return across; }
		set { across = value; }
	}

	public GameObject AdjacentLeft;
	public GameObject AdjacentRight;

	public GameObject DiagLeft;
	public GameObject DiagRight;

	public void Start()
	{
		InitializeNeighbors();
		currentElement = Element.Null;
		image = GetComponent<Image>();
	}

	public void AssignElement(Element element)
	{
		Color color;

		switch (element)
		{
			case Element.Null:
				color = new Color32(173, 173, 173, 255);
				break;
			case Element.Forge:
				color = new Color32(255, 86, 86, 255);
				break;
			case Element.Sky:
				color = new Color32(86, 207, 255, 255);
				break;
			case Element.Crystal:
				color = new Color32(255, 111, 213, 255);
				break;
			case Element.Tomb:
				color = new Color32(149, 255, 111, 255);
				break;
			case Element.School:
				color = new Color32(255, 255, 112, 255);
				break;
			default:
				color = new Color32(173, 173, 173, 255);
				break;
		}

		image.color = color;
	}

	void Update()
	{
		if (CurrentCard != null)
		{
			Taken = true;
		}
	}

	public void AttachCard(GameObject card)
	{
		CurrentCard = card;
	}

	/// Thanks ChatGPT
	protected virtual void InitializeNeighbors()
	{
		// Assuming that the naming convention for the lands follows the pattern
		// p1Land1, p1Land2, etc. for player 1
		// p2Land1, p2Land2, etc. for player 2
		string landName = this.gameObject.name;

		if (landName.StartsWith("p1Land") || landName.StartsWith("p2Land"))
		{
			int landNumber = int.Parse(landName.Substring(6)); // Get the number part of the land

			// Set the Across field
			if (landName.StartsWith("p1Land"))
			{
				_Across = GameObject.Find("p2Land" + landNumber);
			}
			else if (landName.StartsWith("p2Land"))
			{
				_Across = GameObject.Find("p1Land" + landNumber);
			}

			// Set the AdjacentLeft and AdjacentRight fields
			if (landNumber > 1)
			{
				AdjacentLeft = GameObject.Find(landName.Substring(0, 6) + (landNumber - 1));
			}
			if (landNumber < 5) // Assuming there are 5 lands for simplicity
			{
				AdjacentRight = GameObject.Find(landName.Substring(0, 6) + (landNumber + 1));
			}

			// Set the DiagLeft and DiagRight fields
			if (landName.StartsWith("p1Land"))
			{
				if (landNumber > 1)
				{
					DiagLeft = GameObject.Find("p2Land" + (landNumber - 1));
				}
				if (landNumber < 5)
				{
					DiagRight = GameObject.Find("p2Land" + (landNumber + 1));
				}
			}
			else if (landName.StartsWith("p2Land"))
			{
				if (landNumber > 1)
				{
					DiagLeft = GameObject.Find("p1Land" + (landNumber - 1));
				}
				if (landNumber < 5)
				{
					DiagRight = GameObject.Find("p1Land" + (landNumber + 1));
				}
			}
		}
	}
}
