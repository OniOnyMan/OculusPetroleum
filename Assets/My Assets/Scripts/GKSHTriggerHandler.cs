using UnityEngine;
using System.Collections;

public class GKSHTriggerHandler : MonoBehaviour
{
    public bool IsCatchAllowed = true;

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
        var cond = other.gameObject == GameController.Instance.CurrentPipe;
        Debug.LogWarningFormat("GKSH Trigger enter: \"{0}\". Is equal to current pipe \"{1}\" with result: {2}", other.name, GameController.Instance.CurrentPipe.name, cond);
        if (IsCatchAllowed && cond)
            GameController.Instance.OnGKSHTriggerEnter(other);
    }
}
