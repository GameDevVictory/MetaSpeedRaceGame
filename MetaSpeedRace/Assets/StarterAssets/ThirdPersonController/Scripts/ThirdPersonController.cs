using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using Random = UnityEngine.Random;


[RequireComponent(typeof(CharacterController))]
public class ThirdPersonController : MonoBehaviourPunCallbacks,IPunObservable
{
    [Header("Player")]
    [Tooltip("Move speed of the character in m/s")]
    public float MoveSpeed = 2.0f;

   

    [Tooltip("Sprint speed of the character in m/s")]
    public float SprintSpeed = 5.335f;
    [Tooltip("How fast the character turns to face movement direction")]
    [Range(0.0f, 0.3f)]
    public float RotationSmoothTime = 0.12f;
    [Tooltip("Acceleration and deceleration")]
    public float SpeedChangeRate = 10.0f;   

    [Space(10)]
    [Tooltip("The height the player can jump")]
    public float JumpHeight = 1.2f;
    [Tooltip("The character uses its own gravity value. The engine default is -9.81f")]
    public float Gravity = -15.0f;
    [Space(10)]
    [Tooltip("Time required to pass before being able to jump again. Set to 0f to instantly jump again")]
    public float JumpTimeout = 0.50f;
    [Tooltip("Time required to pass before entering the fall state. Useful for walking down stairs")]
    public float FallTimeout = 0.15f;

    [Header("Player Grounded")]
    [Tooltip("If the character is grounded or not. Not part of the CharacterController built in grounded check")]
    public bool Grounded = true;
    [Tooltip("Useful for rough ground")]
    public float GroundedOffset = -0.14f;
    [Tooltip("The radius of the grounded check. Should match the radius of the CharacterController")]
    public float GroundedRadius = 0.28f;
    [Tooltip("What layers the character uses as ground")]
    public LayerMask GroundLayers;
    public LayerMask CheckBuildings;

    [Header("Cinemachine")]
    [Tooltip("The follow target set in the Cinemachine Virtual Camera that the camera will follow")]
    public GameObject CinemachineCameraTarget;
    [Tooltip("How far in degrees can you move the camera up")]
    public float TopClamp = 70.0f;
    [Tooltip("How far in degrees can you move the camera down")]
    public float BottomClamp = -30.0f;
    [Tooltip("Additional degress to override the camera. Useful for fine tuning camera position when locked")]
    public float CameraAngleOverride = 0.0f;
    [Tooltip("For locking the camera position on all axis")]
    public bool LockCameraPosition = false;

    // cinemachine
    private float _cinemachineTargetYaw;
    private float _cinemachineTargetPitch;

    // player
    private float _speed;
    private float _animationBlend;
    private float _targetRotation = 0.0f;
    private float _rotationVelocity;
    private float _verticalVelocity;
    private float _terminalVelocity = 53.0f;
    [SerializeField] float _cameraSensitivity;

    // timeout deltatime
    private float _jumpTimeoutDelta;
    private float _fallTimeoutDelta;

    // animation IDs
    private int _animIDSpeed;
    private int _animIDGrounded;

    

    private int _animIDJump;
    private int _animIDFreeFall;
    private int _animIDMotionSpeed;

    private int _animIDAttack;
    private int _animboolAttack;


    [SerializeField] private Animator _animator;
    [SerializeField] private CharacterController _controller;
    [SerializeField] private InputManager _input;
    private GameObject _mainCamera;
    private bool _rotateOnMove = true;
    private const float _threshold = 0.01f;

    


    #region Events
    public static Action<bool,PrometeoCarController> OnPlayerTriggerCar;    
    public static Action OnPlayerEnteredCar;
    public static Action OnPlayerExitedCar;
    public static Action<PlayerState> OnPlayerStateChanged;
    #endregion


    [SerializeField] GameObject playerBody;

    [Header("Player Race Area")]
    [SerializeField] PlayerState playerState = PlayerState.WORLD;
    public PlayerState _pState { get { return playerState; } }
    [SerializeField] PrometeoCarController playerCar;
    [SerializeField] GameObject playerRenderer;
    public bool isPlayerEnable = true;


