using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

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
    float[] creditsOffsets = new float[3];
    enum MenuLevel {main, level, credits};
    MenuLevel currentMenu = MenuLevel.main;
    
    AudioSource audio;
    AudioClip menuBlip;
    AudioClip menuConfirm;
    Transform canvas;

    void Start() {
        canvas = GameObject.Find("UICanvas").transform;
        arrowLeft = canvas.Find("Arrows").transform.Find("ArrowLeft").gameObject;
        arrowRight= canvas.Find("Arrows").transform.Find("ArrowRight").gameObject;
        arrowLeftOrigin = arrowLeft.GetComponent<RectTransform>().anchoredPosition.x;
        arrowRightOrigin = arrowRight.GetComponent<RectTransform>().anchoredPosition.x;
        arrowYOrigin = arrowRight.GetComponent<RectTransform>().anchoredPosition.y;

        audio = GetComponent<AudioSource>();
        menuBlip = (AudioClip) Resources.Load("audio/ui_menu_blip");
        menuConfirm = (AudioClip) Resources.Load("audio/ui_menu_confirm");

        mainOffsets[0] = 60;
        mainOffsets[1] = 0;
        mainOffsets[2] = 50;
        levelOffsets[0] = 70;
        levelOffsets[1] = 50;
        levelOffsets[2] = 70;
        levelOffsets[3] = 70;
        creditsOffsets[0] = -70;
        creditsOffsets[1] = -70;
        creditsOffsets[2] = 70;
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
                if(index < 0) index = mainOffsets.Length-1;
            } else if(Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return)) {
                if(index == 0) {
                    //Load first level
                    audio.PlayOneShot(menuConfirm);
                    StartCoroutine(LevelLoad(1));
                } else if(index == 1) {
                    audio.PlayOneShot(menuBlip);
                    currentMenu = MenuLevel.level;
                    canvas.Find("MainMenu").gameObject.active = false;
                    canvas.Find("LevelMenu").gameObject.active = true;
                } else if(index == 2) {
                    audio.PlayOneShot(menuBlip);
                    currentMenu = MenuLevel.credits;
                    canvas.Find("MainMenu").gameObject.active = false;
                    canvas.Find("CreditsMenu").gameObject.active = true;
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
                if(index < 0) index = levelOffsets.Length-1;
            } else if(Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return)) {
                if(index == levelOffsets.Length-1) {
                    audio.PlayOneShot(menuBlip);
                    currentMenu = MenuLevel.main;
                    canvas.Find("LevelMenu").gameObject.active = false;
                    canvas.Find("MainMenu").gameObject.active = true;
                    index = 0;
                } else {
                    //Load selected level
                    audio.PlayOneShot(menuConfirm);
                    StartCoroutine(LevelLoad(index+1));
                }
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
                if(index < 0) index = creditsOffsets.Length-1;
            } else if(Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return)) {
                audio.PlayOneShot(menuBlip);
                if(index == creditsOffsets.Length-1) {
                    currentMenu = MenuLevel.main;
                    canvas.Find("CreditsMenu").gameObject.active = false;
                    canvas.Find("MainMenu").gameObject.active = true;
                    index = 0;
                }
            }
        }
    }

    IEnumerator LevelLoad(int index) {
        canvas.GetComponent<UIController>().transition.SetTrigger("Transition");
        yield return new WaitForSeconds(3.0f);
        SceneManager.LoadScene(index);
    }
}
