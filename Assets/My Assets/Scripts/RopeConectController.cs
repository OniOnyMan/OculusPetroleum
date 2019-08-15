using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RopeConectController : MonoBehaviour
{
    public string TargetTag = "ElevatorRopeConnector";

    private Transform _targetPoint;
    private Vector3 _previousPosition;

    private Vector3 _targetPointAsChild
    {
        get
        {
            return transform.InverseTransformPoint(_targetPoint.position);
        }
    }

    private LineRenderer _line
    {
        get
        {
            return GetComponent<LineRenderer>();
        }
    }

    private void Start()
    {
        _targetPoint = GameObject.FindGameObjectWithTag(TargetTag).transform;
        _previousPosition = transform.position;
        SetConnectToTarget();
    }

    private void Update()
    {
        if (_previousPosition != transform.position)
        {
            SetConnectToTarget();
        }
    }    

    private void SetConnectToTarget()
    {
        _line.SetPosition(1, _targetPointAsChild);
    }       
}