    [Header("Photon Mine Check")]
    [SerializeField] PhotonView pv;
    [SerializeField] SpriteRenderer[] top_markeres;
    [SerializeField] GameObject[] characters;
    [SerializeField] int char_no;



    [SerializeField] GameObject meetUI;
    [SerializeField] TMPro.TMP_Text usernameText;
    [SerializeField] GameObject meetCollider;

    public int test_car_id;
    private void Awake()
    {
        //Default Methods for all

        char_no =Int32.Parse(pv.Owner.CustomProperties["char_no"].ToString());
        characters[char_no].SetActive(true);
        usernameText.text = pv.Owner.NickName;
        playerBody = characters[char_no];
        //Mine Player methods
        if (pv.IsMine)
        {
            // get a reference to our main camera
            if (_mainCamera == null)
            {
                _mainCamera = MetaManager.Instance.mainCamera;
            }
            Cursor.lockState = CursorLockMode.Locked;
            
            _animator= characters[char_no].GetComponent<Animator>();
            _input = MetaManager.Instance.inputManager;
            meetCollider.SetActive(true);

            for (int i = 0; i < top_markeres.Length; i++)
            {
                top_markeres[i].color = new Color(0, 1, 0, 1);
            }
            

            int car_index = 1;

            LocalData data= DatabaseManager.Instance.GetLocalData();

            
            if (data != null)
            {
                car_index = data.selected_car;
            }
            usernameText.gameObject.SetActive(false);

            int RandomPoz = Random.Range(0, MetaManager.Instance.carPoz.Length);
            GameObject car = PhotonNetwork.Instantiate("Cars/Car " + car_index.ToString(), MetaManager.Instance.carPoz[RandomPoz].position, MetaManager.Instance.carPoz[RandomPoz].rotation);

            

            playerCar = car.GetComponent<PrometeoCarController>();
            MetaManager.Instance.SetMyPlayer(this, playerCar);

            UIManager.Instance.SetCoinText();
            //playerCar.transform.position = MetaManager.Instance.carPoz[Random.Range(0, MetaManager.Instance.carPoz.Length)].position;


        }
    }

    private void Start()
    {
        if (pv.IsMine)
        {
            _cinemachineTargetYaw = CinemachineCameraTarget.transform.rotation.eulerAngles.y;           
            

            AssignAnimationIDs();

            // reset our timeouts on start
            _jumpTimeoutDelta = JumpTimeout;
            _fallTimeoutDelta = FallTimeout;
        }
    }


    #region Animation Callbacks
    public void AttackComplete()
    {
        TogglePlayerMovement(true);
        isAttacking = false;
    }

