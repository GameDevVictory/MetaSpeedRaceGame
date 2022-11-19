using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaceManager : MonoBehaviour
{
    #region Singleton
    public static RaceManager Instance;
    private void Awake()
    {
        Instance = this;
    }
    #endregion


    public Transform RaceENV;
    public Transform[] RaceENV_pozs;
    public Transform[] raceStartPoz;
    public LayerMask carLayer;

    public Transform[] racePathObjects;

    public Transform[] current_race_path_checkpoints;
    public int GetRacePositionsIndex()
    {
        for (int i = 0; i < raceStartPoz.Length; i++)
        {
            if (Physics.SphereCast(raceStartPoz[i].position, 7f, Vector3.forward, out RaycastHit hit, carLayer))
            {
                continue;
            }
            return i;
        }

        return Random.Range(0, raceStartPoz.Length);
    }
    public int GetPathIndex()
    {
        return Random.Range(0, racePathObjects.Length);
    }
    public int GetRaceEnvironMentPoistionIndex()
    {
        int number = PhotonNetwork.LocalPlayer.ActorNumber;
        if (number >= RaceENV_pozs.Length)
        {
            number = Random.Range(0, RaceENV_pozs.Length);
        }
        return number;
    }


    public void SetCurrentPath(int racePathIndex)
    {
        current_race_path_checkpoints = new Transform[racePathObjects[racePathIndex].childCount];
        for (int i = 0; i < current_race_path_checkpoints.Length; i++)
        {
            current_race_path_checkpoints[i] = racePathObjects[racePathIndex].GetChild(i);
        }
    }

    private void OnDrawGizmos()
    {
        for (int i = 0; i < raceStartPoz.Length; i++)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(raceStartPoz[i].position, 7f);
        }

    }

    public void ResetRaceSettings()
    {
        CheckPointsManager.Instance.ResetCheckPoints();

        current_race_path_checkpoints = null;
    }

    public void SetEnvironLocation(int raceEnvPozIndex)
    {
        RaceENV.transform.position = RaceENV_pozs[raceEnvPozIndex].position;
    }
}

