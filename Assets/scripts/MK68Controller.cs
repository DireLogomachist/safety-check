using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class MK68Controller : WeaponController {

    public override void Start() {
        base.Start();
        ActionTrigger[] triggers = GetComponentsInChildren<ActionTrigger>();
        triggers[0].function.AddListener(Pin);      // Pin
        triggers[1].function.AddListener(Button);   // Button
        triggers[2].function.AddListener(Key);      // Key
        triggers[3].function.AddListener(Wire);     // Wire
    }

    public override void Update() {
        base.Update();
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
        yield return new WaitForSeconds(3.0f);

        GetComponent<Animator>().Play("MK68_shell");
        yield return new WaitForSeconds(4.0f);

        GameObject.Destroy(pivot.Find("MK68_pin").gameObject);
        GameObject.Destroy(pivot.Find("MK68_shell").gameObject);
        GameObject.Destroy(GameObject.Find("MK68_spoon").gameObject);
        inputFlag = false;
    }

    public void Button() {
        QueueInput(ButtonAction());
    }

    IEnumerator ButtonAction() {
        Debug.Log("Button pressed");
        yield break;
    }

    public void Key() {
        QueueInput(KeyAction());
    }

    IEnumerator KeyAction() {
        Debug.Log("Key turned");
        GetComponent<Animator>().Play("MK68_key");
        yield break;
    }

    public void Wire() {
        QueueInput(WireAction());
    }

    IEnumerator WireAction() {
        Debug.Log("Wire cut");
        pivot.Find("MK68_wire_red").gameObject.active = false;
        pivot.Find("MK68_wire_trigger").gameObject.active = false;
        pivot.Find("MK68_wire_red_cut").gameObject.active = true;
        pivot.Find("MK68_wire_trigger_cut").gameObject.active = true;
        yield break;
    }

    public void Fire() {
        QueueInput(FireAction());
    }

    IEnumerator FireAction() {
        inputFlag = true;
        yield return new WaitForSeconds(0.5f);
        inputFlag = false;
    }

    IEnumerator LaunchSpoon() {
        GameObject spoon = pivot.Find("MK68_spoon").gameObject;
        yield return new WaitForSeconds(1.0f);
        spoon.transform.parent = null;
        spoon.GetComponent<Rigidbody>().isKinematic = false;
        spoon.GetComponent<Rigidbody>().AddForceAtPosition(new Vector3(0,15,0), new Vector3(0,-1,0), ForceMode.Impulse);
    }
}
