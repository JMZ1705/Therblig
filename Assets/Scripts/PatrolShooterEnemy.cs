using UnityEngine;

public class PatrolShooterEnemy : MonoBehaviour
{
    [Header("Patrulla")]
    [SerializeField] private float enemySpeed = 2f;
    [SerializeField] private float detectionRadius = 0.5f;
    [SerializeField] private Transform[] enemyMovementPoints;

    [Header("Disparo")]
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Transform firePoint;
    [SerializeField] private float fireInterval = 2.5f;

    private Rigidbody2D enemyRb;
    private Transform actualObjective;
    private Transform playerTransform;
    private bool isFacingRight = true;
    private float fireTimer;

    private void Start()
    {
        enemyRb = GetComponent<Rigidbody2D>();
        actualObjective = enemyMovementPoints[0];

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null) playerTransform = playerObj.transform;
    }

    private void Update()
    {
        
        float distanceToObjective = Vector2.Distance(transform.position, actualObjective.position);

        if (distanceToObjective < detectionRadius)
        {
            if (actualObjective == enemyMovementPoints[0])
                actualObjective = enemyMovementPoints[1];
            else
                actualObjective = enemyMovementPoints[0];
        }

        Vector2 direction = (actualObjective.position - transform.position).normalized;
        int roundDirection = Mathf.RoundToInt(direction.x);

        
        if (roundDirection > 0 && !isFacingRight)
            Flip();
        else if (roundDirection < 0 && isFacingRight)
            Flip();

        enemyRb.MovePosition(enemyRb.position + new Vector2(roundDirection, 0) * enemySpeed * Time.deltaTime);

        
        fireTimer += Time.deltaTime;
        if (fireTimer >= fireInterval)
        {
            ShootAtPlayer();
            fireTimer = 0f;
        }
    }

    private void ShootAtPlayer()
    {
        if (projectilePrefab == null || firePoint == null || playerTransform == null) return;

        GameObject projObj = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);
        Projectile proj = projObj.GetComponent<Projectile>();

        if (proj != null)
        {
            
            Vector2 shootDirection = (playerTransform.position - firePoint.position).normalized;
            proj.SetDirection(shootDirection);
        }
    }

    private void Flip()
    {
        isFacingRight = !isFacingRight;
        Vector3 localScale = transform.localScale;
        localScale.x *= -1f;
        transform.localScale = localScale;
    }
}