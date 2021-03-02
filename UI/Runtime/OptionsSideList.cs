using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class OptionsSideList : Dropdown
{
	// [SerializeField] private Dropdown dropdown;
	[SerializeField] internal UnselectableButton forwardButton;
	[SerializeField] internal UnselectableButton backwardButton;

	[SerializeField] private MoveDirection forwardDirection = MoveDirection.Right;
	[SerializeField] private MoveDirection backwardDirection = MoveDirection.Left;
	
	[SerializeField]
	private DropdownEvent m_OnValueChangedDynamic = new DropdownEvent();

	[SerializeField] private bool cyclic;

	protected override void Awake()
	{
		base.Awake();
		onValueChanged.AddListener(OnValueChangedDynamic);
	}

	private void OnValueChangedDynamic(int value)
	{
		m_OnValueChangedDynamic.Invoke(value);
	}

	public void Forward()
	{
		Debug.Log("Forward");
		EventSystem.current.SetSelectedGameObject(gameObject);
		
		var newValue = value;
		newValue++;
		if (cyclic)
		{
			newValue %= options.Count;
		}
		else
		{
			newValue = Mathf.Min(newValue, options.Count - 1);
		}
		value = newValue;
	}
	
	public void Backward()
	{
		EventSystem.current.SetSelectedGameObject(gameObject);
		
		var newValue = value;
		newValue--;
		if (cyclic)
		{
			var dropDownCount = options.Count;
			newValue = (newValue + dropDownCount) % dropDownCount;
		}
		else
		{
			newValue = Mathf.Max(newValue, 0);
		}
		value = newValue;
	}

	public override void OnMove(AxisEventData eventData)
	{
		if (eventData.moveDir == forwardDirection)
		{
			forwardButton.OnSubmit(eventData);
		}
		else if (eventData.moveDir == backwardDirection)
		{
			backwardButton.OnSubmit(eventData);
		}
		else
		{
			base.OnMove(eventData);
		}
	}


	public override void OnPointerClick(PointerEventData eventData)
	{
	}

	public override void OnSubmit(BaseEventData eventData)
	{
	}

	public override void OnCancel(BaseEventData eventData)
	{
	}
}
