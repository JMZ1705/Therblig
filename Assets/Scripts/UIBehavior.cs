using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
public class UIBehavior : MonoBehaviour
{
    public GameObject[] buttons;
    public GameObject selectedButton;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        buttons = GameObject.FindGameObjectsWithTag("Button");
    }

    // Update is called once per frame
    void Update()
    {
        selectedButton = EventSystem.current.currentSelectedGameObject;
        Button buttonComponent = selectedButton.GetComponent<Button>();
        Image image = selectedButton.GetComponent<Image>();
        GameObject textAttached = selectedButton.transform.GetChild(0).gameObject;

        buttonComponent.interactable = true;
        image.enabled = true;
        textAttached.SetActive(true);

        for (int i = 0; i < buttons.Length; i++)
        {
            GameObject nonSelect = buttons[i];

            Button buttonNonSelect = nonSelect.GetComponent<Button>();
            Image imageNonSelect = nonSelect.GetComponent<Image>();
            GameObject textNonSelect = nonSelect.transform.GetChild(0).gameObject;

            if (nonSelect != selectedButton)
            {
                buttonNonSelect.interactable = false;
                imageNonSelect.enabled = false;
                textNonSelect.SetActive(false);
            }
        }
        
    }
    public void QuitGame(){
        Application.Quit();

        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }
    public void LevelSelect()
    {
        SceneManager.LoadScene("Level Select");
    }
    public void LevelOne()
    {
        SceneManager.LoadScene("N1 Inicio");
    }
    public void LevelTwo()
    {
        SceneManager.LoadScene("N2 Engranajes");
    }
    public void LevelThree()
    {
        SceneManager.LoadScene("N3 Calderas");
    }
    public void LevelFour()
    {
        SceneManager.LoadScene("N4 Bandas T");
    }
    public void LevelFive()
    {
        SceneManager.LoadScene("N5 FinalBoss");
    }
    public void LevelSix()
    {
        SceneManager.LoadScene("N6 Final");
    }
}
