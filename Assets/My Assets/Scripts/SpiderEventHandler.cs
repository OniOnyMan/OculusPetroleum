using UnityEngine;
using System;
using System.Collections;

public class SpiderEventHandler : MonoBehaviour
{
    public event Action OnOpened;

    public void AnimationOpenEventHandler() {
        if (OnOpened != null)
            OnOpened.Invoke();
    }
}
