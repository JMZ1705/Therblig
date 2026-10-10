using System.Collections;
using UnityEngine;

public class BoilerVapor : MonoBehaviour
{
    [Header("Configuración del Ciclo")]
    [SerializeField] private float activeDuration = 2.0f;   // Tiempo encendido
    [SerializeField] private float inactiveDuration = 3.0f; // Tiempo apagado
    [SerializeField] private float initialDelay = 0.0f;    // Desfase inicial

    [Header("Daño")]
    [SerializeField] private float damageAmount = 5.0f;    // Cantidad de daño por cada golpe
    [SerializeField] private float damageInterval = 0.5f;   // Frecuencia del daño

    [Header("Referencias")]
    [SerializeField] private ParticleSystem vaporParticles;
    [SerializeField] private Collider2D vaporDamageCollider; // Debe ser Trigger

    private bool isVaporActive = false;
    private Coroutine damageCoroutine;

    private void Start()
    {
        if (vaporDamageCollider != null)
        {
            vaporDamageCollider.enabled = false;
        }

        if (vaporParticles != null)
        {
            vaporParticles.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        }

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
            // --- ENCENDER VAPOR ---
            isVaporActive = true;
            if (vaporParticles != null)
            {
                vaporParticles.Play();
            }
            if (vaporDamageCollider != null)
            {
                vaporDamageCollider.enabled = true;
            }

            yield return new WaitForSeconds(activeDuration);

            // --- APAGAR VAPOR Y LIMPIAR ---
            isVaporActive = false;
            if (vaporParticles != null)
            {
                vaporParticles.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            }
            if (vaporDamageCollider != null)
            {
                vaporDamageCollider.enabled = false;
            }

            yield return new WaitForSeconds(inactiveDuration);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!isVaporActive) return;

        PlayerController player = other.GetComponentInParent<PlayerController>();

        if (player != null && damageCoroutine == null)
        {
            damageCoroutine = StartCoroutine(ApplyDamageOverTime(player));
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        PlayerController player = other.GetComponentInParent<PlayerController>();

        if (player != null && damageCoroutine != null)
        {
            StopCoroutine(damageCoroutine);
            damageCoroutine = null;
        }
    }

    private IEnumerator ApplyDamageOverTime(PlayerController player)
    {
        while (isVaporActive && player != null)
        {
            // Llama al método oficial del jugador para aplicar daño y actualizar la UI correctamente
            player.TakeDamage(damageAmount);

            // Desactiva el aturdimiento/bloqueo de movimiento para que el jugador pueda seguir moviéndose
            player.hitTime = 0;

            Debug.Log($"<color=red>¡Daño de Caldera!</color> Vida restante: {player.health}");

            yield return new WaitForSeconds(damageInterval);
        }

        damageCoroutine = null;
    }
}