using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

namespace Seralyth.Managers;

internal static class UnityInput
{
	internal static Vector3 mousePosition => Input.mousePosition;

	internal static Vector2 MouseScrollDelta => Input.mouseScrollDelta;

	internal static bool GetKey(Key key)
	{
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		return ((ButtonControl)Keyboard.current[key]).isPressed;
	}

	internal static bool GetKeyDown(Key key)
	{
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		return ((ButtonControl)Keyboard.current[key]).wasPressedThisFrame;
	}

	internal static bool GetKeyUp(Key key)
	{
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		return ((ButtonControl)Keyboard.current[key]).wasReleasedThisFrame;
	}

	internal static bool GetKey(string keyName)
	{
		return Input.GetKey(keyName);
	}

	internal static bool GetKeyDown(string keyName)
	{
		return Input.GetKeyDown(keyName);
	}

	internal static bool GetKeyUp(string keyName)
	{
		return Input.GetKeyUp(keyName);
	}

	internal static bool GetMouseButton(int button)
	{
		return Input.GetMouseButton(button);
	}

	internal static bool GetMouseButtonDown(int button)
	{
		return Input.GetMouseButtonDown(button);
	}

	internal static bool GetMouseButtonUp(int button)
	{
		return Input.GetMouseButtonUp(button);
	}
}
