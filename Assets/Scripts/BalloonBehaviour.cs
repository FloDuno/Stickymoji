using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;

/// <summary>
/// The balloon must not fall to the ground when dropped by the player
/// </summary>
public class BalloonBehaviour : EmojiBehaviour
{
    [SerializeField] private float timeToWait;
    [SerializeField] private float timeToDoubleJump;

    [FormerlySerializedAs("timePlayerIntangability")] [SerializeField]
    private float timePlayerIntangibility;

    private bool isInTheAir;

    protected override void Explode(Vector2 direction)
    {
        base.Explode(direction);
        if (isInTheAir)
        {
            // Make the player go through the object when jumping from it
            PlayerBehaviour.MakeIntangible(timePlayerIntangibility);
        }
    }

    public override void OnCollisionEnter2D(Collision2D other)
    {
        // Need to make sure the object is not trying to stay in the air if it collided
        StopCoroutine(StopPhysics());
        base.OnCollisionEnter2D(other);
        StartCoroutine(DoubleJumpTimer());
    }

    private IEnumerator DoubleJumpTimer()
    {
        yield return new WaitForSeconds(timeToDoubleJump);
        isInTheAir = false;
    }

    public override void Degroup()
    {
        base.Degroup();
        StartCoroutine(StopPhysics());
    }
    
    /// <summary>
    /// Make the balloon stay in the air after timeToWait
    /// </summary>
    /// <returns></returns>
    private IEnumerator StopPhysics()
    {
        yield return new WaitForSeconds(timeToWait);
        Destroy(MyRigidbody2D);
        gameObject.layer = LayerMask.NameToLayer("Emoji");
        // ReSharper disable once Unity.PreferAddressByIdToGraphicsParams
        GetComponent<Animator>().SetTrigger("StopBlink");
        isInTheAir = true;
    }
}