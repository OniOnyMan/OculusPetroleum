using System.Collections.Generic;
using UnityEngine;

public class ElevatorTriggerHandler : MonoBehaviour
{
    public SVGrabbable ElevatorShellGrip;

    private static ElevatorTriggerHandler _instance;

    public static ElevatorTriggerHandler Instance
    {
        get
        {
            return _instance;
        }
    }

    private void Start() {
        _instance = this;
    }

    private void OnTriggerEnter(Collider other) {
        if(other.CompareTag("PipeGrabbable") && ElevatorShellGrip.inHand)
            GameController.Instance.OnElevatorrTriggerEnter(other);
    }
}

