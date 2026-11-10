using UnityEngine;

public class RotateGear : MonoBehaviour
{
    [Header("Configuración de Rotación")]
    [SerializeField] private float rotationSpeed = 100f;
    [SerializeField] private bool rotateClockwise = true;

    [Header("Configuración de Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip gearSound;
    [Range(0f, 1f)][SerializeField] private float volume = 0.5f;

    void Start()
    {
        if (audioSource == null) audioSource = GetComponent<AudioSource>();

        // Si tenemos un AudioSource y un clip asignados, iniciamos el sonido en bucle (loop)
        if (audioSource != null && gearSound != null)
        {
            audioSource.clip = gearSound;
            audioSource.loop = true;
            audioSource.volume = volume;

            if (!audioSource.isPlaying)
            {
                audioSource.Play();
            }
        }
    }

    void Update()
    {
        float direction = rotateClockwise ? -1f : 1f;
        transform.Rotate(0f, 0f, direction * rotationSpeed * Time.deltaTime);
    }
}