    public void EmoteComplete()
    {
        isDoingEmote = false;
        TogglePlayerMovement(true);
    }
    #endregion
    public int selected_emote=-1;
    public bool isDoingEmote=false;
    public bool isAttacking = false;
    private void Update()
    {

        if (!pv.IsMine) return;
        if (!_input) return;


/*#if UNITY_EDITOR
        if (playerState == PlayerState.WORLD)
        {
            if (Input.GetKeyDown(KeyCode.H))
            {
                test_car_id++;
                if (test_car_id >= 13)
                {
                    test_car_id = 1;
                }
                ChangeCar(test_car_id);
            }
        }
#endif*/

        if (Input.GetMouseButtonDown(0))
        {
            if (Cursor.lockState == CursorLockMode.None)
            {
                if (!EventSystem.current.IsPointerOverGameObject())
                {
                    Cursor.lockState = CursorLockMode.Locked;
               }             
            }
            else
            {
                Cursor.lockState = CursorLockMode.None;
            }
        }



        //MOUSE ENABLE DISABLE



        if (_input.CheckLeftAlt())
        {
            if (Cursor.lockState == CursorLockMode.Locked)
            {
                Cursor.lockState = CursorLockMode.None;
            }
            else
            {
                Cursor.lockState = CursorLockMode.Locked;
            }
        }

        if (playerState == PlayerState.WORLD)
        {

            JumpAndGravity();
            GroundedCheck();
            Move();

            #region Emotes
            if (_input.GetEmoteButton())
            {
                Cursor.lockState = CursorLockMode.None;
                UIManager.Instance.ToggleEmoteUI(true);
                //isEmoteUIOpened = true;

                can_move = false;
                
                _animator.SetFloat(_animIDSpeed, 0);
            }
            else
            {                
                UIManager.Instance.ToggleEmoteUI(false);
                //isEmoteUIOpened = false;
                
                if (selected_emote != -1 && Grounded)
                {
                    isDoingEmote = true;
                    _animator.SetFloat("EmoteID", selected_emote);
                    _animator.SetBool("Emote", true);
                    StartCoroutine(changeEmoteBool());
                }
                else
                {
                    TogglePlayerMovement(true);
                }
                
            }
            #endregion


            if (_input.GetMouseLeftButton() && can_move && !isDoingEmote && !isAttacking && !ifUIItemIsHit())
            {

                TogglePlayerMovement(false);

                isAttacking = true;
                _animator.SetFloat(_animIDAttack, (int)UnityEngine.Random.Range(0, 3));
                _animator.SetBool(_animboolAttack, true);
                StartCoroutine(ResetAttack());
            }
            //CHECK FOR F Button
            if (_input.GetInteractButton())
            {
                if (MetaManager.carNearby)
                {
                    if(!MetaManager.Instance.nearby_car_controller.GetComponent<PhotonView>().IsMine)
                    {
                        UIManager.Instance.ShowInformationMsg("Cannot enter in other's car!", 2f);
                        return;
                       
                    }
                    ChangePlayerState(PlayerState.RACE);
                    OnPlayerEnteredCar?.Invoke();
                }               
            }

            
        }
        else if(playerState==PlayerState.RACE)
        {
            GetCarInputs();

           

            if (_input.CheckMouseMiddleButton())
            {                
                MetaManager.Instance.ChangeCamera(CameraType.BACKCAR);                
            }
            else 
            {               
                MetaManager.Instance.ChangeCamera(CameraType.CAR);                
            }

            if (_input.GetInteractButton())
            {

                if (MetaManager.Instance.inRace)
                {
                    if (MetaManager.Instance.inChallengePlayer != null)
                    {
                        UIManager.Instance.ShowInformationMsg("Cannot exit during race!", 2f);
                    }
                    else
                    {
                        if (playerCar.carSpeed < 6f)
                        {

                            if (!Physics.Raycast(playerCar.transform.position + Vector3.up * 2, -playerCar.transform.right, 3, CheckBuildings))
                            {
                                playerCar.Stop();
                                this.transform.position = playerCar.transform.position - playerCar.transform.right * 3f;
                                this.transform.rotation = playerCar.transform.rotation;
                                RaceManager.Instance.ResetRaceSettings();
                                ChangePlayerState(PlayerState.WORLD);
                            }
                            else
                            {
                                UIManager.Instance.ShowInformationMsg("Area Not Parkable!", 2f);
                            }
                        }
                        else
                        {
                            UIManager.Instance.ShowInformationMsg("Stop the car then exit ! You Want to Die?", 2f);
                        }
                    }
                }
                else
                {
                    if (playerCar.carSpeed < 6f)
                    {

                        if (!Physics.Raycast(playerCar.transform.position + Vector3.up*2, -playerCar.transform.right,3, CheckBuildings)){
                            playerCar.Stop();
                            this.transform.position = playerCar.transform.position - playerCar.transform.right * 3f;
                            this.transform.rotation = playerCar.transform.rotation;
                            ChangePlayerState(PlayerState.WORLD);
                        }
                        else
                        {
                            UIManager.Instance.ShowInformationMsg("Area Not Parkable!", 2f);
                        }
                    }
                    else
                    {
                        UIManager.Instance.ShowInformationMsg("Stop the car then exit ! You Want to Die?", 2f);
                    }
                }

            }
        }

      

    }

