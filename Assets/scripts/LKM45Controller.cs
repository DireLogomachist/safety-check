using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LKM45Controller : WeaponController {

    public GameObject casing;
    public GameObject round;

    bool magDropped = false;
    bool laserOn = false;
    bool selectorUp = false;
    bool stockFolded = false;
    bool magReleaseDown = false;
    bool stockSwitchDown = false;

    GameObject laser;
    LineRenderer laserCore;
    LineRenderer laserBlur;

    AudioClip boltCycleClip;
    AudioClip laserButtonClip;
    AudioClip laserOnClip;
    AudioClip laserOffClip;
    AudioClip selectorDownClip;
    AudioClip selectorUpClip;
    AudioClip stockClip;
    AudioClip stockSwitchClip;
    AudioClip triggerClip;

    public override void Start() {
        base.Start();
        ActionTrigger[] triggers = GetComponentsInChildren<ActionTrigger>();
        triggers[0].function.AddListener(FoldStock);        // Charging handle -> Folds stock
        triggers[1].function.AddListener(ToggleLaser);      // Trigger -> Toggles laser
        triggers[2].function.AddListener(CycleAction);      // Mag Release -> Cycles Action
        triggers[3].function.AddListener(DustCover);        // Selector -> Dust Cover
        triggers[4].function.AddListener(Fire);             // Laser Sight Button -> Fires
        triggers[5].function.AddListener(MagToggle);        // Stock Switch -> Mag Drop

        laser = pivot.Find("LKM45_laser").gameObject;
        laserCore = pivot.Find("LKM45_laser_core").GetComponent<LineRenderer>();
        laserBlur = pivot.Find("LKM45_laser_blur").GetComponent<LineRenderer>();

        boltCycleClip = (AudioClip) Resources.Load("audio/AR16_bolt_cycle");
        laserButtonClip = (AudioClip) Resources.Load("audio/LKM45_laser_button");
        laserOnClip = (AudioClip) Resources.Load("audio/LKM45_laser_sight_on");
        laserOffClip = (AudioClip) Resources.Load("audio/LKM45_laser_sight_off");
        selectorDownClip = (AudioClip) Resources.Load("audio/LKM45_selector_down");
        selectorUpClip = (AudioClip) Resources.Load("audio/LKM45_selector_up");
        stockClip = (AudioClip) Resources.Load("audio/LKM45_stock");
        stockSwitchClip = (AudioClip) Resources.Load("audio/LKM45_stock_switch");
        triggerClip = (AudioClip) Resources.Load("audio/LKM45_trigger");
        
        if(!laserOn) {
            laserCore.SetVertexCount(0);
            laserBlur.SetVertexCount(0);
        }
    }

    public override void Update() {
        base.Update();

        if(laserOn)
            DrawLaser();
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
        ToggleStockSwitch();
        ToggleMagazine();
        UpdateAmmoUI();
        if(magDropped)
            canvas.Find("PopupController").GetComponent<PopupController>().Spawn(2);
        yield return new WaitForSeconds(0.6f);
        inputFlag = false;
    }

    public void CycleAction() {
        QueueInput(CycleActionAction());
    }

    IEnumerator CycleActionAction() {
        inputFlag = true;
        GetComponent<Animator>().Play("LKM45_bolt");
        audio.PlayOneShot(boltCycleClip, 0.1f);
        MagReleaseToggle();
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
                pivot.Find("LKM45_mag").Find("round").gameObject.SetActive(false);
            if(ammo == 0)
                StartCoroutine(Safe());
        }
        yield return new WaitForSeconds(0.6f);
        inputFlag = false;
    }

    void MagReleaseToggle() {
        AnimationCurve toggleCurve = AnimationCurve.EaseInOut(0.0f , 0.0f , 1.0f , 1.0f);
        Transform selector = pivot.Find("LKM45_mag_release");
        Vector3 pos = selector.localPosition;
        if(!magReleaseDown) {
            StartCoroutine(CurveLerp(selector, pos, pos, selector.localRotation, Quaternion.Euler(new Vector3(0, 0, -25))*selector.localRotation, toggleCurve, 0.2f));
        } else {
            StartCoroutine(CurveLerp(selector, pos, pos, selector.localRotation, Quaternion.Euler(new Vector3(0, 0, 25))*selector.localRotation, toggleCurve, 0.2f));
        }
        magReleaseDown = !magReleaseDown;
    }

    public void Fire() {
        QueueInput(FireAction());
    }

    IEnumerator FireAction() {
        inputFlag = true;
        GetComponent<Animator>().Play("LKM45_laser_button");
        audio.PlayOneShot(laserButtonClip, 0.2f);
        if(ammo > 0) {
            GetComponent<Animator>().Play("LKM45_bolt");
            GetComponent<Animator>().Play("LKM45_recoil");
            StartCoroutine(Eject(casing, pivot.Find("ejection_firing_spawn")));
            StartCoroutine(MuzzleFlash());
            StartCoroutine(Camera.main.GetComponent<CameraController>().CameraShake());
            audio.PlayOneShot(gunshotClip, 1.0f);
            if(Quaternion.Euler(transform.eulerAngles)*Vector3.right == -Vector3.forward) {
                camCon.MusicMute();
                audio.PlayOneShot(impactClip, 2.0f);
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
                pivot.Find("LKM45_mag").Find("round").gameObject.SetActive(false);
            yield return new WaitForSeconds(0.8f);
            canvas.Find("PopupController").GetComponent<PopupController>().Spawn(4);
            audio.PlayOneShot(misfirePopupClip, 0.2f);
            if(ammo == 0)
                StartCoroutine(Safe());
        }
        yield return new WaitForSeconds(0.5f);
        inputFlag = false;
    }

    public void ToggleLaser() {
        QueueInput(ToggleLaserAction());
    }

    IEnumerator ToggleLaserAction() {
        inputFlag = true;
        GetComponent<Animator>().Play("LKM45_trigger");
        audio.PlayOneShot(triggerClip, 0.2f);
        yield return new WaitForSeconds(0.2f);
        if(!laserOn) {
            DrawLaser();
            audio.PlayOneShot(laserOnClip, 1.0f);
        } else {
            laserCore.SetVertexCount(0);
            laserBlur.SetVertexCount(0);
            audio.PlayOneShot(laserOffClip, 1.0f);
        }
        laserOn = !laserOn;
        yield return new WaitForSeconds(0.4f);
        inputFlag = false;
    }

    void ToggleSelector() {
        AnimationCurve toggleCurve = AnimationCurve.EaseInOut(0.0f , 0.0f , 1.0f , 1.0f);
        Transform selector = pivot.Find("LKM45_selector");
        Vector3 pos = selector.localPosition;
        if(!selectorUp) {
            StartCoroutine(CurveLerp(selector, pos, pos, selector.localRotation, Quaternion.Euler(new Vector3(0, 0, -25))*selector.localRotation, toggleCurve, 0.3f));
            audio.PlayOneShot(selectorDownClip, 1.0f);
        } else {
            StartCoroutine(CurveLerp(selector, pos, pos, selector.localRotation, Quaternion.Euler(new Vector3(0, 0, 25))*selector.localRotation, toggleCurve, 0.3f));
            audio.PlayOneShot(selectorUpClip, 1.0f);
        }
        selectorUp = !selectorUp; 
    }

    void ToggleMagazine() {
        AnimationCurve toggleCurve = AnimationCurve.EaseInOut(0.0f , 0.0f , 1.0f , 1.0f);
        Transform mag = pivot.Find("LKM45_mag");
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

    public void FoldStock() {
        QueueInput(FoldStockAction());
    }

    IEnumerator FoldStockAction() {
        inputFlag = true;
        audio.PlayOneShot(boltCycleClip, 0.08f);
        audio.PlayOneShot(stockClip, 1.0f);
        GetComponent<Animator>().Play("LKM45_bolt");
        ToggleStock();
        yield return new WaitForSeconds(0.8f);
        inputFlag = false;
    }

    void ToggleStock() {
        AnimationCurve toggleCurve = AnimationCurve.EaseInOut(0.0f , 0.0f , 1.0f , 1.0f);
        Transform selector = pivot.Find("LKM45_stock");
        Vector3 pos = selector.localPosition;
        if(!stockFolded) {
            StartCoroutine(CurveLerp(selector, pos, pos, selector.localRotation, Quaternion.Euler(new Vector3(0, 175, 0))*selector.localRotation, toggleCurve, 0.7f));
        } else {
            StartCoroutine(CurveLerp(selector, pos, pos, selector.localRotation, Quaternion.Euler(new Vector3(0, -175, 0))*selector.localRotation, toggleCurve, 0.7f));
        }
        stockFolded = !stockFolded;
    }

    void ToggleStockSwitch() {
        AnimationCurve toggleCurve = AnimationCurve.EaseInOut(0.0f , 0.0f , 1.0f , 1.0f);
        Transform selector = pivot.Find("LKM45_stock_switch");
        Vector3 pos = selector.localPosition;
        if(!stockSwitchDown) {
            StartCoroutine(CurveLerp(selector, pos, pos, selector.localRotation, Quaternion.Euler(new Vector3(0, 0, -100))*selector.localRotation, toggleCurve, 0.3f));
        } else {
            StartCoroutine(CurveLerp(selector, pos, pos, selector.localRotation, Quaternion.Euler(new Vector3(0, -0, 100))*selector.localRotation, toggleCurve, 0.3f));
        }
        stockSwitchDown = !stockSwitchDown;
        audio.PlayOneShot(stockSwitchClip, 0.2f);
    }

    public void DustCover() {
        QueueInput(DustCoverAction());
    }

    IEnumerator DustCoverAction() {
        inputFlag = true;
        ToggleSelector();
        ToggleDustCover();
        yield return new WaitForSeconds(1.0f);
        inputFlag = false;
    }

    void ToggleDustCover() {
        AnimationCurve toggleCurve = AnimationCurve.EaseInOut(0.0f , 0.0f , 1.0f , 1.0f);
        Transform selector = pivot.Find("LKM45_dust_cover");
        Vector3 pos = selector.localPosition;
        if(selectorUp) {
            StartCoroutine(CurveLerp(selector, pos, pos + new Vector3(0, 0.17f, 0), selector.localRotation, selector.localRotation, toggleCurve, 0.7f));
        } else {
            StartCoroutine(CurveLerp(selector, pos, pos - new Vector3(0, 0.17f, 0), selector.localRotation, selector.localRotation, toggleCurve, 0.7f));
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

    void DrawLaser() {
        Vector3 offset = 0.055f*pivot.transform.up;
        laserCore.SetVertexCount(2);
        laserBlur.SetVertexCount(2);
        laserCore.SetPosition(0, laser.transform.position + offset);
        laserCore.SetPosition(1, laser.transform.position + offset + 10*transform.Find("pivot").transform.right);
        laserBlur.SetPosition(0, laser.transform.position + offset);
        laserBlur.SetPosition(1, laser.transform.position + offset + 10*transform.Find("pivot").transform.right);
    }
}
