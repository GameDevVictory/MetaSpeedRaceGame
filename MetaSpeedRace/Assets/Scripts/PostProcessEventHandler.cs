using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class PostProcessEventHandler : MonoBehaviour
{

    [SerializeField] Volume volume;
    [SerializeField] MotionBlur motionBlur;



 

    private void OnEnable()
    {

        PrometeoCarController.OnNitroUsed += HandleBoost;
    }
    private void OnDisable()
    {
        PrometeoCarController.OnNitroUsed -= HandleBoost;
    }

    private void HandleBoost(bool usingNitro)
    {
        if (usingNitro)
        {
          //  Debug.Log(usingNitro + " 1");
            
            if (volume.profile.TryGet<MotionBlur>(out MotionBlur b)) {
               // Debug.Log(usingNitro + " 2");
                motionBlur = b;
                b.intensity.Override(Mathf.Lerp(b.intensity.value, 0.3f, Time.deltaTime));               
            }

           /* if (volume.profile.TryGet<ChromaticAberration>(out ChromaticAberration ca))
            {
                ca.intensity.Override(Mathf.Lerp(ca.intensity.value,1,Time.deltaTime)); 
            }*/

        }
        else
        {
           // Debug.Log(usingNitro + " 1");
            if (volume.profile.TryGet<MotionBlur>(out MotionBlur b))
            {
               // Debug.Log(usingNitro + " 2");
                motionBlur = b;
                b.intensity.Override(Mathf.Lerp(b.intensity.value,0, Time.deltaTime));

            }
            /*if (volume.profile.TryGet<ChromaticAberration>(out ChromaticAberration ca))
            {
                ca.intensity.Override(Mathf.Lerp(ca.intensity.value,0, Time.deltaTime));
            }*/
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

  
}
