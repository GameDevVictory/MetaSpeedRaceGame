using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackDetection : MonoBehaviour
{
    [SerializeField] PhotonView pv;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log(other.gameObject.name);
        if (pv.IsMine)
        {
            if ( other.TryGetComponent<PhotonView>(out PhotonView p_view))
            {
                if (pv.Owner != p_view.Owner)
                {
                    
                    if (other.TryGetComponent<ThirdPersonController>(out ThirdPersonController otherChar))
                    {

                            AudioManager.Instance.playSound(UnityEngine.Random.Range(8, 13));
                            this.gameObject.SetActive(false);
                            pv.RPC("AttackRecieved", p_view.Owner, other.gameObject.GetComponent<Collider>().ClosestPointOnBounds(transform.position));
                        
                    }
                   
                }
            }
            else
            {
                if (other.TryGetComponent<NPC_AI>(out NPC_AI npc))
                {

                    
                    this.gameObject.SetActive(false);
                    npc.Die(true);

                }
            }
        }
    }

   

}
