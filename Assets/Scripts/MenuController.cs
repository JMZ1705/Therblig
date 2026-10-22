using UnityEngine;              // Todo lo básico de Unity: MonoBehaviour, Vector2, Rigidbody2D...
using UnityEngine.InputSystem;  // El New Input System: nos da InputAction.CallbackContext.
using TMPro;                    // TextMeshPro: el texto de la UI (TextMeshProUGUI).
using Unity.Cinemachine;
using Unity.VisualScripting;
using JetBrains.Annotations;
using System.Collections.Generic;
using System.Collections;




public class MenuController : PlayerController
{
    private GameObject player;
    private GameObject spawn;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public MenuController(GameObject nPlayer, GameObject nSpawn)
    {
        this.player = nPlayer;
        this.spawn = nSpawn;
    }
    public IEnumerator RespawnPlayer()
    {
        this.player.transform.position = this.spawn.transform.position;
        yield return new WaitForSeconds(0.1f);

        this.player.SetActive(true);
    }
    
}
