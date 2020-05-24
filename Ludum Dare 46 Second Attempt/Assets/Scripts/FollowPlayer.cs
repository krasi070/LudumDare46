using UnityEngine;

public class FollowPlayer : MonoBehaviour
{
    public Player player;

    public float topBound;
    public float bottomBound;
    public float leftBound;
    public float rightBound;

    private void Update()
    {
        if (!player.IsPaused)
        {
            var pos = new Vector3(player.transform.position.x, player.transform.position.y, transform.position.z);

            pos.x = Mathf.Clamp(pos.x, leftBound, rightBound);
            pos.y = Mathf.Clamp(pos.y, bottomBound, topBound);

            transform.position = pos;
        }
    }
}
