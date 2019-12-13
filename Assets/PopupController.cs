using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PopupController : MonoBehaviour {

    public GameObject SafePopup;
    public GameObject MagRemovalPopup;
    public GameObject EjectionPopup;
    public GameObject MisfirePopup;
    public AnimationCurve popupCurve = AnimationCurve.EaseInOut(0.0f , 0.0f , 1.0f , 1.0f);

    Queue<BufferPair> popupBuffer = new Queue<BufferPair>();
    GameObject newestPopup;
    float popupDuration = 2.0f;
    bool movingPopups = false;

    void Start() {
    }

    void Update() {
        if(!movingPopups && popupBuffer.Count > 0 && ((Time.time - popupBuffer.Peek().timestamp) > popupDuration)) {
            movingPopups = true;
            GameObject popup = popupBuffer.Dequeue().obj;
            if(popupBuffer.Count == 0)
                newestPopup = null;
            StartCoroutine(FadeDestroyShift(popup));
        }
    }

    public void Spawn(float msg) {
        GameObject popup;
        if(msg == 1) {
            popup = GameObject.Instantiate(SafePopup);
        } else if (msg == 2) {
            popup = GameObject.Instantiate(MagRemovalPopup);
        } else if (msg == 3) {
            popup = GameObject.Instantiate(EjectionPopup);
        } else {
            popup = GameObject.Instantiate(MisfirePopup);
        }
        popup.transform.parent = transform;
        if(newestPopup == null)
            popup.GetComponent<RectTransform>().anchoredPosition = new Vector2(0,0);
        else
            popup.GetComponent<RectTransform>().anchoredPosition = new Vector2(0,newestPopup.GetComponent<RectTransform>().anchoredPosition.y-70);

        popupBuffer.Enqueue(new BufferPair(popup, Time.time));
        newestPopup = popup;
    }

    IEnumerator FadeDestroyShift(GameObject obj) {
        movingPopups = true;
        bool empty = popupBuffer.Count > 0 ? false : true;

        float elapsed = 0.0f;
        while(elapsed < 0.5f) {
            elapsed += Time.deltaTime;
            Color c = obj.GetComponent<Image>().color;
            c.a = Mathf.Lerp(1, 0, elapsed/0.5f);
            obj.GetComponent<Image>().color = c;
            obj.transform.GetComponentInChildren<Text>().color = c;
            yield return null;
        }
        GameObject.Destroy(obj);

        elapsed = 0.0f;
        List<BufferPair> buffer = new List<BufferPair>(popupBuffer.ToArray());
        List<float> yOrigins = new List<float>();
        for(int i=0; i < popupBuffer.Count; i++) {
            yOrigins.Add(buffer[i].obj.GetComponent<RectTransform>().anchoredPosition.y);
        }
        
        while(!empty && elapsed < 0.6f) {
            elapsed += Time.deltaTime;
            if((popupBuffer.Count != buffer.Count)) {
                int oldCount = buffer.Count;
                buffer = new List<BufferPair>(popupBuffer.ToArray());
                for(int i=0; i < (popupBuffer.Count - oldCount); i++) {
                    yOrigins.Add(buffer[oldCount + i].obj.GetComponent<RectTransform>().anchoredPosition.y);
                }
            }
            for(int i=0; i < popupBuffer.Count; i++) {
                RectTransform rect = buffer[i].obj.GetComponent<RectTransform>();
                rect.anchoredPosition = new Vector2(0, yOrigins[i] + Mathf.Lerp(0, 70, popupCurve.Evaluate(elapsed/0.6f)));
            }
            yield return null;
        }
        movingPopups = false;
    }
}

public class BufferPair {
    public GameObject obj;
    public float timestamp;

    public BufferPair(GameObject o, float t) {
        obj = o;
        timestamp = t;
    }
}
