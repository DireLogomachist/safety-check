using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ActionTrigger : MonoBehaviour {

    public UnityEvent function;

    void Start() {
    }

    void Update() {
    }

    void OnMouseDown() {
        function.Invoke();
    }
}
