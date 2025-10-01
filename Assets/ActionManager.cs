using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class ActionManager : MonoBehaviour
{
    [Header("Gameplay Events")]
    public UnityEvent jump;             // one-shot when jump is pressed
    public UnityEvent jumpHold;         // fired continuously while jump is held
    public UnityEvent<int> moveCheck;   // -1 left, 0 idle, 1 right
    public UnityEvent click;

    // Called by Input System for Jump action
    public void OnJumpAction(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            Debug.Log("Jump started");
        }
        else if (context.performed)
        {
            Debug.Log("Jump performed");
            jump.Invoke();   // Notify listeners (e.g. PlayerMovement)
        }
        else if (context.canceled)
        {
            Debug.Log("Jump canceled");
        }
    }

    // Called by Input System for JumpHold action
    public void OnJumpHoldAction(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            Debug.Log("JumpHold started");
        }
        else if (context.performed)
        {
            Debug.Log("JumpHold performed");
            jumpHold.Invoke();   // Keep notifying while held
        }
        else if (context.canceled)
        {
            Debug.Log("JumpHold canceled");
        }
    }

    // Called by Input System for Move action (1D axis: A=-1, D=+1)
    public void OnMoveAction(InputAction.CallbackContext context)
    {
        float axisValue = context.ReadValue<float>();

        if (context.started || context.performed)
        {
            int direction = axisValue > 0 ? 1 : axisValue < 0 ? -1 : 0;
            Debug.Log($"Move {direction}");
            moveCheck.Invoke(direction);
        }
        else if (context.canceled)
        {
            Debug.Log("Move stopped");
            moveCheck.Invoke(0); // idle when released
        }
    }

    public void OnClick(InputAction.CallbackContext context)
    {
        if (context.started || context.performed)
        {
            // Invoke click event
            click.Invoke();
            Debug.Log("Click performed");
        }
        else if (context.canceled)
        {
            Debug.Log("Click canceled");
        }
    }
}
