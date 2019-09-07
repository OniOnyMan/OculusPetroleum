using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GKSHAllowGrabController : MonoBehaviour {

    public static GKSHAllowGrabController Instance { get; private set; }
    public bool IsGrabAllowed = true;

    void Awake()
    {
        Instance = this;
    }
}
