using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class charMove : MonoBehaviour
{
    int xDiff;
    int yDiff;
    SpriteRenderer m_image;
    Animator animator;
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

    public void Set(Sprite texture,RuntimeAnimatorController controller,int xDiff,int yDiff)
    {
        m_image = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        this.xDiff = xDiff;
        this.yDiff = yDiff;

        m_image.sprite = texture;
        animator.runtimeAnimatorController = controller;
    }

    public void Act(string command)
    {
        switch (command.Trim())
        {
            case "wu":
                animator.SetTrigger("Walk");
                animator.SetInteger("Direction",12);
            break;
            case "wr":
                animator.SetTrigger("Walk");
                animator.SetInteger("Direction",3);
            break;
            case "wd":
                animator.SetTrigger("Walk");
                animator.SetInteger("Direction",6);
            break;
            case "wl":
                animator.SetTrigger("Walk");
                animator.SetInteger("Direction",9);
            break;
        }
    }
}
