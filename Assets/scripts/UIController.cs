using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIController : MonoBehaviour {

    public Animator transition;

    void Start() {
        if(SceneManager.GetActiveScene().name != "Title") {
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
}
