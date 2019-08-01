using UnityEngine;
using System.Collections;
using System;

public class GKSHKinematicSwitch : MonoBehaviour
{
    private bool _previousInHand;

    private Rigidbody _gksh;
    private SVGrabbable _grabbable;
    public Action<bool> GrabChanged;

    private void React(bool condition) {
        _gksh.isKinematic = !condition;
    }

    // Use this for initialization
    void Start()
    {
        _gksh = GameObject.Find("GKSH").GetComponent<Rigidbody>();
        _grabbable = GetComponent<SVGrabbable>();
        GrabChanged += React;
        _previousInHand = _grabbable.inHand;
    }

    // Update is called once per frame
    void Update()
    {
        if (GrabChanged != null && _previousInHand != _grabbable.inHand)
        {
            GrabChanged.Invoke(_grabbable.inHand);
            _previousInHand = _grabbable.inHand;
        }
    }
}
