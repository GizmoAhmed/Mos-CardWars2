using UnityEngine;
using UnityEngine.UI;
using Mirror;

public class CreatureLand : NetworkBehaviour
{
	private Player player;

	[Header("Image")]
	public Image image;

	public enum LandElement
	{
		Null,
		Forge,
		Spirit,
		Crystal,
		Tomb,
		School
	}

	[Header("Elemental")] public LandElement currentElement = LandElement.Null;

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
		currentElement = LandElement.Null;
		image = GetComponent<Image>();
	}

	public void ChangeElement(LandElement element)
	{
		currentElement = element;

		Color color;

		switch (element)
		{
			case LandElement.Null:
				color = new Color32(173, 173, 173, 255);
				break;
			case LandElement.Forge:
				color = new Color32(255, 86, 86, 255);
				break;
			case LandElement.Spirit:
				color = new Color32(86, 207, 255, 255);
				break;
			case LandElement.Crystal:
				color = new Color32(255, 111, 213, 255);
				break;
			case LandElement.Tomb:
				color = new Color32(149, 255, 111, 255);
				break;
			case LandElement.School:
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

	public virtual void AttachCard(GameObject card)
	{
		CurrentCard = card;
		Taken = true;
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
		SelectElement(LandElement.Forge);
	}

	public void SelectSpirit()
	{
		SelectElement(LandElement.Spirit);
	}

	public void SelectCrystal()
	{
		SelectElement(LandElement.Crystal);
	}

	public void SelectTomb()
	{
		SelectElement(LandElement.Tomb);
	}

	public void SelectSchool()
	{
		SelectElement(LandElement.School);
	}

	private void SelectElement(LandElement element)
	{
		player = NetworkClient.localPlayer.GetComponent<Player>();

		if (player != null)
		{
			player.CmdColorTheLand(this, element);
		}
	}
}
