using UnityEngine;
using UnityEngine.EventSystems;

public class MainMenuButton : MonoBehaviour, ISelectHandler
{
    public AudioSource optionsAudioSource;
    public AudioClip optionsAudioClip;
    
    public GameObject firstButton;

    public void OnSelect(BaseEventData eventData){
        optionsAudioSource.PlayOneShot(optionsAudioClip);
   }
}
