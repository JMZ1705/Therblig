/* ============================================================================
 *  PlayerController.cs
 * ----------------------------------------------------------------------------
 *  Controla al jugador 2D: movimiento, salto, dash, wall slide/jump, vida, UI,
 *  respawn seguro, empujón (knockback), ataques, sonidos y VOLUMEN AJUSTABLE.
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
    public UIPauseBehavior panelesMuerteYFinal;

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
    public float health = 100f;
    [SerializeField] private float maxHealth = 100f;
    public int vidas;
    public int iVidas;

    // --- Golpe / knockback LEJANO ---
    [Header("Configuración de Knockback (Rebote Lejano)")]
    public float hitForce = 18f;
    public float hitUpForce = 12f;
    public float hitDuration = 0.45f;
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

    // --- Ataque en Picada / Pesado ---
    [Header("Configuración de Ataque en Picada")]
    [SerializeField] private float diveForce = 8f;
    [SerializeField] private float diveDamage = 25f;
    private bool isDiving = false;

    // --- Bandas ---
    private float BandasForce = 0f;

    // --- Checkpoint Animator ---
    [SerializeField] private Animator checkpointAnimator;

    // --- AUDIO / EFECTOS DE SONIDO Y VOLUMEN ---
    [Header("Configuración de Audio")]
    [SerializeField] private AudioSource audioSource;
    [Range(0f, 1f)][SerializeField] private float sfxVolume = 0.5f; // Volumen global del jugador

    [SerializeField] private AudioClip soundRunStep1;
    [SerializeField] private AudioClip soundRunStep2;
    [SerializeField] private AudioClip soundJump;
    [SerializeField] private AudioClip soundWallJump;
    [SerializeField] private AudioClip soundDash;
    [SerializeField] private AudioClip soundLightAttack;
    [SerializeField] private AudioClip soundHeavyAttack;
    [SerializeField] private AudioClip soundHurt;
    [SerializeField] private AudioClip soundDie;

    private bool useStep1 = true;

    // ========================================================================
    // START()
    // ========================================================================
    void Start()
    {
        playerAnimator = GetComponent<Animator>();
        gm = FindFirstObjectByType<GameManager>();

        if (rb == null) rb = GetComponent<Rigidbody2D>();
        if (audioSource == null) audioSource = GetComponent<AudioSource>();

        vidas = iVidas;
        UpdateUI();
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
            if (isDiving)
            {
                isDiving = false;
            }
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

        // 3. Movimiento normal vs Estado de Knockback (Daño)
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
                rb.linearVelocity = new Vector2((direction * speed) + BandasForce, rb.linearVelocity.y);

                if (playerAnimator != null)
                {
                    playerAnimator.SetFloat("Direction", direction);
                    playerAnimator.SetBool("isGround", canJump);
                    yVelocity = rb.linearVelocity.y;
                    playerAnimator.SetFloat("yVelocity", yVelocity);
                }
            }
        }
        else
        {
            hitTime -= Time.deltaTime;
            if (playerAnimator != null)
            {
                playerAnimator.SetFloat("isHit", hitTime);
            }
        }

        // Animación Checkpoint
        if (checkpointAnimator != null)
        {
            checkpointAnimator.SetBool("HasCheckpoint", checkPoint);
        }

        // 4. Orientación del Sprite
        if (hitTime <= 0)
        {
            if (!isFacingRight && direction < 0f)
            {
                Flip();
            }
            else if (isFacingRight && direction > 0f)
            {
                Flip();
            }
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
        if (!canJump)
        {
            PerformDiveAttack();
            return;
        }

        if (playerAnimator != null) playerAnimator.SetTrigger("Attack");

        ExecuteHitDetection(attackDamage);
    }

    private void PerformDiveAttack()
    {
        isDiving = true;
        if (playerAnimator != null) playerAnimator.SetTrigger("DiveAttack");
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, -diveForce);
        ExecuteHitDetection(diveDamage);
    }

    private void ExecuteHitDetection(float damageValue)
    {
        if (attackPoint == null) return;

        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayers);

        foreach (Collider2D enemy in hitEnemies)
        {
            BossEnemy boss = enemy.GetComponent<BossEnemy>();
            if (boss != null)
            {
                boss.TakeDamageFromPlayer(damageValue, transform.position);
                continue;
            }

            EnemyHealth enemyHealth = enemy.GetComponent<EnemyHealth>();
            if (enemyHealth != null)
            {
                enemyHealth.TakeDamage(damageValue);
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

        EventPlayDashSound();

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
        if (hitTime > 0) return;

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

            EventPlayWallJumpSound();

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
    // DAÑO Y KNOCKBACK
    // ========================================================================
    public void ApplyKnockbackAndDamage(float damage, bool fromRight)
    {
        hitFromRight = fromRight;
        TakeDamage(damage);

        if (gameObject.activeInHierarchy)
        {
            StopCoroutine(nameof(KnockbackRoutine));
            StartCoroutine(KnockbackRoutine());
        }
    }

    private IEnumerator KnockbackRoutine()
    {
        hitTime = hitDuration;

        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            float knockDir = hitFromRight ? -1f : 1f;
            rb.linearVelocity = new Vector2(knockDir * hitForce, hitUpForce);
        }

        yield return new WaitForSeconds(hitDuration);
    }

    public void TakeDamage(float damage)
    {
        health -= damage;
        if (health < 0) health = 0;

        EventPlayHurtSound();

        UpdateUI();

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

        UpdateUI();
    }

    private void UpdateUI()
    {
        if (healthText != null) healthText.text = $"Health: {health}/{maxHealth}";
        if (vidasText != null) vidasText.text = $"Vidas: {vidas}";
    }

    private void KillPlayer()
    {
        EventPlayDieSound();

        SpriteRenderer playerSpriteRenderer = player != null ? player.GetComponent<SpriteRenderer>() : GetComponent<SpriteRenderer>();
        if (playerSpriteRenderer != null) playerSpriteRenderer.enabled = false;

        if (rb != null) rb.bodyType = RigidbodyType2D.Static;

        if (health <= 0)
        {
            vidas--;
            UpdateUI();
            health = maxHealth;
        }

        if (vidas <= 0)
        {
            panelesMuerteYFinal.DeathPanel();
        }
        else
        {
            if (gameObject.activeInHierarchy)
            {
                StartCoroutine(RespawnPlayer());
            }
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
    // TRIGGERS Y COLISIONES
    // ========================================================================
    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("checkPoint"))
        {
            checkPoint = true;
            if (spawn != null && player != null)
            {
                spawn.transform.position = player.transform.position;
            }
        }
        if (collision.gameObject.CompareTag("Ending"))
        {
            panelesMuerteYFinal.FinishPanel();
        }
        if (collision.gameObject.CompareTag("Kill-Planes"))
        {
            TakeDamage(50);
            KillPlayer();
        }
    }

    public void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("confiner"))
        {
            TakeDamage(50);
            KillPlayer();
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (attackPoint == null) return;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }

    public void UpgradeDamage(float extraDamage)
    {
        attackDamage += extraDamage;
        Debug.Log($"<color=cyan>¡Nuevo daño de ataque del personaje: {attackDamage}!</color>");
    }

    public void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("bandaIzq"))
        {
            BandasForce = -5f;
        }
        if (collision.gameObject.CompareTag("bandaDer"))
        {
            BandasForce = 5f;
        }
    }

    public void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("bandaIzq") || canJump == false)
        {
            BandasForce = 0f;
        }
    }

    // ========================================================================
    // MÉTODOS DE AUDIO Y REPRODUCCIÓN CON VOLUMEN
    // ========================================================================

    private void PlaySFX(AudioClip clip, float customVolumeMultiplier = 1.0f)
    {
        if (audioSource != null && clip != null)
        {
            float finalVolume = Mathf.Clamp01(sfxVolume * customVolumeMultiplier);
            audioSource.PlayOneShot(clip, finalVolume);
        }
    }

    public void EventPlayRunSound()
    {
        if (useStep1)
        {
            PlaySFX(soundRunStep1, 0.7f);
        }
        else
        {
            PlaySFX(soundRunStep2, 0.7f);
        }
        useStep1 = !useStep1;
    }

    public void EventPlayJumpSound() => PlaySFX(soundJump);
    public void EventPlayLightAttackSound() => PlaySFX(soundLightAttack);
    public void EventPlayHeavyAttackSound() => PlaySFX(soundHeavyAttack);

    // --- AQUÍ SE CONTROLA EL VOLUMEN DEL WALL JUMP (0.3f = 30% de intensidad) ---
    public void EventPlayWallJumpSound() => PlaySFX(soundWallJump, 0.25f);

    public void EventPlayDashSound() => PlaySFX(soundDash);
    public void EventPlayHurtSound() => PlaySFX(soundHurt);
    public void EventPlayDieSound() => PlaySFX(soundDie);
}