using UnityEngine;
using UnityEngine.UI; // Necesario para trabajar con componentes de UI como Image

public class HealthUI : MonoBehaviour
{
    [Header("Referencias de UI")]
    public Image healthImage; // El componente Image en tu Canvas que mostrará el sprite

    [Header("Sprites de Salud (Ordenados de mayor a menor vida)")]
    // Elemento 0: 100 de vida (Llena)
    // Elemento 1: 80 de vida
    // Elemento 2: 60 de vida
    // Elemento 3: 40 de vida
    // Elemento 4: 20 de vida
    // Elemento 5: 0 de vida (Vacía / Muerte)
    public Sprite[] healthSprites;

    // Método para actualizar la imagen según la salud actual del personaje
    public void ActualizarSaludVisual(float vidaActual)
    {
        // Aseguramos que la vida esté entre 0 y 100 para evitar errores
        vidaActual = Mathf.Clamp(vidaActual, 0f, 100f);

        // Como cada sprite representa un escalón de 20 puntos:
        // 100 -> Indice 0
        // 80  -> Indice 1
        // 60  -> Indice 2
        // 40  -> Indice 3
        // 20  -> Indice 4
        // 0   -> Indice 5
        int indiceSprite = Mathf.FloorToInt((100f - vidaActual) / 20f);

        // Verificamos que el índice esté dentro de los límites del array
        if (indiceSprite >= healthSprites.Length)
        {
            indiceSprite = healthSprites.Length - 1;
        }

        // Cambiamos el sprite de la UI
        if (healthImage != null && healthSprites[indiceSprite] != null)
        {
            healthImage.sprite = healthSprites[indiceSprite];
        }
    }
}