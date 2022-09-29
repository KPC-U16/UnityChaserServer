using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;

public class ShowData : MonoBehaviour
{
    public Text statusText;
    public Text nameText;
    public Text ipText;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Reflect(string status,string name,string ip)
    {
        if (status != null)
        {
            statusText.text = status;
        }

        if (name != null)
        {
            nameText.text = name;
        }

        if (ip != null)
        {
            ipText.text = ip;
        }
    }
}
