using Mirror;
using UnityEngine;

public class CardMovement : NetworkBehaviour
{
    [HideInInspector] public Player player;

    private bool Grabbed;
    private bool returningToParent = false;

    private GameObject StartParent;
    private Vector2 StartPos;
    private GameObject NewDropZone;
    private bool isOverDropZone;

    [Tooltip("ideally should be 12f")] public float cardSnapSpeed = 12f;
    
    private void Start()
    {
        // NetworkIdentity networkIdentity = NetworkClient.connection.identity;
        // player = networkIdentity.GetComponent<Player>();
        
        Debug.Log("woah");
    }

    public void OnPointerEnter()
    {
        // Debug.Log("Pointer Entered: " + gameObject.name);
    }

    public void OnPointerExit()
    {
        // Debug.Log("OnPointerExit: " +  gameObject.name);
    }

    public void BeginDrag()
    {
        Grabbed = true;

        StartParent = transform.parent.gameObject;
        StartPos = transform.position;
    }

    public void EndDrag()
    {
        Grabbed = false;

        if (isOverDropZone)
        {
            Debug.Log("Dropping on dropzone: " + NewDropZone.name);
        }
        else
        {
            returningToParent = true;
        }
    }
    
    void Update()
    {
        Vector2 currentMousePos = new Vector2(Input.mousePosition.x, Input.mousePosition.y);

        if (Grabbed)
        {
            // Drag movement
            transform.position = Vector3.Lerp(transform.position, currentMousePos, Time.deltaTime * cardSnapSpeed);
        }
        else if (returningToParent)
        {
            // Snap back to start position
            transform.position = Vector3.Lerp(transform.position, StartPos, Time.deltaTime * cardSnapSpeed);

            // Optional: stop lerping when close enough
            if (Vector3.Distance(transform.position, StartPos) < 0.1f)
            {
                transform.position = StartPos;
                transform.SetParent(StartParent.transform, false);
                returningToParent = false;
            }
        }
    }
}
