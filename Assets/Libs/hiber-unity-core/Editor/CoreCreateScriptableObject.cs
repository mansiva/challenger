using UnityEngine;
using UnityEditor;
using System.Collections;
using System.IO;
using System.Linq;

public static class CoreCreateScriptableObject
{
	[MenuItem ("Assets/Create/ScriptableObject")]
	private static void Create()
	{
		var objects = Selection.GetFiltered(typeof(UnityEngine.Object), SelectionMode.DeepAssets).ToList();
		var folder = objects.Select(x=>AssetDatabase.GetAssetPath(x)).Where(x=>Directory.Exists(x)).OrderByDescending(x=>x).FirstOrDefault();

		if(string.IsNullOrEmpty(folder))
		{
			folder = Path.GetDirectoryName(AssetDatabase.GetAssetPath(Selection.activeObject));
		}
		if(!string.IsNullOrEmpty(folder))
		{
			var so = ScriptableObject.CreateInstance(typeof(CoreTempScriptableObject));
			AssetDatabase.CreateAsset(so, folder + "/NewScriptableObject.asset");
			AssetDatabase.Refresh();
		}
	}
}
