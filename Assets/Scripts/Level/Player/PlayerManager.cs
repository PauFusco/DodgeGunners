using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    [SerializeField]
    private GameObject hostObj, remoteObj, networkManagerObj;

    private GameManager gameManager;
    private PlayerBehaviour local, remote;
    private NetworkManager networkManager;

    private Vector3 netPos;

    private bool localIsHost;

    private void Start()
    {
        networkManager = networkManagerObj.GetComponent<NetworkManager>();
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();

        if (gameManager.GetRemote().GetPlayerType() == GameManager.Player.Type.REMOTE)
        {
            localIsHost = true;

            local = hostObj.GetComponent<PlayerBehaviour>();
            remote = remoteObj.GetComponent<PlayerBehaviour>();

            netPos = remote.transform.position;
        }
        else if (gameManager.GetRemote().GetPlayerType() == GameManager.Player.Type.HOST)
        {
            localIsHost = false;

            remote = hostObj.GetComponent<PlayerBehaviour>();
            local = remoteObj.GetComponent<PlayerBehaviour>();

            netPos = local.transform.position;
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
        networkManager.SendNetMovement(local);
        remote.SetPosition(netPos);
    }

    private void CheckKeyMovement(PlayerBehaviour localPlayerToMove)
    {
        if (Input.GetKey(KeyCode.W)) localPlayerToMove.MoveUp();
        if (Input.GetKey(KeyCode.A)) localPlayerToMove.MoveLeft();
        if (Input.GetKey(KeyCode.S)) localPlayerToMove.MoveDown();
        if (Input.GetKey(KeyCode.D)) localPlayerToMove.MoveRight();
    }

    public void SetNetPosition(Vector3 pos)
    { netPos = pos; }

    public bool GetLocalIsHost()
    { return localIsHost; }
}