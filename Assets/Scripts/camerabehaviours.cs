using UnityEngine;
// ReSharper disable All

/// <summary>
/// Camera script to follow the player
/// Need cleaning reaaaal bad
/// </summary>
public class camerabehaviours : MonoBehaviour
{
public Vector2 offset;
public Transform player;

    // Update is called once per frame
    void Update()
    {
      transform.position = new Vector3(player.position.x + offset.x, player.position.y + offset.y, -10);
    }
}
