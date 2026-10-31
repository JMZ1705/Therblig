using System.Collections;
using UnityEngine;

public class BoilerVapor : MonoBehaviour
{
    [Header("Configuración del Ciclo")]
    [SerializeField] private float activeDuration = 2.0f;   
    [SerializeField] private float inactiveDuration = 3.0f; 
    [SerializeField] private float initialDelay = 0.0f;    

    [Header("Daño y Empujón")]
    [SerializeField] private float damageAmount = 5.0f;    
    [SerializeField] private float damageInterval = 0.5f;   
    [SerializeField] private float knockbackForce = 6.0f;   

    [Header("Referencias")]
    [SerializeField] private SpriteRenderer vaporSprite;
    [SerializeField] private Collider2D vaporDamageCollider;

    private bool isVaporActive = false;
    private Coroutine damageCoroutine;

    private void Start()
    {
        if (vaporSprite == null) vaporSprite = GetComponent<SpriteRenderer>();
        if (vaporDamageCollider == null) vaporDamageCollider = GetComponent<Collider2D>();

        if (vaporDamageCollider != null) vaporDamageCollider.enabled = false;
        if (vaporSprite != null) vaporSprite.enabled = false;

        StartCoroutine(VaporCycleRoutine());
    }

    private IEnumerator VaporCycleRoutine()
    {
        if (initialDelay > 0)
        {
            yield return new WaitForSeconds(initialDelay);
        }

        while (true)
        {
           
            isVaporActive = true;
            if (vaporSprite != null) vaporSprite.enabled = true;
            if (vaporDamageCollider != null) vaporDamageCollider.enabled = true;

            yield return new WaitForSeconds(activeDuration);

            
            isVaporActive = false;
            if (vaporSprite != null) vaporSprite.enabled = false;
            if (vaporDamageCollider != null) vaporDamageCollider.enabled = false;

            yield return new WaitForSeconds(inactiveDuration);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!isVaporActive) return;

        PlayerController player = other.GetComponentInParent<PlayerController>();

        if ((player != null || other.CompareTag("Player")) && damageCoroutine == null)
        {
            if (player == null) player = other.GetComponent<PlayerController>();

            if (player != null)
            {
                damageCoroutine = StartCoroutine(ApplyDamageOverTime(player));
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        PlayerController player = other.GetComponentInParent<PlayerController>();

        if ((player != null || other.CompareTag("Player")) && damageCoroutine != null)
        {
            StopCoroutine(damageCoroutine);
            damageCoroutine = null;
        }
    }

    private IEnumerator ApplyDamageOverTime(PlayerController player)
    {
        while (isVaporActive && player != null)
        {
            
            player.TakeDamage(damageAmount);

            
            player.hitTime = 0.3f; 
            player.hitForce = knockbackForce; 

           
            player.hitFromRight = transform.position.x > player.transform.position.x;

            Debug.Log($"<color=red>¡Daño de Caldera!</color> Vida restante: {player.health}");

            yield return new WaitForSeconds(damageInterval);
        }

        damageCoroutine = null;
    }
}