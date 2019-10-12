using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class camerabehaviours : MonoBehaviour
{
public Vector2 offset;
public Transform player;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
      transform.position = new Vector3(player.position.x + offset.x, player.position.y + offset.y, -10);
    }
}
