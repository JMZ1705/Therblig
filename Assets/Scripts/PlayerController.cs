/* ============================================================================
 *  PlayerController.cs
 * ----------------------------------------------------------------------------
 *  Controla al jugador 2D: movimiento, salto, dash, wall slide/jump, vida, UI,
 *  respawn, empujón (knockback) y ataque a enemigos.
 * ========================================================================= */

using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using Unity.Cinemachine;
using System.Collections;

public class PlayerController : MonoBehaviour
{
    // ========================================================================
    // VARIABLES
    // ========================================================================
    public GameObject player;
    public GameObject spawn;
    public GameManager gm;

    // --- Movimiento horizontal ---
    public float direction;
    public float speed;
    public Rigidbody2D rb;

    // --- Salto ---
    public float jumpForce;
    public bool canJump;

    // --- Detección de suelo ---
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckRadius;
    [SerializeField] private LayerMask groundLayer;

    // --- Dash ---
    private bool canDash = true;
    private bool isDashing = false;
    private float dashingPower = 24f;
    private float dashingTime = 0.2f;
    private float dashingCooldown = 1f;

    [SerializeField] private TrailRenderer tr;

    // --- WallSlide ---
    private bool isWallSliding;
    private float wallSlidingSpeed = 0.1f;

    // --- WallJump ---
    private bool isWallJumping;
    private float wallJumpingDirection;
    private float wallJumpingTime = 0.2f;
    private float wallJumpingCounter;
    private float wallJumpingDuration = 0.4f;
    private Vector2 wallJumpingPower = new Vector2(0f, 8f);

    private int wallJumpCant = 0;

    // --- Detección de pared ---
    [SerializeField] private Transform wallCheck;
    [SerializeField] private LayerMask wallLayer;

    // --- Animación y orientación ---
    public Animator playerAnimator;
    public bool isFacingRight;
    private float yVelocity;

    // --- Vida ---
    public float health;
    [SerializeField] private float maxHealth;
    public int vidas;
    public int iVidas;

    // --- Golpe / knockback ---
    public float hitForce;
    public float hitTime;
    public bool hitFromRight;

    // --- UI ---
    public TextMeshProUGUI healthText;
    public TextMeshProUGUI vidasText;

    public CinemachineImpulseSource ImpulseSource;

    public bool checkPoint = false;

    // --- Ataque ---
    [Header("Configuración de Ataque")]
    [SerializeField] private Transform attackPoint;
    [SerializeField] private float attackRange = 0.5f;
    [SerializeField] private float attackDamage = 15f;
    [SerializeField] private LayerMask enemyLayers;
    [SerializeField] private float attackCooldown = 0.4f;
    private float nextAttackTime = 0f;

    // ========================================================================
    // START()
    // ========================================================================
    void Start()
    {
        playerAnimator = GetComponent<Animator>();
        gm = FindFirstObjectByType<GameManager>();

        vidas = iVidas;
        if (healthText != null) healthText.text = $"Health: {health}/{maxHealth}";
        if (vidasText != null) vidasText.text = $"Vidas: {vidas}";
    }

    // ========================================================================
    // FIXEDUPDATE()
    // ========================================================================
    void FixedUpdate()
    {
        if (isDashing) return;
        if (isWallJumping) return;

        // 1. Detección de suelo
        canJump = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

        if (canJump)
        {
            wallJumpCant = 0;
        }

        // 2. Lógica de deslizar por la pared (WallSlide)
        if (IsWalled() && !canJump && direction != 0f)
        {
            isWallSliding = true;
            wallJumpingCounter = wallJumpingTime;
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, Mathf.Clamp(rb.linearVelocity.y, -wallSlidingSpeed, float.MaxValue));
        }
        else
        {
            isWallSliding = false;
            wallJumpingCounter -= Time.deltaTime;
        }

        // 3. Movimiento o aturdimiento por daño
        if (hitTime <= 0)
        {
            if (isWallSliding)
            {
                rb.linearVelocity = new Vector2(0f, Mathf.Clamp(rb.linearVelocity.y, -wallSlidingSpeed, float.MaxValue));
                if (playerAnimator != null)
                {
                    playerAnimator.SetFloat("Direction", 0f);
                    playerAnimator.SetBool("isGround", canJump);
                }
            }
            else
            {
                rb.linearVelocity = new Vector2(direction * speed, rb.linearVelocity.y);

                if (playerAnimator != null)
                {
                    playerAnimator.SetFloat("Direction", direction);
                    playerAnimator.SetBool("isGround", canJump);
                    yVelocity = rb.linearVelocity.y;
                    playerAnimator.SetFloat("yVelocity", yVelocity);
                }
            }
        }
        else // Me golpearon: aplicar empujón
        {
            if (hitFromRight)
            {
                rb.linearVelocity = new Vector2(-hitForce, hitForce);
            }
            else
            {
                rb.linearVelocity = new Vector2(hitForce, hitForce);
            }

            hitTime -= Time.deltaTime;
            if (playerAnimator != null)
            {
                playerAnimator.SetFloat("isHit", hitTime);
            }
        }

