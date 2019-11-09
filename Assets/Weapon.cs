using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour {

    public AnimationCurve curve = AnimationCurve.EaseInOut(0.0f , 0.0f , 1.0f , 1.0f);
    public int ammo = 5;

    Queue<InputPair> inputBuffer = new Queue<InputPair>();
    float bufferLength = 1.5f;
    bool inputFlag = false;

    void Start() {
    }

    void Update() {
        int c = inputBuffer.Count;
        for(int i=0; i<c; i++) {
            if((Time.time - inputBuffer.Peek().timestamp) > bufferLength) {
                inputBuffer.Dequeue();
            } else {
                if(inputFlag == false)
                    StartCoroutine(inputBuffer.Dequeue().input);
                break;
            }
        }
    }

    public void RotateUp() {
        QueueInput(WeaponRotate(new Vector3(90, 0, 0)));
    }

    public void RotateDown() {
        QueueInput(WeaponRotate(new Vector3(-90, 0, 0)));
    }

    public void RotateLeft() {
        QueueInput(WeaponRotate(new Vector3(0, 90, 0)));
    }

    public void RotateRight() {

        QueueInput(WeaponRotate(new Vector3(0, -90, 0)));
    }

    public IEnumerator WeaponRotate(Vector3 rot) {
        Quaternion x = transform.rotation;
        Quaternion y = Quaternion.Euler(rot)*transform.rotation;
        StartCoroutine(CurveLerp(x, y, curve));
        yield return null;
    }

    public IEnumerator CurveLerp(Quaternion start, Quaternion end, AnimationCurve curve) {
        float time = 0.5f;
        float elapsed = 0.0f;
        inputFlag = true;

        while(elapsed < time) {
            elapsed += Time.deltaTime;
            transform.rotation = Quaternion.LerpUnclamped(start, end, curve.Evaluate(elapsed/time));
            yield return null;
        }

        transform.rotation = end;
        inputFlag = false;
        yield return null;
    }

    public void QueueInput(IEnumerator input) {
        inputBuffer.Enqueue(new InputPair(input, Time.time));
    }

    public void MagDrop() {
        if(ammo > 0) ammo = 1;
    }
}
