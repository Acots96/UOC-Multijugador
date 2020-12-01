using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using Complete;

public class TankEnemy : NetworkBehaviour {

    private enum Targets { OnlyPlayers, OnlyEnemies, All }

    [SerializeField] private float Speed, FireRateTime, MaxDistance, MinDistanceToStop;
    [SerializeField] private Transform FireTransform;
    [SerializeField] private AudioSource FireSound;
    [SerializeField] private GameObject ShellGO;
    [SerializeField] private Targets targets;

    private float rateDeltaTime, findTargetRateTime, findTargetRateDeltaTime;
    private Transform targetTr, tr;


    private void Start() {
        GameManager.AddTank(transform);
        enabled = isServer;
        tr = transform;
        rateDeltaTime = Random.value * FireRateTime + 2f;
        findTargetRateTime = 0.5f;
        FindClosestTank();
    }


    private void Update() {
        if (rateDeltaTime <= 0) {
            Fire();
            rateDeltaTime = Random.value * FireRateTime + 2f;
        }
        if (findTargetRateDeltaTime <= 0) {
            FindClosestTank();
            findTargetRateDeltaTime = findTargetRateTime;
        }
        rateDeltaTime -= Time.deltaTime;
        findTargetRateDeltaTime -= Time.deltaTime;
        //
        RotateSmoothly();
        if (!targetTr)
            return;
        if (Vector3.Distance(targetTr.position, tr.position) > MinDistanceToStop)
            tr.position += tr.forward * Speed * Time.deltaTime;
    }


    private void FindClosestTank() {
        Vector3 pos = transform.position;
        //
        List<Transform> tanks = new List<Transform>();
        if (targets == Targets.OnlyPlayers || targets == Targets.All)
            tanks.AddRange(GameManager.GetPlayersTanks());
        if (targets == Targets.OnlyEnemies || targets == Targets.All)
            tanks.AddRange(GameManager.GetNpcsTanks());
        tanks.Remove(tr);
        //
        float dist = float.MaxValue;
        Transform targ = null;
        foreach (Transform t in tanks) {
            float d = Vector3.Distance(t.position, pos);
            if (d < MaxDistance && d < dist) {
                dist = d;
                targ = t;
            }
        }
        targetTr = targ;
    }


    private void RotateSmoothly() {
        if (!targetTr)
            return;
        Vector3 forward = (targetTr.position - tr.position).normalized;
        forward.y = 0;
        tr.rotation = Quaternion.Lerp(tr.rotation, Quaternion.LookRotation(forward), 0.1f);
    }


    private void Fire() {
        if (!targetTr)
            return;

        GameObject shell = Instantiate(ShellGO, FireTransform.position, FireTransform.rotation);
        NetworkServer.Spawn(shell);
        //
        Vector3 targetPos = targetTr.position;
        Vector3 firePos = shell.transform.position;
        Vector3 vel = new Vector3(
            targetPos.x - firePos.x,
            targetPos.y - firePos.y - 0.5f * Physics.gravity.y,
            targetPos.z - firePos.z
        );
        shell.transform.forward = vel;
        shell.GetComponent<Rigidbody>().velocity = vel;
        //
        RpcFire(shell, vel);
        FireSound.Play();
    }

    [ClientRpc]
    private void RpcFire(GameObject go, Vector3 vel)
    {
        go.GetComponent<Rigidbody>().velocity = vel;
    }

    private void OnEnable() {
        GameManager.AddTank(transform);
    }

    private void OnDisable() {
        GameManager.RemoveTank(transform);
    }
    private void OnDestroy() {
        GameManager.RemoveTank(transform);
    }

}
