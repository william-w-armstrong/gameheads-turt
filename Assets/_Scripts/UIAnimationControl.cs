using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIAnimationControl : MonoBehaviour
{
    public Animator animator;

    private int horizontalParameter = Animator.StringToHash("horizontal");
    private int verticalParameter = Animator.StringToHash("vertical");
    private int buttonParameter = Animator.StringToHash("button");

    private int turnParameter = Animator.StringToHash("turn");
    private int forwardParameter = Animator.StringToHash("forward");
    private int biteParameter = Animator.StringToHash("bite");

    [Tooltip("Set this up to pass calculated input to turtle")]
    public SimpleTurtleController turtleController;

    // Update is called once per frame
    void Update()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");
        bool bite = Input.GetButtonDown("Jump");

        animator.SetFloat(horizontalParameter, horizontalInput);
        animator.SetFloat(verticalParameter, verticalInput);
        animator.SetBool(buttonParameter, bite);

        //<HACK> - for educational purposes, I am driving the turtle based on the displayed stick position
        if (turtleController != null)
        {
            // we can't animate non-float values - so...
            turtleController.shouldBite = animator.GetFloat(biteParameter) > 0.5f;

            float turn = animator.GetFloat(turnParameter);
            float forward = animator.GetFloat(forwardParameter);
            turtleController.input = new Vector2(turn, forward);
        }
        //</HACK>
        
    }
}
