using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombBehaviour : EmojiBehaviour
{
    [SerializeField] private float power;

    public override void Bounce()
    {
        Explode();
    }

    public virtual void Explode()
    {
        var parent = transform.parent;
        // ReSharper disable once Unity.InefficientPropertyAccess
        var position = transform.position;
        var forceDirection = parent.position - position;
        parent.GetComponent<Rigidbody2D>().AddForce(forceDirection.normalized * power);
    }
}