    IEnumerator changeEmoteBool()
    {
        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();
        _animator.SetBool("Emote", false);
        selected_emote = -1;
    }

    public GameObject WeaponCollider;
    [SerializeField] private bool can_move=true;

    [PunRPC]
    public void AttackRecieved(Vector3 triggerPoint)
    {
        MetaManager.Instance.myPlayer.GetComponent<ThirdPersonController>().GotAttack(this.transform.position, triggerPoint);
    }
    public void GotAttack(Vector3 position, Vector3 triggerPoint)
    {
        can_move = false;
        //play Hurt Sound
        AudioManager.Instance.playSound(Random.Range(8,13));

        TogglePlayerMovement(false);
        _animator.SetBool(_animboolAttack, true);
        _animator.SetFloat(_animIDAttack, 3);

        GameObject go = PhotonNetwork.Instantiate("HitParticle", triggerPoint, Quaternion.identity);

        StartCoroutine(resetHitImpact(go.GetComponent<PhotonView>()));


        /*impact = (this.transform.position - position);
        LeanTween.move(MetaManager.Instance.tempObject, Vector3.zero, 0.3f).setFrom(impact).setEase(punchEffect).setOnUpdate((Vector3 pos) => {
            impact = pos;
        }).setOnComplete(() => {
            impact = Vector3.zero;
        });*/
    }
    IEnumerator resetHitImpact(PhotonView pv)
    {
        yield return new WaitForSeconds(0.1f);

        _animator.SetBool(_animboolAttack, false);
        
        TogglePlayerMovement(true);

        yield return new WaitForSeconds(1f);

        if (pv != null)
        {
            PhotonNetwork.Destroy(pv);
        }
        

    }

    public void TogglePlayerMovement(bool enable)
    {
        can_move = enable;
        
    }
    IEnumerator ResetAttack()
    {
        yield return new WaitForSeconds(0.1f);
        WeaponCollider.SetActive(true);
        _animator.SetBool(_animboolAttack, false);
        yield return new WaitForSeconds(0.1f);
        AudioManager.Instance.playSound(7);
        WeaponCollider.SetActive(false);
        yield return new WaitForSeconds(0.4f);
        isAttacking = false;
        TogglePlayerMovement(true);

    }

    private void ChangePlayerState(PlayerState _newState)
    {
        playerState = _newState;
        if (_newState == PlayerState.RACE)
        {
            TogglePlayer(false);
            UIManager.Instance.ToggleBoostUI(true);
            MetaManager.Instance.ChangeCamera(CameraType.CAR);
        }
        else
        {

            Debug.Log("Test 1");
            TogglePlayer(true);
            MetaManager.Instance.inRace = false;
            MetaManager.Instance.UpdatePlayerWorldProperties(false);
            UIManager.Instance.ToggleBoostUI(false);
            MetaManager.Instance.ChangeCamera(CameraType.TP);
            MetaManager.Instance.myCarController.Stop();

        }

        OnPlayerStateChanged?.Invoke(playerState);
    }


    #region Car Controll Management
    void GetCarInputs()
    {
        if (playerState == PlayerState.RACE)
        {
            playerCar.gameObject.SetActive(true);          
            
        }
    }
    #endregion

    private void LateUpdate()
    {
        if (!pv.IsMine)
        {
            usernameText.transform.LookAt(MetaManager.Instance.mainCamera.transform);
            usernameText.transform.rotation = Quaternion.LookRotation(MetaManager.Instance.mainCamera.transform.forward);

            meetUI.transform.LookAt(MetaManager.Instance.mainCamera.transform);
            meetUI.transform.rotation = Quaternion.LookRotation(MetaManager.Instance.mainCamera.transform.forward);

            return;
        }

        if (playerState == PlayerState.WORLD)
        {
            CameraRotation();
        }
    }


