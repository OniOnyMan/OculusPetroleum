using System;
using System.Collections;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public GameObject PipeStaticPrefab;
    public Vector3 PipeShellJointPosition = new Vector3(2.153215f, 1.401268f, 0.3389995f);
    public Vector3 PipeShellJointRotation = new Vector3(0, -90, 0);
    public Vector3 RetainerShellJointPosition = new Vector3(-2.173f, 1.3848f, 0.339f);
    public Vector3 RetainerShellJointRotation = new Vector3(0, 90, 0);
    public SVLever LiftUpLever;
    public SVLever LiftDownLever;
    public Transform Retainer;
    public float TargetRetainerHeight = 5.5f;
    public float RetainerMovingSpeed = 5;
    public event Action<RetainerLiftStage> LiftLeversSwitched;
    public SVLever SpiderLever;
    public Animator Spider0Animator;
    public Animator Spider1Animator;


    private RetainerLiftStage _movingDirection = RetainerLiftStage.Idle;
    private PipeStaticTriggerHandler _currentPipe;
    private float _startRetainerHeight;
    private static GameController _instance;

    public GameObject CurrentPipe
    {
        get
        {
            return _currentPipe.gameObject;
        }
    }

    public static GameController Instance
    {
        get
        {
            return _instance;
        }
    }

    public void OnRingTriggerEnter(Collider other)
    {

    }

    public void OnRetainerTriggerEnter(Collider other)
    {
        var grabbable = other.GetComponent<OVRGrabbable>();
        if (grabbable.isGrabbed)
            grabbable.grabbedBy.ForceRelease(grabbable);
        Destroy(other.transform.parent.gameObject);
        RetainerTriggerHandler.Instance.RetainerShellGrip.DropFromHand();

        var retainerCatch = RetainerTriggerHandler.Instance.transform;
        var retainerShell = retainerCatch.parent;
        var retainerShellRigidbody = retainerShell.GetComponent<Rigidbody>();

        retainerShellRigidbody.isKinematic = true;
        retainerShell.position = RetainerShellJointPosition;
        retainerShell.rotation = Quaternion.Euler(RetainerShellJointRotation);
        _currentPipe = Instantiate(PipeStaticPrefab, PipeShellJointPosition, Quaternion.Euler(PipeShellJointRotation)).GetComponentInChildren<PipeStaticTriggerHandler>();
        _currentPipe.transform.parent = retainerCatch;
        //yield return new WaitForSecondsRealtime(0.22f);
        retainerShellRigidbody.isKinematic = false;
    }

    void Start()
    {
        _startRetainerHeight = Retainer.position.y;
        _instance = this;
        LiftLeversSwitched += delegate (RetainerLiftStage cond) { _movingDirection = cond; };
    }

    void Update()
    {
        LiftRetainer();
        if (_currentPipe != null)
            Debug.Log(_currentPipe.IsTriggered);
        if (SpiderLever.leverWasSwitched)
        {
            Spider0Animator.SetTrigger(SpiderLever.leverIsOn ? "Up" : "Down");
            Spider1Animator.SetTrigger(SpiderLever.leverIsOn ? "Up" : "Down");
        }
    }

    private void LiftRetainer()
    {
        if (LiftLeversSwitched != null)
        {
            if (LiftUpLever.leverWasSwitched && _movingDirection != RetainerLiftStage.Down)
                LiftLeversSwitched.Invoke(LiftUpLever.leverIsOn ? RetainerLiftStage.Up : RetainerLiftStage.Idle);
            if (LiftDownLever.leverWasSwitched && _movingDirection != RetainerLiftStage.Up)
                LiftLeversSwitched.Invoke(LiftDownLever.leverIsOn ? RetainerLiftStage.Down : RetainerLiftStage.Idle);
        }
        if (_movingDirection == RetainerLiftStage.Up)
        {
            if (Retainer.position.y < TargetRetainerHeight)
                Retainer.Translate(Vector3.up * Time.deltaTime * RetainerMovingSpeed);
        }
        else if (_movingDirection == RetainerLiftStage.Down)
        {
            if (_currentPipe != null && _currentPipe.IsTriggered || Retainer.position.y > _startRetainerHeight)
                Retainer.Translate(Vector3.down * Time.deltaTime * RetainerMovingSpeed);
        }
    }
}
