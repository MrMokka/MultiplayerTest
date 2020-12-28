using Mirror;
using UnityEngine;

public class PlayerScript : NetworkBehaviour {

    public TextMesh playerNameText;
    public GameObject floatingInfo;
    public GameObject GlowCube;
    public Light GlowCubeLight;
    public GameObject Body;

    private Material playerMaterialClone;
    private Material glowCubeMaterial;
    private SceneScript sceneScript;

    [SyncVar(hook = nameof(OnNameChanged))]
    public string playerName;

    [SyncVar(hook = nameof(OnColorChanged))]
    public Color playerColor = Color.white;

    private void Awake() {
        sceneScript = GameObject.FindObjectOfType<SceneScript>();
    }

    public override void OnStartLocalPlayer() {
        // Camera.main.transform.SetParent(transform);
        // Camera.main.transform.localPosition = new Vector3(0, 0, 0);

        // floatingInfo.transform.localPosition = new Vector3(0, -0.3f, 0.6f);
        // floatingInfo.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);

        sceneScript.playerScript = this;
        string name = "Player" + Random.Range(100, 999);
        Color color = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f));
        CmdSetupPlayera(name, color);
    }

    private void OnNameChanged(string _Old, string _New) {
        playerNameText.text = playerName;
    }

    private void OnColorChanged(Color _Old, Color _New) {
        playerNameText.color = _New;

        playerMaterialClone = new Material(Body.GetComponent<Renderer>().material);
        playerMaterialClone.color = _New;
        Body.GetComponent<Renderer>().material = playerMaterialClone;

        glowCubeMaterial = new Material(GlowCube.GetComponent<Renderer>().material);
        glowCubeMaterial.color = _New;
        GlowCube.GetComponent<Renderer>().material = glowCubeMaterial;

        GlowCubeLight.color = _New;
    }

    [Command]
    public void CmdSetupPlayera(string _name, Color _col) {
        // player info sent to server, then server updates sync vars which handles it on all clients
        playerName = _name;
        playerColor = _col;
        sceneScript.statusText = $"{playerName} joined.";
    }

    [Command]
    public void CmdSendPlayerMessage() {
        if (sceneScript) {
            sceneScript.statusText = $"{playerName} says hello {Random.Range(10, 99)}";
        }
    }

    private void Update() {
        floatingInfo.transform.LookAt(Camera.main.transform);
        if (!isLocalPlayer) {
            // make non-local players run this
            return;
        }

        float moveX = Input.GetAxis("Horizontal") * Time.deltaTime * 110.0f;
        float moveZ = Input.GetAxis("Vertical") * Time.deltaTime * 4f;

        transform.Rotate(0, moveX, 0);
        transform.Translate(0, 0, moveZ);
    }
}