using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using TMPro;
using UnityEngine;

public class InAppManager : MonoBehaviour
{
    public static InAppManager Instance;

    [SerializeField] TMP_Text[] coinCost;
    private void Awake()
    {
        Instance = this;
    }

    async void OnEnable()
    {
        balanceText.text = "";

        if (BlockchainManager.Instance)
        {
            BlockchainManager.Instance.CheckUserBalance();
        }
        SetBalanceText();
    }

    [SerializeField] TMP_Text balanceText;
    public void SetBalanceText()
    {
        balanceText.text = "Balance : " + BlockchainManager.userBalance.ToString();
    }
    public void purchaseCoins(int index)
    {
        BlockchainManager.Instance.CoinBuyOnSendContract(index);
    }
    public void ExchangeCoins(int index)
    {
        BigInteger tokenBalance = BigInteger.Parse(SingletonDataManager.userTokenBalance);

        if (tokenBalance >= index)
        {
            BlockchainManager.Instance.ExchangeTokenUI(index);
        }
        else
        {
            MessaeBox.insta.showMsg("Not Enough Tokens to exchange",true);
        }
    }

}
