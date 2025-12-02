using UnityEngine.Assertions;
using UnityEngine;
using System.Collections.Generic;

#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

namespace UnityChan {

#if ENABLE_INPUT_SYSTEM
[RequireComponent(typeof(PlayerInput))]
#endif
public class SDUnityChanInputReader : MonoBehaviour {
	private void Awake() {
#if ENABLE_INPUT_SYSTEM
		InputActionMap currentMap = m_playerInput.currentActionMap;

		// initialize actions
		foreach ((string actionName, SDUnityChan1DAxisInput index) in m_1dAxisActionMappings) {
			AddAction(m_1dAxisActions, currentMap, actionName, (int)index);
		}
		foreach ((string actionName, SDUnityChan2DAxisInput index) in m_2dAxisActionMappings) {
			AddAction(m_2dAxisActions, currentMap, actionName, (int)index);
		}
		foreach ((string actionName, SDUnityChanButtonInput index) in m_buttonActionMappings) {
			AddAction(m_buttonActions, currentMap, actionName, (int)index);
		}		
#endif
	}

	private void OnValidate() {
#if ENABLE_INPUT_SYSTEM
		m_playerInput = GetComponent<PlayerInput>();
#endif
	}
	
	
//----------------------------------------------------------------------------------------------------------------------	

	public float Read1DAxis(int index)
	{

#if ENABLE_INPUT_SYSTEM
		return m_1dAxisActions[index].ReadValue<float>();
#else
		switch (index) {
			case (int)SDUnityChan1DAxisInput.MouseScroll: {
				return Input.GetAxis("Mouse ScrollWheel");
			}
		}
		return 0;
#endif
	}

	public Vector2 Read2DAxis(int index)
	{

#if ENABLE_INPUT_SYSTEM
		return m_2dAxisActions[index].ReadValue<Vector2>();
#else
		switch (index) {
			case (int)SDUnityChan2DAxisInput.Move: {
				float x = Input.GetAxis("Horizontal");
				float y = Input.GetAxis("Vertical");
				return new Vector2(x, y);
			}
			case (int)SDUnityChan2DAxisInput.Look: {
				float x = Input.GetAxis("Mouse X");
				float y = Input.GetAxis("Mouse Y");
				return new Vector2(x, y);
			}
		}
		return Vector2.zero;

#endif
	}


	public bool ReadButton(int index) {
#if ENABLE_INPUT_SYSTEM
		return m_buttonActions[index].IsPressed();
#else
		switch (index) {
			case (int)SDUnityChanButtonInput.Attack: return Input.GetMouseButton(0);
			case (int)SDUnityChanButtonInput.Next: return Input.GetKey("down");
			case (int)SDUnityChanButtonInput.Prev: return Input.GetKey("up");
			case (int)SDUnityChanButtonInput.Reset: return Input.GetKey("r");
			case (int)SDUnityChanButtonInput.Jump: return Input.GetButton ("Jump");
			case (int)SDUnityChanButtonInput.LeftCtrl: return Input.GetKey(KeyCode.LeftControl); 
			case (int)SDUnityChanButtonInput.LeftAlt: return Input.GetKey(KeyCode.LeftAlt);
		}

		return false;
#endif

	}

	public bool ReadButtonDown(int index) {
#if ENABLE_INPUT_SYSTEM
		return m_buttonActions[index].WasPressedThisFrame();
#else
		switch (index) {
			case (int)SDUnityChanButtonInput.Attack: return Input.GetMouseButton(0); 
			case (int)SDUnityChanButtonInput.Next: return Input.GetKeyDown("down");
			case (int)SDUnityChanButtonInput.Prev: return Input.GetKeyDown("up"); 
			case (int)SDUnityChanButtonInput.Reset: return Input.GetKeyDown("r"); 
			case (int)SDUnityChanButtonInput.Jump: return Input.GetButtonDown ("Jump");
			case (int)SDUnityChanButtonInput.LeftCtrl: return Input.GetKeyDown(KeyCode.LeftControl);
			case (int)SDUnityChanButtonInput.LeftAlt: return Input.GetKeyDown(KeyCode.LeftAlt);
		}

		return false;
#endif
		
	}
	

	


#if ENABLE_INPUT_SYSTEM
	void AddAction(List<InputAction> actionList, InputActionMap inputActionMap, string actionNameOrID, int index) {
		
		InputAction action = inputActionMap.FindAction(actionNameOrID, throwIfNotFound: false);
		
		Assert.IsTrue(index >= 0);
		while (actionList.Count <= index) {
			actionList.Add(null);
		}
		actionList[index] = action;		
	}

//----------------------------------------------------------------------------------------------------------------------

	[SerializeField] private PlayerInput m_playerInput;
	
	private readonly List<InputAction> m_1dAxisActions = new List<InputAction>();
	private readonly List<InputAction> m_2dAxisActions = new List<InputAction>();
	private readonly List<InputAction> m_buttonActions = new List<InputAction>();

	//Mappings
	private static readonly (string name, SDUnityChan1DAxisInput index)[] m_1dAxisActionMappings = {
		("MouseScrollY", SDUnityChan1DAxisInput.MouseScroll)
	};

	private static readonly (string name, SDUnityChan2DAxisInput index)[] m_2dAxisActionMappings = {
		("Move", SDUnityChan2DAxisInput.Move),
		("Look", SDUnityChan2DAxisInput.Look),
	};
	
	private static readonly (string name, SDUnityChanButtonInput index)[] m_buttonActionMappings = {
		("Attack", SDUnityChanButtonInput.Attack),
		("Next", SDUnityChanButtonInput.Next),
		("Previous", SDUnityChanButtonInput.Prev),
		("Jump", SDUnityChanButtonInput.Jump),
		("Reset", SDUnityChanButtonInput.Reset),
		("LeftAlt", SDUnityChanButtonInput.LeftAlt),
		("LeftControl", SDUnityChanButtonInput.LeftCtrl)
	};	
#endif
	
}


} //end namespace
