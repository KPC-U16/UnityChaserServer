/************************************************************
gkngkc/UnityStandaloneFileBrowser
	https://github.com/gkngkc/UnityStandaloneFileBrowser
	
Unity Standalone File Browser がMac OSで動作しなくなった件
	https://qiita.com/YoshitakaAtarashi/items/198f40a96de1aa6721c8
	
	DLL not found issue in Mac build #109
		https://github.com/gkngkc/UnityStandaloneFileBrowser/issues/109
		-> clean build
	

【Unity】Unity2020 にアップグレードしたら package cache (PackageManager) のエラーが色々出る…   
	http://fantom1x.blog130.fc2.com/blog-entry-404.html
	
【Unity】Plastic SCM関連のエラー
	https://rarafy.com/blog/2022/03/04/unity-error-plasticscm/
************************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using SFB; // need this to use StandaloneFileBrowser

using System.Threading;

/************************************************************
************************************************************/
public class FileDiaglog : MonoBehaviour
{
	/****************************************
	****************************************/
	float timer = 0;
	int c_timer = 0;
	
	List<string> str_PathsFromFileDialog = new List<string>();
	int counter = 0;
	bool b_GetPathFrom_OpenFileDialog = false;
	bool b_GetPathFrom_SaveFileDialog = false;
	
	/****************************************
	****************************************/
	/******************************
	******************************/
    void Start()
    {
        
    }
	
	/******************************
	******************************/
    void Update()
    {
		/********************
		********************/
		if(Input.GetKeyUp(KeyCode.O)){
			string str_message = string.Format("OpenFile{0}", counter++);
			str_PathsFromFileDialog.Clear();
			str_PathsFromFileDialog.Add(str_message);
			
			// OpenFilePanel(); // don´t use on mac
			OpenFilePanelAsync();
			
		}else if(Input.GetKeyUp(KeyCode.S)){
			string str_message = string.Format("SaveFile{0}", counter++);
			str_PathsFromFileDialog.Clear();
			str_PathsFromFileDialog.Add(str_message);
			
			SaveFilePanelAsync();
			// Invoke("SaveFilePanelAsync", 0.5f);
		}
		
		/********************
		********************/
		if(b_GetPathFrom_OpenFileDialog){
			b_GetPathFrom_OpenFileDialog = false;
			
			Debug.Log("> open file");
			foreach(var s in str_PathsFromFileDialog){
				Debug.Log(s); // 1shot 動作をここに記述
			}
		}
		
		/********************
		********************/
		if(b_GetPathFrom_SaveFileDialog){
			b_GetPathFrom_SaveFileDialog = false;
			
			Debug.Log("> save file");
			foreach(var s in str_PathsFromFileDialog){
				Debug.Log(s); // 1shot 動作をここに記述
			}
		}
		
		/********************
		********************/
		/*
		timer += Time.deltaTime;
		if(1.0 < timer){
			timer = 0;
			c_timer++;
			Debug.Log(c_timer);
		}
		*/
    }
	
	/******************************
	******************************/
	void OpenFilePanel(){
		var extensions = new [] {
			new ExtensionFilter("Image Files", "png", "jpg", "jpeg" ),
			new ExtensionFilter("Sound Files", "mp3", "wav" ),
			new ExtensionFilter("All Files", "*" ),
		};
		
		var paths = StandaloneFileBrowser.OpenFilePanel(
															"Open File",	// title
															"",				// directory
															"",			// extension filter
															// extensions,		// extension filter
															true 			// multiselect
														);
		
		/********************
		cancel時、
			mac : paths[0]		== ""
			win : paths.Length	== 0
		となる。
		********************/
		if(0 < paths.Length){
			if(paths[0] != ""){
				str_PathsFromFileDialog.Clear();
				foreach(var s in paths){
					str_PathsFromFileDialog.Add(s);
				}
				
				b_GetPathFrom_OpenFileDialog = true;
				
			}else{
				str_PathsFromFileDialog.Clear();
				str_PathsFromFileDialog.Add("open sync : canceled(blank)");
				Debug.Log("open sync : canceled(blank)");
			}
		}else{
			str_PathsFromFileDialog.Clear();
			str_PathsFromFileDialog.Add("open sync : canceled(zero)");
			Debug.Log("open sync : canceled(zero)");
		}
	}
	
	/******************************
	******************************/
	void OpenFilePanelAsync(){
		var extensions = new [] {
			new ExtensionFilter("Image Files", "png", "jpg", "jpeg" ),
			new ExtensionFilter("Sound Files", "mp3", "wav" ),
			new ExtensionFilter("All Files", "*" ),
		};
		
		StandaloneFileBrowser.OpenFilePanelAsync(
													"Open File",	// title
													"",				// directory
													"",				// extension filter
													// extensions,				// extension filter
													true, 			// multiselect
													(string[] paths) => {
														/********************
														cancel時、
															mac : paths[0]		== ""
															win : paths.Length	== 0
														となる。
														********************/
														if(0 < paths.Length){
															if(paths[0] != ""){
																str_PathsFromFileDialog.Clear();
																foreach(var s in paths){
																	str_PathsFromFileDialog.Add(s);
																}
																
																b_GetPathFrom_OpenFileDialog = true;
															}else{
																str_PathsFromFileDialog.Clear();
																str_PathsFromFileDialog.Add("open async : canceled(blank)");
																Debug.Log("open async : canceled(blank)");
															}
														}else{
															str_PathsFromFileDialog.Clear();
															str_PathsFromFileDialog.Add("open async : canceled(zero)");
															Debug.Log("open async : canceled(zero)");
														}
													}
												);
	}
	
	/******************************
	******************************/
	void SaveFilePanel(){
		var extensionList = new [] {
			new ExtensionFilter("Text", "txt"),
			new ExtensionFilter("Binary", "bin"),
		};
		
		var path = StandaloneFileBrowser.SaveFilePanel(
														"Save File",	// title
														"",				// directory
														"Nobu.txt",		// defaultName
														extensionList	// extension filter
														);
														
		if(path != ""){
			str_PathsFromFileDialog.Clear();
			str_PathsFromFileDialog.Add(path);
			
			b_GetPathFrom_SaveFileDialog = true;
		}else{
			str_PathsFromFileDialog.Clear();
			str_PathsFromFileDialog.Add("save sync : canceled");
			Debug.Log("save sync : canceled");
		}
	}
	
	/******************************
	******************************/
	void SaveFilePanelAsync(){
		var extensionList = new [] {
			new ExtensionFilter("Text", "txt"),
			new ExtensionFilter("Binary", "bin"),
		};
		
		StandaloneFileBrowser.SaveFilePanelAsync(
													"Save File",	// title
													"",				// directory
													"Nobu.txt",		// defaultName
													extensionList,	// extension filter
													(string path) => {
														if(path != ""){
															str_PathsFromFileDialog.Clear();
															str_PathsFromFileDialog.Add(path);
															
															b_GetPathFrom_SaveFileDialog = true;
														}else{
															str_PathsFromFileDialog.Clear();
															str_PathsFromFileDialog.Add("save async : canceled");
															Debug.Log("save async : canceled");
														}
													}
												);
												
	}
	
	/******************************
	******************************/
	private void OnGUI()
	{
		GUI.skin.button.fontSize = 25;
		GUI.skin.label.fontSize = 25;
		GUI.color = Color.white;
		
		if (GUILayout.Button("clear log"))	str_PathsFromFileDialog.Clear();
		
		foreach (var s in str_PathsFromFileDialog){
			GUILayout.Label(s);
		}
	}
}
