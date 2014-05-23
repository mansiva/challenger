using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace WordSaga
{
	public class PopupController : SingletonBehaviour<PopupController> 
	{
		private GameObject _root;
		private List<Popup> _stackedPopupList = new List<Popup>();
		private Popup _currentDisplayedPopup = null;


		// ------------------------------------------------------------
		//	Initialize controller
		// ------------------------------------------------------------
		void Start()
		{
			
		}

		// ------------------------------------------------------------
		//	Load scene
		// ------------------------------------------------------------
		void OnLoadScene(object root)
		{
			_root = (GameObject) root;
		}

		public void DisplayPopup(string popupName)
		{
			if(_currentDisplayedPopup != null)
			{
				_stackedPopupList.Add(_currentDisplayedPopup);
				_currentDisplayedPopup.gameObject.SetActive(false);
			}

			GameObject popupGo = Resources.Load<GameObject> ("Prefabs/Popups/" + popupName);
			GameObject popupInstance = NGUITools.AddChild(_root, popupGo);
			_currentDisplayedPopup = popupInstance.GetComponent<Popup>();
		}

		public void ClosePopup(Popup popupClosing)
		{
			if(popupClosing == _currentDisplayedPopup)
			{
				_currentDisplayedPopup.Clean();
				_currentDisplayedPopup = null;

				if(_stackedPopupList.Count > 0)
				{
					int lastIndex = _stackedPopupList.Count - 1;

					_currentDisplayedPopup = _stackedPopupList[lastIndex];
					_currentDisplayedPopup.gameObject.SetActive(true);
					_stackedPopupList.RemoveAt(lastIndex);
				}
			}
		}
	}
}