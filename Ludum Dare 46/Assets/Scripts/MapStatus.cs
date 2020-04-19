using System.Collections.Generic;
using UnityEngine;

public class MapStatus : MonoBehaviour
{
    public static Vector2 PlayerPosition { get; set; }

    public static HashSet<string> InteractedWith { get; set; } = new HashSet<string>();

    public static void Save()
    {
        PlayerPosition = GameObject.FindGameObjectWithTag("Player").transform.position;
    }

    public static void Reset()
    {
        PlayerPosition = Vector2.zero;
        InteractedWith = new HashSet<string>();
    }

}
