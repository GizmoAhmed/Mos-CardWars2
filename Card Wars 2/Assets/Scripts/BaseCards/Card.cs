using UnityEngine;
using Mirror;
using TMPro;

// 0.825 is width/heigth ratio
public class Card : NetworkBehaviour
{
	[Header("Base Card Includes: ")]
	public string CardName;

	public GameObject InfoSlide;

	private TextMeshProUGUI NameText;

	[HideInInspector] public Player player;

	private bool Grabbed;

	private GameObject StartParent;
	private Vector2 StartPos;
	private GameObject NewDropZone;
	private bool isOverDropZone;

	[HideInInspector] public bool Movable = true;

	public string landTag;

	[HideInInspector] public Vector2 currentMousePos;

	public GameObject MyLand;

	public enum CardState { Deck, Hand, Placed, Discard }

	public CardState currentState = CardState.Deck;

	[HideInInspector] public CardAnimator Animate;

	private TextMeshProUGUI MagicText;
	[SyncVar] public int MagicCost = 0;

	public void SetState(CardState newState) { currentState = newState; }

	[HideInInspector] public Vector2 clickSave;

	protected virtual void Start()
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

	protected virtual void OnTriggerStay2D(Collider2D other)
	{
		bool EnoughMagic = MagicCost <= player.CurrentMagic;

		CreatureLand landscript = other.GetComponent<CreatureLand>();

		if (landscript == null) return;

		if (other.CompareTag(landTag) && isOwned && EnoughMagic)
		{
			//there are only creatuere and building tagged lands, probably trying to place one of those two
			NewDropZone = other.gameObject;
			isOverDropZone = true;
		}
		else if (landTag == "SpellLand")
		{
			if (ValidCastPlacement(landscript))
			{
				NewDropZone = other.gameObject;
				isOverDropZone = true;
			}
		}
	}

	// only spell card ever reaches here, so don't worry about creature or building return false 
	public virtual bool ValidCastPlacement(CreatureLand landscript) { return false; }

	private void OnTriggerExit2D(Collider2D collision)
	{
		NewDropZone = null;
		isOverDropZone = false;
	}

	[ClientRpc]
	public virtual void RpcDecay() { }

	public virtual void ShowCardInfo(bool b) { InfoSlide.SetActive(b); }

	public void PointerEnter()
	{
		CardFlipper flip = GetComponent<CardFlipper>();

		ShowCardInfo(flip.currentFace == CardFlipper.FaceState.FaceUp && !Grabbed);
	}

	public void PointerExit()
	{
		ShowCardInfo(false);
	}

	public virtual void PointerDown()
	{
		clickSave = currentMousePos;
	}

	public virtual void PointerUp()
	{
		CardFlipper flip = GetComponent<CardFlipper>();

		// If the mouse position has not changed, then it was a simple click
		if ((currentMousePos == clickSave) && flip.currentFace == CardFlipper.FaceState.FaceUp)
		{
			Debug.Log("Base Card Click: " + name);
		}
	}

	public virtual void Grab()
	{
		if (!Movable) return;

		ShowCardInfo(false);

		Grabbed = true;

		StartParent = transform.parent.gameObject;
		StartPos = transform.position;
	}

	public virtual void LetGo()
	{
		if (!Movable) return;

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

	public virtual void PlaceCard(GameObject land)
	{
		// Debug.LogWarning(name + ", which is a normal card, is placing...");

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

	public virtual void CardReset()
	{
		// make card reset, aka ethans attack boost build up and a some buildings stat or something
	}
}
