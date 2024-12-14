using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    [SerializeField]
    private GameObject hostObj, remoteObj, networkManagerObj, projectileControllerObj;

    private GameManager gameManager;
    private PlayerBehaviour local, remote;
    private NetworkManager networkManager;
    private ProjectileController projectileController;

    private bool localIsHost;

    private Vector3 tempNetPos;

    private void Start()
    {
        networkManager = networkManagerObj.GetComponent<NetworkManager>();
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();

        if (gameManager.GetRemote().GetPlayerType() == GameManager.NetPlayer.Type.REMOTE)
        {
            localIsHost = true;

            local = hostObj.GetComponent<PlayerBehaviour>();
            remote = remoteObj.GetComponent<PlayerBehaviour>();

            tempNetPos = remote.transform.position;
        }
        else if (gameManager.GetRemote().GetPlayerType() == GameManager.NetPlayer.Type.HOST)
        {
            localIsHost = false;

            remote = hostObj.GetComponent<PlayerBehaviour>();
            local = remoteObj.GetComponent<PlayerBehaviour>();

            tempNetPos = local.transform.position;
        }

        local.SetPlayerTag(gameManager.GetLocal().GetUsername());
        remote.SetPlayerTag(gameManager.GetRemote().GetUsername());
    }

    private void Update()
    {
        CheckKeyMovement(local);
    }

    private void FixedUpdate()
    {
        networkManager.SendNetInfo(local);
        remote.SetPosition(tempNetPos);
    }

    private void CheckKeyMovement(PlayerBehaviour localPlayerToMove)
    {
        if (Input.GetKey(KeyCode.W)) localPlayerToMove.MoveUp();
        if (Input.GetKey(KeyCode.A)) localPlayerToMove.MoveLeft();
        if (Input.GetKey(KeyCode.S)) localPlayerToMove.MoveDown();
        if (Input.GetKey(KeyCode.D)) localPlayerToMove.MoveRight();
    }

    public void SetNetPosition(Vector3 pos)
    { tempNetPos = pos; }

    public bool GetLocalIsHost()
    { return localIsHost; }

    public PlayerBehaviour GetLocal()
    { return local; }

    public PlayerBehaviour GetRemote()
    { return remote; }
}