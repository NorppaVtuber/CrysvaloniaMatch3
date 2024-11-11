using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// This script will handle all the menus
/// </summary>
public class newMenuSystem : MonoBehaviour
{
    GameController controllerInstance;

    [SerializeField] Canvas menuBackground;
    [SerializeField] Sprite menuSprite;
    [SerializeField] Button buttonPrefab;

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
