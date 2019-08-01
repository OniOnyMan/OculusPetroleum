using UnityEngine;
using System.Collections;

public class RingTriggerHandler : MonoBehaviour
{
    private static RingTriggerHandler _instance;

    public static RingTriggerHandler Instance
    {
        get
        {
            return _instance;
        }
    }

    private void Start()
    {
        _instance = this;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == GameController.Instance.CurrentPipe)
            GameController.Instance.OnRingTriggerEnter(other);
    }
}
