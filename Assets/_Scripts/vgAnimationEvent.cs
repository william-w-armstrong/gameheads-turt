using UnityEngine;

using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class vgAnimationEvent
{
	// TODO - make this automatically determined by the parameter you last changed
	public enum ParameterType
	{
		floatParameter,
		intParameter,
		stringParameter,
		objectParameter,
		noParameter,
	}

	[Tooltip("Name of Animation Event in animation event handler.")]
	public string functionName;

	[Tooltip("Time in seconds through animation to trigger")]
	public float time;

	[HideInInspector]
	public float floatToPass;

	[HideInInspector]
	public int intToPass;

	[HideInInspector]
	public string stringToPass;

	[HideInInspector]
	public Object objectReferenceToPass;

	[Tooltip("If true - this event always triggers on State Exit even if its time isn't up")]
	public bool alwaysTrigger = false;

	[Tooltip("If true - this event plays every loop through")]
	public bool repeatOnLoop = true;

	[Tooltip("Type of parameter you want to pass to the anim event. You only get one.")]
	public ParameterType parameterToPass;

	public void TriggerOn( GameObject owner )
	{
		if( System.String.IsNullOrEmpty(functionName) )
		{
			return;
		}

		object parameter = null;

		switch( parameterToPass )
		{
			case ParameterType.floatParameter:
				parameter = floatToPass;
				break;
			case ParameterType.intParameter:
				parameter = intToPass;
				break;
			case ParameterType.stringParameter:
				parameter = stringToPass;
				break;
			case ParameterType.objectParameter:
				parameter = objectReferenceToPass;
				break;
			case ParameterType.noParameter:
			default:
				//parameter = noneShallPass;
				break;
		}

		if( parameter != null )
		{
			owner.SendMessage(functionName, parameter);
		}
		else
		{
			owner.SendMessage(functionName);
		}
	}

	public void TriggerIfPast( float currentTime, float previousTime, GameObject owner )
	{
		if( previousTime < time && time <= currentTime )
		{
			TriggerOn( owner );
		}
	}
}


public class vgMecanimEvent : StateMachineBehaviour 
{
	public vgAnimationEvent onEnterEvent;
	public vgAnimationEvent onExitEvent;

	public List<vgAnimationEvent> events;

	private float lastRealPlayTime = 0.0f;
	private float lastFractionalPlayTime = 0.0f;

	 // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
	override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
        // on enter, set our last play time to just a tiny bit smaller than zero to catch first frame events
        lastRealPlayTime = -1.0f * float.Epsilon;
		lastFractionalPlayTime = lastRealPlayTime;

		if( onEnterEvent != null )
		{
			onEnterEvent.TriggerOn(animator.gameObject);
		}
	}

	// OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
	override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		float length = stateInfo.length;
		float normalizedPlayTime = stateInfo.normalizedTime;
		float realPlayTime = normalizedPlayTime * length;
		float fractionalPlayTime = normalizedPlayTime - (int)normalizedPlayTime;
		fractionalPlayTime *= length;
				
		for( int i = 0; i < events.Count; ++i )
		{
			vgAnimationEvent animEvent = events[i];

			if( animEvent.repeatOnLoop )
			{
				// if our fractional play time is greater than our current one, we looped, assume we are back at the beginning
				if( lastFractionalPlayTime > fractionalPlayTime )
				{
					// play anything from last frame to the loop
					animEvent.TriggerIfPast(length, lastFractionalPlayTime, animator.gameObject);

					// play anything from the loop to our current time
					animEvent.TriggerIfPast(fractionalPlayTime, 0.0f, animator.gameObject);
				}
				else
				{
					animEvent.TriggerIfPast(fractionalPlayTime, lastFractionalPlayTime, animator.gameObject);
				}
			}
			else
			{
				animEvent.TriggerIfPast(realPlayTime, lastRealPlayTime, animator.gameObject);
			}
		}

		lastRealPlayTime = realPlayTime;
		lastFractionalPlayTime = fractionalPlayTime;
	}

	// OnStateExit is called when a transition ends and the state machine finishes evaluating this state
	override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		CheckForReliableEvents(animator.gameObject);

		if( onExitEvent != null )
		{
			onExitEvent.TriggerOn(animator.gameObject);
		}
	}

	private void CheckForReliableEvents(GameObject owner)
	{
		for( int i = 0; i < events.Count; ++i )
		{
			vgAnimationEvent animEvent = events[i];

			if( animEvent.alwaysTrigger )
			{
				if( animEvent.repeatOnLoop )
				{
					if( lastFractionalPlayTime < animEvent.time )
					{
						animEvent.TriggerOn(owner);
					}
				}
				else
				{
					if( lastRealPlayTime < animEvent.time )
					{
						animEvent.TriggerOn(owner);
					}
				}
			}
		}
	}
}
