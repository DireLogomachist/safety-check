using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;

public class MK68Controller : WeaponController {

    public float timer = 20.0f;
    TextMeshPro timerText;

    bool antennaUp = false;
    bool pinPulled = false;

    bool countdownStarted = false;
    float countdownSpeed = 1.0f;

    public override void Start() {
        base.Start();
        ActionTrigger[] triggers = GetComponentsInChildren<ActionTrigger>();
        triggers[0].function.AddListener(Pin);      // Pin
        triggers[1].function.AddListener(Button);   // Button
        triggers[2].function.AddListener(Key);      // Key
        triggers[3].function.AddListener(Wire);     // Wire

        timerText = pivot.Find("Timer").GetComponent<TextMeshPro>();
    }

    public override void Update() {
        base.Update();

        if(countdownStarted) {
            timer -= Time.deltaTime*countdownSpeed;
            timerText.text = Mathf.CeilToInt(timer).ToString("00");

            if(timer <= 0.0f) {
                countdownStarted = false;
                timerText.text = "00";
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
        inputFlag = false;
    }

    public void Fire() {
        QueueInput(FireAction());
    }

    IEnumerator FireAction() {
        inputFlag = true;
        Debug.Log("BOOM");
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
