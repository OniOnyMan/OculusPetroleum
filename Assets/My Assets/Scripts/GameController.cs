using System;
using System.Collections;
using UnityEngine;
using DG.Tweening;

public class GameController : MonoBehaviour
{
    public GameObject PipeGrabablePrefab;
    public Vector3 PipeGrabableSpawnPosition = new Vector3(2.887f, 1.4023f, 0.34f);
    public GameObject PipeStaticPrefab;
    public Vector3 PipeShellJointPosition = new Vector3(2.153215f, 1.401268f, 0.3389995f);
    public Vector3 PipeShellJointRotation = new Vector3(0, -90, 0);
    public Vector3 ElevatorShellJointPosition = new Vector3(-2.173f, 1.3848f, 0.339f);
    public Vector3 ElevatorShellJointRotation = new Vector3(0, 90, 0);
    public SVLever LiftUpLever;
    public SVLever LiftDownLever;
    public Transform Elevator;
    public float TargetElevatorHeight = 5.5f;
    public float DownElevatorHeight = 1.06f;
    public float ElevatorMovingSpeed = 5;
    public event Action<ElevatorLiftStage> LiftLeversSwitched;
    public SVLever GKSHGateLever;
    public SVLever GKSHRingLever;
    public SVLever SpiderLever;
    public SpiderEventHandler Spider0;
    public SpiderEventHandler Spider1;
    public float RingRotatingOnceTime = 1.5f;
    public int RingRotatingCount = 3;
    public Vector3 ElevatorRotatingPosition = new Vector3(-2.018f, 5.089307f, 0.3390008f);
    public Vector3 PipeRotatingPosition = new Vector3(-2.150962f, 0.743f, 0.3368171f);

    private ElevatorLiftStage _movingDirection = ElevatorLiftStage.Idle;
    private Transform _elevatorCatcher;
    [SerializeField]
    private PipeStaticTriggerHandler _currentPipe;
    [SerializeField]
    private PipeStaticTriggerHandler _previousPipe;
    private Transform _gkshRing;
    private float _startElevatorHeight;
    private bool _isRingRotating;
    private bool _isGKSHCatchAllowed = true;
    private int _cycleCount = 0;

    private Transform ElevatorCatch
    {
        get
        {
            if (!_elevatorCatcher)
                _elevatorCatcher = ElevatorTriggerHandler.Instance.transform;
            return _elevatorCatcher;
        }
    }

