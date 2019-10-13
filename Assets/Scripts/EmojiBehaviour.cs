using System;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;

/// <summary>
/// Base script for all emojis except the player 
/// </summary>
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
    
    /// <summary>
    /// Stored parameters for the rigidbody to make the delete and add component invisible
    /// Todo : Need to investigate if making it Kinematic instead when needed is simpler /// </summary>
    private Rigidbody2DParameters rigidbody2DParameters;

    protected Rigidbody2D MyRigidbody2D;
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

    /// <summary>
    /// When the player drop an emoji
    /// </summary>
    public virtual void Degroup()
    {
        var playerPos = transform.parent.position;
        transform.SetParent(null);
        
        // Todo : Clean to make sure the effect is done even when there's already a Rigidbody (but it shouldn't happen)
        if (MyRigidbody2D != null) return;

        MyRigidbody2D = gameObject.AddComponent<Rigidbody2D>();
        MyRigidbody2D.mass = rigidbody2DParameters.mass;
        MyRigidbody2D.drag = rigidbody2DParameters.linearDrag;
        MyRigidbody2D.angularDrag = rigidbody2DParameters.angularDrag;
        MyRigidbody2D.gravityScale = rigidbody2DParameters.gravityScale;
        // ReSharper disable once Unity.InefficientPropertyAccess
        MyRigidbody2D.AddForce((transform.position - playerPos).normalized * dropPower);
        // Todo : Add every layer index or name to a static file to avoid typos
        gameObject.layer = LayerMask.NameToLayer("Ungrabable");

        // ReSharper disable once Unity.PreferAddressByIdToGraphicsParams
        GetComponent<Animator>().SetTrigger("Blink");
    }
    
    /// <summary>
    /// Pre explosion effect
    /// </summary>
    public virtual void Bounce()
    {
        var parent = transform.parent;
        // ReSharper disable once Unity.InefficientPropertyAccess
        var position = transform.position;
        var forceDirection = parent.position - position;
        Explode(forceDirection.normalized);
    }

    protected virtual void Explode(Vector2 direction)
    {
        transform.parent.GetComponent<Rigidbody2D>().AddForce(direction * launchPower);
        if (GetComponent<HingeJoint2D>())
        {
            PlayerBehaviour.MakeIntangible(timeIntangibleAfterRope);
        }
    }

    /// <summary>
    /// Todo : Need to be entirely redone to avoid double check layers
    /// </summary>
    /// <param name="other"></param>
    public virtual void OnCollisionEnter2D(Collision2D other)
    {
        if (MyRigidbody2D != null && other.gameObject.layer == LayerMask.NameToLayer("Floor"))
            Destroy(MyRigidbody2D);
        gameObject.layer = LayerMask.NameToLayer("Emoji");
        if (other.gameObject.layer != LayerMask.NameToLayer("Emoji"))
            GetComponent<Animator>().SetTrigger("StopBlink");
    }
}