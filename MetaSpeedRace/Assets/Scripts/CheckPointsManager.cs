using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckPointsManager : MonoBehaviour
{
    #region Singleton
    public static CheckPointsManager Instance;
    private void Awake()
    {
        Instance = this;
    }
    #endregion

    [SerializeField] GameObject checkPoint;
    [SerializeField] GameObject finishLine;
    // Start is called before the first frame update
    public void SetCheckPoint(Transform t,Transform next_t=null)
    {
        checkPoint.transform.position = t.position;
        checkPoint.transform.rotation = t.rotation *Quaternion.Euler(-90 , 0, 0);

        checkPoint.transform.GetChild(0).transform.rotation = Quaternion.LookRotation(next_t.position - t.position) * Quaternion.Euler(30,0,0);
        checkPoint.SetActive(true);
    }
    public void SetFinishLine(Transform t)
    {
        finishLine.transform.position = t.position;
        finishLine.transform.rotation = t.rotation * Quaternion.Euler(-90, 0, 0);
        finishLine.SetActive(true);
    }

    internal void ResetCheckPoints()
    {
        finishLine.SetActive(false);
        checkPoint.SetActive(false);
    }
}
