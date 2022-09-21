using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;

public class InputPort : MonoBehaviour
{
    private InputField inputField;
    public ConnectButton connect;

    // Start is called before the first frame update
    void Start()
    {
        inputField = gameObject.GetComponent<InputField>();
        string text = inputField.text;
        connect.ReceiveChangeText(text);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnEndEdit()
    {
        string text = inputField.text;
        connect.ReceiveChangeText(text);
    }
}
