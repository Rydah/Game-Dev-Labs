using System.Drawing;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class ActionManager : MonoBehaviour
{
    public UnityEvent clickEvent;
    public UnityEvent<Vector2> posEvent;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void onClickAction(InputAction.CallbackContext context)
    {
        if (context.started)
            Debug.Log("mouse click started");
        else if (context.performed)
        {
            Debug.Log("mouse click performed");
        }
        else if (context.canceled)
            Debug.Log("mouse click cancelled");
    }

    public void OnPointAction(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            Vector2 point = context.ReadValue<Vector2>();
            posEvent?.Invoke(point);
            Debug.Log($"Point detected: {point}");

        }
    }
}
