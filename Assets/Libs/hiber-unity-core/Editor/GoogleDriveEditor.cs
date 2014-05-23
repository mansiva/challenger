using UnityEngine;
using UnityEditor;

using System.Collections.Generic;
using System.Diagnostics;
using System.Text;


public abstract class GoogleDriveEditor : Editor
{
	protected class DocumentInfo
	{
		public string name;
		public string scriptPath;
		public string targetPath;

		public DocumentInfo(string name, string scriptPath, string targetPath)
		{
			this.name = name;
			this.scriptPath = scriptPath;
			this.targetPath = targetPath;
		}
	}

	private string _credentialsPath;
	private string _username;
	private string _password;
	
	protected List<DocumentInfo> _documentInfos;


	protected virtual void OnEnable()
	{
		// Set credentials and document path
		_credentialsPath = Application.temporaryCachePath + "/credentials";
		
		try
		{
			string[] lines = System.IO.File.ReadAllLines(@_credentialsPath);
			_username = lines[0];
			_password = lines[1];
		}
		catch
		{
			UnityEngine.Debug.LogWarning("Couldn't find credentials! Enter and save");
		}
		
		// Setup documents
		_documentInfos = new List<DocumentInfo>();

		// Add something like this in your inherited class
		//_documentInfos.Add(new DocumentInfo("WOCA_LevelData", "/Editor/updateLevels.py", "/Resources/Levels"));

	}
	
	
	override public void OnInspectorGUI()
	{
		// Credentials
		_username = EditorGUILayout.TextField("Username", _username);
		_password = EditorGUILayout.PasswordField("Password", _password);

		if (GUILayout.Button("Save Credentials"))
		{
			string[] lines = new string[] { _username, _password };
			System.IO.File.WriteAllLines(@_credentialsPath, lines);
		}

		EditorGUILayout.Space();

		// Update buttons
		foreach (DocumentInfo documentInfo in _documentInfos)
		{
			if (GUILayout.Button("Update " + documentInfo.name))
			{
				string[] arguments = new string[]
				{
					"-document=" + documentInfo.name,
					"-username=" + _username + "@hibernum.com",
					"-password=" + _password,
					"-targetPath=" + Application.dataPath + documentInfo.targetPath
				};
				ExecuteProcess("python", Application.dataPath + documentInfo.scriptPath, arguments);
				AssetDatabase.Refresh();
			}
		}
	}
	
	
	static void ExecuteProcess(string filename, string path, string[] arguments) 
	{
		StringBuilder args = new StringBuilder();
		
		// 1st argument is our script
		args.Append(path);
		
		// add any aditional arguments
		for (int i = 0; i<arguments.Length; i++) {
			args.Append(" \"" + arguments[i] + "\"");
		}
		
		Process proc = new Process();
		proc.StartInfo.FileName = filename;
		proc.StartInfo.Arguments = args.ToString();
		proc.StartInfo.UseShellExecute = false;
		proc.StartInfo.RedirectStandardOutput = true;
		proc.StartInfo.RedirectStandardError = true;
		proc.Start();
		
		// TODO, clean up the way we fetch script output
		StringBuilder str = new StringBuilder();
		while ( ! proc.HasExited )
		{
			str.Append(proc.StandardOutput.ReadToEnd());
			str.Append(proc.StandardError.ReadToEnd());
		}
		
		string output = str.ToString();
		if (!string.IsNullOrEmpty(output))
			UnityEngine.Debug.Log(output);
	}
}
