using UnityEngine;

/// <summary>
/// The bomb must give the player a big boost when dropped 
/// </summary>
public sealed class BombBehaviour : EmojiBehaviour
{
    [SerializeField] private float power;

    public override void Bounce()
    {
        Explode();
    }

    private void Explode()
    {
        var parent = transform.parent;
        // ReSharper disable once Unity.InefficientPropertyAccess
        var position = transform.position;
        var forceDirection = parent.position - position;
        parent.GetComponent<Rigidbody2D>().AddForce(forceDirection.normalized * power);
    }
}