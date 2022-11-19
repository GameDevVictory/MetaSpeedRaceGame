using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
public class NPC_AI : MonoBehaviour
{
    [SerializeField] CapsuleCollider npc_collider;
    [SerializeField] Animator animator;
    [SerializeField] NavMeshAgent agent;
    [SerializeField] Transform dest;
    [SerializeField] Transform start_point;
    public NPC_TYPE type;

    private NPC_TYPE prev_type;

    [SerializeField] GameObject[] clothes1;
    [SerializeField] GameObject[] clothes2;
    [SerializeField] GameObject[] clothes3;

    [SerializeField] float walk_speed;
    [SerializeField] float run_speed;

    [SerializeField] int group_path;

    [SerializeField] Rigidbody[] rbs;
    [SerializeField] CharacterJoint[] joints;

    [SerializeField] GameObject coin;

    bool died = false;
    // Start is called before the first frame update

    public void SetGroupIndex(int index)
    {
        group_path = index;

        if (type != NPC_TYPE.IDLE)
        {
            ResetDestination();
        }  
    }
    void Start()
    {

        //group_path = UnityEngine.Random.Range(0, MetaManager.Instance.footpath_poz.Length);
        EnableRandomClothes();
        type =(NPC_TYPE)UnityEngine.Random.Range(0, 3);

        switch (type)
        {
            case NPC_TYPE.IDLE:
                {
                    animator.SetFloat("Speed", 0);
                    break;
                }
            case NPC_TYPE.WALK:
                {
                   
                    animator.SetFloat("Speed", 1);
                    agent.speed = walk_speed;
                    
                    break;
                }
            case NPC_TYPE.RUN:
                {                  
                    animator.SetFloat("Speed", 2);
                    agent.speed = run_speed;
                    
                    break;
                }
        }

        rbs = GetComponentsInChildren<Rigidbody>();
        joints = GetComponentsInChildren<CharacterJoint>();

        EnableAnimator();
    }

    internal void Die(bool isMine=false)
    {
        if (!died)
        {
            if (isMine)
            {               
                AudioManager.Instance.playSound(UnityEngine.Random.Range(8, 13));
            }
            died = true;
            EnableRagdoll();
            StartCoroutine(ResetNPC());
        }
    }

    private void EnableAnimator()
    {
        npc_collider.enabled = true;
        animator.enabled = true;
        agent.enabled = true;


        for (int i = 0; i < joints.Length; i++)
        {            
            joints[i].enableCollision = false;
           
        }

        for (int i = 0; i < rbs.Length; i++)
        {
            rbs[i].velocity = Vector3.zero;
            rbs[i].useGravity = false;
            //rbs[i].isKinematic = true;
        }
        
    }
    private void EnableRagdoll()
    {
        agent.enabled = false;
        npc_collider.enabled = false;
      
        animator.enabled = false;


        for (int i = 0; i < rbs.Length; i++)
        {


            rbs[i].useGravity = true;
            rbs[i].isKinematic = false;
        }

        for (int i = 0; i < joints.Length; i++)
        {
            joints[i].enableCollision = true;

        }

        

    }

    private void EnableRandomClothes()
    {
        int random = UnityEngine.Random.Range(0, 3);
        switch (random)
        {
            case 0: {
                    foreach(GameObject g in clothes1)
                    {
                        g.SetActive(true);
                    }
                    break;
                }
            case 1:
                {
                    foreach (GameObject g in clothes2)
                    {
                        g.SetActive(true);
                    }
                    break;
                }
            case 2:
                {
                    foreach (GameObject g in clothes3)
                    {
                        g.SetActive(true);
                    }
                    break;
                }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (died) return;


        switch (type)
        {
            case NPC_TYPE.IDLE:
                {
                    
                    break;
                }
            case NPC_TYPE.WALK:
                {
                    if (agent.remainingDistance < 0.05f)
                    {
                        ResetDestination();
                    }
                    break;
                }
            case NPC_TYPE.RUN:
                {
                    if (agent.remainingDistance < 0.05f)
                    {
                        ResetDestination();
                    }
                    break;
                }
        }
    }

    private void ResetDestination()
    {
        if (!died)
        {
            dest = MetaManager.Instance.footpath_poz[group_path].pozs[UnityEngine.Random.Range(0, MetaManager.Instance.footpath_poz[group_path].pozs.Length)];
            agent.SetDestination(dest.position);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.CompareTag("car") )
        {
            if (other.GetComponentInParent<PrometeoCarController>().carSpeed > 5)
            {
                bool isMine = other.GetComponentInParent<PhotonView>().IsMine;
               
                Die(isMine);
            }
            else
            {
                ChangeStateToIdle();
            }
        }
        
    }

    private void ChangeStateToIdle()
    {
        if (type == NPC_TYPE.WALK || type == NPC_TYPE.RUN)
        {
            agent.isStopped = true;
            prev_type = type;
            type = NPC_TYPE.IDLE;
            animator.SetFloat("Speed", 0);

            AudioManager.Instance.playSound(UnityEngine.Random.Range(8, 13));

            StartCoroutine(ResetPrevState());
        }
    }
    IEnumerator ResetPrevState()
    {
        yield return new WaitForSeconds(3f);
        type = prev_type;

        switch (type)
        {
            case NPC_TYPE.IDLE:
                {
                    animator.SetFloat("Speed", 0);
                    break;
                }
            case NPC_TYPE.WALK:
                {
                    animator.SetFloat("Speed", 1);
                    agent.speed = walk_speed;
                    agent.isStopped = false;
                    agent.SetDestination(dest.position);
                    break;
                }
            case NPC_TYPE.RUN:
                {
                    animator.SetFloat("Speed", 2);
                    agent.speed = run_speed;
                    agent.isStopped = false;
                    agent.SetDestination(dest.position);
                    break;
                }
        }
        


    }

    IEnumerator ResetNPC()
    {

        yield return new WaitForSeconds(3f);

        Instantiate(coin,this.transform.position+Vector3.up*1f,Quaternion.identity);

       
        int groupId = UnityEngine.Random.Range(0, MetaManager.Instance.footpath_poz.Length);
        SetGroupIndex(groupId);
        this.transform.position = MetaManager.Instance.footpath_poz[groupId].pozs[UnityEngine.Random.Range(0, MetaManager.Instance.footpath_poz[groupId].pozs.Length)].position;        
        
        died = false;
       
        EnableAnimator();
        if (type == NPC_TYPE.WALK || type == NPC_TYPE.RUN)
        {

            ResetDestination();

        }
    }
    public enum NPC_TYPE
    {
        IDLE,WALK,RUN
    }
}