        // 4. Orientación del Sprite
        if (!isFacingRight && direction < 0f)
        {
            Flip();
        }
        else if (isFacingRight && direction > 0f)
        {
            Flip();
        }
    }

    // ========================================================================
    // ATAQUE
    // ========================================================================
    public void Attack(InputAction.CallbackContext context)
    {
        if (context.performed && Time.time >= nextAttackTime && hitTime <= 0)
        {
            PerformAttack();
            nextAttackTime = Time.time + attackCooldown;
        }
    }

    private void PerformAttack()
    {
        // Se comenta hasta que crees el parámetro en el Animator:
        // if (playerAnimator != null) playerAnimator.SetTrigger("Attack");

        if (attackPoint == null) return;

        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayers);

        foreach (Collider2D enemy in hitEnemies)
        {
            EnemyHealth enemyHealth = enemy.GetComponent<EnemyHealth>();
            if (enemyHealth != null)
            {
                enemyHealth.TakeDamage(attackDamage);
            }
        }
    }

    // ========================================================================
    // DASH
    // ========================================================================
    public void DashInput(InputAction.CallbackContext context)
    {
        if (context.performed && canDash && hitTime <= 0)
        {
            StartCoroutine(Dash());
        }
    }

    private IEnumerator Dash()
    {
        canDash = false;
        isDashing = true;

        float originalGravity = rb.gravityScale;
        rb.gravityScale = 0f;

        float dashDirection = isFacingRight ? -1f : 1f;

        rb.linearVelocity = new Vector3(dashDirection * dashingPower, 0f, 0f);

        if (tr != null) tr.emitting = true;
        yield return new WaitForSeconds(dashingTime);
        if (tr != null) tr.emitting = false;

        rb.gravityScale = originalGravity;
        isDashing = false;

        yield return new WaitForSeconds(dashingCooldown);
        canDash = true;
    }

    private bool IsWalled()
    {
        if (wallCheck == null) return false;
        return Physics2D.OverlapCircle(wallCheck.position, 0.2f, wallLayer);
    }

    private void StopWallJumping()
    {
        isWallJumping = false;
    }

    // ========================================================================
    // INPUT SYSTEM (MOVE & JUMP)
    // ========================================================================
    public void Move(InputAction.CallbackContext context)
    {
        direction = context.ReadValue<Vector2>().x;
    }

    public void Jump(InputAction.CallbackContext context)
    {
        if (isWallSliding)
        {
            isWallJumping = false;
            wallJumpingDirection = isFacingRight ? -1f : 1f;
            wallJumpingCounter = wallJumpingTime;
            CancelInvoke(nameof(StopWallJumping));
        }
        else
        {
            wallJumpingCounter -= Time.deltaTime;
        }

        if (context.performed && wallJumpingCounter > 0f && wallJumpCant < 3)
        {
            isWallJumping = true;
            rb.linearVelocity = new Vector2(wallJumpingDirection * wallJumpingPower.x, wallJumpingPower.y);
            wallJumpingCounter = 0f;
            isWallSliding = false;

            if ((wallJumpingDirection > 0f && !isFacingRight) || (wallJumpingDirection < 0f && isFacingRight))
            {
                Flip();
            }
            wallJumpCant++;
            Invoke(nameof(StopWallJumping), wallJumpingDuration);
        }
        else if (canJump && context.performed)
        {
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        }
    }

    // ========================================================================
    // FLIP
    // ========================================================================
    private void Flip()
    {
        isFacingRight = !isFacingRight;

        Vector3 localScale = transform.localScale;
        localScale.x *= -1f;
        transform.localScale = localScale;
    }

    // ========================================================================
    // DAÑO Y RESPNAWN
    // ========================================================================
    public void TakeDamage(float damage)
    {
        hitTime = 1f;
        health -= damage;

        if (healthText != null) healthText.text = $"Health: {health}/{maxHealth}";

        if (health <= 0)
        {
            KillPlayer();
        }
    }

    public void AddHealth(float _health)
    {
        if (health + _health > maxHealth)
        {
            health = maxHealth;
        }
        else
        {
            health += _health;
        }

        if (healthText != null) healthText.text = $"Health: {health}/{maxHealth}";
    }

    private void KillPlayer()
    {
        SpriteRenderer playerSpriteRenderer = player != null ? player.GetComponent<SpriteRenderer>() : GetComponent<SpriteRenderer>();
        if (playerSpriteRenderer != null) playerSpriteRenderer.enabled = false;

        if (rb != null) rb.bodyType = RigidbodyType2D.Static;

        if (health <= 0)
        {
            vidas--;
            if (vidasText != null) vidasText.text = $"Vidas: {vidas}";
            health = maxHealth;
        }

        if (vidas <= 0)
        {
            if (gm != null) gm.LevelFour();
        }
        else
        {
            StartCoroutine(RespawnPlayer());
        }
    }

    IEnumerator RespawnPlayer()
    {
        yield return new WaitForSeconds(0.25f);

        if (player != null && spawn != null)
        {
            player.transform.position = spawn.transform.position;
        }

        SpriteRenderer playerSpriteRenderer = player != null ? player.GetComponent<SpriteRenderer>() : GetComponent<SpriteRenderer>();
        if (playerSpriteRenderer != null) playerSpriteRenderer.enabled = true;

        if (rb != null) rb.bodyType = RigidbodyType2D.Dynamic;
    }

    // ========================================================================
    // TRIGGERS
    // ========================================================================
    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("checkPoint"))
        {
            checkPoint = true;
            if (spawn != null && player != null) spawn.transform.position = player.transform.position;
        }
        if (collision.gameObject.CompareTag("Ending"))
        {
            if (gm != null) gm.LevelSelect();
        }
        if (collision.gameObject.CompareTag("Kill-Planes"))
        {
            KillPlayer();
        }
    }

    public void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("confiner"))
        {
            KillPlayer();
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (attackPoint == null) return;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
}