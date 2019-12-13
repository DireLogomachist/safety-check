using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class WeaponController : MonoBehaviour {

    public AnimationCurve curve = AnimationCurve.EaseInOut(0.0f , 0.0f , 1.0f , 1.0f);
    public int ammo = 5;
    public int magAmmo;
    public bool safe = false;

    protected Queue<InputPair> inputBuffer = new Queue<InputPair>();
    protected float bufferLength = 1.0f;
    protected bool inputFlag = false;
    protected Transform pivot;
    protected Transform canvas;
    
    public virtual void Start() {
        pivot = transform.Find("pivot");
        canvas = GameObject.Find("UICanvas").transform;
        magAmmo = ammo - 1;
        UpdateAmmoUI();
    }

    public virtual void Update() {
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

        if(ammo <= 0 && !safe) {
            // "Safe!" popup
            // disable input
            // load next level
            safe = true;
            Debug.Log("Safe!");
            canvas.Find("PopupController").GetComponent<PopupController>().Spawn(1);
        }
    }

    public void QueueInput(IEnumerator input) {
        inputBuffer.Enqueue(new InputPair(input, Time.time));
    }

    public void ClearInput() {
        inputBuffer.Clear();
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

    protected IEnumerator WeaponRotate(Vector3 rot) {
        inputFlag = true;
        Quaternion x = transform.rotation;
        Quaternion y = Quaternion.Euler(rot)*transform.rotation;

        if(Quaternion.Euler(y.eulerAngles)*Vector3.right == -Vector3.forward) {
            ClearInput();
            StartCoroutine(Misfire());
        }

        StartCoroutine(CurveLerp(transform, transform.localPosition, transform.localPosition, x, y, curve, 0.5f));
        yield return new WaitForSeconds(0.5f);
        inputFlag = false;
    }

    protected IEnumerator CurveLerp(Transform transform, Vector3 startPos, Vector3 endPos, Quaternion startRot, Quaternion endRot, AnimationCurve curve, float time) {
        float elapsed = 0.0f;
        while(elapsed < time) {
            elapsed += Time.deltaTime;
            transform.localPosition = Vector3.LerpUnclamped(startPos, endPos, curve.Evaluate(elapsed/time));
            transform.localRotation = Quaternion.LerpUnclamped(startRot, endRot, curve.Evaluate(elapsed/time));
            yield return null;
        }

        transform.localPosition = endPos;
        transform.localRotation = endRot;
    }

    protected void UpdateAmmoUI() {
        GameObject.Find("AmmoText").GetComponent<Text>().text = "Ammo: " + ammo;
    }

    protected virtual IEnumerator Misfire() {
        yield break;
    }
}
