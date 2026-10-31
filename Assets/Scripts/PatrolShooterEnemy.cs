using UnityEngine;

public class PatrolShooterEnemy : MonoBehaviour
{
    [Header("Configuración de Movimiento / Patrulla")]
    [SerializeField] private Transform pointA;
    [SerializeField] private Transform pointB;
    [SerializeField] private float speed = 3f;

    [Header("Configuración de Disparo")]
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Transform firePoint;
    [SerializeField] private float fireInterval = 2f;

    [Header("Referencias")]
    [SerializeField] private Transform playerTransform;
    [SerializeField] private bool isFacingRight = true;

    private Vector3 targetPosition;
    private float fireTimer;

    private void Start()
    {
        if (playerTransform == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null) playerTransform = playerObj.transform;
        }

        if (pointA != null)
        {
            targetPosition = pointA.position;
        }
    }

    private void Update()
    {
        Patrol();

        
        if (playerTransform == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null) playerTransform = playerObj.transform;
            return;
        }

        fireTimer += Time.deltaTime;
        if (fireTimer >= fireInterval)
        {
            Shoot();
            fireTimer = 0f;
        }
    }

    private void Patrol()
    {
        if (pointA == null || pointB == null) return;

        
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);

       
        if (Vector3.Distance(transform.position, targetPosition) < 0.1f)
        {
            if (targetPosition == pointA.position)
            {
                targetPosition = pointB.position;
                if (isFacingRight) Flip();
            }
            else
            {
                targetPosition = pointA.position;
                if (!isFacingRight) Flip();
            }
        }
    }

    private void Shoot()
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