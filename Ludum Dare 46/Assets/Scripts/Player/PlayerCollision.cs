using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class PlayerCollision : MonoBehaviour
{
    public TextMeshProUGUI interactPrompt;

    private GameObject _collided;

    private void Update()
    {
        if (interactPrompt.gameObject.activeSelf && Input.GetAxis("Interact") > 0 && !PlayerStatus.IsPaused)
        {
            EnemyType fightingWith = (EnemyType)Enum.Parse(typeof(EnemyType), _collided.tag);
            GetComponent<Player>().PrepareForEncounter(fightingWith);
            MapStatus.InteractedWith.Add(_collided.name);
            MapStatus.Save();
            SceneManager.LoadScene("Battle");
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        interactPrompt.gameObject.SetActive(true);
        if (MapStatus.InteractedWith.Contains(collision.gameObject.name))
        {
            interactPrompt.text = "It's dead.";
        }
        else
        {
            interactPrompt.text = "[E] Attack";
        }

        _collided = collision.gameObject;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        interactPrompt.gameObject.SetActive(false);
    }
}
