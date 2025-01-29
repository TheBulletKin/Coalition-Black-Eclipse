using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* Everything stems from this base state machine
 * Needs only know about the state that is currently active, and execute that state alone.
 * Creating a method with new will override unity's base implementation.
 * 
 */
public class BaseStateMachine : MonoBehaviour
{
	[SerializeField] private BaseState initialState;

	public BaseState CurrentState { get; set; }

	private Dictionary<Type, Component> cachedComponents;

	private void Awake()
	{
		CurrentState = initialState;
		cachedComponents = new Dictionary<Type, Component>();
	}

	private void Update()
	{
		CurrentState.Execute(this);
	}

	/*
	* T is a generic type parameter, so this method can take any object of type Component
	* Dictionary holds a metadata dataType, so Type myName = typeof(RigidBody) would return RigidBody
	* Component is an actual component on an object.
	* So if the type passed into the getComponent call has been called before, fetch it. Improves performance.
	* Allows for it to work regardless of type if that type is a subclass of component.
	* Good for when multiple different potential types run the same thing.
	*/
	public new T GetComponent<T>() where T : Component
	{
		if (cachedComponents.ContainsKey(typeof(T)))
			return cachedComponents[typeof(T)] as T;

		var component = base.GetComponent<T>();
		if (component != null)
		{
			cachedComponents.Add(typeof(T), component);
		}
		return component;
	}
}
