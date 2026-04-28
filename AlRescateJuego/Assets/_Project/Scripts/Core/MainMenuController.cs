using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour
{
    public Button btnPlay, btnCollection, btnOptions, btnCredits, btnQuit;

    void Start()
    {
        btnPlay.onClick.AddListener(() => SceneManager.LoadScene("01_GachaHub"));
        btnCollection.onClick.AddListener(() =>
        {
            PlayerPrefs.SetInt("OpenCollectionOnStart", 1);
            SceneManager.LoadScene("01_GachaHub");
        });
        btnOptions.onClick.AddListener(() => Debug.Log("Opciones (placeholder)"));
        btnCredits.onClick.AddListener(() => Debug.Log("Creditos (placeholder)"));
        btnQuit.onClick.AddListener(() =>
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        });
    }
}
