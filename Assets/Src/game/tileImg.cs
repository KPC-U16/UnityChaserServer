using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;
using System;

public class tileImg : MonoBehaviour
{
    SpriteRenderer m_image;
    Animator animator;
    RuntimeAnimatorController controllers;
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
                    animator.runtimeAnimatorController = controllers;
                    animator.SetTrigger("BlockGen");
                }
                break;
        }
    }

    public void SetView(string type,Sprite[] tex,RuntimeAnimatorController[] animators)
    {
        m_image = GetComponent<SpriteRenderer>();

        texture = new Sprite[tex.Length];
        Array.Copy(tex,texture,tex.Length);

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
            controllers = animators[1];
            animatoin = true;
            animator = GetComponent<Animator>();
        }

        if (animatoin && type == "block")
        {
            animator.runtimeAnimatorController = controllers;
            animator.SetTrigger("BlockGen");
        }
    }
}
