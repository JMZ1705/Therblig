using UnityEngine;
using UnityEngine.InputSystem;

public class ChalkLever : MonoBehaviour
{
    [Header("Sprites de la Palanca")]
    [SerializeField] private Sprite leverOffSprite; 
    [SerializeField] private Sprite leverOnSprite;  
    [Header("Referencias")]
    [SerializeField] private DoorController doorToOpen; 
    [SerializeField] private GameObject interactPrompt; 

    private SpriteRenderer spriteRenderer;
    private bool isActivated = false;
    private bool isPlayerInside = false;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (leverOffSprite != null && spriteRenderer != null)
        {
            spriteRenderer.sprite = leverOffSprite;
        }

        if (interactPrompt != null)
        {
            interactPrompt.SetActive(false);
        }
    }

    private void Update()
    {
        
        if (isPlayerInside && !isActivated && Keyboard.current != null && Keyboard.current.eKey.wasPressedThisFrame)
        {
            ActivateLever();
        }
    }

    public void ActivateLever()
    {
        if (isActivated) return;

        isActivated = true;

        
        if (spriteRenderer != null && leverOnSprite != null)
        {
            spriteRenderer.sprite = leverOnSprite;
        }

        
        if (doorToOpen != null)
        {
            doorToOpen.OpenDoor();
        }

        if (interactPrompt != null)
        {
            interactPrompt.SetActive(false);
        }

        Debug.Log("<color=cyan>¡Palanca de tiza activada!</color>");
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") || other.GetComponentInParent<PlayerController>() != null)
        {
            isPlayerInside = true;
            if (!isActivated && interactPrompt != null)
            {
                interactPrompt.SetActive(true);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player") || other.GetComponentInParent<PlayerController>() != null)
        {
            isPlayerInside = false;
            if (interactPrompt != null)
            {
                interactPrompt.SetActive(false);
            }
        }
    }
}