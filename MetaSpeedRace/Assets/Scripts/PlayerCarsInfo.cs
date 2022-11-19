using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCarsInfo : MonoBehaviour
{
    #region Singleton
    public static PlayerCarsInfo Instance;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }
    #endregion

    public List<CarUpgradeInfo> all_car_details = new List<CarUpgradeInfo>();        
    public List<CarUpgradeInfo> carDefaultData = new List<CarUpgradeInfo>();



    public List<CarUpgradePrice> car_upgrades = new List<CarUpgradePrice>();

    public void SetCarsData()
    {
        LocalData data = DatabaseManager.Instance.GetLocalData();
        all_car_details = data.carDetails;

    }
}

[System.Serializable]
public class CarUpgradePrice
{
    public int car_index;

    public CostAndAmount acceleratoin;
    public CostAndAmount speed;
    public CostAndAmount braking;
    public CostAndAmount nitrus_time;
    public CostAndAmount handling;

   
}
[System.Serializable]
public class CostAndAmount
{
    public int[] cost;
    public float[] amount;
}