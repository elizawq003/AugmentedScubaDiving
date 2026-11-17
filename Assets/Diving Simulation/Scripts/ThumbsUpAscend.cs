using UnityEngine;
using UnityEngine.XR.Hands;
using UnityEngine.XR.Hands.Gestures;
using UnityEngine.InputSystem;

//While the RIGHT hand holds a thumbs-up, the diver rises toward the surface.
// Stops once the diver reaches the surface. Hold U to test in the editor.
public class ThumbsUpAscend : MonoBehaviour
{
    public XRHandTrackingEvents rightHand;   // the XR Hand Tracking Events set to Right
    public XRHandPose thumbsUpPose;          // Samples/XR Hands/1.7.0/Gestures/Examples/Hand Poses/Thumbs Up
    public Transform rig;                    // XR Origin (XR Rig)
    public Transform head;                   // Main Camera
    public InformationManager iM;            // Information Manager (to know where the surface is)

    public float ascendSpeed = 1.0f;         // metres per second
    public Key testKey = Key.U;              // hold to rise in the editor

    private bool thumbsUp;

    void OnEnable()
    {
        if (thumbsUpPose != null && thumbsUpPose.relativeOrientation != null)
            thumbsUpPose.relativeOrientation.targetTransform = head;

        rightHand.jointsUpdated.AddListener(OnJointsUpdated);
    }

    void OnDisable()
    {
        rightHand.jointsUpdated.RemoveListener(OnJointsUpdated);
    }


    // Called every time the right hand's joints update: is it a thumbs-up?
    void OnJointsUpdated(XRHandJointsUpdatedEventArgs args)
    {
        thumbsUp = thumbsUpPose.CheckConditions(args);
    }


    void Update()
    {
        bool gesture = thumbsUp && rightHand.handIsTracked;
        bool testKeyHeld = Keyboard.current != null && Keyboard.current[testKey].isPressed;

        if (!gesture && !testKeyHeld) return;
        if (iM.GetDepth() >= 0f) return;     // already at the surface

        rig.position += Vector3.up * ascendSpeed * Time.deltaTime;
    }
}
