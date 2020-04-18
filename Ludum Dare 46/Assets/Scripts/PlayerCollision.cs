using System;
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
            EnemyType fightingWith = (EnemyType)Enum.Parse(typeof(EnemyType), _collided.tag);
            GetComponent<Player>().PrepareForEncounter(fightingWith);
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
}
