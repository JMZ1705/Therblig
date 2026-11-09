/* ============================================================================
 *  BossEnemy.cs
 * ----------------------------------------------------------------------------
 *  Jefe final melee: Persigue al jugador horizontalmente usando Rigidbody2D.
 *  Inflige 25 de daño y sufre daño si el jugador lo ataca desde el aire o picada.
 * ========================================================================= */

using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody2D))]
public class BossEnemy : MonoBehaviour
{
    [Header("Estadísticas del Jefe")]
    [SerializeField] private float maxHealth = 150f;
    [SerializeField] private float currentHealth;
    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private float contactDamage = 25f;
    [SerializeField] private float attackCooldown = 1.2f;
    [SerializeField] private float stoppingDistance = 0.8f;
    private float nextAttackTime = 0f;

    [Header("Referencias")]
    [SerializeField] private Transform playerTransform;
    [SerializeField] private bool isFacingRight = true;

    [Header("Efectos Visuales")]
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private float flashDuration = 0.15f;

    private Rigidbody2D rb;
    private Color originalColor;
    private bool isDead = false;
    private bool isStunned = false;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.freezeRotation = true;

        if (spriteRenderer == null) spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null) originalColor = spriteRenderer.color;

        currentHealth = maxHealth;
    }

    private void Start()
    {
        FindPlayerReference();
    }

    private void Update()
    {
        if (isDead) return;

        if (playerTransform == null)
        {
            FindPlayerReference();
            return;
        }

        float targetX = playerTransform.position.x;
        float currentX = transform.position.x;

        if (targetX > currentX && !isFacingRight)
        {
            Flip();
        }
        else if (targetX < currentX && isFacingRight)
        {
            Flip();
        }
    }

    private void FixedUpdate()
    {
        if (isDead || isStunned || playerTransform == null) return;

        float distanceToPlayer = Mathf.Abs(playerTransform.position.x - transform.position.x);

        if (distanceToPlayer > stoppingDistance)
        {
            float moveDirection = playerTransform.position.x > transform.position.x ? 1f : -1f;
            rb.linearVelocity = new Vector2(moveDirection * moveSpeed, rb.linearVelocity.y);
        }
        else
        {
            rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
        }
    }

    private void FindPlayerReference()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null) playerTransform = playerObj.transform;
    }

    private void Flip()
    {
        isFacingRight = !isFacingRight;
        Vector3 localScale = transform.localScale;
        localScale.x *= -1f;
        transform.localScale = localScale;
    }

    public void TakeDamageFromPlayer(float damageAmount, Vector3 attackerPosition)
    {
        if (isDead) return;

        PlayerController player = playerTransform != null ? playerTransform.GetComponent<PlayerController>() : null;

        bool playerIsInAir = (player != null && !player.canJump);
        bool isAttackedFromAbove = attackerPosition.y > (transform.position.y - 0.2f);

        if (playerIsInAir || isAttackedFromAbove)
        {
            currentHealth -= damageAmount;
            Debug.Log($"<color=red>¡Daño aéreo al Boss! Vida restante: {currentHealth}/{maxHealth}</color>");

            float pushDir = attackerPosition.x > transform.position.x ? -1f : 1f;
            rb.linearVelocity = new Vector2(pushDir * 3f, rb.linearVelocity.y);

            StartCoroutine(FlashRedAndPause());

            if (currentHealth <= 0)
            {
                Die();
            }
        }
        else
        {
            Debug.Log("<color=yellow>¡Ataque terrestre bloqueado! El Boss solo recibe daño en el aire.</color>");
        }
    }

    private IEnumerator FlashRedAndPause()
    {
        isStunned = true;

        if (spriteRenderer != null)
        {
            spriteRenderer.color = Color.red;
            yield return new WaitForSeconds(flashDuration);
            spriteRenderer.color = originalColor;
        }
        else
        {
            yield return new WaitForSeconds(flashDuration);
        }

        isStunned = false;
    }

    private void Die()
    {
        isDead = true;
        rb.linearVelocity = Vector2.zero;
        Debug.Log("<color=green>¡El Jefe Final ha sido derrotado!</color>");
        gameObject.SetActive(false);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        HandlePlayerContact(collision.gameObject);
    }

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
        if (Time.time < nextAttackTime) return;

        PlayerController player = obj.GetComponentInParent<PlayerController>();
        if (player == null) player = obj.GetComponent<PlayerController>();
        if (player == null) player = obj.GetComponentInChildren<PlayerController>();

        if (player != null && player.hitTime <= 0)
        {
            nextAttackTime = Time.time + attackCooldown;

            bool fromRight = transform.position.x > player.transform.position.x;

            player.ApplyKnockbackAndDamage(contactDamage, fromRight);

            float bossBounceDir = fromRight ? 2f : -2f;
            rb.linearVelocity = new Vector2(bossBounceDir, rb.linearVelocity.y);
        }
    }
}