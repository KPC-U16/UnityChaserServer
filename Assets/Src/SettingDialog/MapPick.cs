using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;
using SFB;
using System.Linq;

public class MapPick : MonoBehaviour
{
    private string path;
    public InputField inputField;
    ExtensionFilter[] extensions;
    // Start is called before the first frame update
    void Start()
    {
        extensions = new [] {
            new ExtensionFilter("Map Files","map"),
            new ExtensionFilter("All Files","*"),
        };
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnClick()
    {
        string[] files = StandaloneFileBrowser.OpenFilePanel("Open File", "", extensions, false);
        if (files.Any())
        {
            inputField.text = files[0];
        }
    }
}
