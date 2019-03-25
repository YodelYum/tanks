using CommandTerminal;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class photonController : MonoBehaviour
{

    bool alive = false;

    
    // Start is called before the first frame update
    void Start()
    {
        PhotonNetwork.ConnectUsingSettings("0.1");
        

    }

    // Update is called once per frame
    void Update()
    {
        spawn();
    }


    

    public void OnConnectedToPhoton()
    {
        Debug.Log("connected to photon");
    }

    public void OnConnectedToMaster()
    {
        Debug.Log("connected to MASTER");
        joinRoom();
    }

    void joinRoom()
    {
        RoomOptions options = new RoomOptions()
        {
            isVisible = true,
            maxPlayers = 2
        };
        PhotonNetwork.JoinOrCreateRoom("testroom", options, TypedLobby.Default);
    }

    void OnJoinedRoom()
    {
        Debug.Log("Joined ROOM");
    }

    void spawn()
    {
        if (!alive && PhotonNetwork.inRoom)
        {
            alive = true;
            Transform spawnPoint = GameObject.Find("spawnpoint").transform;
            PhotonNetwork.Instantiate("tiger", spawnPoint.position, spawnPoint.rotation, 0);
        }
    }
}
