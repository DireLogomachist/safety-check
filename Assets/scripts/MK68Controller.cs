using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class MK68Controller : WeaponController {

    public float timer = 30.0f;
    TextMeshPro timerText;

    bool antennaUp = false;
    bool pinPulled = false;
    bool switchOn = false;

    bool countdownStarted = false;
    float countdownSpeed = 1.0f;
    float beepTimer = 1.0f;

    AudioClip countdownBeep1;
    AudioClip countdownBeep2;
    AudioClip countdownBeepEnd;

    public override void Start() {
        base.Start();
        ActionTrigger[] triggers = GetComponentsInChildren<ActionTrigger>();
        triggers[0].function.AddListener(Pin);      // Pin
        triggers[1].function.AddListener(Button);   // Button
        triggers[2].function.AddListener(Key);      // Key
        triggers[3].function.AddListener(Wire);     // Wire
        triggers[4].function.AddListener(Switch);   // Switch

        timerText = pivot.Find("Timer").GetComponent<TextMeshPro>();
        timerText.text = Mathf.CeilToInt(timer).ToString("00");
        countdownBeep1 = (AudioClip) Resources.Load("audio/countdown_beep_1");
        countdownBeep2 = (AudioClip) Resources.Load("audio/countdown_beep_2");
        countdownBeepEnd = (AudioClip) Resources.Load("audio/countdown_beep_end");
    }

    public override void Update() {
        base.Update();

        if(countdownStarted) {
            timer -= Time.deltaTime*countdownSpeed;
            timerText.text = Mathf.CeilToInt(timer).ToString("00");

            if(Time.time > beepTimer && timer > 0.0f) {
                if(countdownSpeed <= 1.0f)audio.PlayOneShot(countdownBeep1);
                else audio.PlayOneShot(countdownBeep2);
                beepTimer += 1.0f/countdownSpeed;
            }

            if(timer <= 0.0f) {
                countdownStarted = false;
                timerText.text = "00";
                audio.PlayOneShot(countdownBeepEnd);
                Fire();
            }
        }
    }

    public void Pin() {
        QueueInput(PinAction());
    }

    IEnumerator PinAction() {
        inputFlag = true;
        if(!pinPulled) {
            pinPulled = true;
            GetComponent<Animator>().Play("MK68_pin");
            yield return new WaitForSeconds(1.0f);

            GameObject spoon = pivot.Find("MK68_spoon").gameObject;
            spoon.transform.parent = null;
            spoon.GetComponent<Rigidbody>().isKinematic = false;
            spoon.GetComponent<Rigidbody>().AddForceAtPosition(new Vector3(0,15,0), new Vector3(0,-1,0), ForceMode.Impulse);
            yield return new WaitForSeconds(2.0f);

            GetComponent<Animator>().Play("MK68_shell");
            yield return new WaitForSeconds(4.0f);

            pivot.Find("MK68_pin").gameObject.active = false;
            pivot.Find("MK68_shell").gameObject.active = false;
            GameObject.Find("MK68_spoon").gameObject.active = false;
            beepTimer = Time.time + 1.0f;
            countdownStarted = true;
        }
        inputFlag = false;
    }

    public void Button() {
        QueueInput(ButtonAction());
    }

    IEnumerator ButtonAction() {
        inputFlag = true;
        GetComponent<Animator>().Play("MK68_button");
        if(!antennaUp) ExtendAntenna(); else RetractAntenna();
        yield return new WaitForSeconds(1.0f);
        inputFlag = false;
    }

    public void Key() {
        QueueInput(KeyAction());
    }

    IEnumerator KeyAction() {
        inputFlag = true;
        GetComponent<Animator>().Play("MK68_key");
        countdownSpeed = 1.5f;
        yield return new WaitForSeconds(0.5f);
        inputFlag = false;
    }

    public void Wire() {
        QueueInput(WireAction());
    }

    IEnumerator WireAction() {
        inputFlag = true;
        pivot.Find("MK68_wire_red").gameObject.active = false;
        pivot.Find("MK68_wire_trigger").gameObject.active = false;
        pivot.Find("MK68_wire_red_cut").gameObject.active = true;
        pivot.Find("MK68_wire_trigger_cut").gameObject.active = true;
        yield return new WaitForSeconds(0.5f);
        canvas.GetComponent<UIController>().transition.SetTrigger("Splatter");
        yield return new WaitForSeconds(2.0f);
        canvas.GetComponent<UIController>().transition.SetTrigger("Transition");
        yield return new WaitForSeconds(3.0f);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        inputFlag = false;
    }

    public void Switch() {
        QueueInput(SwitchAction());
    }

    IEnumerator SwitchAction() {
        inputFlag = true;
        //AnimationCurve curve = AnimationCurve.EaseInOut(0.0f , 0.0f , 1.0f , 1.0f);
        Transform swtch = pivot.Find("MK68_switch");
        Vector3 pos = swtch.localPosition;
        if(!switchOn) StartCoroutine(CurveLerp(swtch, pos, pos, swtch.localRotation, Quaternion.Euler(10,0,0), curve, 0.5f));
        else StartCoroutine(CurveLerp(swtch, pos, pos, swtch.localRotation, Quaternion.Euler(-10,0,0), curve, 0.5f));
        switchOn = !switchOn;
        yield return new WaitForSeconds(0.5f);
        inputFlag = false;
    }

    public void Fire() {
        QueueInput(FireAction());
    }

    IEnumerator FireAction() {
        inputFlag = true;
        Debug.Log("BOOM");
        StartCoroutine(Camera.main.GetComponent<CameraController>().CameraShake());
        yield return new WaitForSeconds(0.5f);
        inputFlag = false;
    }

    void ExtendAntenna() {
        antennaUp = true;
        AnimationCurve curve = AnimationCurve.EaseInOut(0.0f , 0.0f , 1.0f , 1.0f);
        float curveTime = 0.6f;
        Transform antennaHead = pivot.Find("MK68_antenna_head");
        Transform antennaWire = pivot.Find("MK68_antenna_wire");
        Vector3 headPos = antennaHead.localPosition;
        Quaternion headRot = antennaHead.localRotation;
        StartCoroutine(CurveLerp(antennaHead, headPos, headPos + new Vector3(0,0.7f,0), headRot, headRot, curve, curveTime));
        StartCoroutine(CurveScaleLerp(antennaWire, antennaWire.localScale, new Vector3(1,13,1), curve, curveTime));
    }

    void RetractAntenna() {
        antennaUp = false;
        AnimationCurve curve = AnimationCurve.EaseInOut(0.0f , 0.0f , 1.0f , 1.0f);
        float curveTime = 0.6f;
        Transform antennaHead = pivot.Find("MK68_antenna_head");
        Transform antennaWire = pivot.Find("MK68_antenna_wire");
        Vector3 headPos = antennaHead.localPosition;
        Quaternion headRot = antennaHead.localRotation;
        StartCoroutine(CurveLerp(antennaHead, headPos, new Vector3(0,0,0), headRot, headRot, curve, curveTime));
        StartCoroutine(CurveScaleLerp(antennaWire, antennaWire.localScale, new Vector3(1,1,1), curve, curveTime));
    }
}
