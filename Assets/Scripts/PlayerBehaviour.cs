﻿using System.Collections;
using FMOD.Studio;
using FMODUnity;
using UnityEngine;
using UnityEngine.Experimental.Input;
using UnityEngine.SceneManagement;

public class PlayerBehaviour : MonoBehaviour
{
#pragma warning disable 
    [SerializeField] private float baseSpeed, maxSpeed, degroupBounce, delayBeforeDegroup;

    [SerializeField] private InputActionAsset asset;

    [Header("Sound")] [Space] [SerializeField, Range(1, 100)]
    private int maxPitchShiftStep;

    [SerializeField] [EventRef] private string snapSound, jumpSound, degroupSound;

    // Fmod need this
    private EventInstance snapEvent;
    private PARAMETER_ID snapPitchShiftID;

    // Store actual input
    private InputAction action;
    
    private new Rigidbody2D rigidbody2D;
    private Bounds playerBounds;
    private Vector2 direction;

    private bool canDegroup;

    // Start is called before the first frame update
    private void Start()
    {
        rigidbody2D = GetComponent<Rigidbody2D>();
        playerBounds = GetComponent<Collider2D>().bounds;
        EnableInputs();
        snapEvent = RuntimeManager.CreateInstance(snapSound);
        snapEvent.getDescription(out var snapEventDescription);
        snapEventDescription.getParameterDescriptionByName("PitchShift", out var snapParameterDescription);
        snapPitchShiftID = snapParameterDescription.id;
    }

    /// <summary>
    /// Todo : Redo to activate the whole action map instead of action one by one
    /// </summary>
    private void EnableInputs()
    {
        var actionMap = asset.GetActionMap("Player");
        action = actionMap.GetAction("Move");
        action.Enable();
        action.performed += OnMove;
        action.cancelled += context => { direction = Vector2.zero; };
        action = actionMap.GetAction("Degroup");
        action.Enable();
        action.performed += OnDegroup;
        action = actionMap.GetAction("Reset");
        action.Enable();
        action.performed += OnReset;
    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        // direction get by input
        rigidbody2D.AddForceAtPosition(direction * baseSpeed, (Vector2) transform.position + Vector2.up);
        rigidbody2D.velocity = Vector2.ClampMagnitude(rigidbody2D.velocity, maxSpeed);
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        // Add object to the player
        if (other.gameObject.CompareTag("Emoji"))
        {
            var colliderToAdd = (PolygonCollider2D) other.collider;
            colliderToAdd.transform.SetParent(transform, true);
            playerBounds.Encapsulate(colliderToAdd.bounds);
            if (other.gameObject.GetComponent<HingeJoint2D>())
            {
                var hinge = gameObject.AddComponent<HingeJoint2D>();
                hinge.connectedBody = other.rigidbody;
                canDegroup = true;
            }

            // Added a range to avoid loss of fraction
            // ReSharper disable once PossibleLossOfFraction
            snapEvent.setParameterByID(snapPitchShiftID, (float) transform.childCount / maxPitchShiftStep);
            snapEvent.start();
        }

        canDegroup = true;
    }

    // Better store the input and process it in FixedUpdate than check for input in every FixedUpdate
    private void OnMove(InputAction.CallbackContext context)
    {
        direction = new Vector2(context.ReadValue<float>(), 0);
    }

    /// <summary>
    /// Degroup is done after a jump
    /// </summary>
    /// <param name="context"></param>
    private void OnDegroup(InputAction.CallbackContext context)
    {
        if (!canDegroup)
            return;
        if (GetComponent<HingeJoint2D>())
            Destroy(GetComponent<HingeJoint2D>());
        rigidbody2D.AddForce(Vector2.up * degroupBounce);
        var usables = GetComponentsInChildren<IUsable>();
        StartCoroutine(DegroupDelay(usables));
        foreach (var usable in usables)
        {
            usable.Bounce();
        }
    }
    
    private IEnumerator DegroupDelay(IUsable[] usables)
    {
        canDegroup = false;
        RuntimeManager.PlayOneShot(jumpSound, transform.position);
        yield return new WaitForSeconds(delayBeforeDegroup);
        foreach (var usable in usables)
        {
            usable.Degroup();
        }

        if (usables.Length > 0)
            RuntimeManager.PlayOneShot(degroupSound, transform.position);
    }

    public static void Die()
    {
        Scene scene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(scene.name);
    }

    private void OnReset(InputAction.CallbackContext context)
    {
        Die();
    }

    public void MakeIntangible(float time)
    {
        StartCoroutine(Intangibility(time));
    }

    /// <summary>
    /// Make the player not colliding with emojis thanks to layers
    /// </summary>
    /// <param name="time"></param>
    /// <returns></returns>
    private IEnumerator Intangibility(float time)
    {
        gameObject.layer = LayerMask.NameToLayer("Ungrabable");
        yield return new WaitForSeconds(time);
        gameObject.layer = LayerMask.NameToLayer("Emoji");
    }

    private void OnDestroy()
    {
        snapEvent.release();
        // Todo : Unsubscribe input events
    }
}