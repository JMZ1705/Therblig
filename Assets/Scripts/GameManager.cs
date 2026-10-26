using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
   
    public void QuitGame(){
        Application.Quit();

        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }
    public void LoadTitleScreen(){
        SceneManager.LoadScene("Menu Principal", LoadSceneMode.Single);
    }
    public void LevelSelect()
    {
        SceneManager.LoadScene("Level Select", LoadSceneMode.Single);
    }
    public void LevelOne()
    {
        SceneManager.LoadScene("N1 Inicio", LoadSceneMode.Single);
    }
    public void LevelTwo()
    {
        SceneManager.LoadScene("N2 Engranajes", LoadSceneMode.Single);
    }
    public void LevelThree()
    {
        SceneManager.LoadScene("N3 Calderas", LoadSceneMode.Single);
    }
    public void LevelFour()
    {
        SceneManager.LoadScene("N4 Bandas T", LoadSceneMode.Single);
    }
    public void LevelFive()
    {
        SceneManager.LoadScene("N5 FinalBoss", LoadSceneMode.Single);
    }
    public void LevelSix()
    {
        SceneManager.LoadScene("N6 Final", LoadSceneMode.Single);
    }
}
