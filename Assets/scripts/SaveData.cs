using UnityEngine;
using System.Collections.Generic;

namespace Challenger
{
	public class SaveData : SingletonBehaviour<SaveData>
	{
		public int version = 1;

		// ------------------------------------------------------------
		// Initialize save data
		// ------------------------------------------------------------
		protected override void OnSingletonAwake()
		{
			// First launch
			if (GetVersion() == 0)
			{
				/*
				// Set AB Testing group
				string rawData = Resources.Load<TextAsset>("Config/Distribution").text;
				Dictionary<string, object> data = Json.Deserialize(rawData) as Dictionary<string, object>;

				float randomValue = Random.value;
				foreach (KeyValuePair<string, object> item in data)
				{
					float probability = float.Parse(item.Value.ToString());
					if (randomValue <= probability)
					{
						SetTestGroup(item.Key);
						break;
					}
					else
						randomValue -= probability;
				}
				*/

				// Set language
				string language = Application.systemLanguage.ToString();
				if (Resources.Load<TextAsset>("Localization/" + Application.systemLanguage.ToString()) == null)
				    language = "English";

				//Localization.language = language;

				// Set Version
				SetVersion(version);
			}

			// Upgrade version
			else if (GetVersion() < version)
			{
				// Do appropriate modifications here

				// Set Version
				SetVersion(version);
			}
		}

		// ------------------------------------------------------------
		// SaveData Version
		// ------------------------------------------------------------
		public int GetVersion()
		{
			return PlayerPrefs.GetInt("Version", 0);
		}

		public void SetVersion(int value)
		{
			PlayerPrefs.SetInt("Version", value);
			PlayerPrefs.Save();
		}


		// ------------------------------------------------------------
		// AB Testing group
		// ------------------------------------------------------------
		public string GetTestGroup()
		{
			return PlayerPrefs.GetString("TestGroup", "A");
		}
		
		public void SetTestGroup(string value)
		{
			PlayerPrefs.SetString("TestGroup", value);
			PlayerPrefs.Save();
		}


		// ------------------------------------------------------------
		// Music
		// ------------------------------------------------------------
		public bool IsMusicMuted()
		{
			return PlayerPrefs.GetInt("IsMusicMuted", 0) == 1;
		}

		public void ToggleMusicMuted()
		{
			PlayerPrefs.SetInt("IsMusicMuted", IsMusicMuted() ? 0 : 1);
			PlayerPrefs.Save();
		}

		
		// ------------------------------------------------------------
		// Sfx
		// ------------------------------------------------------------
		public bool IsSfxMuted()
		{
			return PlayerPrefs.GetInt("IsSfxMuted", 0) == 1;
		}
		
		public void ToggleSfxMuted()
		{
			PlayerPrefs.SetInt("IsSfxMuted", IsSfxMuted() ? 0 : 1);
			PlayerPrefs.Save();
		}
	}
}