using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleTurtleController : MonoBehaviour
{
    // tuning variables for motion
    [Tooltip("How far can we move off root in meters. The extent of our leash")]
    public float maxLocalOffset = 0.5f;

    [Tooltip("How fast, in m/s, to move at full stick deflection")]
    public float maxSpeed = 0.5f;

    [Tooltip("Degrees Per Second at max stick deflection")]
    public float maxTurnSpeedDegrees = 5.0f;
        
    // horrible hacks to feed in input from the UI for the class - have an Input Manager that everything pulls from
    [HideInInspector]
    public Vector2 input;

    [HideInInspector]
    public bool shouldBite;

    // Animation Hookup
    [Tooltip("Controller for the turtle.")]
    public Animator animator;

    private int forwardVelocityParameter = Animator.StringToHash("ForwardVelocity");
    private int turnVelocityParameter = Animator.StringToHash("TurnVelocity");
    private int shouldBiteParameter = Animator.StringToHash("Bite");


    // Update is called once per frame
    void Update()
    {
        float time = Time.deltaTime;

        // calculate velocity
        float forwardVelocity = maxSpeed * input.y;
        float rotationVelocity = maxTurnSpeedDegrees * input.x;

        transform.Rotate(Vector3.up, rotationVelocity * time, Space.Self);
        transform.Translate(forwardVelocity * time * Vector3.forward, Space.Self);

        // apply leash
        Transform parent = transform.parent;
        Vector3 toParent = transform.position - parent.position;
        float distance = toParent.magnitude;
        if (distance > maxLocalOffset && distance > 0)
        {
            Vector3 offsetDirection = toParent / distance;
            transform.position = parent.position + offsetDirection * maxLocalOffset;
        }

        // pass in input values to animation. Use normalized values, so we don't have to tune Animator when we tune speed
        animator.SetFloat(forwardVelocityParameter, input.y);
        animator.SetFloat(turnVelocityParameter, input.x);
        animator.SetBool(shouldBiteParameter, shouldBite);
        
    }
}