    #region Car CHANGE RUNTIME
    public void ChangeCar(int carIndex)
    {
        if (!pv.IsMine) return;

        if (playerCar != null)
        {
            GameObject car = PhotonNetwork.Instantiate("Cars/Car " + carIndex.ToString(), playerCar.transform.position,playerCar.transform.rotation);
            PhotonNetwork.Destroy(playerCar.gameObject);
            playerCar = car.GetComponent<PrometeoCarController>();
            MetaManager.Instance.SetMyPlayer(this, playerCar);
        }
    }
    #endregion

    #region Evnet Action Handling
    private void OnEnable()
    {
        if (!pv.IsMine) return;

        this.GetComponent<ChallengeRequestManager>().OnPlayerAcceptedChallenge += HandleChallenge;
       
        PhotonNetwork.AddCallbackTarget(this);

    }

    

    private void OnDisable()
    {
        if (!pv.IsMine) return;
        this.GetComponent<ChallengeRequestManager>().OnPlayerAcceptedChallenge -= HandleChallenge;        
        PhotonNetwork.RemoveCallbackTarget(this);
    }

    Vector3 playerLastPoz;
    Quaternion playerLastRot;

    Vector3 carLastPoz;
    Quaternion carLastRot;
    private void HandleChallenge(int racePozIndex,int racePathIndex, int subPoz)
    {
        playerLastPoz = this.transform.position;
        playerLastRot = this.transform.rotation;

        carLastPoz = playerCar.transform.position;
        carLastRot = playerCar.transform.rotation;
        //RaceManager.Instance.SetEnvironLocation(raceEnvPozIndex);

        ChangePlayerState(PlayerState.RACE);

        playerCar.transform.position = RaceManager.Instance.raceStartPoz[racePozIndex].GetChild(subPoz).position;
        playerCar.transform.rotation = RaceManager.Instance.raceStartPoz[racePozIndex].GetChild(subPoz).rotation;

        RaceManager.Instance.SetCurrentPath(racePathIndex);

        MetaManager.Instance.UpdatePlayerWorldProperties(true);

        

        StartCoroutine(RaceCountDown());
    }
    IEnumerator RaceCountDown()
    {
        playerCar.Stop();

        float timer = 3;
        while (timer > 0)
        {
            AudioManager.Instance.playSound(1);
            UIManager.Instance.ShowInformationMsg(timer.ToString(), 1);
            yield return new WaitForSeconds(1);
            timer -= 1;
        }

        AudioManager.Instance.playSound(2);
        UIManager.Instance.ShowInformationMsg("GO!!", 3f);

        Transform firstPoint = RaceManager.Instance.current_race_path_checkpoints[0];
        Transform secondPoint = RaceManager.Instance.current_race_path_checkpoints[1];
        CheckPointsManager.Instance.SetCheckPoint(firstPoint,secondPoint);

        playerCar.StartRace();

    }
    #endregion
    public IEnumerator ResetPlayerPozRot()
    {
        yield return new WaitForSeconds(1);
        this.transform.position = playerLastPoz;
        this.transform.rotation = playerLastRot;

        playerCar.transform.position = carLastPoz;
        playerCar.transform.rotation = carLastRot;
    }

    #region Character Physics
    private void GroundedCheck()
    {
        // set sphere position, with offset
        Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y - GroundedOffset,
            transform.position.z);
        Grounded = Physics.CheckSphere(spherePosition, GroundedRadius, GroundLayers,
            QueryTriggerInteraction.Ignore);

        // update animator if using character
        
