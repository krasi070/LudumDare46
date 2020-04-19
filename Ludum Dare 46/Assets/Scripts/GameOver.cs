using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOver : MonoBehaviour
{
    private void Awake()
    {
        Cursor.visible = false;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            MapStatus.Reset();
            PlayerStatus.Reset();
            SceneManager.LoadScene("Map");
        }
    }
}
