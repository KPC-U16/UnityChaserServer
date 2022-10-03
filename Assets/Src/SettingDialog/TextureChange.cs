using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;

public class TextureChange : MonoBehaviour
{
    Text text;
    public GameManager gameManager;

    public Sprite[] yugu;
    public Sprite[] anim;
    public RuntimeAnimatorController[] animator;
    // Start is called before the first frame update
    void Start()
    {
        text = gameObject.GetComponentInChildren<Text>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnValueChenged()
    {
        if (text.text == "yugu")
        {
            gameManager.SetTexture(yugu,null);
        }
        if (text.text == "animation")
        {
            gameManager.SetTexture(anim,animator);
        }
    }
}
