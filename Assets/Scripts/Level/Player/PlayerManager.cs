using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    [SerializeField]
    private GameObject hostObj, remoteObj, networkManagerObj, projectileControllerObj;
    [SerializeField]
    private Countdown countdown;

    private GameManager gameManager;
    private PlayerBehaviour local, remote;
    private NetworkManager networkManager;
    private ProjectileController projectileController;
    private bool localIsHost;

    private Vector3 tempNetPos;
    public int bulletOffset;

    private void Start()
    {
        networkManager = networkManagerObj.GetComponent<NetworkManager>();
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        projectileController = projectileControllerObj.GetComponent<ProjectileController>();

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
        CheckStatus(local);
    }

    private void FixedUpdate()
    {
        networkManager.SendPlayerNetInfo(local);
        remote.SetPosition(tempNetPos);
    }

    private void CheckKeyMovement(PlayerBehaviour localPlayerToMove)
    {
        if (Input.GetKeyDown(KeyCode.W)) localPlayerToMove.MoveUp();
        if (Input.GetKey(KeyCode.A)) localPlayerToMove.MoveLeft();
        if (Input.GetKeyDown(KeyCode.S)) localPlayerToMove.MoveDown();
        if (Input.GetKey(KeyCode.D)) localPlayerToMove.MoveRight();

        if (Input.GetKeyDown(KeyCode.Space)) projectileController.LocalSpawnProjectile(GetBulletDirection());
    }

    private void CheckStatus(PlayerBehaviour localPlayerToMove)
    {
        if (!localPlayerToMove._alive)
        {
            localPlayerToMove._alive = true;
            localPlayerToMove.enemyScore.Increase();
            countdown.ResetCountdown();
        }
    }

    public void SetNetPosition(Vector3 pos)
    { tempNetPos = pos; }

    public bool GetLocalIsHost()
    { return localIsHost; }

    public PlayerBehaviour GetLocal()
    { return local; }

    public PlayerBehaviour GetRemote()
    { return remote; }

    public Vector3 GetBulletDirection()
    {
        if (local.transform.position.z < 0)
            return new Vector3(local.transform.position.x, local.transform.position.y, local.transform.position.z + bulletOffset);
        else
            return new Vector3(local.transform.position.x, local.transform.position.y, local.transform.position.z - bulletOffset);
    }
}