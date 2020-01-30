using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIController : MonoBehaviour {

    public Animator transition;
    string sceneName;
    AudioSource audio;
    AudioClip levelWinClip;

    void Start() {
        sceneName = SceneManager.GetActiveScene().name;
        if(sceneName != "Title" && sceneName != "End") {
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

        audio = Camera.main.GetComponent<AudioSource>();
        levelWinClip = (AudioClip) Resources.Load("audio/level_win");
        
        if(sceneName == "End") {
            transform.Find("EndCard").gameObject.GetComponent<CanvasGroup>().alpha = 0.0f;
            StartCoroutine(Ending());
        }
    }

    void Update() {
        if (Time.timeSinceLevelLoad > 7.0f && Input.anyKeyDown && sceneName == "End" && !Input.GetKeyDown("f")) {
            if(!Input.GetKey(KeyCode.Mouse0) && !Input.GetKey(KeyCode.Mouse1))
                StartCoroutine(TitleReturn());
        }
    }

    IEnumerator Ending() {
        yield return new WaitForSeconds(3.0f);
        GameObject.Find("Confetti").GetComponent<ParticleSystem>().Play();
        audio.PlayOneShot(levelWinClip, 1.2f);
        yield return new WaitForSeconds(2.0f);
        StartCoroutine(FadeInMessage());
    }

    IEnumerator FadeInMessage() {
        GameObject message = transform.Find("EndCard").gameObject;
        float elapsed = 0.0f;
        float time = 1.0f;
        while(elapsed < time) {
            elapsed += Time.deltaTime;
            message.GetComponent<CanvasGroup>().alpha = elapsed/time;
            yield return null;
        }
        message.GetComponent<CanvasGroup>().alpha = 1.0f;
    }

    IEnumerator TitleReturn() {
        StartCoroutine(Camera.main.GetComponent<CameraController>().MusicFadeOut());
        transition.SetTrigger("Transition");
        yield return new WaitForSeconds(3.0f);
        SceneManager.LoadScene(0);
    }
}
