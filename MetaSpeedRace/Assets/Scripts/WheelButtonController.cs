using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class WheelButtonController : MonoBehaviour
{
    public int ID=-1;
    public string ItemName;
    public TMP_Text itemText;
    public Image selectedItem;
    private bool selected = false;
    public Sprite icon;
    // Start is called before the first frame update
    void Start()
    {
        
    }

  

   
    public void SetItemIndex(int index)
    {
        ID = index;
        MetaManager.Instance.myPlayerController.selected_emote = ID;
    }

    public void HoverEnter(GameObject g)
    {
        LeanTween.cancel(g);
        LeanTween.scale(g, Vector3.one * 1.15f, 0.2f);
    }
    public void HoverExit(GameObject g)
    {
        LeanTween.cancel(g);
        LeanTween.scale(g, Vector3.one, 0.2f);
    }
}
