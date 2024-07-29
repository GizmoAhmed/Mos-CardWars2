using UnityEngine;
using Mirror;
using TMPro;

public class Card : NetworkBehaviour
{
	public string	Name;

	private TextMeshProUGUI NameText;

	private Player	player;

	private bool	Grabbed;

	private GameObject	StartParent;
	private Vector2		StartPos;
	private GameObject	NewDropZone;
	private bool		isOverDropZone;

	[HideInInspector] public bool	Movable = true;
	[HideInInspector] public string	landTag;
	[HideInInspector] public bool	isZoomLocked;

	private Vector2		currentMousePos;
	private Vector2		clickSave;

	public GameObject MyLand;

	public enum CardState { Deck, Hand, Placed, Discard }

	public CardState currentState = CardState.Deck;

	public CardAnimator Animate;

	private TextMeshProUGUI MagicText;
	[SyncVar] public int MagicCost = 0;

	public void SetState(CardState newState) { currentState = newState; }

	public void Start()
	{
		NetworkIdentity networkIdentity = NetworkClient.connection.identity;
		player = networkIdentity.GetComponent<Player>();

		Movable = isOwned;

		Animate = GetComponent<CardAnimator>();

		MagicText = transform.Find("Magic").GetComponent<TextMeshProUGUI>();
		MagicText.text = MagicCost.ToString();

		NameText = transform.Find("Name").GetComponent<TextMeshProUGUI>();
		NameText.text = Name.ToUpper();
	}

	public bool IsOwnedByLocalPlayer() { return isOwned; }

	protected virtual void OnTriggerStay2D(Collider2D other)
	{
		bool EnoughMagic = MagicCost <= player.CurrentMagic;

		CreatureLand landscript = other.GetComponent<CreatureLand>();
		
		if (landscript != null && other.CompareTag(landTag) && isOwned && landscript.Taken == false && EnoughMagic)
		{
			NewDropZone		= other.gameObject;
			isOverDropZone	= true;
		}
	}

	private void OnTriggerExit2D(Collider2D collision)
	{
		NewDropZone = null;
		isOverDropZone = false;
	}

	public void Zoom() { GetComponent<ZoomClick>().ZoomIn(); }

	[ClientRpc]
	public virtual void RpcDecay() { }

	public void PointerDown() { clickSave = new Vector2(Input.mousePosition.x, Input.mousePosition.y); }

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
			player.CmdShowStats(player.CurrentMagic - MagicCost, "current_magic");
		}
		else
		{
			transform.position = StartPos;
			transform.SetParent(StartParent.transform, false);
		}
	}

	public void PlaceCard(GameObject land)
	{
		transform.SetParent(land.transform, true);
		transform.localPosition = Vector2.zero;

		Movable = false;

		player.CmdDropCard(gameObject, CardState.Placed, land);
	}

	public virtual void Discard() 
	{
		SetState(CardState.Discard);

		MyLand.GetComponent<CreatureLand>().DetachCard(gameObject);

		player.discard.AddtoDiscard(gameObject, isOwned);
	}

	void Update()
	{
		currentMousePos = new Vector2(Input.mousePosition.x, Input.mousePosition.y);

		if (Movable && Grabbed) { transform.position = currentMousePos; }
	}
}
