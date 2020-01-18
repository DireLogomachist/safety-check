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
	
    
    void Start() {
    }

    void Update() {
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
}
