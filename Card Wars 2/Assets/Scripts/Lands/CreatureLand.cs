using UnityEngine;
using UnityEngine.UI;
using Mirror;
using static CreatureLand;

public class CreatureLand : NetworkBehaviour
{
	private Player player;

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

	[Header("Elemental")] public Element currentElement = Element.Null;

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

	public void OnElementChanged(Element element)
	{
		currentElement = element;
		UpdateElementColor(element);
	}

	public void UpdateElementColor(Element element)
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

		if (image != null)
		{
			image.color = color;
		}
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

	protected virtual void InitializeNeighbors()
	{
		string landName = this.gameObject.name;

		if (landName.StartsWith("p1Land") || landName.StartsWith("p2Land"))
		{
			int landNumber = int.Parse(landName.Substring(6));

			if (landName.StartsWith("p1Land"))
			{
				_Across = GameObject.Find("p2Land" + landNumber);
			}
			else if (landName.StartsWith("p2Land"))
			{
				_Across = GameObject.Find("p1Land" + landNumber);
			}

			if (landNumber > 1)
			{
				AdjacentLeft = GameObject.Find(landName.Substring(0, 6) + (landNumber - 1));
			}
			if (landNumber < 5)
			{
				AdjacentRight = GameObject.Find(landName.Substring(0, 6) + (landNumber + 1));
			}

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

	// Methods for each button to assign an element
	public void SelectForge()
	{
		SelectElement(Element.Forge);
	}

	public void SelectSky()
	{
		SelectElement(Element.Sky);
	}

	public void SelectCrystal()
	{
		SelectElement(Element.Crystal);
	}

	public void SelectTomb()
	{
		SelectElement(Element.Tomb);
	}

	public void SelectSchool()
	{
		SelectElement(Element.School);
	}

	private void SelectElement(Element element)
	{
		player = NetworkClient.localPlayer.GetComponent<Player>();

		if (player != null)
		{
			player.CmdColorTheLand(this, element);
		}
	}
}
