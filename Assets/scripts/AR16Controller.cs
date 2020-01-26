using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class AR16Controller : WeaponController {

    public GameObject casing;
    public GameObject round;

    bool magDropped = false;
    bool sightsUp = false;
    bool selectorUp = false;

    AudioClip boltCycleClip;
    AudioClip selectorUpClip;
    AudioClip selectorDownClip;
    AudioClip triggerClip;

    public override void Start() {
        base.Start();
        ActionTrigger[] triggers = GetComponentsInChildren<ActionTrigger>();
        triggers[0].function.AddListener(CycleAction);     // Charging Handle
        triggers[1].function.AddListener(ToggleSights);    // Trigger
        triggers[2].function.AddListener(Fire);            // Mag Release
        triggers[3].function.AddListener(MagToggle);       // Selector

        selectorUpClip = (AudioClip) Resources.Load("audio/AR16_switch_1");
        selectorDownClip = (AudioClip) Resources.Load("audio/AR16_switch_2");
        boltCycleClip = (AudioClip) Resources.Load("audio/AR16_bolt_cycle");
        triggerClip = (AudioClip) Resources.Load("audio/AR16_trigger");
    }

    public override void Update() {
        base.Update();
    }

    protected override IEnumerator Misfire() {
        ClearInput();
        yield return new WaitForSeconds(1.0f);
        ClearInput();
        QueueInput(FireAction());
    }

    public void MagToggle() {
        QueueInput(MagToggleAction());
    }

    IEnumerator MagToggleAction() {
        inputFlag = true;
        ToggleSelector();
        ToggleMagazine();
        UpdateAmmoUI();
        if(magDropped)
            canvas.Find("PopupController").GetComponent<PopupController>().Spawn(2);
        yield return new WaitForSeconds(0.8f);
        inputFlag = false;
    }

    public void CycleAction() {
        QueueInput(CycleActionAction());
    }

    IEnumerator CycleActionAction() {
        inputFlag = true;
        GetComponent<Animator>().Play("AR16_charging_handle");
        GetComponent<Animator>().Play("AR16_bolt");
        audio.PlayOneShot(boltCycleClip, 0.1f);
        
        if(ammo > 0) {
            StartCoroutine(Eject(round, pivot.Find("ejection_cycling_spawn")));
            StartCoroutine(PlayCaseEjectAudio());
            ammo -= 1;
            if(!magDropped) {
                magAmmo = ammo - 1;
                if(magAmmo < 0) magAmmo = 0;
            }
            canvas.Find("PopupController").GetComponent<PopupController>().Spawn(3);
            UpdateAmmoUI();
            if(magAmmo == 0)
                pivot.Find("mag").Find("round").gameObject.SetActive(false);
            if(ammo == 0)
                StartCoroutine(Safe());
        }
        yield return new WaitForSeconds(0.8f);
        inputFlag = false;
    }

    public void Fire() {
        QueueInput(FireAction());
    }

    IEnumerator FireAction() {
        inputFlag = true;
        GetComponent<Animator>().Play("AR16_mag_release");
        if(ammo > 0) {
            GetComponent<Animator>().Play("AR16_bolt");
            GetComponent<Animator>().Play("AR16_recoil");
            StartCoroutine(Eject(casing, pivot.Find("ejection_firing_spawn")));
            StartCoroutine(MuzzleFlash());
            StartCoroutine(Camera.main.GetComponent<CameraController>().CameraShake());
            audio.PlayOneShot(gunshotClip, 1.0f);
            if(Quaternion.Euler(transform.eulerAngles)*Vector3.right == -Vector3.forward) {
                camCon.MusicMute();
                canvas.GetComponent<UIController>().transition.SetTrigger("Splatter");
                yield return new WaitForSeconds(2.0f);
                canvas.GetComponent<UIController>().transition.SetTrigger("Transition");
                yield return new WaitForSeconds(3.0f);
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            } else {
                StartCoroutine(camCon.MusicTempMute());
            }
            ammo -= 1;
            if(!magDropped) {
                magAmmo = ammo - 1;
                if(magAmmo < 0) magAmmo = 0;
            }
            UpdateAmmoUI();
            if(magAmmo == 0)
                pivot.Find("mag").Find("round").gameObject.SetActive(false);
            yield return new WaitForSeconds(0.8f);
            canvas.Find("PopupController").GetComponent<PopupController>().Spawn(4);
            audio.PlayOneShot(misfirePopupClip, 0.2f);
            if(ammo == 0)
                StartCoroutine(Safe());
        }
        yield return new WaitForSeconds(0.5f);
        inputFlag = false;
    }

    public void ToggleSights() {
        QueueInput(ToggleSightsAction());
    }

    IEnumerator ToggleSightsAction() {
        inputFlag = true;
        AnimationCurve toggleCurve = AnimationCurve.EaseInOut(0.0f , 0.0f , 1.0f , 1.0f);
        Transform front = pivot.Find("sight_full_front").Find("sight_front");
        Transform back = pivot.Find("sight_full_rear").Find("sight_back");
        GetComponent<Animator>().Play("AR16_trigger");
        audio.PlayOneShot(triggerClip, 0.1f);
        if(!sightsUp) {
            StartCoroutine(CurveLerp(front, front.localPosition, front.localPosition, front.localRotation, Quaternion.Euler(new Vector3(0, 0, -90))*front.localRotation, toggleCurve, 0.4f));
            StartCoroutine(CurveLerp(back, back.localPosition, back.localPosition, back.localRotation, Quaternion.Euler(new Vector3(0, 0, -90))*back.localRotation, toggleCurve, 0.4f));
        } else {
            StartCoroutine(CurveLerp(front, front.localPosition, front.localPosition, front.localRotation, Quaternion.Euler(new Vector3(0, 0, 90))*front.localRotation, toggleCurve, 0.4f));
            StartCoroutine(CurveLerp(back, back.localPosition, back.localPosition, back.localRotation, Quaternion.Euler(new Vector3(0, 0, 90))*back.localRotation, toggleCurve, 0.4f));
        }
        sightsUp = !sightsUp;
        yield return new WaitForSeconds(0.5f);
        inputFlag = false;
    }

    void ToggleSelector() {
        AnimationCurve toggleCurve = AnimationCurve.EaseInOut(0.0f , 0.0f , 1.0f , 1.0f);
        Transform selector = pivot.Find("selector");
        Vector3 pos = selector.localPosition;
        if(!selectorUp) {
            StartCoroutine(CurveLerp(selector, pos, pos, selector.localRotation, Quaternion.Euler(new Vector3(0, 0, -90))*selector.localRotation, toggleCurve, 0.4f));
            audio.PlayOneShot(selectorUpClip, 0.1f);
        } else {
            StartCoroutine(CurveLerp(selector, pos, pos, selector.localRotation, Quaternion.Euler(new Vector3(0, 0, 90))*selector.localRotation, toggleCurve, 0.4f));
            audio.PlayOneShot(selectorDownClip, 0.1f);
        }
        selectorUp = !selectorUp;
    }

    void ToggleMagazine() {
        AnimationCurve toggleCurve = AnimationCurve.EaseInOut(0.0f , 0.0f , 1.0f , 1.0f);
        Transform mag = pivot.Find("mag");
        Vector3 move = new Vector3(0, -0.261f, 0);
        if(!magDropped) {
            StartCoroutine(CurveLerp(mag, mag.localPosition, mag.localPosition + move, mag.localRotation, mag.localRotation, toggleCurve, 0.4f));
        } else {
            StartCoroutine(CurveLerp(mag, mag.localPosition, mag.localPosition - move, mag.localRotation, mag.localRotation, toggleCurve, 0.4f));
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

    IEnumerator Eject(GameObject obj, Transform spawn) {
        GameObject ejection = GameObject.Instantiate(obj, spawn.position, Quaternion.Euler(new Vector3(0,90,0))*spawn.rotation);
        ejection.transform.parent = null;
        Vector3 force = Quaternion.Euler(spawn.transform.eulerAngles)*(new Vector3(Random.RandomRange(-0.10f, -0.15f), Random.RandomRange(0.35f, .45f), Random.RandomRange(0, .01f)));
        Vector3 forcePostion = Quaternion.Euler(spawn.transform.eulerAngles)*(new Vector3(Random.RandomRange(40.0f,150.0f), Random.RandomRange(-1.0f,1.0f), Random.RandomRange(-15.0f,0.0f)));
        yield return new WaitForSeconds(.1f);
        ejection.GetComponent<Rigidbody>().AddForceAtPosition(force, forcePostion, ForceMode.Impulse);
    }

    IEnumerator MuzzleFlash() {
        GameObject muzzleFlash = pivot.Find("muzzle_flash").gameObject;
        muzzleFlash.transform.localEulerAngles = muzzleFlash.transform.localEulerAngles + new Vector3(Random.RandomRange(0,290),0,0);
        muzzleFlash.SetActive(true);
        yield return new WaitForSeconds(0.15f);
        muzzleFlash.SetActive(false);
    }
}
