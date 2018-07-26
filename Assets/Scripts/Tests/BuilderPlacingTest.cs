using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuilderPlacingTest : MonoBehaviour
{

    public Builder builder;
    public Node[] Nodes;

    int index;

	void Start ()
    {
        SwitchCurrentNode();
    }

    void SwitchCurrentNode()
    {
        if(builder.CurrentNode)
            Destroy(builder.CurrentNode.gameObject);
        builder.SetBuildingNode(Instantiate(Nodes[index]));
    }

    void Update()
    {
        index = Mathf.Clamp(index, 0, Nodes.Length - 1);
        builder.ProcessPlacing();

        if (Input.GetMouseButtonDown(0))
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
            if (builder.TryPlaceCurrentNode())
            {
                builder.SetBuildingNode(Instantiate(Nodes[index]));
            }
        }
        if (!Cursor.visible && Input.GetKeyDown(KeyCode.Escape))
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
        if (Input.GetKeyDown(KeyCode.Q))
        {
        
            index++;
            if (index >= Nodes.Length)
                index = 0;
            SwitchCurrentNode();
        }


    }

}
