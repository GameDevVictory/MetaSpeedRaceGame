using Defective.JSON;
using Nethereum.Util;
using System;
using System.Collections;
using System.Numerics;
using UnityEngine;
using UnityEngine.Networking;
public class CovalentManager : MonoBehaviour
{
    public static CovalentManager insta;

    string BalanceFetchPreURL;
    string BalanceFetchPostURL = "/balances_v2/?quote-currency=USD&format=JSON&nft=false&no-nft-fetch=false&key=ckey_8b741a7fb36a427ab1497d4c612";
    public static string chainID;
    public static string usrMainBalance;
    public static string usrTokenBalance;


    private void Awake()
    {
        insta = this;
    }

    public IEnumerator GetAddressBalanaces()
    {
        yield break;

        while (true)
        {
            BalanceFetchPreURL = "https://api.covalenthq.com/v1/" + chainID + "/address/";

            using (UnityWebRequest webRequest = UnityWebRequest.Get(BalanceFetchPreURL + SingletonDataManager.userethAdd + BalanceFetchPostURL))
            {
                webRequest.timeout = 10;
                yield return webRequest.SendWebRequest();
                switch (webRequest.result)
                {
                    case UnityWebRequest.Result.ConnectionError:
                        break;
                    case UnityWebRequest.Result.DataProcessingError:
                        Debug.LogError("Error: " + webRequest.error);
                        break;
                    case UnityWebRequest.Result.ProtocolError:
                        Debug.LogError("HTTP Error: " + webRequest.error);
                        break;
                    case UnityWebRequest.Result.Success:
                        Debug.Log("Received: " + webRequest.downloadHandler.text);

                        JSONObject _data = new JSONObject(webRequest.downloadHandler.text);

                        if (_data.GetField("data").HasField("items"))
                        {
                            for (int i = 0; i < _data.GetField("data").GetField("items").list.Count; i++)
                            {
                                var _add = _data.GetField("data").GetField("items")[i].GetField("native_token").boolValue;
                                var temp2 = _data.GetField("data").GetField("items")[i].GetField("contract_address").stringValue.ToLower();

                                if (_add)
                                {
                                    //usrMainBalance = Math.Round((double)UnitConversion.Convert.FromWei(BigInteger.Parse(_data.GetField("data").GetField("items")[i].GetField("balance").stringValue)), 4).ToString();
                                }

                                if (BlockchainManager.contractToken.ToLower().Equals(temp2))
                                {
                                   // usrTokenBalance = Math.Round((double)UnitConversion.Convert.FromWei(BigInteger.Parse(_data.GetField("data").GetField("items")[i].GetField("balance").stringValue)), 4).ToString();

                                }
                            }
                        }
                        break;
                }


            }
            yield return new WaitForSeconds(UnityEngine.Random.Range(11, 30));
        }
    }

}
