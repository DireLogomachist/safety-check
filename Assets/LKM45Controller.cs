﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class LKM45Controller : MonoBehaviour {

    public GameObject casing;
    public GameObject round;
    public AnimationCurve curve = AnimationCurve.EaseInOut(0.0f , 0.0f , 1.0f , 1.0f);

    Queue<InputPair> inputBuffer = new Queue<InputPair>();
    float bufferLength = 1.5f;
    bool inputFlag = false;

    public int ammo = 5;
    int magAmmo;
    bool safe = false;
    bool magDropped = false;
    bool laserOn = false;
    bool selectorUp = false;
    bool stockFolded = false;
    bool magReleaseDown = false;

    void Start() {
        ActionTrigger[] triggers = GetComponentsInChildren<ActionTrigger>();
        triggers[0].function.AddListener(FoldStock);        // Charging handle -> Folds stock
        triggers[1].function.AddListener(ToggleLaser);      // Trigger -> Toggles laser
        triggers[2].function.AddListener(CycleAction);      // Mag release -> Cycles Action
        triggers[3].function.AddListener(MagToggle);        // Selector -> Mag Drop (maybe will swap? same as AR16... maybe button in buttstock???
        triggers[4].function.AddListener(Fire);             // Laser Sight Button -> Fires
        magAmmo = ammo - 1;
        updateAmmoUI();
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
            // disable input
            // load next level
            safe = true;
            Debug.Log("Safe!");
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

    IEnumerator WeaponRotate(Vector3 rot) {
        inputFlag = true;
        Quaternion x = transform.rotation;
        Quaternion y = Quaternion.Euler(rot)*transform.rotation;
        StartCoroutine(CurveLerp(transform, transform.localPosition, transform.localPosition, x, y, curve, 0.5f));
        yield return new WaitForSeconds(0.5f);
        inputFlag = false;
    }

    IEnumerator CurveLerp(Transform transform, Vector3 startPos, Vector3 endPos, Quaternion startRot, Quaternion endRot, AnimationCurve curve, float time) {
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

    public void QueueInput(IEnumerator input) {
        inputBuffer.Enqueue(new InputPair(input, Time.time));
    }

    public void MagToggle() {
        QueueInput(MagToggleAction());
    }

    IEnumerator MagToggleAction() {
        inputFlag = true;
        ToggleSelector();
        ToggleMagazine();
        updateAmmoUI();
        yield return new WaitForSeconds(0.6f);
        inputFlag = false;
    }

    public void CycleAction() {
        QueueInput(CycleActionAction());
    }

    IEnumerator CycleActionAction() {
        inputFlag = false;
        GetComponent<Animator>().Play("LKM45_bolt");
        if(ammo > 0) {
            StartCoroutine(Eject(round, transform.Find("pivot").Find("ejection_cycling_spawn")));
            ammo -= 1;
            if(!magDropped) {
                magAmmo = ammo - 1;
                if(magAmmo < 0) magAmmo = 0;
            }
            updateAmmoUI();
            if(magAmmo == 0)
                transform.Find("pivot").Find("LKM45_mag").Find("round").gameObject.SetActive(false);
        }
        yield return new WaitForSeconds(0.8f);
        inputFlag = false;
    }

    public void Fire() {
        QueueInput(FireAction());
    }

    IEnumerator FireAction() {
        inputFlag = true;
        //GetComponent<Animator>().Play("LKM45_laser_button");
        if(ammo > 0) {
            GetComponent<Animator>().Play("LKM45_bolt");
            GetComponent<Animator>().Play("LKM45_recoil");
            StartCoroutine(Eject(casing, transform.Find("pivot").Find("ejection_firing_spawn")));
            StartCoroutine(MuzzleFlash());
            ammo -= 1;
            if(!magDropped) {
                magAmmo = ammo - 1;
                if(magAmmo < 0) magAmmo = 0;
            }
            updateAmmoUI();
            if(magAmmo == 0)
                transform.Find("pivot").Find("LKM45_mag").Find("round").gameObject.SetActive(false);
        }
        yield return new WaitForSeconds(0.5f);
        inputFlag = false;
    }

    public void ToggleLaser() {
        QueueInput(ToggleLaserAction());
    }

    IEnumerator ToggleLaserAction() {
        inputFlag = true;
        /*
        AnimationCurve toggleCurve = AnimationCurve.EaseInOut(0.0f , 0.0f , 1.0f , 1.0f);
        Transform front = transform.Find("pivot").Find("sight_full_front").Find("sight_front");
        Transform back = transform.Find("pivot").Find("sight_full_rear").Find("sight_back");
        GetComponent<Animator>().Play("AR16_trigger");
        if(!laserOn) {
            StartCoroutine(CurveLerp(front, front.localPosition, front.localPosition, front.localRotation, Quaternion.Euler(new Vector3(0, 0, -90))*front.localRotation, toggleCurve, 0.4f));
            StartCoroutine(CurveLerp(back, back.localPosition, back.localPosition, back.localRotation, Quaternion.Euler(new Vector3(0, 0, -90))*back.localRotation, toggleCurve, 0.4f));
        } else {
            StartCoroutine(CurveLerp(front, front.localPosition, front.localPosition, front.localRotation, Quaternion.Euler(new Vector3(0, 0, 90))*front.localRotation, toggleCurve, 0.4f));
            StartCoroutine(CurveLerp(back, back.localPosition, back.localPosition, back.localRotation, Quaternion.Euler(new Vector3(0, 0, 90))*back.localRotation, toggleCurve, 0.4f));
        }
        laserOn = !laserOn;
        */
        yield return new WaitForSeconds(0.5f);
        inputFlag = false;
    }

    void ToggleSelector() {
        AnimationCurve toggleCurve = AnimationCurve.EaseInOut(0.0f , 0.0f , 1.0f , 1.0f);
        Transform selector = transform.Find("pivot").Find("LKM45_selector");
        Vector3 pos = selector.localPosition;
        if(!selectorUp) {
            StartCoroutine(CurveLerp(selector, pos, pos, selector.localRotation, Quaternion.Euler(new Vector3(0, 0, -25))*selector.localRotation, toggleCurve, 0.3f));
        } else {
            StartCoroutine(CurveLerp(selector, pos, pos, selector.localRotation, Quaternion.Euler(new Vector3(0, 0, 25))*selector.localRotation, toggleCurve, 0.3f));
        }
        selectorUp = !selectorUp; 
    }

    void ToggleMagazine() {
        AnimationCurve toggleCurve = AnimationCurve.EaseInOut(0.0f , 0.0f , 1.0f , 1.0f);
        Transform mag = transform.Find("pivot").Find("LKM45_mag");
        Vector3 move = new Vector3(0, -0.261f, 0);
        if(!magDropped) {
            StartCoroutine(CurveLerp(mag, mag.localPosition, mag.localPosition + move, mag.localRotation, mag.localRotation, toggleCurve, 0.6f));
        } else {
            StartCoroutine(CurveLerp(mag, mag.localPosition, mag.localPosition - move, mag.localRotation, mag.localRotation, toggleCurve, 0.6f));
        }
        magDropped = !magDropped;
        if(magDropped) {
            if(ammo > 0) {
                magAmmo = ammo - 1;
                ammo = 1;
            }
        } else {
            ammo += magAmmo;
        }
    }

    void FoldStock() {
        QueueInput(FoldStockAction());
    }

    IEnumerator FoldStockAction() {
        inputFlag = true;
        GetComponent<Animator>().Play("LKM45_bolt");
        ToggleStock();
        yield return new WaitForSeconds(0.8f);
        inputFlag = false;
    }

    void ToggleStock() {
        AnimationCurve toggleCurve = AnimationCurve.EaseInOut(0.0f , 0.0f , 1.0f , 1.0f);
        Transform selector = transform.Find("pivot").Find("LKM45_stock");
        Vector3 pos = selector.localPosition;
        if(!stockFolded) {
            StartCoroutine(CurveLerp(selector, pos, pos, selector.localRotation, Quaternion.Euler(new Vector3(0, 175, 0))*selector.localRotation, toggleCurve, 0.7f));
        } else {
            StartCoroutine(CurveLerp(selector, pos, pos, selector.localRotation, Quaternion.Euler(new Vector3(0, -175, 0))*selector.localRotation, toggleCurve, 0.7f));
        }
        stockFolded = !stockFolded;
    }

    IEnumerator Eject(GameObject obj, Transform spawn) {
        GameObject ejection = GameObject.Instantiate(obj, spawn.position, Quaternion.Euler(new Vector3(0,90,0))*spawn.rotation);
        ejection.transform.parent = null;
        Vector3 force = Quaternion.Euler(spawn.transform.eulerAngles)*(new Vector3(Random.RandomRange(-0.10f, -0.15f), Random.RandomRange(0.35f, .45f), Random.RandomRange(0, .01f)));
        Vector3 forcePostion = Quaternion.Euler(spawn.transform.eulerAngles)*(new Vector3(Random.RandomRange(40.0f,150.0f), Random.RandomRange(-1.0f,1.0f), Random.RandomRange(-15.0f,0.0f)));
        yield return new WaitForSeconds(.1f);
        ejection.GetComponent<Rigidbody>().AddForceAtPosition(force, forcePostion, ForceMode.Impulse);
    }

    IEnumerator MuzzleFlash() {
        GameObject muzzleFlash = transform.Find("pivot").Find("muzzle_flash").gameObject;
        muzzleFlash.transform.eulerAngles = new Vector3(Random.RandomRange(0,290), 0, 0);
        muzzleFlash.SetActive(true);
        yield return new WaitForSeconds(0.15f);
        muzzleFlash.SetActive(false);
    }

    void updateAmmoUI() {
        GameObject.Find("Ammo").GetComponent<Text>().text = "Ammo: " + ammo;
    }
}
