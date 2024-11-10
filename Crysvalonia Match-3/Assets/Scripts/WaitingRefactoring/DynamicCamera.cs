using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DynamicCamera : MonoBehaviour
{
    private Board board;
    public float w;
    public float h;
    public float cameraDistance;
    public float aspectRatio;
    public float padding;

    // Start is called before the first frame update
    void Start()
    {
        board = FindObjectOfType<Board>();
        if(board != null)
        {
            CameraPos(board.width + w, board.height + h);
        }
    }

    void CameraPos(float x, float y)
    {
        Vector3 tempPos = new Vector3((x/2), (y/2), cameraDistance);
        transform.position = tempPos;

        if(board.width >= board.height)
        {
            Camera.main.orthographicSize = (board.width / 2 + padding) / aspectRatio;
        }
        else
        {
            Camera.main.orthographicSize = (board.height / 2 + padding);
        }
    }
}
