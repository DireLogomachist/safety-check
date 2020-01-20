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
    float arrowYOrigin;
    float menuOffset = 67;
    float[] mainOffsets = new float[3];
    float[] levelOffsets = new float[4];
    float[] creditsOffsets = new float[1];
    enum MenuLevel {main, level, credits};
    MenuLevel currentMenu = MenuLevel.main;
    
    AudioSource audio;
    AudioClip menuBlip;
    AudioClip menuSelect;

    void Start() {
        arrowLeft = GameObject.Find("UICanvas").transform.Find("Arrows").transform.Find("ArrowLeft").gameObject;
        arrowRight= GameObject.Find("UICanvas").transform.Find("Arrows").transform.Find("ArrowRight").gameObject;
        arrowLeftOrigin = arrowLeft.GetComponent<RectTransform>().anchoredPosition.x;
        arrowRightOrigin = arrowRight.GetComponent<RectTransform>().anchoredPosition.x;
        arrowYOrigin = arrowRight.GetComponent<RectTransform>().anchoredPosition.y;

        audio = GetComponent<AudioSource>();
        menuBlip = (AudioClip) Resources.Load("audio/ui_menu_blip");
        menuSelect = (AudioClip) Resources.Load("audio/ui_menu_blip");

        mainOffsets[0] = 60;
        mainOffsets[1] = 0;
        mainOffsets[2] = 50;

        levelOffsets[0] = 70;
        levelOffsets[1] = 50;
        levelOffsets[2] = 70;
        levelOffsets[3] = 70;

        creditsOffsets[0] = 500;
    }

    void Update() {
        if(currentMenu == MenuLevel.main) {
            float left = arrowLeftOrigin + mainOffsets[index] + 8*Mathf.Sin(Time.time*arrowSpeed);
            float right = arrowRightOrigin - mainOffsets[index] - 8*Mathf.Sin(Time.time*arrowSpeed);
            arrowLeft.GetComponent<RectTransform>().anchoredPosition = new Vector2(left,arrowYOrigin + index*-menuOffset);
            arrowRight.GetComponent<RectTransform>().anchoredPosition = new Vector2(right,arrowYOrigin + index*-menuOffset);

            if(Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown("s")) {
                audio.PlayOneShot(menuBlip);
                index = index + 1;
                if(index > mainOffsets.Length-1) index = 0;
            } else if(Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown("w")) {
                audio.PlayOneShot(menuBlip);
                index = index - 1;
                if(index < 0) index = 2;
            } else if(Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.KeypadEnter)) {
                if(index == 0) {
                    //Load first level
                } else if(index == 1) {
                    currentMenu = MenuLevel.level;
                    GameObject.Find("UICanvas").transform.Find("MainMenu").gameObject.active = false;
                    GameObject.Find("UICanvas").transform.Find("LevelMenu").gameObject.active = true;
                } else if(index == 2) {
                    currentMenu = MenuLevel.credits;
                    GameObject.Find("UICanvas").transform.Find("MainMenu").gameObject.active = false;
                    GameObject.Find("UICanvas").transform.Find("CreditsMenu").gameObject.active = true;
                }
                index = 0;
            }
        } else if(currentMenu == MenuLevel.level) {
            float left = arrowLeftOrigin + levelOffsets[index] + 8*Mathf.Sin(Time.time*arrowSpeed);
            float right = arrowRightOrigin - levelOffsets[index] - 8*Mathf.Sin(Time.time*arrowSpeed);
            arrowLeft.GetComponent<RectTransform>().anchoredPosition = new Vector2(left,arrowYOrigin + index*-menuOffset);
            arrowRight.GetComponent<RectTransform>().anchoredPosition = new Vector2(right,arrowYOrigin + index*-menuOffset);

            if(Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown("s")) {
                audio.PlayOneShot(menuBlip);
                index = index + 1;
                if(index > levelOffsets.Length-1) index = 0;
            } else if(Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown("w")) {
                audio.PlayOneShot(menuBlip);
                index = index - 1;
                if(index < 0) index = 2;
            }
        } else if(currentMenu == MenuLevel.credits) {
            float left = arrowLeftOrigin + creditsOffsets[index] + 8*Mathf.Sin(Time.time*arrowSpeed);
            float right = arrowRightOrigin - creditsOffsets[index] - 8*Mathf.Sin(Time.time*arrowSpeed);
            arrowLeft.GetComponent<RectTransform>().anchoredPosition = new Vector2(left,arrowYOrigin + index*-menuOffset);
            arrowRight.GetComponent<RectTransform>().anchoredPosition = new Vector2(right,arrowYOrigin + index*-menuOffset);

            if(Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown("s")) {
                audio.PlayOneShot(menuBlip);
                index = index + 1;
                if(index > creditsOffsets.Length-1) index = 0;
            } else if(Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown("w")) {
                audio.PlayOneShot(menuBlip);
                index = index - 1;
                if(index < 0) index = 2;
            }
        }
    }
}
