using System.Collections;
using UnityEngine;

public class PipeStaticTriggerHandler : MonoBehaviour
{
    public bool IsTriggered { get; private set; }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == GameController.Instance.PreviousPipe)
        {
            IsTriggered = true;
            //GameController.Instance.PreviousPipe.transform.GetChild(0).gameObject.SetActive(false);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject == GameController.Instance.PreviousPipe)
        {
            IsTriggered = false;
           // GameController.Instance.PreviousPipe.transform.GetChild(0).gameObject.SetActive(true);
        }
    }
}
