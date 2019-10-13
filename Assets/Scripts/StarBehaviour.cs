using System.Linq;
using UnityEngine;

public class StarBehaviour : EmojiBehaviour
{
    private bool isOnWall;
    [SerializeField] private float timePlayerIntangability;

    public override void OnCollisionEnter2D(Collision2D other)
    {
        if (MyRigidbody2D != null && other.gameObject.layer == LayerMask.NameToLayer("StuckStar"))
            Destroy(MyRigidbody2D);
        base.OnCollisionEnter2D(other);
        if (other.gameObject.layer != LayerMask.NameToLayer("Emoji"))
        {
            isOnWall = other.gameObject.layer != LayerMask.NameToLayer("Floor");
        }

        if (other.gameObject.GetComponent<HingeJoint2D>())
        {
            RemoveRope(other);
        }
    }

    private void RemoveRope(Collision2D other)
    {
        var parent = other.transform.parent;
        var ropeAndEmoji = parent.GetComponentsInChildren<HingeJoint2D>();
        var ropeParts = ropeAndEmoji.Where(x => !x.GetComponent<EmojiBehaviour>());
        var emoji = parent.GetComponentInChildren<EmojiBehaviour>();
        Destroy(emoji.GetComponent<HingeJoint2D>());
        foreach (var ropePart in ropeParts)
        {
           Destroy(ropePart.gameObject);
        }
        MyRigidbody2D.velocity = Vector2.zero;

    }

    protected override void Explode(Vector2 direction)
    {
        if (isOnWall)
        {
            base.Explode(Vector2.up);
            PlayerBehaviour.MakeIntangible(timePlayerIntangability);
        }
        else
        {
            base.Explode(direction);
        }
    }
}