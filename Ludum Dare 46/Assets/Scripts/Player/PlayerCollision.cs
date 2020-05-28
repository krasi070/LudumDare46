using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class PlayerCollision : MonoBehaviour
{
    public TextMeshProUGUI interactPrompt;

    private MapElement _collided;

    private void Update()
    {
        if (interactPrompt.gameObject.activeSelf && Input.GetAxis("Interact") > 0 && !PlayerStatus.IsPaused)
        {
            if (_collided != null)
            {
                if (_collided.isEnemy)
                {
                    GetComponent<Player>().PrepareForEncounter(_collided.enemyBattlePrefab);
                    MapStatus.InteractedWith.Add(_collided.name);
                    MapStatus.Save();
                    LevelManager.instance.LoadScene("Battle");
                }
                else if (_collided.isSearchable)
                {
                    // TODO
                }
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Interactable")
        {
            _collided = collision.GetComponent<MapElement>();
            interactPrompt.gameObject.SetActive(true);

            if (MapStatus.InteractedWith.Contains(collision.gameObject.name))
            {
                interactPrompt.text = _collided.defeatedText;
            }
            else
            {
                interactPrompt.text = _collided.interactionText;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        interactPrompt.gameObject.SetActive(false);
    }
}
