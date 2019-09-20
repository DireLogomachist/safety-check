using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    void Start() {
        
    }

    void Update() {
        
    }

    public void RotateUp() {
        transform.RotateAround(transform.position, Camera.main.transform.right, 90);
    }
    
    public void RotateDown() {
        transform.RotateAround(transform.position, Camera.main.transform.right, -90);
    }
    
    public void RotateLeft() {
        transform.RotateAround(transform.position, Camera.main.transform.up, 90);
    }
    
    public void RotateRight() {
        transform.RotateAround(transform.position, Camera.main.transform.up, -90);
    }
}
