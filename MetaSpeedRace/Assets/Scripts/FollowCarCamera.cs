using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCarCamera : MonoBehaviour
{

    [SerializeField] bool cam_enabled=false;
    [SerializeField] Camera top_camera;
    [SerializeField] float y_offset;
    [SerializeField] Transform player;
    [SerializeField] Transform car;
    [SerializeField] ThirdPersonController playerController;
    [SerializeField] PrometeoCarController carController;

    public float x_rot;

    [SerializeField] float min_size;
    [SerializeField] float max_size;
    [SerializeField] float default_size;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void EnableCamera(Transform t_car,Transform t_player)
    {
        
        car = t_car;
        player = t_player;
        playerController = player.GetComponent<ThirdPersonController>();
        carController = car.GetComponent<PrometeoCarController>();
        cam_enabled = true;

    }
    // Update is called once per frame
    private void LateUpdate()
    {
        if (cam_enabled && playerController!=null)
        {
            
            switch (playerController._pState)
            {
                case PlayerState.WORLD:
                    {
                       // Debug.Log("WORLD");
                        top_camera.orthographicSize = default_size;
                        top_camera.transform.position = new Vector3(player.transform.position.x, player.transform.position.y+y_offset, player.transform.position.z);
                        top_camera.transform.rotation = Quaternion.Euler(x_rot,player.transform.rotation.eulerAngles.y,0);
                        break;
                    }
                case PlayerState.RACE:
                    {
                        //Debug.Log("CAR");
                        top_camera.transform.position = new Vector3(car.transform.position.x, car.transform.position.y + y_offset, car.transform.position.z);
                        top_camera.transform.rotation = Quaternion.Euler(x_rot, car.transform.rotation.eulerAngles.y, 0);

                        float size = min_size + (carController.carSpeed/carController.maxSpeed) * max_size;
                        float camera_speed = (size < top_camera.orthographicSize) ? 10 : 1;
                        top_camera.orthographicSize =Mathf.Lerp(top_camera.orthographicSize, Mathf.Clamp(size, min_size, max_size),Time.deltaTime * camera_speed);
                        break;
                    }
            }
        }
    }
}
