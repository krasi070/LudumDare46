using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerCollision : MonoBehaviour
{
    public GameObject interactPrompt;

    private GameObject _collided;

    private void Update()
    {
        if (interactPrompt.activeSelf && Input.GetAxis("Interact") > 0)
        {
            UpdatePlayerStatus();
            SceneManager.LoadScene("Battle");
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        interactPrompt.SetActive(true);
        _collided = collision.gameObject;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        interactPrompt.SetActive(false);
    }

    private void UpdatePlayerStatus()
    {
        Player player = GetComponent<Player>();
        PlayerStatus.Vitality = player.vitality;
        PlayerStatus.DemonMeter = player.demonMeter;
    }
}
