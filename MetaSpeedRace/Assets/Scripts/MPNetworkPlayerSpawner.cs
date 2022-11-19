using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MPNetworkPlayerSpawner : MonoBehaviourPunCallbacks
{
    [SerializeField] GameObject spawnedPlayerPrefab;
  

    public void GeneratePlayer(int _no = 0, bool _custmer = false)
    {        
        var randomNo = PhotonNetwork.LocalPlayer.ActorNumber;
        if (randomNo >= MetaManager.Instance.playerPoz.Length)
        {
            randomNo = Random.Range(0, MetaManager.Instance.playerPoz.Length);
        }
        Debug.Log("GeneratePlayer " + _no);
        spawnedPlayerPrefab = PhotonNetwork.Instantiate("Player", MetaManager.Instance.playerPoz[randomNo].position, MetaManager.Instance.playerPoz[randomNo].rotation);
       

        // if (_custmer) spawnedPlayerPrefab.GetComponent<NetworkPlayer>().myNoIs = _no;
        //CoreManager.myDefaultStartPoz = _no;
        //PhotonView photonView = GetComponent<PhotonView>();
        //NetworkManager.insta.SendUserRole(PhotonNetwork.LocalPlayer.UserId);
    }

    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();
    }


    public override void OnLeftRoom()
    {
        Debug.Log("I LEFT");
        base.OnLeftRoom();
        Debug.Log("OnLeftRoom");
        if(spawnedPlayerPrefab) PhotonNetwork.Destroy(spawnedPlayerPrefab);
    }

   
}
