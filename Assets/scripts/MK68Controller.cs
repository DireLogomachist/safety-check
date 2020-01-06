using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;

public class MK68Controller : WeaponController {

    public float timer = 20.0f;
    TextMeshPro timerText;

    bool countdownStarted = true;
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
        Debug.Log("Pulling pin");
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
        inputFlag = false;
    }

    public void Button() {
        QueueInput(ButtonAction());
    }

    IEnumerator ButtonAction() {
        inputFlag = true;
        Debug.Log("Button pressed");
        GetComponent<Animator>().Play("MK68_button");
        yield return new WaitForSeconds(0.5f);
        inputFlag = false;
    }

    public void Key() {
        QueueInput(KeyAction());
    }

    IEnumerator KeyAction() {
        inputFlag = true;
        Debug.Log("Key turned");
        GetComponent<Animator>().Play("MK68_key");
        yield return new WaitForSeconds(0.5f);
        inputFlag = false;
    }

    public void Wire() {
        QueueInput(WireAction());
    }

    IEnumerator WireAction() {
        inputFlag = true;
        Debug.Log("Wire cut");
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
}
