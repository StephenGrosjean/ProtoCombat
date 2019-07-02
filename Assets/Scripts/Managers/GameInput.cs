using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using InControl;

public static class GameInput {

    static bool logInput = true;

    static float deadZoneKeyboard = 0.1f;
    public static bool keyboardLastPressed = false;

    public enum InputType
    {
        LEFT, // Directions like thoses are usefull for the navigation in the menu
        RIGHT,
        UP,
        DOWN,

        ACTION, //shoot in game, confirm in menus
        DEFENSE,
        DASH,
        PAUSE
    }
    public enum AxisType
    {
        L_HORIZONTAL,
        L_VERTICAL,
        R_HORIZONTAL,
        R_VERTICAL
    }
    public enum TimeType
    {
        UP,
        DOWN,
        HOLD
    }
    public enum DirectionType
    {
        R_INPUT,
        L_INPUT
    }

    /* public functions, callable on any scripts */
    public static bool GetInputDown(InputType inputType)
    {
        return GetInput(inputType, TimeType.DOWN);
    }
    public static bool GetInputUp(InputType inputType)
    {
        return GetInput(inputType, TimeType.UP);
    }
    public static bool GetInput(InputType inputType)
    {
        return GetInput(inputType, TimeType.HOLD);
    }

    /* Fonction principale d'appel des inputs
     *
     * Pour changer la configuration des manettes ou du clavier, veuillez le faire ici !     
     */
    private static bool GetInput(InputType inputType, TimeType timeType)
    {
        InputDevice device = InputManager.ActiveDevice;
        bool result = false;
        switch (inputType)
        {
            case InputType.LEFT:
                result = GetInput(InputControlType.LeftStickLeft, timeType) ||
                         GetInput(InputControlType.DPadLeft, timeType) ||
                         GetInput(InputControlType.RightStickLeft, timeType) ||
                         GetInput(KeyCode.D, timeType) ||
                         GetInput(KeyCode.RightArrow, timeType);
                break;
            case InputType.RIGHT:
                result = GetInput(InputControlType.LeftStickRight, timeType) ||
                         GetInput(InputControlType.DPadRight, timeType) ||
                         GetInput(InputControlType.RightStickRight, timeType) ||
                         GetInput(KeyCode.A, timeType) ||
                         GetInput(KeyCode.LeftArrow, timeType);
                break;
            case InputType.UP:
                result = GetInput(InputControlType.LeftStickUp, timeType) ||
                         GetInput(InputControlType.DPadUp, timeType) ||
                         GetInput(InputControlType.RightStickUp, timeType) ||
                         GetInput(KeyCode.W, timeType) ||
                         GetInput(KeyCode.UpArrow, timeType);
                break;
            case InputType.DOWN:
                result = GetInput(InputControlType.LeftStickDown, timeType) ||
                         GetInput(InputControlType.DPadDown, timeType) ||
                         GetInput(InputControlType.RightStickDown, timeType) ||
                         GetInput(KeyCode.S, timeType) ||
                         GetInput(KeyCode.DownArrow, timeType);
                break;
            case InputType.ACTION:
                result = GetInput(InputControlType.RightTrigger, timeType) ||
                         GetInput(KeyCode.Q, timeType);
                break;
            case InputType.DEFENSE:
                result = GetInput(InputControlType.RightBumper, timeType) ||
                         GetInput(KeyCode.E, timeType);
                break;
            case InputType.DASH:
                result = GetInput(InputControlType.Action2, timeType) ||
                         GetInput(KeyCode.F, timeType);
                break;
            case InputType.PAUSE:
                result = GetInput(InputControlType.Start, timeType) ||
                         GetInput(InputControlType.Menu, timeType) ||
                         GetInput(KeyCode.Escape, timeType);
                break;
        }

        if (logInput && result)
            Debug.Log("Input : " + inputType.ToString() + " was pressed and called !");

        CheckLastControllerUsed();
        return result;
    }

    private static bool GetInput(InputControlType input, TimeType timeType)
    {
        InputDevice device = InputManager.ActiveDevice;
        switch (timeType)
        {
            case TimeType.UP:
                return device.GetControl(input).WasReleased;
            case TimeType.DOWN:
                return device.GetControl(input).WasPressed;
            case TimeType.HOLD:
                return device.GetControl(input).IsPressed;
        }
        return false;
    }
    private static bool GetInput(KeyCode keyCode, TimeType timeType)
    {
        switch (timeType)
        {
            case TimeType.UP:
                return Input.GetKeyUp(keyCode);
            case TimeType.DOWN:
                return Input.GetKeyDown(keyCode);
            case TimeType.HOLD:
                return Input.GetKey(keyCode);
        }
        return false;
    }
    private static bool GetInput(int mouseBtn, TimeType timeType)
    {
        switch (timeType)
        {
            case TimeType.UP:
                return Input.GetMouseButtonUp(mouseBtn);
            case TimeType.DOWN:
                return Input.GetMouseButtonDown(mouseBtn);
            case TimeType.HOLD:
                return Input.GetMouseButton(mouseBtn);
        }
        return false;
    }

