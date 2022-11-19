using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

public class PathHelpler : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        for (int i = 0; i < transform.childCount-1; i++)
        {
            Gizmos.color = new Color(1, 0, 0,(float) i+1 / (float)transform.childCount);
            Gizmos.DrawLine(transform.GetChild(i).position, transform.GetChild(i + 1).position);
                       
            Handles.Label(transform.GetChild(i).position, i.ToString());
        }
    }
#endif
}
