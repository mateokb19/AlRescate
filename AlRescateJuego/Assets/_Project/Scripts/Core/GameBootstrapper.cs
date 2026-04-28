using UnityEngine;
using UnityEngine.SceneManagement;

public class GameBootstrapper : MonoBehaviour
{
    [SerializeField] private string sceneToLoad = "00_MainMenu";

    void Awake() { SaveSystem.Load(); }
    void Start() { SceneManager.LoadScene(sceneToLoad); }
}