    public GameObject CurrentPipe
    {
        get
        {
            if (!_currentPipe) return null;
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

    public void PreviusPipeDestroyed()
    {
        _previousPipe = _currentPipe;
        _currentPipe = null;
    }

    public bool IsSpiderOpened
    {
        get
        {
            return Spider0.IsOpened && Spider1.IsOpened;
        }
    }

    public void OnGKSHTriggerEnter(Collider other)
    {
        if (_currentPipe.IsTriggered && _isGKSHCatchAllowed)
        {
            GKSHTriggerHandler.Instance.IsCatchAllowed = false;
            _currentPipe.transform.parent = _gkshRing;
            GKSHAllowGrabController.Instance.IsGrabAllowed = false;
            GKSHAllowGrabController.Instance.GetComponent<Rigidbody>().isKinematic = true;
        }
    }

    private void StartRingRotating()
    {
        if (!_isRingRotating)
        {
            _isRingRotating = true;
            _gkshRing.DORotate(new Vector3(0, 360, 0), RingRotatingOnceTime, RotateMode.FastBeyond360).SetEase(Ease.Linear).SetLoops(RingRotatingCount, LoopType.Restart)
                .OnComplete(() =>
                {
                    _currentPipe.transform.parent = ElevatorCatch;
                    GKSHAllowGrabController.Instance.IsGrabAllowed = true;
                    _previousPipe.transform.parent = _currentPipe.transform;
                });
            if (!GKSHAllowGrabController.Instance.IsGrabAllowed)
            {
                _currentPipe.transform.DOMoveY(PipeRotatingPosition.y, RingRotatingOnceTime * RingRotatingCount);
                Elevator.DOMoveY(ElevatorRotatingPosition.y, RingRotatingOnceTime * RingRotatingCount).SetEase(Ease.Linear);
            }
        }
        else _isRingRotating = false;
    }

    private void StopRingRotating()
    {
        if (!_isRingRotating)
            _gkshRing.DOPause();
    }

    public void OnElevatorTriggerEnter(Collider other)
    {
        var grabbable = other.GetComponent<OVRGrabbable>();
        if (grabbable.isGrabbed)
            grabbable.grabbedBy.ForceRelease(grabbable);
        Destroy(other.transform.parent.gameObject);
        ElevatorTriggerHandler.Instance.ElevatorShellGrip.DropFromHand();
        SetCurrentPipe();
    }

    private void SetCurrentPipe()
    {
        var elevatorShell = ElevatorCatch.parent;
        var elevatorShellRigidbody = elevatorShell.GetComponent<Rigidbody>();

        elevatorShellRigidbody.isKinematic = true;
        elevatorShell.position = ElevatorShellJointPosition;
        elevatorShell.rotation = Quaternion.Euler(ElevatorShellJointRotation);
        _currentPipe = Instantiate(PipeStaticPrefab, PipeShellJointPosition, Quaternion.Euler(PipeShellJointRotation))
                        .GetComponentInChildren<PipeStaticTriggerHandler>();
        _currentPipe.name = string.Format("{0} (Cycle: {1})", _currentPipe.name, _cycleCount);
        _currentPipe.transform.parent = ElevatorCatch;
        elevatorShellRigidbody.isKinematic = false;
    }

    void Start()
    {
        _startElevatorHeight = Elevator.position.y;
        LiftLeversSwitched += (ElevatorLiftStage cond) => _movingDirection = cond;
        _previousPipe = GameObject.FindGameObjectWithTag("PreviousPipe").GetComponent<PipeStaticTriggerHandler>();
        PreviousPipe.transform.parent = null;
        SetGateOpenClose();
        _gkshRing = GKSHTriggerHandler.Instance.transform;
    }

    private void Awake()
    {
        Instance = this;
    }

    void Update()
    {
        LiftElevator();
        if (GKSHGateLever.leverWasSwitched)
            SetGateOpenClose();
        if (SpiderLever.leverWasSwitched)
            SetSpiderAnimatorParams();
        if (GKSHRingLever.leverWasSwitched)
            StartRingRotating();

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
        hinge.spring = spring;
    }

    private void LiftElevator()
    {
        if (LiftLeversSwitched != null)
        {
            var currentPipeChildCountCond = _currentPipe ? _currentPipe.transform.childCount == PipeStaticPrefab.transform.childCount : true;
            if (currentPipeChildCountCond || IsSpiderOpened)
                SetElevatorLeftStage();
        }
        if (_movingDirection == ElevatorLiftStage.Up)
        {
            if (Elevator.position.y < TargetElevatorHeight)
                Elevator.Translate(Vector3.up * Time.deltaTime * ElevatorMovingSpeed);
        }
        else if (_movingDirection == ElevatorLiftStage.Down)
        {
            var heightCondition = Elevator.position.y > DownElevatorHeight;
            var targetCondition = !_currentPipe ? heightCondition : _currentPipe.IsTriggered ? false : heightCondition;
            if (targetCondition)
                Elevator.Translate(Vector3.down * Time.deltaTime * ElevatorMovingSpeed);
        }
        if (!_currentPipe && ElevatorCatch.childCount > 0 && ElevatorCatch.GetChild(0) == _previousPipe.transform)
        {
            _movingDirection = ElevatorLiftStage.Idle;
            _previousPipe.transform.parent = null;
            Elevator.DOMoveY(_startElevatorHeight, Math.Abs(DownElevatorHeight - _startElevatorHeight) / ElevatorMovingSpeed).SetEase(Ease.Linear)
                .OnComplete(() =>
                {
                    GKSHTriggerHandler.Instance.IsCatchAllowed = true;
                    Instantiate(PipeGrabablePrefab, PipeGrabableSpawnPosition, PipeGrabablePrefab.transform.rotation, null);
                    _cycleCount++;
                });
        }

    }

    private void SetElevatorLeftStage()
    {
        if (LiftUpLever.leverWasSwitched && _movingDirection != ElevatorLiftStage.Down)
            LiftLeversSwitched.Invoke(LiftUpLever.leverIsOn ? ElevatorLiftStage.Up : ElevatorLiftStage.Idle);
        if (LiftDownLever.leverWasSwitched && _movingDirection != ElevatorLiftStage.Up)
            LiftLeversSwitched.Invoke(LiftDownLever.leverIsOn ? ElevatorLiftStage.Down : ElevatorLiftStage.Idle);
    }
}
