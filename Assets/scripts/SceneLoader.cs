using UnityEngine;
using System.Collections;

namespace Challenger
{
	public enum Scene
	{
		Main,
		Map,
		Game
	}

	public class SceneLoader : SingletonBehaviour<SceneLoader>
	{
		// ------------------------------------------------------------
		//	Initialize
		// ------------------------------------------------------------
		protected override void OnSingletonAwake()
		{
			DontDestroyOnLoad(gameObject);

			SendMessage("OnLoadScene", GameObject.FindGameObjectWithTag("Root"));
		}

		// ------------------------------------------------------------
		//	Load scene
		// ------------------------------------------------------------
		public void LoadScene(Scene scene)
		{
			StartCoroutine(DoLoadScene(scene));
		}

		IEnumerator DoLoadScene(Scene scene)
		{
			AsyncOperation async = Application.LoadLevelAsync(scene.ToString());
			yield return async;

			// Update components of scene change
			SendMessage("OnLoadScene", GameObject.FindGameObjectWithTag("Root"));
		}
	}
}
