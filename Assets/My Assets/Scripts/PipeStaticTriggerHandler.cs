using System.Collections;
using UnityEngine;

public class PipeStaticTriggerHandler : MonoBehaviour
{
    public bool IsTriggered { get; private set; }

    private void OnTriggerStay(Collider other)
    {
        IsTriggered = true;
    }

    private void OnTriggerExit(Collider other)
    {
        IsTriggered = false;
    }
}
