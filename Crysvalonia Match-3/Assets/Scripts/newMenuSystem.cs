using UnityEngine;

/// <summary>
/// This script will handle all the menus
/// </summary>
public class newMenuSystem : MonoBehaviour
{
    GameController controllerInstance;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        controllerInstance = GameController.Instance;
        if(controllerInstance == null)
        {
            Debug.LogError("newMenuSystem: Controller Instance not found!");
        }
    }
}
