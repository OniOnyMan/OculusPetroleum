using UnityEngine;
using System;
using System.Collections;

public class SpiderEventHandler : MonoBehaviour
{
    public event Action<bool> OnStatusChanged;

    public bool IsOpened { get; private set; }

    public Animator Animator { get; private set; }

    private void Start()
    {
        Animator = GetComponent<Animator>();
    }

    public void AnimationOpenEventHandler()
    {
        Animator.SetBool("Up", false);
        IsOpened = true;
        if (OnStatusChanged != null)
            OnStatusChanged.Invoke(IsOpened);
    }

    public void AnimationCloseEventHandler()
    {
        IsOpened = false;
        Animator.SetBool("Down", false);
        if (OnStatusChanged != null)
            OnStatusChanged.Invoke(IsOpened);
    }
}
