using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spin : MonoBehaviour {

    public float speed = 1.0f;

    void Start() {
    }

    void Update() {
        transform.Rotate(new Vector3(speed*Time.deltaTime,0,0));
    }
}
