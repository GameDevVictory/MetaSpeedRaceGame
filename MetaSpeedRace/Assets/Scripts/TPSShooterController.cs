using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;


public class TPSShooterController : MonoBehaviour
{
    [SerializeField] Camera mainCam;
    [SerializeField] ThirdPersonController playerController;
    [SerializeField] CinemachineVirtualCamera aimCamera;

    [SerializeField] InputManager _inputManager;


    [SerializeField] LayerMask aimLayerMask;
    [Header("Sensitivity")]
    [SerializeField] float normalSensi;
    [SerializeField] float AimSensi;

    [Header("Shooter Propertires")]
    [SerializeField] Animator animator;
    [SerializeField] Transform pfBulletProjectile;
    [SerializeField] Transform spawnBulletPosition;

    [Header("DEBUG")]
    [SerializeField] bool debug;
    [SerializeField] GameObject debug_poz;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 mouseWorldPosition = Vector3.zero;
        Vector2 screenCenterPoint = new Vector2(Screen.width / 2, Screen.height / 2);

        Ray ray = mainCam.ScreenPointToRay(screenCenterPoint);

      



        #region Aim Position Methods
                
        if(Physics.Raycast(ray,out RaycastHit hit, 999f, aimLayerMask))
        {
            if(debug) debug_poz.transform.position = hit.point;
            mouseWorldPosition = hit.point;
        }
        #endregion

        bool aimed = _inputManager.GetAimButton();
        aimCamera.gameObject.SetActive(aimed);
        playerController.SetRotationOnMove(!aimed);
        playerController.SetSensitivity(aimed ? AimSensi : normalSensi);

        if (aimed){
            Vector3 worldAimtarget = mouseWorldPosition;
            worldAimtarget.y = transform.position.y;
            Vector3 aimDirection = (worldAimtarget - transform.position).normalized;

            
            transform.forward = Vector3.Lerp(transform.forward, aimDirection, Time.deltaTime * 20f);
            animator.SetLayerWeight(1, Mathf.Lerp(animator.GetLayerWeight(1), 1f, Time.deltaTime * 10f));

        }
        else
        {
            animator.SetLayerWeight(1, Mathf.Lerp(animator.GetLayerWeight(1), 0, Time.deltaTime * 10f));
        }

        if (_inputManager.ShootPressed())
        {
            Vector3 aimDir = (mouseWorldPosition - spawnBulletPosition.position).normalized;
            Instantiate(pfBulletProjectile, spawnBulletPosition.position, Quaternion.LookRotation(aimDir, Vector3.up));

           
        }
    }
}
