using UnityEngine;

public class RotateGear : MonoBehaviour
{
    [SerializeField] private float rotationSpeed = 100f; 
    [SerializeField] private bool rotateClockwise = true; 

    void Update()
    {
       
        float direction = rotateClockwise ? -1f : 1f;

        
        transform.Rotate(0f, 0f, direction * rotationSpeed * Time.deltaTime);
    }
}
