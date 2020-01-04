﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour {
    void Start() {
        WeaponController weapon = FindObjectsOfType<WeaponController>()[0];

        transform.GetChild(0).GetComponent<Button>().onClick = new Button.ButtonClickedEvent();
        transform.GetChild(1).GetComponent<Button>().onClick = new Button.ButtonClickedEvent();
        transform.GetChild(2).GetComponent<Button>().onClick = new Button.ButtonClickedEvent();
        transform.GetChild(3).GetComponent<Button>().onClick = new Button.ButtonClickedEvent();
        transform.GetChild(0).GetComponent<Button>().onClick.AddListener(weapon.RotateUp);
        transform.GetChild(1).GetComponent<Button>().onClick.AddListener(weapon.RotateDown);
        transform.GetChild(2).GetComponent<Button>().onClick.AddListener(weapon.RotateLeft);
        transform.GetChild(3).GetComponent<Button>().onClick.AddListener(weapon.RotateRight);
    }
}
