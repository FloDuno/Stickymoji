using System.Collections;
using UnityEngine;

public class BalloonBehaviour : EmojiBehaviour
{
    [SerializeField] private float timeToWait;
    [SerializeField] private float timeToDoubleJump;
    [SerializeField] private float timePlayerIntangability;

    private bool isInTheAir;

    public override void Explode(Vector2 direction)
    {
        if (isInTheAir)
        {
            base.Explode(Vector2.up);
            PlayerBehaviour.MakeIntangible(timePlayerIntangability);
        }
        else
        {
            base.Explode(direction);
        }
    }

    public override void OnCollisionEnter2D(Collision2D other)
    {
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

    private IEnumerator StopPhysics()
    {
        yield return new WaitForSeconds(timeToWait);
        Destroy(MyRigidbody2D);
        gameObject.layer = LayerMask.NameToLayer("Emoji");
        GetComponent<Animator>().SetTrigger("StopBlink");
        isInTheAir = true;
    }
}