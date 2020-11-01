using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.UI;

public class TankController : NetworkBehaviour {

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


    //Coloring

    public void Awake() {
        renderers = gameObject.GetComponentsInChildren<MeshRenderer>();
    }

    public override void OnStartLocalPlayer() {
        GameObject[] btns = GameObject.FindGameObjectsWithTag("PlayerColorButton");
        for (int i = 0; i < btns.Length; i++) {
            GameObject b = btns[i];
            Color c = b.GetComponent<Image>().color;
            b.GetComponent<Button>().onClick.AddListener(delegate { ColorChanged(c); });
        }
        string[] s = PlayerPrefs.GetString("SelectedColor").Split(';');
        CmdColorChanged(new Color(float.Parse(s[0]), float.Parse(s[1]), float.Parse(s[2])));
        //
        base.OnStartLocalPlayer();
    }


    private MeshRenderer[] renderers;

    [SyncVar(hook = "SyncPlayerColor")]
    private Color color = Color.blue;

    private void SyncPlayerColor(Color oldColor, Color newColor) {
        color = newColor;
        foreach (MeshRenderer rend in renderers)
            rend.material.color = color;
    }

    private void ColorChanged(Color c) {
        CmdColorChanged(c);
    }

    [Command]
    private void CmdColorChanged(Color c) {
        SyncPlayerColor(color, c);
    }
}
