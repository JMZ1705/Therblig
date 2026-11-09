/* ============================================================================
 *  BossEnemy.cs
 * ----------------------------------------------------------------------------
 *  Jefe final melee: Persigue al jugador, inflige 25 de daño con empujón y
 *  solo puede ser dañado cuando recibe ataques desde arriba.
 * ========================================================================= */

using UnityEngine;

public class BossEnemy : MonoBehaviour
{
    [Header("Estadísticas del Jefe")]
    [SerializeField] private float maxHealth = 150f;
    [SerializeField] private float currentHealth;
    [SerializeField] private float moveSpeed = 2.5f;
    [SerializeField] private float contactDamage = 25f;

    [Header("Referencias")]
    [SerializeField] private Transform playerTransform;
    [SerializeField] private bool isFacingRight = true;

    [Header("Efectos Visuales / Daño")]
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private float flashDuration = 0.15f;

    private Rigidbody2D rb;
    private Color originalColor;
    private bool isDead = false;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        if (spriteRenderer == null) spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null) originalColor = spriteRenderer.color;

        currentHealth = maxHealth;
    }

    private void Start()
    {
        if (playerTransform == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null) playerTransform = playerObj.transform;
        }
    }

    private void Update()
    {
        if (isDead) return;

        // Buscar referencia si el Player hizo respawn
        if (playerTransform == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null) playerTransform = playerObj.transform;
            return;
        }

        FollowPlayer();
    }

    // Persigue horizontalmente al personaje
    private void FollowPlayer()
    {
        float targetX = playerTransform.position.x;
        float currentX = transform.position.x;

        // Movimiento horizontal hacia la posición del jugador
        Vector2 targetPosition = new Vector2(targetX, transform.position.y);
        transform.position = Vector2.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);

        // Voltear sprite según la dirección
        if (targetX > currentX && !isFacingRight)
        {
            Flip();
        }
        else if (targetX < currentX && isFacingRight)
        {
            Flip();
        }
    }

    private void Flip()
    {
        isFacingRight = !isFacingRight;
        Vector3 localScale = transform.localScale;
        localScale.x *= -1f;
        transform.localScale = localScale;
    }

    // Método que evalúa la dirección del golpe recibido
    public void TakeDamageFromPlayer(float damageAmount, Vector3 attackerPosition)
    {
        if (isDead) return;

        // Comprueba si el atacante está ubicado por encima de la cabeza del jefe
        bool isAttackedFromAbove = attackerPosition.y > (transform.position.y + 0.3f);

        if (isAttackedFromAbove)
        {
            currentHealth -= damageAmount;
            Debug.Log($"<color=red>¡Jefe golpeado desde arriba! Vida restante: {currentHealth}/{maxHealth}</color>");

            StartCoroutine(FlashRed());

            if (currentHealth <= 0)
            {
                Die();
            }
        }
        else
        {
            Debug.Log("<color=orange>¡El ataque rebotó! El jefe solo recibe daño desde arriba.</color>");
        }
    }

    private System.Collections.IEnumerator FlashRed()
    {
        if (spriteRenderer != null)
        {
            spriteRenderer.color = Color.red;
            yield return new WaitForSeconds(flashDuration);
            spriteRenderer.color = originalColor;
        }
    }

    private void Die()
    {
        isDead = true;
        Debug.Log("<color=yellow>¡Jefe final derrotado!</color>");
        gameObject.SetActive(false);
    }

    // Colisión cuerpo a cuerpo con el jugador
    private void OnCollisionStay2D(Collision2D collision)
    {
        HandlePlayerContact(collision.gameObject);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        HandlePlayerContact(other.gameObject);
    }

    private void HandlePlayerContact(GameObject obj)
    {
        if (isDead) return;

        PlayerController player = obj.GetComponentInParent<PlayerController>();
        if (player == null) player = obj.GetComponent<PlayerController>();

        if (player != null)
        {
            // Determinar si el golpe viene de la derecha para aplicar el empujón correcto
            player.hitFromRight = transform.position.x > player.transform.position.x;
            player.TakeDamage(contactDamage);
        }
    }
}