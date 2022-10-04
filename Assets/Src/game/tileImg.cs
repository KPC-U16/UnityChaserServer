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

    public void ImgChange(string type)
    {
        m_image = GetComponent<SpriteRenderer>();

        switch (type)
        {
            case "none":
                m_image.sprite = texture[0];
                m_image.size = new Vector2(1,1);
                break;
            case "block":
                m_image.sprite = texture[1];
                m_image.size = new Vector2(1,1);
                if (animatoin)
                {
                    animator.SetTrigger("BlockGen");
                }
                break;
            case "item":
                m_image.sprite = texture[2];
                m_image.size = new Vector2(1,1);
                break;
        }
    }

    public void SetView(string type,Sprite[] tex,RuntimeAnimatorController[] animators)
    {
        texture = new Sprite[tex.Length];
        Array.Copy(tex,texture,tex.Length);

        m_image = GetComponent<SpriteRenderer>();

        switch (type)
        {
            case "none":
                m_image.sprite = texture[0];
                m_image.size = new Vector2(1,1);
                break;
            case "block":
                m_image.sprite = texture[1];
                m_image.size = new Vector2(1,1);
                break;
            case "item":
                m_image.sprite = texture[2];
                m_image.size = new Vector2(1,1);
                break;
        }

        if (animators != null)
        {
            animator = GetComponent<Animator>();
            animator.runtimeAnimatorController = animators[1];
            animatoin = true;
        }

        if (animatoin && type == "block")
        {
            animator.SetTrigger("BlockGen");
        }
    }
}
