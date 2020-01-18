using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuController : MonoBehaviour {

    public float arrowSpeed = 3.0f;

    int index = 0;
    GameObject arrowLeft;
    GameObject arrowRight;
    float arrowLeftOrigin;
    float arrowRightOrigin;
    
    AudioSource audio;
    AudioClip menuBlip;
    AudioClip menuSelect;

    void Start() {
        arrowLeft = GameObject.Find("UICanvas").transform.Find("Menu").transform.Find("ArrowLeft").gameObject;
        arrowRight= GameObject.Find("UICanvas").transform.Find("Menu").transform.Find("ArrowRight").gameObject;
        arrowLeftOrigin = arrowLeft.GetComponent<RectTransform>().anchoredPosition.x;
        arrowRightOrigin = arrowRight.GetComponent<RectTransform>().anchoredPosition.x;

        audio = GetComponent<AudioSource>();
        menuBlip = (AudioClip) Resources.Load("audio/ui_menu_blip");
        menuSelect = (AudioClip) Resources.Load("audio/ui_menu_blip");
    }

    void Update() {
        float left = arrowLeftOrigin + 50*Mathf.Sin(Time.time*arrowSpeed);
        float right = arrowRightOrigin - 50*Mathf.Sin(Time.time*arrowSpeed);
        arrowLeft.GetComponent<RectTransform>().anchoredPosition = new Vector2(left,index*-600);
        arrowRight.GetComponent<RectTransform>().anchoredPosition = new Vector2(right,index*-600);

        if(Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown("s")) {
            audio.PlayOneShot(menuBlip);
            index = index + 1;
            if(index > 2) index = 0;
        } else if(Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown("w")) {
            audio.PlayOneShot(menuBlip);
            index = index - 1;
            if(index < 0) index = 2;
        }
    }
}
