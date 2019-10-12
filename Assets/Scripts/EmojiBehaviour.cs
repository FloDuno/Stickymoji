using System;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;

public class EmojiBehaviour : MonoBehaviour, IUsable
{
    [SuppressMessage("ReSharper", "UnassignedField.Global")]
    [Serializable]
    // ReSharper disable once MemberCanBePrivate.Global
    public struct Rigidbody2DParameters
    {
        public float mass;
        public float linearDrag;
        public float angularDrag;
        public float gravityScale;
    }

    [SerializeField] private float dropPower, launchPower, timeIntangibleAfterRope;
    private Rigidbody2DParameters rigidbody2DParameters;

    protected new Rigidbody2D MyRigidbody2D;
    protected PlayerBehaviour PlayerBehaviour;

    // Start is called before the first frame update
    public virtual void Start()
    {
        MyRigidbody2D = GetComponent<Rigidbody2D>();
        if (MyRigidbody2D)
        {
            rigidbody2DParameters = new Rigidbody2DParameters
            {
                mass = MyRigidbody2D.mass,
                linearDrag = MyRigidbody2D.drag,
                angularDrag = MyRigidbody2D.angularDrag,
                gravityScale = MyRigidbody2D.gravityScale
            };
            var hinge = GetComponent<HingeJoint2D>();
            if (hinge == null)
                Destroy(MyRigidbody2D);
        }
        else
        {
            Debug.LogError("Missing rigidbody2D");
        }

        PlayerBehaviour = GameObject.FindWithTag("Player").GetComponent<PlayerBehaviour>();
    }

    public virtual void Degroup()
    {
        var playerPos = transform.parent.position;
        transform.SetParent(null);
        if (MyRigidbody2D != null) return;

        MyRigidbody2D = gameObject.AddComponent<Rigidbody2D>();
        MyRigidbody2D.mass = rigidbody2DParameters.mass;
        MyRigidbody2D.drag = rigidbody2DParameters.linearDrag;
        MyRigidbody2D.angularDrag = rigidbody2DParameters.angularDrag;
        MyRigidbody2D.gravityScale = rigidbody2DParameters.gravityScale;
        // ReSharper disable once Unity.InefficientPropertyAccess
        MyRigidbody2D.AddForce((transform.position - playerPos).normalized * dropPower);
        gameObject.layer = LayerMask.NameToLayer("Ungrabable");

        GetComponent<Animator>().SetTrigger("Blink");
    }

    public virtual void Bounce()
    {
        var parent = transform.parent;
        // ReSharper disable once Unity.InefficientPropertyAccess
        var position = transform.position;
        var forceDirection = parent.position - position;
        Explode(forceDirection.normalized);
    }

    public virtual void Explode(Vector2 direction)
    {
        transform.parent.GetComponent<Rigidbody2D>().AddForce(direction * launchPower);
        if (GetComponent<HingeJoint2D>())
        {
            PlayerBehaviour.MakeIntangible(timeIntangibleAfterRope);
        }
    }

    public virtual void OnCollisionEnter2D(Collision2D other)
    {
        if (MyRigidbody2D != null && other.gameObject.layer == LayerMask.NameToLayer("Floor"))
            Destroy(MyRigidbody2D);
        gameObject.layer = LayerMask.NameToLayer("Emoji");
        if (other.gameObject.layer != LayerMask.NameToLayer("Emoji"))
            GetComponent<Animator>().SetTrigger("StopBlink");
    }
}