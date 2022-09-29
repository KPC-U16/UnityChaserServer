using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;

public class tileImg : MonoBehaviour
{
    Image m_image;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ImgChange(string type,Sprite[] texture)
    {
        m_image = GetComponent<Image>();

        switch (type)
        {
            case "none":
                m_image.sprite = texture[0];
                break;
            case "block":
                m_image.sprite = texture[1];
                break;
            case "item":
                m_image.sprite = texture[2];
                break;
            case "cool":
                m_image.sprite = texture[3];
                break;
            case "hot":
                m_image.sprite = texture[4];
                break;
        }
    }
}
