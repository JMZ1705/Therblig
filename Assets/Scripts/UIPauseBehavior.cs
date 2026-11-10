using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using System.Collections;
public class UIPauseBehavior : MonoBehaviour
{
    
    [SerializeField] private GameObject pauseMenuPanel;

    [SerializeField] private GameObject afterDeathPanel;
    [SerializeField] private GameObject levelFinishPanel;
    [SerializeField] private InputActionReference pauseActionReference;

    [SerializeField] private Button resumeButton;
    [SerializeField] private Button journalButton;
    [SerializeField] private Button settingsButton;
    [SerializeField] private Button titleButton;
    [SerializeField] private Button stageFinishButton;
    [SerializeField] private Button stageFailButton;
    
    [SerializeField] private GameObject journalPanel;
    [SerializeField] private GameObject settingsPanel;
    [SerializeField] private Animator camAnimator;
    public AudioClip openSound;
    public AudioClip closeSound;
    public AudioSource canvasAudioSource;
    

    private bool isPaused = false;

    public void Update()
    {
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
        canvasAudioSource.PlayOneShot(openSound);
        resumeButton.Select();
        Time.timeScale = 0f; // Detiene el tiempo del juego
    }

    public void ResumeGame()
    {
        if (journalPanel.activeInHierarchy){
            journalPanel.SetActive(false);

            resumeButton.gameObject.SetActive(true);
            journalButton.gameObject.SetActive(true);
            settingsButton.gameObject.SetActive(true);
            titleButton.gameObject.SetActive(true);

            journalButton.Select();
        }
        else if (settingsPanel.activeInHierarchy){
            settingsPanel.SetActive(false);
            
            resumeButton.gameObject.SetActive(true);
            journalButton.gameObject.SetActive(true);
            settingsButton.gameObject.SetActive(true);
            titleButton.gameObject.SetActive(true);

            settingsButton.Select();
        }
        else{
            isPaused = false;
            pauseMenuPanel.SetActive(false);
            canvasAudioSource.PlayOneShot(closeSound);
            Time.timeScale = 1f; // Reanuda el tiempo del juego

        }
    }
    public void DeathPanel()
    {
        afterDeathPanel.SetActive(true);
        stageFailButton.Select();
    }
    public void FinishPanel()
    {
        levelFinishPanel.SetActive(true);
        stageFinishButton.Select();
    }
}
