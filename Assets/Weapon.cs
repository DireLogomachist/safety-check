using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Weapon : MonoBehaviour {

    public AnimationCurve curve = AnimationCurve.EaseInOut(0.0f , 0.0f , 1.0f , 1.0f);

    Queue<InputPair> inputBuffer = new Queue<InputPair>();
    float bufferLength = 1.5f;
    bool inputFlag = false;

    public int ammo = 5;
    bool safe = false;
    bool magDropped = false;
    bool sightsUp = false;
    bool selectorUp = false;

    void Start() {
        ActionTrigger[] triggers = GetComponentsInChildren<ActionTrigger>();
        triggers[0].function.AddListener(CycleAction);     // Charging Handle
        triggers[1].function.AddListener(ToggleSights);    // Trigger
        triggers[2].function.AddListener(Fire);            // Mag Release
        triggers[3].function.AddListener(MagDrop);         // Selector
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

        if(ammo <= 0 && !safe) {
            // "Safe!" popup
            // load next level
            safe = true;
            Debug.Log("Safe");
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
        StartCoroutine(CurveLerp(transform, x, y, curve, 0.5f));
        yield return null;
    }

    public IEnumerator CurveLerp(Transform transform, Quaternion start, Quaternion end, AnimationCurve curve, float time) {
        float elapsed = 0.0f;
        inputFlag = true;

        while(elapsed < time) {
            elapsed += Time.deltaTime;
            transform.localRotation = Quaternion.LerpUnclamped(start, end, curve.Evaluate(elapsed/time));
            yield return null;
        }

        transform.localRotation = end;
        inputFlag = false;
        yield return null;
    }

    public void QueueInput(IEnumerator input) {
        inputBuffer.Enqueue(new InputPair(input, Time.time));
    }

    public void MagDrop() {
        if(!selectorUp) {
            GetComponent<Animator>().Play("AR16_selector_up");
            selectorUp = true;
        } else  {
            GetComponent<Animator>().Play("AR16_selector_down");
            selectorUp = false;
        }
        if(!magDropped) {
            GetComponent<Animator>().Play("AR16_mag_removal");
            if(ammo > 0)
                ammo = 1;
            magDropped = true;
        }
    }

    public void CycleAction() {
        GetComponent<Animator>().Play("AR16_charging_handle");
        GetComponent<Animator>().Play("AR16_bolt");
        // eject casing
        if(ammo > 0) {
            ammo -= 1;
            if(ammo == 0)
                transform.Find("pivot").Find("mag").Find("round").gameObject.SetActive(false);
        }
        Debug.Log("Cycle Action");
    }

    public void Fire() {
        GetComponent<Animator>().Play("AR16_mag_release");
        if(ammo > 0) {
            // fire animation
            ammo -= 1;
            if(ammo == 0 && !magDropped)
                transform.Find("pivot").Find("mag").Find("round").gameObject.SetActive(false);
        }
        
        Debug.Log("Fire");
    }

    public void ToggleSights() {
        AnimationCurve toggleCurve = AnimationCurve.EaseInOut(0.0f , 0.0f , 1.0f , 1.0f);
        Transform front = transform.Find("pivot").Find("sight_full_front").Find("sight_front");
        Transform back = transform.Find("pivot").Find("sight_full_rear").Find("sight_back");
        GetComponent<Animator>().Play("AR16_trigger");
        if(!sightsUp) {
            StartCoroutine(CurveLerp(front, front.localRotation, Quaternion.Euler(new Vector3(0, 0, -90))*front.localRotation, toggleCurve, 0.4f));
            StartCoroutine(CurveLerp(back, back.localRotation, Quaternion.Euler(new Vector3(0, 0, -90))*back.localRotation, toggleCurve, 0.4f));
            sightsUp = true;
        } else {
            StartCoroutine(CurveLerp(front, front.localRotation, Quaternion.Euler(new Vector3(0, 0, 90))*front.localRotation, toggleCurve, 0.4f));
            StartCoroutine(CurveLerp(back, back.localRotation, Quaternion.Euler(new Vector3(0, 0, 90))*back.localRotation, toggleCurve, 0.4f));
            sightsUp = false;
        }
    }
}
