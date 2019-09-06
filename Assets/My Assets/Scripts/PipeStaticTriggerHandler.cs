using System.Collections;
using UnityEngine;

public class PipeStaticTriggerHandler : MonoBehaviour
{
    public float DeathLevel = -7.32f;
    public bool IsTriggered { get; private set; }

    private void OnTriggerEnter(Collider other)
    {
        if (!IsTriggered)
        {
            var parent = other.transform.parent;
            if (parent == GameController.Instance.PreviousPipe.transform)
            {
                IsTriggered = true;
                //parent.GetChild(0).gameObject.SetActive(false);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (IsTriggered)
        {
            var parent = other.transform.parent;
            if (other.transform.parent == GameController.Instance.PreviousPipe.transform)
            {
                IsTriggered = false;
                //parent.GetChild(0).gameObject.SetActive(true);
            }
        }
    }

    private void Update()
    {
        if (transform.position.y < DeathLevel && !GameController.Instance.IsSpiderOpened)
        {
            GameController.Instance.PreviusPipeDestroyed();
            Destroy(gameObject);
        }
    }
}
