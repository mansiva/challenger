using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace WordSaga
{
	public class Popup : MonoBehaviour 
	{
		// Use this for initialization
		void Start () 
		{

		}

		virtual public void OnPopupClose()
		{
			PopupController.Instance.ClosePopup(this);
		}

		public void Clean()
		{
			Destroy(this.gameObject);
		}
	}
}
