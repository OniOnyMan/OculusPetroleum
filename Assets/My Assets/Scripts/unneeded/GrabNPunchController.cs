using UnityEngine;
using System.Collections;

public class GrabNPunchController : MonoBehaviour
{
    private enum NodeState {
        Release, Finger, Punch
    }

    public OVRGrabber TargetGrab;
    public GameObject FingerColliders;
    public GameObject PunchColliders;
    public GameObject ReleaseColliders;

    private bool _isLeft = true;
    private bool _bamperTouched = false;
    private float _triggerForce = 0;

    void Start()
    {
        //_isLeft = TargetGrab.Controller == OVRInput.Controller.LTouch;
        //TargetGrab.OnGrab += OnGrabBegin;
    }

    private void Update()
    {
        if (_isLeft)
        {
            _bamperTouched = OVRInput.Get(OVRInput.Touch.PrimaryIndexTrigger);
            _triggerForce = OVRInput.Get(OVRInput.Axis1D.PrimaryHandTrigger);
        }
        else {
            _bamperTouched = OVRInput.Get(OVRInput.Touch.SecondaryIndexTrigger);
            _triggerForce = OVRInput.Get(OVRInput.Axis1D.SecondaryHandTrigger);
        }
        CheckNodeState();
    }

    private void OnGrabBegin(bool condition)
    {
        if (condition)
            PunchColliders.transform.parent.gameObject.SetActive(false);
        else PunchColliders.transform.parent.gameObject.SetActive(true);
    }

    private void CheckNodeState()
    {
        if (_triggerForce > 0.5)
            if (_bamperTouched)
                SetNodeState(NodeState.Punch);
            else SetNodeState(NodeState.Finger);
        else SetNodeState(NodeState.Release);
    }

    private void SetNodeState(NodeState state) {
        switch (state)
        {
            case NodeState.Release:
                {
                    FingerColliders.SetActive(false);
                    PunchColliders.SetActive(false);
                    ReleaseColliders.SetActive(true);
                }
                break;
            case NodeState.Finger:
                {
                    FingerColliders.SetActive(true);
                    PunchColliders.SetActive(false);
                    ReleaseColliders.SetActive(false);
                }
                break;
            case NodeState.Punch:
                {
                    FingerColliders.SetActive(false);
                    PunchColliders.SetActive(true);
                    ReleaseColliders.SetActive(false);
                }
                break;
        }
    }
}
