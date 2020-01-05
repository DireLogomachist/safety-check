using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class MK68Controller : WeaponController {

    public override void Start() {
        base.Start();
        ActionTrigger[] triggers = GetComponentsInChildren<ActionTrigger>();
        //triggers[0].function.AddListener();       // Pin
        //triggers[1].function.AddListener();       // Button
        //triggers[2].function.AddListener();       // Key
        //triggers[3].function.AddListener();       // Wire
    }

    public override void Update() {
        base.Update();
    }

    public void Fire() {
        QueueInput(FireAction());
    }

    IEnumerator FireAction() {
        inputFlag = true;
        yield return new WaitForSeconds(0.5f);
        inputFlag = false;
    }
}
