using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;


    [SerializeField] AudioSource bgSource;    
    [SerializeField] AudioSource soundSource;

    [SerializeField] AudioClip[] audioClips;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else {
            Destroy(gameObject);
            return;
        }
    }

    public void playSound(int _no, float _vol = 1f) {
        //if (soundSource.isPlaying) soundSource.Stop();
        soundSource.PlayOneShot(audioClips[_no], _vol);
    }
  
}
