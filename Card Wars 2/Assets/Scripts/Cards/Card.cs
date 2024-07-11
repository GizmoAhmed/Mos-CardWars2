using UnityEngine;
using Mirror;
using TMPro;

public class Card : NetworkBehaviour
{
	[SyncVar] public string Name;
	private TextMeshProUGUI NameText;

	private Player player;

	[Header("Card Movement")]
	public bool Grabbed;
	public bool Movable = true;

	private GameObject StartParent;
	private Vector2 StartPos;
	private GameObject NewDropZone;
	public bool isOverDropZone;

	public string landTag;

	[Header("Zooms")]
	public bool isZoomLocked;

	private Vector2 currentMousePos;
	private Vector2 clickSave;

	[Header("Card Traits")]
	[Tooltip("Is this your Card?")] [SyncVar] public bool Ally;
	private TextMeshProUGUI MagicText;

	[SyncVar] public int MagicCost = 0;

	[SyncVar] public GameObject MyLand;

	public enum CardState
	{
		Deck,
		Hand,
		Placed,
		Discard
	}

	public CardState currentState = CardState.Deck;

	public void SetState(CardState newState)
	{
		currentState = newState;
	}

	public void Start()
	{
		NetworkIdentity networkIdentity = NetworkClient.connection.identity;
		player = networkIdentity.GetComponent<Player>();

		if (isOwned)
		{
			Movable = true;
		}
		else
		{
			Movable = false;
		}

		MagicText = transform.Find("Magic").GetComponent<TextMeshProUGUI>();
		MagicText.text = MagicCost.ToString();

		NameText = transform.Find("Name").GetComponent<TextMeshProUGUI>();
		NameText.text = Name;
	}

	public bool IsOwnedByLocalPlayer()
	{
		return isOwned;
	}

	protected virtual void OnTriggerStay2D(Collider2D other)
	{
		int currentMagic = player.Magic;
		bool EnoughMagic = MagicCost <= currentMagic;

		CreatureLand landscript = other.GetComponent<CreatureLand>();

		if (landscript != null)
		{
			if (other.CompareTag(landTag) && isOwned && landscript.Taken == false && EnoughMagic)
			{
				NewDropZone = other.gameObject;
				isOverDropZone = true;
			}
		}
	}

	private void OnTriggerExit2D(Collider2D collision)
	{
		NewDropZone = null;
		isOverDropZone = false;
	}

	public void Zoom()
	{
		ZoomClick zoom = GetComponent<ZoomClick>();

		zoom.ZoomIn();
	}

	public void PointerDown()
	{
		clickSave = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
	}

	public void PointerUp()
	{
		CardFlipper flip = GetComponent<CardFlipper>();

		// If the mouse position has not changed, zoom into the card
		if (clickSave == currentMousePos && flip.currentFace == CardFlipper.FaceState.FaceUp && !isZoomLocked)
		{
			Zoom();
		}
	}

	public void Grab()
	{
		if (!Movable || isZoomLocked) return;

		Grabbed = true;

		StartParent = transform.parent.gameObject;
		StartPos = transform.position;
	}

	public void LetGo()
	{
		if (!Movable || isZoomLocked) return;

		Grabbed = false;

		if (isOverDropZone)
		{
			PlaceCard(NewDropZone);
			player.CmdShowConsumable(player.Magic - MagicCost, "magic");
		}
		else
		{
			transform.position = StartPos;
			transform.SetParent(StartParent.transform, false);
		}
	}

	public void PlaceCard(GameObject land)
	{
		Movable = false;

		CreatureLand landscript = land.GetComponent<CreatureLand>();

		// Link them
		landscript.AttachCard(gameObject);
		MyLand = land;

		transform.SetParent(land.transform, true);
		transform.localPosition = Vector2.zero;

		SetState(CardState.Placed);
		player.CmdDropCard(gameObject, currentState, land);
	}

	void Update()
	{
		currentMousePos = new Vector2(Input.mousePosition.x, Input.mousePosition.y);

		if (currentState == CardState.Placed)
		{
			Movable = false;
		}

		if (Movable && Grabbed)
		{
			transform.position = currentMousePos;
		}
	}
}
