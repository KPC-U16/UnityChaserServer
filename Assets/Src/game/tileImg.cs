using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;
using System;

public class tileImg : MonoBehaviour
{
    SpriteRenderer m_image;
    Animator animator;
    Sprite[] texture;
    bool animatoin = false;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void LateUpdate()
    {
        m_image.size = new Vector2(1,1);
    }

    public void ImgChange(string type)
    {
        m_image = GetComponent<SpriteRenderer>();

        switch (type)
        {
            case "none":
                if (animatoin)
                {
                    animator.SetBool("isItem",false);
                }
                else
                {
                    m_image.sprite = texture[0];
                }
                break;
            case "block":
                if (animatoin)
                {
                    animator.SetTrigger("BlockGen");
                }
                else
                {
                    m_image.sprite = texture[1];
                }
                break;
            case "item":
                if (animatoin)
                {
                    animator.SetBool("isItem",true);
                }
                else
                {
                    m_image.sprite = texture[2];
                }
                break;
        }
    }

    public void SetView(RuntimeAnimatorController[] animators,Sprite[] tex)
    {
        if (animators == null)
        {
            texture = new Sprite[tex.Length];
            Array.Copy(tex,texture,tex.Length);
        }
        else
        {
            animator = GetComponent<Animator>();
            animator.runtimeAnimatorController = animators[0];
            animatoin = true;
        }
    }
}
