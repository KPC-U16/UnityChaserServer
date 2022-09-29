using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;

public class StartButton : MonoBehaviour
{
    Button button;
    bool cFlag = false;
    bool hFlag = false;
    public GameManager gameManager;
    public InputField inputField;
    // Start is called before the first frame update
    void Start()
    {
        button = gameObject.GetComponent<Button>();
        button.interactable = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Ready(string team)
    {
        switch (team)
        {
            case "Cool":
                cFlag = true;
                break;
            case "Hot":
                hFlag = true;
                break;
        }

        if (cFlag && hFlag) button.interactable = true;
    }

    public void NotReady(string team)
    {
        switch (team)
        {
            case "Cool":
                cFlag = false;
                break;
            case "Hot":
                hFlag = false;
                break;
        }

        button.interactable = false;
    }

    public void OnClick()
    {
        gameManager.GameStart(inputField.text);
    }
}