    public static float GetAxis(AxisType axisType)
    {
        float deltaMove = 0.0f;
        InputDevice device = InputManager.ActiveDevice;
        switch (axisType)
        {
            case AxisType.L_HORIZONTAL:
                if (device.GetControl(InputControlType.LeftStickX).IsPressed)
                    deltaMove = device.GetControl(InputControlType.LeftStickX).Value;
                else if (device.GetControl(InputControlType.DPadX).IsPressed)
                    deltaMove = device.GetControl(InputControlType.DPadX).Value;
                else if(Mathf.Abs(Input.GetAxis("L_Horizontal")) > deadZoneKeyboard)
                    deltaMove = Input.GetAxis("L_Horizontal");
                break;
            case AxisType.L_VERTICAL:
                if (device.GetControl(InputControlType.LeftStickY).IsPressed)
                    deltaMove = device.GetControl(InputControlType.LeftStickY).Value;
                else if (device.GetControl(InputControlType.DPadY).IsPressed)
                    deltaMove = device.GetControl(InputControlType.DPadY).Value;
                else if(Mathf.Abs(Input.GetAxis("L_Vertical")) > deadZoneKeyboard)
                    deltaMove = Input.GetAxis("L_Vertical");
                break;
            case AxisType.R_HORIZONTAL:
                if (device.GetControl(InputControlType.RightStickX).IsPressed)
                    deltaMove = device.GetControl(InputControlType.RightStickX).Value;
                else if (Mathf.Abs(Input.GetAxis("R_Horizontal")) > deadZoneKeyboard)
                    deltaMove = Input.GetAxis("R_Horizontal");
                break;
            case AxisType.R_VERTICAL:
                if (device.GetControl(InputControlType.RightStickY).IsPressed)
                    deltaMove = device.GetControl(InputControlType.RightStickY).Value;
                else if (Mathf.Abs(Input.GetAxis("R_Vertical")) > deadZoneKeyboard)
                    deltaMove = Input.GetAxis("R_Vertical");
                break;
        }
        if (logInput && deltaMove != 0.0f)
            Debug.Log("Moving : " + axisType.ToString() + " " + deltaMove);

        CheckLastControllerUsed();
        return deltaMove;
    }
    public static float GetAxisRaw(AxisType axisType)
    {
        float deltaMove = 0.0f;
        InputDevice device = InputManager.ActiveDevice;
        switch (axisType)
        {
            case AxisType.L_HORIZONTAL:
                if (device.GetControl(InputControlType.LeftStickX).IsPressed)
                    deltaMove = device.GetControl(InputControlType.LeftStickX).RawValue;
                else if (device.GetControl(InputControlType.DPadX).IsPressed)
                    deltaMove = device.GetControl(InputControlType.DPadX).RawValue;
                else if (Mathf.Abs(Input.GetAxisRaw("L_Horizontal")) > deadZoneKeyboard)
                    deltaMove = Input.GetAxisRaw("L_Horizontal");
                break;
            case AxisType.L_VERTICAL:
                if (device.GetControl(InputControlType.LeftStickY).IsPressed)
                    deltaMove = device.GetControl(InputControlType.LeftStickY).RawValue;
                else if (device.GetControl(InputControlType.DPadY).IsPressed)
                    deltaMove = device.GetControl(InputControlType.DPadY).RawValue;
                else if (Mathf.Abs(Input.GetAxisRaw("L_Vertical")) > deadZoneKeyboard)
                    deltaMove = Input.GetAxisRaw("L_Vertical");
                break;
            case AxisType.R_HORIZONTAL:
                if (device.GetControl(InputControlType.RightStickX).IsPressed)
                    deltaMove = device.GetControl(InputControlType.RightStickX).RawValue;
                else if (Mathf.Abs(Input.GetAxisRaw("R_Horizontal")) > deadZoneKeyboard)
                    deltaMove = Input.GetAxisRaw("R_Horizontal");
                break;
            case AxisType.R_VERTICAL:
                if (device.GetControl(InputControlType.LeftStickY).IsPressed)
                    deltaMove = device.GetControl(InputControlType.RightStickY).RawValue;
                else if (Mathf.Abs(Input.GetAxisRaw("R_Vertical")) > deadZoneKeyboard)
                    deltaMove = Input.GetAxisRaw("R_Vertical");
                break;
        }
        if (logInput && deltaMove != 0.0f)
            Debug.Log("Moving : " + axisType.ToString() + " " + deltaMove);

        CheckLastControllerUsed();
        return deltaMove;
    }

    public static Vector2 GetDirection(DirectionType directionType, Vector2 origin)
    {
        Vector2 direction = new Vector2(0, 0);
        CheckLastControllerUsed();
        
        if(directionType == DirectionType.R_INPUT && keyboardLastPressed)
        {
            Vector3 v3 = Input.mousePosition;
            v3.z = 10.0f;
            v3 = Camera.main.ScreenToWorldPoint(v3);

            direction = new Vector2(origin.x - v3.x, origin.y - v3.y);
        }
        else if(directionType == DirectionType.R_INPUT)
        {
            direction.x = GetAxis(AxisType.R_HORIZONTAL);
            direction.y = GetAxis(AxisType.R_VERTICAL);
        }
        else
        {
            direction.x = GetAxis(AxisType.L_HORIZONTAL);
            direction.y = GetAxis(AxisType.L_VERTICAL);
        }
        
        direction.Normalize();

        if (logInput && (direction.x != 0.0f || direction.y != 0.0f))
            Debug.Log("Direction : " + direction.ToString());

        return direction;
    }

    private static void CheckLastControllerUsed()
    {
        InputDevice device = InputManager.ActiveDevice;
        if (device.AnyButtonIsPressed)
        {
            keyboardLastPressed = false;
        }
        if (Input.anyKey)
        {
            keyboardLastPressed = true;
        }
    }
}
