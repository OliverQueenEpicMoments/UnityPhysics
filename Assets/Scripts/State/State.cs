using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditorInternal;
using UnityEngine;

public abstract class State {
    protected float Timer { get; set; }
    protected float FixedTimer { get; set; }
    protected float LateTimer { get; set; }


    public StateMachine statemachine;

    public virtual void OnEnter(StateMachine stateMachine) {
        statemachine = stateMachine;
    }

    public virtual void OnUpdate() {
        Timer += Time.deltaTime;
    }

    public virtual void OnFixedUpdate() { 
        FixedTimer += Time.deltaTime;
    }

    public virtual void OnLateUpdate() { 
        LateTimer += Time.deltaTime;
    }

    public virtual void OnExit() { 
        
    }

    #region Passthrough Methods

    /// <summary>
    /// Removes a gameobject, component, or asset.
    /// </summary>
    /// <param name="obj">The type of Component to retrieve.</param>
    protected static void Destroy(UnityEngine.Object obj) {
        UnityEngine.Object.Destroy(obj);
    }

    /// <summary>
    /// Returns the component of type T if the game object has one attached, null if it doesn't.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    protected T GetComponent<T>() where T : Component { return statemachine.GetComponent<T>(); }

    /// <summary>
    /// Returns the component of Type <paramref name="type"/> if the game object has one attached, null if it doesn't.
    /// </summary>
    /// <param name="type">The type of Component to retrieve.</param>
    /// <returns></returns>
    protected Component GetComponent(System.Type type) { return statemachine.GetComponent(type); }

    /// <summary>
    /// Returns the component with name <paramref name="type"/> if the game object has one attached, null if it doesn't.
    /// </summary>
    /// <param name="type">The type of Component to retrieve.</param>
    /// <returns></returns>
    protected Component GetComponent(string type) { return statemachine.GetComponent(type); }
    #endregion
}