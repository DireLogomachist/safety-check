using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopupController : MonoBehaviour {

    public GameObject SafePopup;
    public GameObject MagRemovalPopup;
    public GameObject EjectionPopup;
    public GameObject MisfirePopup;

    Queue<GameObject> popupBuffer = new Queue<GameObject>();

    void Start() {
        
    }

    void Update() {
        
    }

    public void Spawn() {
        GameObject popup = GameObject.Instantiate(SafePopup);
        popup.transform.parent = transform;
        popup.transform.GetComponent<RectTransform>().anchoredPosition = new Vector2(0,-70*popupBuffer.Count);
        popupBuffer.Enqueue(popup);
    }
}
