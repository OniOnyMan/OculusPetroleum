using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OculusSampleFramework;

public class MainController : MonoBehaviour
{
    public float MovingSpeed = 10;

    [SerializeField] private int _movingFix = 84;
    private Joint _captureJoin;
    private bool _moving = false;

    void Start()
    {
        _captureJoin = GetComponent<Joint>();

    }

    void Update()
    {
        if (_moving)
        {
            transform.Translate(transform.up * Time.deltaTime / _movingFix * MovingSpeed);
            if (transform.position.y > 3)
                _moving = false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("CapturedGrabble") && _captureJoin.connectedBody == null)
        {
            other.transform.rotation = Quaternion.identity;
            var parent = other.transform.parent;
            parent.GetComponent<ConfigurableJoint>().angularYMotion = ConfigurableJointMotion.Locked;
            parent.GetComponent<ConfigurableJoint>().angularZMotion = ConfigurableJointMotion.Locked;
            parent.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
            _captureJoin.connectedBody = other.GetComponent<Rigidbody>();
            var temp = other.GetComponent<DistanceGrabbable>();
            //if(temp.isGrabbed)
            //    temp.grabbedBy.EndingGrab();
            Destroy(temp);
            var tempTransform = other.transform;
            for (var i = 0; i < tempTransform.childCount; i++) 
                tempTransform.GetChild(i).gameObject.SetActive(false);
            _moving = true;
            //TODO: Fix it
            //tempTransform.position = _captureJoin.connectedAnchor;
            //other.transform.rotation = Quaternion.identity;
            //var otherRigidbody = _captureJoin.connectedBody = other.GetComponent<Rigidbody>();
            //var grabbableRigidbody = other.GetComponent<Joint>().connectedBody;
            //other.GetComponent<Joint>().connectedBody.GetComponent<Collider>().isTrigger = true;
            //temp.constraints = ~RigidbodyConstraints.FreezePositionY;

        }
    }
}
