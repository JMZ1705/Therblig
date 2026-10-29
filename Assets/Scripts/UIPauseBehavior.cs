using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using System.Collections;
public class UIPauseBehavior : MonoBehaviour
{
    
    [SerializeField] private GameObject pauseMenuPanel;


    [SerializeField] private InputActionReference pauseActionReference;

    [SerializeField] private Button resumeButton;
    [SerializeField] private GameObject journalPanel;
    [SerializeField] private GameObject settingsPanel;
    [SerializeField] private Animator camAnimator;

    private bool isPaused = false;

    public void Update(){
        camAnimator.SetBool("isPaused", isPaused);
    }

    private void OnEnable()
    {
        pauseActionReference.action.Enable();
        pauseActionReference.action.performed += OnPauseTriggered;
    }

    private void OnDisable()
    {
        pauseActionReference.action.performed -= OnPauseTriggered;
        pauseActionReference.action.Disable();
    }

    private void OnPauseTriggered(InputAction.CallbackContext context)
    {
        if (isPaused)
        {
            ResumeGame();
        }
        else
        {
            PauseGame();
        }
    }

    public void PauseGame()
    {
        isPaused = true;
        pauseMenuPanel.SetActive(true);
        resumeButton.Select();
        Time.timeScale = 0f; // Detiene el tiempo del juego
    }

    public void ResumeGame()
    {
        if (journalPanel.activeInHierarchy){
            journalPanel.SetActive(false);
            pauseMenuPanel.SetActive(true);
            resumeButton.Select();
        }
        else if (settingsPanel.activeInHierarchy){
            settingsPanel.SetActive(false);
            pauseMenuPanel.SetActive(true);
            resumeButton.Select();
        }
        else{
            isPaused = false;
            pauseMenuPanel.SetActive(false);
            Time.timeScale = 1f; // Reanuda el tiempo del juego

        }
    }
}
