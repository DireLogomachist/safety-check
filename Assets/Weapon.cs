using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{

    public AnimationCurve curve = AnimationCurve.EaseInOut(0.0f , 0.0f , 1.0f , 1.0f);
    bool lerpFlag = false;

    void Start() {
    }

    void Update() {
    }

    public void RotateUp() {
        Quaternion x = transform.rotation;
        Quaternion y = Quaternion.Euler(new Vector3(90, 0, 0))*transform.rotation;
        if(!lerpFlag) StartCoroutine(CurveLerp(x, y, curve));
    }
    
    public void RotateDown() {
        Quaternion x = transform.rotation;
        Quaternion y = Quaternion.Euler(new Vector3(-90, 0, 0))*transform.rotation;
        if(!lerpFlag) StartCoroutine(CurveLerp(x, y, curve));
    }
    
    public void RotateLeft() {
        Quaternion x = transform.rotation;
        Quaternion y = Quaternion.Euler(new Vector3(0, 90, 0))*transform.rotation;
        if(!lerpFlag) StartCoroutine(CurveLerp(x, y, curve));
    }
    
    public void RotateRight() {
        Quaternion x = transform.rotation;
        Quaternion y = Quaternion.Euler(new Vector3(0, -90, 0))*transform.rotation;
        if(!lerpFlag) StartCoroutine(CurveLerp(x, y, curve));
    }

    public IEnumerator CurveLerp(Quaternion start, Quaternion end, AnimationCurve curve) {
        float time = 0.5f;
        float elapsed = 0.0f;
        lerpFlag = true;

        while(elapsed < time) {
            elapsed += Time.deltaTime;
            transform.rotation = Quaternion.LerpUnclamped(start, end, curve.Evaluate(elapsed/time));
            yield return null;
        }

        transform.rotation = end;
        lerpFlag = false;
        yield return null;
    }
}
