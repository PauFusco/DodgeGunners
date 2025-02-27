using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    [SerializeField] private GameObject hostObj, remoteObj;

    [SerializeField] private NetworkManager networkManager;
    [SerializeField] private MenuController menuController;
    [SerializeField] private ProjectileController projectileController;
    [SerializeField] private Countdown countdown;

    private GameManager gameManager;

    private PlayerBehaviour local, remote;

    private bool localIsHost;

    private Vector3 tempNetPos;
    private float tempHealth;
    private int tempScore;
    private int tempAnim;

    public int bulletOffset;

    private void Start()
    {
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
        CheckStatus();
    }

    private void FixedUpdate()
    {
        networkManager.SendPlayerNetInfo(local);
        remote.SetPosition(tempNetPos);
        remote.healthBarScript.SetHealth(tempHealth);
        remote.scoreScript.SetScore(tempScore);
        remote.SetAnimation(tempAnim);

        TimerAndScoreCheck();
    }

    private void TimerAndScoreCheck()
    {
        if (local.GetScore() < 3 && remote.GetScore() < 3)
        {
            if (countdown.GetRoundTime() <= 0)
            { NewRound(); }
        }
        else
        {
            local.canMove = false;
            remote.canMove = false;
            projectileController.ClearAllProjectiles();

            // menu / popup
            string winnerName = local.GetScore() >= 3 ? gameManager.GetLocal().GetUsername() : gameManager.GetRemote().GetUsername();

            menuController.EnableMenu(winnerName);
        }
    }

    private void NewRound()
    {
        countdown.ResetCountdown();
        Vector3 startPos = localIsHost ? new(0, 2.5f, -5) : new(0, 2.5f, 5);
        local.ResetValues();
        local.SetPosition(startPos);
    }

    private void CheckKeyMovement(PlayerBehaviour localPlayerToMove)
    {
        if (!local.canMove) return;

        if (Input.GetKeyDown(KeyCode.W)) localPlayerToMove.MoveUp();
        if (Input.GetKey(KeyCode.A)) localPlayerToMove.MoveLeft();
        if (Input.GetKeyDown(KeyCode.S)) localPlayerToMove.MoveDown();
        if (Input.GetKey(KeyCode.D)) localPlayerToMove.MoveRight();

        if (Input.GetKeyDown(KeyCode.Space) && local.GetAmmo() > 0)
        {
            projectileController.LocalSpawnProjectile(GetBulletDirection());
            local.ReduceAmmo();
        }

        if (!Input.GetKey(KeyCode.A) && !Input.GetKey(KeyCode.D) && local.IsGrounded())
        {
            local.SetIdle();
        }
    }

    private void CheckStatus()
    {
        if (!local.isAlive)
        {
            remote.scoreScript.Increase();
            NewRound();
            local.healthBarScript.ResetHealth();
            local.isAlive = true;
        }
        if (!remote.isAlive)
        {
            local.scoreScript.Increase();
            NewRound();
            remote.healthBarScript.ResetHealth();
            remote.isAlive = true;
        }
    }

    public void SetNetPosition(Vector3 pos)
    { tempNetPos = pos; }

    public void SetNetHealth(float health)
    { tempHealth = health; }

    public void SetNetScore(int score)
    { tempScore = score; }

    public void SetNetAnimation(int anim)
    { tempAnim = anim; }

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