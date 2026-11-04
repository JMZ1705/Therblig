using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public Animator transitionAnimator;
    public float transitionTime = 0.5f;
   
    public void QuitGame(){
        Application.Quit();

        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }
    public void LoadTitleScreen(){
        StartCoroutine(LoadScene(0));
    }
    public void LevelSelect()
    {
        StartCoroutine(LoadScene(1));
    }
    public void LevelOne()
    {
        StartCoroutine(LoadScene(2));
    }
    public void LevelTwo()
    {
        StartCoroutine(LoadScene(3));
    }
    public void LevelThree()
    {
        StartCoroutine(LoadScene(4));
    }
    public void LevelFour()
    {
        StartCoroutine(LoadScene(5));
    }
    public void LevelFive()
    {
        StartCoroutine(LoadScene(6));
    }
    public void LevelSix()
    {
        StartCoroutine(LoadScene(7));
    }
    IEnumerator LoadScene(int sceneIndex){
        transitionAnimator.SetTrigger("End");
        yield return new WaitForSeconds(transitionTime);
        SceneManager.LoadScene(sceneIndex);
    }
}
