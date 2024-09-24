using UnityEngine;
using Mirror;
using TMPro;
// 0.825 is width/heigth ratio
public class Card : NetworkBehaviour
{
	public string	CardName;

	public GameObject InfoSlide;

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

	public GameObject	MyLand;

	public enum CardState { Deck, Hand, Placed, Discard }

	public CardState currentState = CardState.Deck;

	[HideInInspector] public CardAnimator Animate;

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

		InfoSlide = gameObject.transform.Find("InfoSlide").gameObject;
		InfoSlide.SetActive(false);

		NameText = InfoSlide.transform.Find("NameText").GetComponent<TextMeshProUGUI>();
		NameText.text = CardName.ToUpper();
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

		// If the mouse position has not changed, then it was a simple click, zoom into the card
		if (clickSave == currentMousePos && flip.currentFace == CardFlipper.FaceState.FaceUp && !isZoomLocked)
		{
			// Zoom();
		}
	}

	public void PointerEnter() 
	{
		CardFlipper flip = GetComponent<CardFlipper>();

		if (flip.currentFace == CardFlipper.FaceState.FaceUp) 
		{
			ShowCardInfo(true);
		}
	}

	public void PointerExit()
	{
		ShowCardInfo(false);
	}

	public void ShowCardInfo(bool b) { InfoSlide.SetActive(b); }

	public void Grab()
	{
		if (!Movable || isZoomLocked) return;

		ShowCardInfo(false);

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
			player.CmdChangeStats(player.CurrentMagic - MagicCost, "current_magic");
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

		CanvasGroup canvasGroup = GetComponent<CanvasGroup>();

		Animate.FadeOut(canvasGroup, gameObject, player, isOwned);
	}

	void Update()
	{
		currentMousePos = new Vector2(Input.mousePosition.x, Input.mousePosition.y);

		if (Movable && Grabbed) { transform.position = currentMousePos; }
	}
}
