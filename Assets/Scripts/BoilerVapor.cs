using System.Collections;
using UnityEngine;

public class BoilerVapor : MonoBehaviour
{
    [Header("Configuración del Ciclo")]
    [SerializeField] private float activeDuration = 2.0f;   // Tiempo que el vapor está activo
    [SerializeField] private float inactiveDuration = 3.0f; // Tiempo que el vapor está apagado
    [SerializeField] private float initialDelay = 0.0f;     // Retraso inicial del ciclo

    [Header("Daño y Empujón")]
    [SerializeField] private float damageAmount = 5.0f;     // Daño por cada tick
    [SerializeField] private float damageInterval = 0.5f;   // Frecuencia del daño

    [Header("Referencias")]
    [SerializeField] private Animator vaporAnimator;
    [SerializeField] private SpriteRenderer vaporSprite;
    [SerializeField] private Collider2D vaporDamageCollider;

    private bool isVaporActive = false;
    private Coroutine damageCoroutine;

    private void Start()
    {
        // Auto-asignación de referencias si están vacías
        if (vaporAnimator == null) vaporAnimator = GetComponent<Animator>();
        if (vaporSprite == null) vaporSprite = GetComponent<SpriteRenderer>();
        if (vaporDamageCollider == null) vaporDamageCollider = GetComponent<Collider2D>();

        // Estado inicial: apagado
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
            // --- FASE 1: ACTIVAR VAPOR ---
            isVaporActive = true;

            if (vaporSprite != null) vaporSprite.enabled = true;
            if (vaporDamageCollider != null) vaporDamageCollider.enabled = true;

            if (vaporAnimator != null)
            {
                vaporAnimator.SetBool("isVaporActive", true);
            }

            yield return new WaitForSeconds(activeDuration);

            // --- FASE 2: DESACTIVAR Y OCULTAR VAPOR ---
            isVaporActive = false;

            if (vaporDamageCollider != null) vaporDamageCollider.enabled = false;
            if (vaporSprite != null) vaporSprite.enabled = false; // Se oculta visualmente

            if (vaporAnimator != null)
            {
                vaporAnimator.SetBool("isVaporActive", false);
            }

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

    private void OnTriggerExit2D(Collider2D collision)
    {
        PlayerController player = collision.GetComponentInParent<PlayerController>();

        if ((player != null || collision.CompareTag("Player")) && damageCoroutine != null)
        {
            StopCoroutine(damageCoroutine);
            damageCoroutine = null;
        }
    }

    private IEnumerator ApplyDamageOverTime(PlayerController player)
    {
        while (isVaporActive && player != null)
        {
            bool fromRight = transform.position.x > player.transform.position.x;

            // Dispara el daño y rebote en el PlayerController
            player.ApplyKnockbackAndDamage(damageAmount, fromRight);

            Debug.Log($"<color=red>¡Daño de Vapor!</color> Vida restante: {player.health}");

            yield return new WaitForSeconds(damageInterval);
        }

        damageCoroutine = null;
    }
}