        _animator.SetBool(_animIDGrounded, Grounded);
        
    }
    private void CameraRotation()
    {
        // if there is an input and camera position is not fixed
        if (_input.GetMouseDelta().sqrMagnitude >= _threshold && !LockCameraPosition)
        {
            //Don't multiply mouse input by Time.deltaTime;
            float deltaTimeMultiplier =_cameraSensitivity;

            _cinemachineTargetYaw += _input.GetMouseDelta().x * deltaTimeMultiplier ;
            _cinemachineTargetPitch += _input.GetMouseDelta().y * deltaTimeMultiplier;
        }

        // clamp our rotations so our values are limited 360 degrees
        _cinemachineTargetYaw = ClampAngle(_cinemachineTargetYaw, float.MinValue, float.MaxValue);
        _cinemachineTargetPitch = ClampAngle(_cinemachineTargetPitch, BottomClamp, TopClamp);

        // Cinemachine will follow this target
        CinemachineCameraTarget.transform.rotation = Quaternion.Euler(_cinemachineTargetPitch + CameraAngleOverride,
            _cinemachineTargetYaw, 0.0f);
    }
    private void Move()
    {


        if (isDoingEmote && _input.GetPlayerMovement().magnitude > 0)
        {
            isDoingEmote = false;
            selected_emote = -1;
            can_move = true;
            _animator.Play("Base Layer.Movement", 0, 0);
        }

        if (!can_move) return;

        // set target speed based on move speed, sprint speed and if sprint is pressed
        float targetSpeed = _input.ISSpriting() ? SprintSpeed : MoveSpeed;

        // a simplistic acceleration and deceleration designed to be easy to remove, replace, or iterate upon

        // note: Vector2's == operator uses approximation so is not floating point error prone, and is cheaper than magnitude
        // if there is no input, set the target speed to 0
        if (_input.GetPlayerMovement() == Vector2.zero) targetSpeed = 0.0f;

        // a reference to the players current horizontal velocity
        float currentHorizontalSpeed = new Vector3(_controller.velocity.x, 0.0f, _controller.velocity.z).magnitude;

        float speedOffset = 0.1f;
        float inputMagnitude = _input.GetPlayerMovement().magnitude;

        // accelerate or decelerate to target speed
        if (currentHorizontalSpeed < targetSpeed - speedOffset ||
            currentHorizontalSpeed > targetSpeed + speedOffset)
        {
            // creates curved result rather than a linear one giving a more organic speed change
            // note T in Lerp is clamped, so we don't need to clamp our speed
            _speed = Mathf.Lerp(currentHorizontalSpeed, targetSpeed * inputMagnitude,
                Time.deltaTime * SpeedChangeRate);

            // round speed to 3 decimal places
            _speed = Mathf.Round(_speed * 1000f) / 1000f;
        }
        else
        {
            _speed = targetSpeed;
        }

        _animationBlend = Mathf.Lerp(_animationBlend, targetSpeed, Time.deltaTime * SpeedChangeRate);
        if (_animationBlend < 0.01f) _animationBlend = 0f;

        // normalise input direction
        Vector3 inputDirection = new Vector3(_input.GetPlayerMovement().x, 0.0f, _input.GetPlayerMovement().y).normalized;

        // note: Vector2's != operator uses approximation so is not floating point error prone, and is cheaper than magnitude
        // if there is a move input rotate player when the player is moving
        if (_input.GetPlayerMovement() != Vector2.zero)
        {
            _targetRotation = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg +
                              _mainCamera.transform.eulerAngles.y;
            float rotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, _targetRotation, ref _rotationVelocity,
                RotationSmoothTime);

            if (_rotateOnMove)
            {
                // rotate to face input direction relative to camera position
                transform.rotation = Quaternion.Euler(0.0f, rotation, 0.0f);
            }
        }


        Vector3 targetDirection = Quaternion.Euler(0.0f, _targetRotation, 0.0f) * Vector3.forward;

        // move the player
        _controller.Move(targetDirection.normalized * (_speed * Time.deltaTime) +
                         new Vector3(0.0f, _verticalVelocity, 0.0f) * Time.deltaTime);

        // update animator if using character
      
            _animator.SetFloat(_animIDSpeed, _animationBlend);
          //  _animator.SetFloat(_animIDMotionSpeed, inputMagnitude);
        
    }

    private void JumpAndGravity()
    {
        if (Grounded)
        {
            // reset the fall timeout timer
            _fallTimeoutDelta = FallTimeout;

            // update animator if using character
            
                _animator.SetBool(_animIDJump, false);
                _animator.SetBool(_animIDFreeFall, false);
            

            // stop our velocity dropping infinitely when grounded
            if (_verticalVelocity < 0.0f)
            {
                _verticalVelocity = -2f;
            }

            // Jump
            if (_input.PlayerJumpedThisFrame() && _jumpTimeoutDelta <= 0.0f)
            {
                // the square root of H * -2 * G = how much velocity needed to reach desired height
                _verticalVelocity = Mathf.Sqrt(JumpHeight * -2f * Gravity);

                // update animator if using character
                
                    _animator.SetBool(_animIDJump, true);
                
            }

            // jump timeout
            if (_jumpTimeoutDelta >= 0.0f)
            {
                _jumpTimeoutDelta -= Time.deltaTime;
            }
        }
        else
        {
            // reset the jump timeout timer
            _jumpTimeoutDelta = JumpTimeout;

            // fall timeout
            if (_fallTimeoutDelta >= 0.0f)
            {
                _fallTimeoutDelta -= Time.deltaTime;
            }
            else
            {
                // update animator if using character
               
                   _animator.SetBool(_animIDFreeFall, true);
                
            }

            // if we are not grounded, do not jump

        }

        // apply gravity over time if under terminal (multiply by delta time twice to linearly speed up over time)
        if (_verticalVelocity < _terminalVelocity)
        {
            _verticalVelocity += Gravity * Time.deltaTime;
        }
    }
    #endregion
    private void AssignAnimationIDs()
    {
        _animIDSpeed = Animator.StringToHash("Speed");
        _animIDGrounded = Animator.StringToHash("Grounded");
        _animIDJump = Animator.StringToHash("Jump");
        _animIDFreeFall = Animator.StringToHash("FreeFall");
        _animIDMotionSpeed = Animator.StringToHash("MotionSpeed");

        _animIDAttack=Animator.StringToHash("AttackID");
        _animboolAttack= Animator.StringToHash("Attack"); 
}
    private static float ClampAngle(float lfAngle, float lfMin, float lfMax)
    {
        if (lfAngle < -360f) lfAngle += 360f;
        if (lfAngle > 360f) lfAngle -= 360f;
        return Mathf.Clamp(lfAngle, lfMin, lfMax);
    }

    private void OnDrawGizmosSelected()
    {
        Color transparentGreen = new Color(0.0f, 1.0f, 0.0f, 0.35f);
        Color transparentRed = new Color(1.0f, 0.0f, 0.0f, 0.35f);

        if (Grounded) Gizmos.color = transparentGreen;
        else Gizmos.color = transparentRed;

        // when selected, draw a gizmo in the position of, and matching radius of, the grounded collider
        Gizmos.DrawSphere(
            new Vector3(transform.position.x, transform.position.y - GroundedOffset, transform.position.z),
            GroundedRadius);
    }       

    #region Setter Methods
    public void SetSensitivity(float newSensi)
    {
        _cameraSensitivity = newSensi;
    }
    public void SetRotationOnMove(bool newRotateOnMove)
    {
        _rotateOnMove = newRotateOnMove;
    }

   
    #endregion

    #region Player Enable/Disable
    public void TogglePlayer(bool state)
    {
        _controller.enabled = state;
        this.GetComponent<BasicRigidBodyPush>().enabled = state;

        if (meetUI.activeInHierarchy)
        {
            meetUI.SetActive(false);
        }
        if (!pv.IsMine)
        {
            usernameText.gameObject.SetActive(state);
        }
        playerBody.SetActive(state);

        if (pv.IsMine) isPlayerEnable = state;
    }
    #endregion

    #region Photon Callbacks
    void IPunObservable.OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
       
        if (stream.IsWriting)
        {
            stream.SendNext(isPlayerEnable);
        }
        else
        {
            isPlayerEnable = (bool)stream.ReceiveNext();
            if (_controller.enabled && !isPlayerEnable)
            {
                TogglePlayer(false);
            }
            else if (!_controller.enabled && isPlayerEnable)
            {
                TogglePlayer(true);
            }

        }
    }
    #endregion

    #region Collision/Triggers
    private void OnTriggerEnter(Collider other)
    {
        if(pv.IsMine && other.TryGetComponent<PrometeoCarController>(out PrometeoCarController car))
        {
            OnPlayerTriggerCar?.Invoke(true,car);
        }

        if (!MetaManager.Instance.inRace)
        {
            if (!pv.IsMine && (bool)pv.Owner.CustomProperties["isRacing"] == false)
            {
                if (other.CompareTag("Meet"))
                {
                    Debug.Log("Meet him");
                    meetUI.SetActive(true);
                    //virtualWorldUI.SetActive(true);
                }
            }
        }

        if (other.CompareTag("coin"))
        {
            AudioManager.Instance.playSound(6);           
            Destroy(other.gameObject);

            LocalData data = DatabaseManager.Instance.GetLocalData();
            data.coins += Random.Range(1, 8);
            DatabaseManager.Instance.UpdateData(data);
            UIManager.Instance.SetCoinText();

        }
        if (other.CompareTag("nitro"))
        {
            other.gameObject.SetActive(false);
            playerCar.current_nitroTimer = playerCar.nitroTimer;
        }

    }

    private void OnTriggerExit(Collider other)
    {
        if (pv.IsMine && other.TryGetComponent<PrometeoCarController>(out PrometeoCarController car))
        {
            OnPlayerTriggerCar?.Invoke(false, car);
        }

        if (!pv.IsMine)
        {
            if (other.CompareTag("Meet"))
            {
                Debug.Log("Meet bye");
                meetUI.SetActive(false);
                // virtualWorldUI.SetActive(false);
            }
        }
    }

    #endregion


    #region Race Management
    public void RaceOver(bool won,bool sendRPC)
    {
        bool showTokenUI = false;

        if (sendRPC )
        {
            if (MetaManager.Instance.inChallengePlayer != null)
            {
                
                pv.RPC("RPC_RaceOver", MetaManager.Instance.inChallengePlayer, false);
            }
            else
            {
                LocalData data = DatabaseManager.Instance.GetLocalData();
                data.soloRaceWon++;
                DatabaseManager.Instance.UpdateData(data);
                if (data.soloRaceWon % 1 == 0)
                {
                    showTokenUI = true;
                }

            }
        }
        if(won) showTokenUI = true;
        MetaManager.Instance.inRace = false;
        MetaManager.Instance.UpdatePlayerWorldProperties(false);
        
        Debug.Log(pv.Owner.UserId + " is womn? " + won);

        RaceManager.Instance.ResetRaceSettings();
        UIManager.Instance.ShowInformationMsg(won?"Congrats! You Won":"Hard Luck! You Lost.",3f);
        UIManager.Instance.ShowGameCompleteUI(won, MetaManager.Instance.inChallengePlayer == null, showTokenUI);

        MetaManager.Instance.inChallengePlayer = null;

    }
    
    

    [PunRPC]
    public void RPC_RaceOver(bool i_won)
    {
        RaceOver(i_won, false);        
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        //base.OnPlayerLeftRoom(otherPlayer);

        if (pv.IsMine)
        {
            if (MetaManager.Instance.inRace)
            {
                if (MetaManager.Instance.inChallengePlayer != null && otherPlayer.UserId.Equals(MetaManager.Instance.inChallengePlayer.UserId))
                {
                    RaceOver(true, false);
                }
            }
        }

    }
    #endregion

   

    private bool ifUIItemIsHit()
    {
        PointerEventData pointerEventData = new PointerEventData(EventSystem.current);
        pointerEventData.position = Input.mousePosition;

        List<RaycastResult> raycastResultList = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerEventData, raycastResultList);
        for (int i = 0; i < raycastResultList.Count; i++)
        {
            if (raycastResultList[i].gameObject.layer == 5)
            {
                return true;
            }
        }
        return false;
    }
}


public enum PlayerState
{
    WORLD,
    RACE
}