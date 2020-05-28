using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public static LevelManager instance;

    private static string _lastScene;

    public Animator transition; 

    private void Awake()
    {
        string currentScene = SceneManager.GetActiveScene().name;

        // Transition played when a scene is loaded
        if (_lastScene == "Map" && currentScene == "Battle")
        {
            transition.SetTrigger("MapToBattleEnd");
        }

        if (_lastScene == "Battle" && currentScene == "Map")
        {
            transition.SetTrigger("BattleToMapEnd");
        }

        _lastScene = currentScene;

        if (instance == null)
        {
            instance = this;
            
        }
    }

    public void LoadScene(string scene)
    {
        StartCoroutine(LoadWithTransition(scene));
    }

    private IEnumerator LoadWithTransition(string scene)
    {
        // transitions played when exiting a scene
        if (_lastScene == "Map" && scene == "Battle")
        {
            transition.SetTrigger("MapToBattleStart");
        }

        if (_lastScene == "Battle" && scene == "Map")
        {
            transition.SetTrigger("BattleToMapStart");
        }

        yield return new WaitForSeconds(1);

        SceneManager.LoadScene(scene);
    }
}
