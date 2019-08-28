using UnityEngine;
using System.Collections;

public class GKSHTriggerHandler : MonoBehaviour
{
    private static GKSHTriggerHandler _instance;

    public static GKSHTriggerHandler Instance
    {
        get
        {
            return _instance;
        }
    }

    private void Awake()
    {
        _instance = this;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == GameController.Instance.CurrentPipe)
            GameController.Instance.OnGKSHTriggerEnter(other);
    }
}
