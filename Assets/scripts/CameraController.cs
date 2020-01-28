using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CameraController : MonoBehaviour {

    public AnimationCurve shakeCurve;

    float shakeIndex = 0.0f;
    float shakeTime = 0.5f;
    float shakeFallspeed = 0.4f;
    Vector3 shakeOffset;
	Vector3 shakeRot;
	float shakeMaxRot = 0.4f;
	float shakeMaxOffset = 0.1f;

    AudioSource audio;
	
    
    void Start() {
        audio = GetComponent<AudioSource>();
        StartCoroutine(MusicStart());
    }

    void Update() {
        if(Input.GetKeyDown("f")) {
            Screen.fullScreen = !Screen.fullScreen;
        }
    }

    public IEnumerator CameraShake() {
        float elapsed = 0.0f;
        while(elapsed < shakeTime) {
            elapsed += Time.deltaTime;

            shakeIndex = shakeCurve.Evaluate(elapsed/shakeTime);

            float yaw = shakeMaxRot*shakeIndex*shakeIndex*Random.Range(-1f,1f);
			float pitch = shakeMaxRot*shakeIndex*shakeIndex*Random.Range(-1f,1f);
			float roll = shakeMaxRot*shakeIndex*shakeIndex*Random.Range(-1f,1f);
			float offsetX = shakeMaxOffset*shakeIndex*shakeIndex*Random.Range(-1f,1f);
			float offsetY = shakeMaxOffset*shakeIndex*shakeIndex*Random.Range(-1f,1f);
			float offsetZ = shakeMaxOffset*shakeIndex*shakeIndex*Random.Range(-1f,1f);

            transform.position = transform.position - shakeOffset;
		    transform.eulerAngles = transform.eulerAngles - shakeRot;
			transform.position = transform.position + new Vector3(offsetX, offsetY, offsetZ);
			transform.eulerAngles = transform.eulerAngles + new Vector3(yaw, pitch, roll);

			shakeOffset = new Vector3(offsetX,offsetY,offsetZ);
			shakeRot = new Vector3(yaw,pitch,roll);

            yield return null;

            transform.position = transform.position - shakeOffset;
		    transform.eulerAngles = transform.eulerAngles - shakeRot;
            shakeOffset = new Vector3(0,0,0);
			shakeRot = new Vector3(0,0,0);
        }
    }

    IEnumerator MusicStart() {
        yield return new WaitForSeconds(0.7f);
        audio.Play();
    }

    public IEnumerator MusicFadeOut() {
        float endTime = 2.0f;
        float elapsed = 0.0f;
        float maxVol = audio.volume;
        while(elapsed < endTime) {
            elapsed += Time.deltaTime;
            audio.volume = Mathf.Lerp(maxVol, 0, elapsed/endTime);
            yield return null;
        }
    }

    public IEnumerator MusicTempMute() {
        float maxVol = audio.volume;
        audio.volume = 0;
        yield return new WaitForSeconds(5.0f);
        
        float endTime = 3.0f;
        float elapsed = 0.0f;
        while(elapsed < endTime) {
            elapsed += Time.deltaTime;
            audio.volume = Mathf.Lerp(0, maxVol, elapsed/endTime);
            yield return null;
        }
    }

    public void MusicMute() {
        audio.volume = 0;
    }
}
