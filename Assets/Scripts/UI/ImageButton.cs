using UnityEngine;
using System.Collections;

public class ImageButton : MonoBehaviour
{
	public enum ButtonType { OnClick, OnPress, OnRelease, OnPressRelease }
	public GameObject target;
	public string functionName;
	public bool isDisabled { get; set; }

	public bool onPressScaling = true;
	public ButtonType functionTrigger = ButtonType.OnClick; 
	public float onPressScaleValue = 0.9f;

	protected Vector3 _scale;


	// ------------------------------------------------------------
	// Get base name (assume sprite ends with "Off")
	// ------------------------------------------------------------
	void Start()
	{
		_scale = transform.localScale;
	}
	
	// ------------------------------------------------------------
	// Switch between on/off sprites
	// ------------------------------------------------------------
	void OnPress(bool isDown)
	{
		//if there is an animator modifying the gameObject's scale, it will prevent us to scale the button when we press it
		Animator animator = gameObject.GetComponent<Animator>();

		if(!isDisabled && onPressScaling)
		{
			if(isDown)
			{
				if(animator != null)
				{
					animator.enabled = false;
				}

				transform.localScale = _scale * onPressScaleValue;

				if(functionTrigger == ButtonType.OnPress)
					SendMessage();
			}

			else
			{
				if(animator != null)
				{
					animator.enabled = true;
				}

				transform.localScale = _scale;

				if(functionTrigger == ButtonType.OnRelease)
					SendMessage();
			}

			if(functionTrigger == ButtonType.OnPressRelease)
				SendMessage();
		}
	}

	// ------------------------------------------------------------
	// Send message to receiver
	// ------------------------------------------------------------
	protected virtual void OnClick()
	{
		if(!isDisabled && functionTrigger == ButtonType.OnClick)
		{
			SendMessage();
		}
	}

	private void SendMessage()
	{
		if (target != null)
			target.SendMessage(functionName, gameObject, SendMessageOptions.DontRequireReceiver);
		else
			transform.parent.SendMessageUpwards(functionName, gameObject, SendMessageOptions.DontRequireReceiver);
	}
}