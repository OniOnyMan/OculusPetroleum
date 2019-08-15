using System;
using System.Collections;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public GameObject PipeStaticPrefab;
    public Vector3 PipeShellJointPosition = new Vector3(2.153215f, 1.401268f, 0.3389995f);
    public Vector3 PipeShellJointRotation = new Vector3(0, -90, 0);
    public Vector3 ElevatorShellJointPosition = new Vector3(-2.173f, 1.3848f, 0.339f);
    public Vector3 ElevatorShellJointRotation = new Vector3(0, 90, 0);
    public SVLever LiftUpLever;
    public SVLever LiftDownLever;
    public Transform Elevator;
    public float TargetElevatorHeight = 5.5f;
    public float ElevatorMovingSpeed = 5;
    public event Action<ElevatorLiftStage> LiftLeversSwitched;
    public SVLever GKSHGateLever;
    public SVLever GKSHRingLever;
    public SVLever SpiderLever;
    public SpiderEventHandler Spider0;
    public SpiderEventHandler Spider1;         

    private ElevatorLiftStage _movingDirection = ElevatorLiftStage.Idle;
    private PipeStaticTriggerHandler _currentPipe;
    private PipeStaticTriggerHandler _previousPipe;
    private float _startElevatorHeight;
    private bool _spiderIsOpened;

    public GameObject CurrentPipe
    {
        get
        {
            return _currentPipe.gameObject;
        }
    }

    public GameObject PreviousPipe
    {
        get
        {
            return _previousPipe.gameObject;
        }
    }

    public static GameController Instance
    {
        get; private set;
    }

    private bool IsSpiderOpened
    {
        get
        {
            return _spiderIsOpened = Spider0.IsOpened && Spider1.IsOpened;
        }
    }

    public void OnGKSHTriggerEnter(Collider other)
    {
        var obj = other.transform;
        obj.parent = GKSHTriggerHandler.Instance.transform;
    }

    public void OnElevatorrTriggerEnter(Collider other)
    {
        var grabbable = other.GetComponent<OVRGrabbable>();
        if (grabbable.isGrabbed)
            grabbable.grabbedBy.ForceRelease(grabbable);
        Destroy(other.transform.parent.gameObject);
        ElevatorTriggerHandler.Instance.ElevatorShellGrip.DropFromHand();

        var elevatorCatch = ElevatorTriggerHandler.Instance.transform;
        var elevatorShell = elevatorCatch.parent;
        var elevatorShellRigidbody = elevatorShell.GetComponent<Rigidbody>();

        elevatorShellRigidbody.isKinematic = true;
        elevatorShell.position = ElevatorShellJointPosition;
        elevatorShell.rotation = Quaternion.Euler(ElevatorShellJointRotation);
        _currentPipe = Instantiate(PipeStaticPrefab, PipeShellJointPosition, Quaternion.Euler(PipeShellJointRotation)).GetComponentInChildren<PipeStaticTriggerHandler>();
        _currentPipe.transform.parent = elevatorCatch;
        //yield return new WaitForSecondsRealtime(0.22f);
        elevatorShellRigidbody.isKinematic = false;
    }

    void Start()
    {
        _startElevatorHeight = Elevator.position.y;
        Instance = this;
        LiftLeversSwitched += delegate (ElevatorLiftStage cond) { _movingDirection = cond; };
        _previousPipe = GameObject.FindGameObjectWithTag("PreviousPipe").GetComponent<PipeStaticTriggerHandler>();
        PreviousPipe.transform.parent = null;
        SetGateOpenClose();
    }

    void Update()
    {
        LiftElevator();
        if (GKSHGateLever.leverWasSwitched)
            SetGateOpenClose();
        if (SpiderLever.leverWasSwitched)
            SetSpiderAnimatorParams();
    }

    private void SetSpiderAnimatorParams()
    {
        Spider0.Animator.SetBool("Up", SpiderLever.leverIsOn);
        Spider1.Animator.SetBool("Up", SpiderLever.leverIsOn);
        Spider0.Animator.SetBool("Down", !SpiderLever.leverIsOn);
        Spider1.Animator.SetBool("Down", !SpiderLever.leverIsOn);
    }

    private void SetGateOpenClose()
    {
        var hinge = GateInstance.Instance.GetComponent<HingeJoint>();
        var spring = hinge.spring;
        spring.targetPosition = GKSHGateLever.leverIsOn ? hinge.limits.min : hinge.limits.max;
    }

    private void LiftElevator()
    {
        if (LiftLeversSwitched != null)
        {
            if (LiftUpLever.leverWasSwitched && _movingDirection != ElevatorLiftStage.Down)
                LiftLeversSwitched.Invoke(LiftUpLever.leverIsOn ? ElevatorLiftStage.Up : ElevatorLiftStage.Idle);
            if (LiftDownLever.leverWasSwitched && _movingDirection != ElevatorLiftStage.Up)
                LiftLeversSwitched.Invoke(LiftDownLever.leverIsOn ? ElevatorLiftStage.Down : ElevatorLiftStage.Idle);
        }
        if (_movingDirection == ElevatorLiftStage.Up)
        {
            if (Elevator.position.y < TargetElevatorHeight)
                Elevator.Translate(Vector3.up * Time.deltaTime * ElevatorMovingSpeed);
        }
        else if (_movingDirection == ElevatorLiftStage.Down)
        {
            var heightCondition = Elevator.position.y > _startElevatorHeight;
            var targetCondition = _currentPipe == null ? heightCondition : _currentPipe.IsTriggered ? false : heightCondition;
            if (targetCondition)
                Elevator.Translate(Vector3.down * Time.deltaTime * ElevatorMovingSpeed);
        }
    }
}
