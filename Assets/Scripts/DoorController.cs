using UnityEngine;

public class DoorController : MonoBehaviour
{
    [Header("Configuración de Apertura")]
    [SerializeField] private bool destroyOnOpen = false; 
    [SerializeField] private float openSpeed = 3.0f;
    [SerializeField] private Vector3 openOffset = new Vector3(0, 3, 0); 

    private bool isOpen = false;
    private Vector3 targetPosition;

    private void Start()
    {
        targetPosition = transform.position + openOffset;
    }

    private void Update()
    {
        if (isOpen && !destroyOnOpen)
        {
            
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, openSpeed * Time.deltaTime);
        }
    }

    public void OpenDoor()
    {
        isOpen = true;

        if (destroyOnOpen)
        {
            gameObject.SetActive(false);
        }
    }
}