using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Transitioner : MonoBehaviour {

    public float TextureScale = 2.0f;
    CanvasRenderer renderer;
    Image image;
    
    void Start() {
        renderer = gameObject.GetComponent<CanvasRenderer>();
        image = gameObject.GetComponent<Image>();
        image.material = Instantiate(image.material);
        image.material.SetFloat("_TexScale", 0.0f);
        image.material.SetColor("_Color", image.material.GetColor("_Color")+new Color(0,0,0,1));
    }

    void Update() {
        image.material.SetFloat("_TexScale", TextureScale);
    }
}
