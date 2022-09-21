using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;
using System.Threading.Tasks;

public class ConnectButton : MonoBehaviour
{
    private bool clicked = false;
    private string port;
    public string team;
    public GameManager gameManager;
    public InputField inputPort;
    public Dropdown selectUser;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ReceiveChangeText(string text)
    {
        port = text;
    }

    public void OnClick()
    {
        Image buttonImage = gameObject.GetComponent<Image>();
        Text buttonText = gameObject.GetComponentInChildren<Text>();

        if (clicked)
        {
            buttonImage.color = Color.white;
            buttonText.text = "接続開始";

            inputPort.interactable = true;
            selectUser.interactable = true;
            gameManager.ConClose(team);
        }
        else if (!clicked)
        {
            buttonImage.color = Color.gray;
            buttonText.text = "待機終了";

            inputPort.interactable = false;
            selectUser.interactable = false;
            gameManager.ConWait(team,port);
        }
        clicked = !clicked;
    }
}
