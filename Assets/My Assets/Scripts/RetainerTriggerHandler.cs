using System.Collections.Generic;
using UnityEngine;

public class RetainerTriggerHandler : MonoBehaviour
{
    public SVGrabbable RetainerShellGrip;

    private static RetainerTriggerHandler _instance;

    public static RetainerTriggerHandler Instance
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
        if(other.CompareTag("PipeGrabbable") && RetainerShellGrip.inHand)
            GameController.Instance.OnRetainerTriggerEnter(other);
    }
}

