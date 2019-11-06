using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{

    public AnimationCurve xCurve = AnimationCurve.EaseInOut(0.0f , 0.0f , 1.0f , 1.0f);
    public AnimationCurve yCurve = AnimationCurve.EaseInOut(0.0f , 0.0f , 1.0f , 1.0f);
    public AnimationCurve zCurve = AnimationCurve.EaseInOut(0.0f , 0.0f , 1.0f , 1.0f);
    
    Animator ao;
    Animation anim;
    AnimationClip clip;

    void Start() {
        ao = GetComponent<Animator>();
        anim = GetComponent<Animation>();
        clip = new AnimationClip();
        clip.legacy = true;
    }

    void Update() {
        
    }

    public void RotateUp() {
        //transform.RotateAround(transform.position, Camera.main.transform.right, 90);
        Debug.Log("up");

        transform.Rotate(new Vector3(90.0f, 0, 0), Space.World);
        Vector3 newRot = transform.eulerAngles;
        transform.Rotate(new Vector3(-90.0f, 0, 0), Space.World);

        //Quaternion.EulerRotation();

        xCurve = AnimationCurve.EaseInOut(0.0f, transform.eulerAngles.x, 1.0f, newRot.x);
        yCurve = AnimationCurve.EaseInOut(0.0f, transform.eulerAngles.y, 1.0f, newRot.y);
        zCurve = AnimationCurve.EaseInOut(0.0f, transform.eulerAngles.z, 1.0f, newRot.z);

        clip.ClearCurves();
        clip.SetCurve("", typeof(Transform), "localRotation.x", xCurve);
        clip.SetCurve("", typeof(Transform), "localRotation.y", yCurve);
        clip.SetCurve("", typeof(Transform), "localRotation.z", zCurve);

        anim.AddClip(clip, "up");
        anim.Play("up");

        // get local rot x,y,z
        // set curve inits with them
        // transform from button to get new rots
        // set end curve values from them
        // reverse if wrapping around 360 (maybe?)
        // set clip
        // let it rip

    }
    
    public void RotateDown() {
        Debug.Log("down");
        transform.RotateAround(transform.position, Camera.main.transform.right, -90);
    }
    
    public void RotateLeft() {
        Debug.Log("left");
        transform.RotateAround(transform.position, Camera.main.transform.up, 90);
    }
    
    public void RotateRight() {
        Debug.Log("right");
        transform.RotateAround(transform.position, Camera.main.transform.up, -90);
    }
}
