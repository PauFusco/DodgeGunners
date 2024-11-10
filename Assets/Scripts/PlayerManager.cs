using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    [SerializeField]
    private GameObject hostObj, remoteObj, gameManagerObj;

    private GameManager gameManager;
    private PlayerBehaviour local, remote;

    private Vector3 netPos;

    private bool localIsHost;

    private void Start()
    {
        gameManager = gameManagerObj.GetComponent<GameManager>();
        local = hostObj.GetComponent<PlayerBehaviour>();
        remote = remoteObj.GetComponent<PlayerBehaviour>();

        // This makes the distinction between p1 and p2
        if (gameManager.GetEnemy().GetPlayerType() == GameManager.Player.Type.HOST) localIsHost = false;
        else localIsHost = true;

        Thread sendNetMovement = new(SendNetMovement);
        Thread receiveNetMovement = new(ReceiveNetMovement);

        sendNetMovement.Start();
        receiveNetMovement.Start();
    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        if (localIsHost)
        {
            CheckKeyMovement(local);
            remote.SetPosition(netPos);
        }
        else
        {
            CheckKeyMovement(remote);
            local.SetPosition(netPos);
        }
    }

    private void CheckKeyMovement(PlayerBehaviour localPlayerToMove)
    {
        //Detect Key presses here, call playerBehaviour functions to move objects (example: host.moveleft)
    }

    private void SendNetMovement()
    {
        // Send local position to remote
        while (true)
        { }
    }

    private void ReceiveNetMovement()
    {
        // Assign netPos with recieved position
        while (true)
        { }
    }
}