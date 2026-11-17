using UnityEngine;
using UnityEngine.XR.Hands;
using UnityEngine.XR.Management;
using UnityEngine.InputSystem;

//Shows the round button menu when the LEFT hand waves left -> right, close left <- right

public class WaveMenuController : MonoBehaviour
{
    //References
    public Transform head; //Main Camera
    public GameObject menuRoot; //button menu
    public GameObject[] panels; //InfoPanel, Calldashboard, FindCrewPanel, CrewInfoPanel

    //Menuplacement
    public float menuDistance = 0.4f;         // metres in front of the head
    public float menuDrop = 0.12f;            // metres below eye level
    public float autoHideTime = 10f;          // hide after this many seconds (0 = never)


    //left hand gesture - wave
    public float palmOutwardThreshold = 0.6f; // 1 = palm pointing exactly forward
    public float waveDistance = 0.15f;        // metres the hand must travel left -> right
    public float waveMaxTime = 0.8f;          // ...within this many seconds


    //Testing in Unity
    public Key testKey = Key.M;

    private XRHandSubsystem hands;
    private bool tracking;                    // currently measuring a possible wave
    private float startX;                     // hand's left/right position when the wave started
    private float startTime;
    private float cooldownUntil;              // ignore gestures briefly after one fires
    private float shownAt;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        menuRoot.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        // Find the hand-tracking system (it may not be ready on the first frame)
        if (hands == null)
        {
            var loader = XRGeneralSettings.Instance?.Manager?.activeLoader;
            if (loader != null) hands = loader.GetLoadedSubsystem<XRHandSubsystem>();
        }

        // Keyboard shortcut for testing without a headset
        if (Keyboard.current != null && Keyboard.current[testKey].wasPressedThisFrame)
            ToggleMenu();

        DetectWave();

        // Auto-hide
        if (menuRoot.activeSelf && autoHideTime > 0f && Time.time - shownAt > autoHideTime)
            HideMenu();
        
    }

    //for wave detection: 
    /*
    1. Is the left palm facing outward (same way the diver is looking)?
    2. While it is, did the hand move to the right by waveDistance quickly enough?
    */
    void DetectWave(){
        if (hands == null || Time.time < cooldownUntil) return;

        XRHand left = hands.leftHand;
        if (!left.isTracked || !left.GetJoint(XRHandJointID.Palm).TryGetPose(out Pose palm))
        {
            tracking = false;
            return;
        }

        // Hand poses are relative to the rig's Camera Offset (the head's parent) -> convert to world
        Transform space = head.parent;
        Vector3 palmPos = space.TransformPoint(palm.position);
        Quaternion palmRot = space.rotation * palm.rotation;

        // Palm normal points out of the palm (the hand's local -Y axis)
        Vector3 palmNormal = palmRot * Vector3.down;

        Vector3 lookFlat = head.forward;
        lookFlat.y = 0f;
        lookFlat.Normalize();

                bool palmOutward = Vector3.Dot(palmNormal, lookFlat) > palmOutwardThreshold;
        if (!palmOutward)
        {
            tracking = false;
            return;
        }

        // How far right is the hand, from the diver's point of view?
        float x = Vector3.Dot(palmPos - head.position, head.right);

        // (Re)start measuring if: just started, too slow, or the hand moved further left
        if (!tracking || Time.time - startTime > waveMaxTime || x < startX)
        {
            tracking = true;
            startX = x;
            startTime = Time.time;
            return;
        }

        // Moved far enough to the right in time -> it's a wave
        if (x - startX >= waveDistance)
        {
            tracking = false;
            cooldownUntil = Time.time + 1f;
            ToggleMenu();
        }
    }

    /* Shows the menu in front of the diver, facing them, then leaves it still. */
    public void ShowMenu(){
        Vector3 lookFlat = head.forward;
        lookFlat.y = 0f;
        lookFlat.Normalize();

        menuRoot.transform.position = head.position + lookFlat * menuDistance + Vector3.down * menuDrop;
        menuRoot.transform.rotation = Quaternion.LookRotation(lookFlat);
        menuRoot.SetActive(true);
        shownAt = Time.time;

    }

    public void HideMenu()
    {
        menuRoot.SetActive(false);
    }

    public void ToggleMenu()
    {
        if (menuRoot.activeSelf) HideMenu();
        else ShowMenu();
    }

    //open + close panel for each button
    public void OpenPanel(GameObject panel)
    {
        foreach (GameObject p in panels)
            if (p != null) p.SetActive(false);

        panel.SetActive(true);
        HideMenu();
    }

    public void CloseAllPanels()
    {
        foreach (GameObject p in panels)
            if (p != null) p.SetActive(false);
    }


}
