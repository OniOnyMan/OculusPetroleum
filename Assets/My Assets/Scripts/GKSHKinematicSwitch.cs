using UnityEngine;
using System.Collections;
using System;

public class GKSHKinematicSwitch : MonoBehaviour
{
    private bool _previousInHand;

    private Rigidbody _gksh;
    private SVGrabbable _grabbable;
    public Action<bool> GrabChanged;
    public bool Allowed = true;

    public static GKSHKinematicSwitch Instance { get; private set; }
    private void React(bool condition) {
        _gksh.isKinematic = !condition;
    }

    private void Awake()
    {
        Instance = this;
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
        if (GrabChanged != null && _previousInHand != _grabbable.inHand && Allowed)
        {
            GrabChanged.Invoke(_grabbable.inHand);
            _previousInHand = _grabbable.inHand;
        }
    }
}
