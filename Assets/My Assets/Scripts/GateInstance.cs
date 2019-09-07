using UnityEngine;
using System.Collections;

public class GateInstance : MonoBehaviour
{
    public static GateInstance Instance { get; private set; }

    private void Start() {
        Instance = this;
    }
}
