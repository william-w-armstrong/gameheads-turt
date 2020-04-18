using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class DebugBreakState : StateMachineBehaviour
{
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);
        EditorApplication.isPaused = true;        
    }
}
