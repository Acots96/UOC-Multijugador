using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankController : MonoBehaviour {

    // Start is called before the first frame update
    private void Start() {
        Complete.GameManager.AddTank(transform);
    }
    private void OnEnable() {
        Complete.GameManager.AddTank(transform);
    }

    private void OnDisable() {
        Complete.GameManager.RemoveTank(transform);
    }
    private void OnDestroy() {
        Complete.GameManager.RemoveTank(transform);
    }
}
