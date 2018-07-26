using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Builder : MonoBehaviour
{
    public new Transform transform { get; private set; }
    public Node CurrentNode { get; private set; }

    Camera cam;
    RaycastHit[] raycastBuffer = new RaycastHit[32];
    Collider[] colliderBuffer = new Collider[32];
    const string GROUND_TAG = "Ground";
    Socket previousSocket;

    void Awake()
    {
        cam = Camera.main;
        transform = base.transform;
    }

    /// <summary>
    /// Sets up current node and activates building process
    /// </summary>
    /// <param name="Node"></param>
    public void SetBuildingNode(Node Node)
    {
      
        CurrentNode = Node;
        CurrentNode.IgnoreCollisions(true);
        CurrentNode.gameObject.SetActive(true);
    }

    /// <summary>
    /// Returns raycast sorted by the distance and mask.
    /// </summary>
    /// <returns></returns>
    bool IsRaycastingOver(out RaycastHit result, LayerMask mask, float maxDistance = 100)
    {
        var rayOrigin = transform.position;
        var ray = cam.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2));
        int hits = Physics.RaycastNonAlloc(ray, raycastBuffer, maxDistance, mask);
        result = raycastBuffer[0];
        if (hits > 0)
        {
            var dist = float.MaxValue;
           
            for (int i = 0; i < hits; i++)
            {
                var hit = raycastBuffer[i];
                if (hit.collider.gameObject == CurrentNode.gameObject) // skip collider if we hit our current Node
                    continue;
                var d = Vector3.SqrMagnitude(hit.point - rayOrigin);
                if (d < dist)
                {
                    dist = d;
                    result = hit;
                }
            }
            return true;
        }

        return false;
    }

    /// <summary>
    /// Tries to place node and returns corresponding boolean
    /// </summary>
    /// <returns></returns>
    public bool TryPlaceCurrentNode()
    {
        if(CurrentNode != null && CurrentNode.IsReadyToBuild && !IsOverlappingOtherNodes(CurrentNode))
        {
            CurrentNode.SetToRegularState();
            CurrentNode.IgnoreCollisions(false);
            CurrentNode = null;
            previousSocket = null;
            return true;
        }
        return false;
    }

    /// <summary>
    /// Returns raycast from camera sorted by the distance.
    /// </summary>
    /// <returns></returns>
    bool IsRaycastingOver(out RaycastHit result, float maxDistance = 100)
    {
        return IsRaycastingOver(out result, ~0, maxDistance);
    }

    /// <summary>
    /// Tries to put node using sockets and plugs
    /// </summary>
    /// <param name="hit"></param>
    /// <param name="Node"></param>
    void ProcessPlacingOnTopOfOtherNode(RaycastHit hit, Node Node)
    {
        Plug plug = CurrentNode.GetPlug(Node.type); // check if current node has corresponing plug
        if (plug != null)
        {
            var socket = Node.GetClosestSocket(hit.point, CurrentNode, plug); // finds closest socket with the same type as the plug that we have found

            if (previousSocket != socket && socket != null) // if found, connect the node to the socket, using plug position as a pivot point
            {
                var prevPos = CurrentNode.transform.position;

                CurrentNode.transform.rotation = socket.transform.rotation;
                CurrentNode.transform.position = socket.transform.position + CurrentNode.transform.TransformDirection(plug.transform.localPosition);

                if (!IsOverlappingOtherNodes(CurrentNode, out Node)) // if not overlapping - set to ready to build
                {
                    CurrentNode.SetReadyToBuild();
                }
                else
                {
                    //   CurrentNode.transform.position = prevPos;
                    CurrentNode.SetNotReadyToBuild();
                }
                previousSocket = socket;
            }
        }
        else
        {
            CurrentNode.SetNotReadyToBuild();
        }
    }


    /// <summary>
    /// Tries to place node on the ground, if overlaps with other node - tries to place on top of other node
    /// </summary>
    /// <param name="hit"></param>
    void ProcessPlacingOnGround(RaycastHit hit)
    {
        previousSocket = null;
        CurrentNode.transform.position = hit.point;
        Node otherNode;
        bool overlappingOthers = IsOverlappingOtherNodes(CurrentNode, out otherNode);
        // sets the visual state according to the value we got from CanBePlacedOnTheGround method
        if(!overlappingOthers && CurrentNode.canBePlacedOnTheGround)
        {
            CurrentNode.SetReadyToBuild();
        }
        else
        {
            if(otherNode != null)
                ProcessPlacingOnTopOfOtherNode(hit, otherNode);
            else CurrentNode.SetNotReadyToBuild();
        }
    }
    /// <summary>
    /// Checks if current node position overlaps with other nodes and returns the node overlapped
    /// </summary>
    /// <param name="Node"></param>
    /// <param name="other"></param>
    /// <returns></returns>
    bool IsOverlappingOtherNodes(Node Node, out Node other)
    {
        // store every collider overlapping with boundCollider of the Node in the colliderBuffer array, and return number of overlaps (includes current Node)
        var hits = Physics.OverlapBoxNonAlloc(Node.transform.position + Node.boundsCollider.center, (Node.boundsCollider.size / 2) * .95f, colliderBuffer);
        if(hits > 0)
        {
            for (int i = 0; i < hits; i++)
            {
                var coll = colliderBuffer[i];
                // return false if bounding box overlaps with other Node
                if(coll != Node.boundsCollider && coll.CompareTag(GROUND_TAG) == false)
                {
                    other = coll.GetComponent<Node>();
                    return true;
                }
            }
        }
        other = null;
        return false;
    }

    /// <summary>
    /// Checks if current node position overlaps with other nodes
    /// </summary>
    /// <param name="Node"></param>
    /// <returns></returns>
    bool IsOverlappingOtherNodes(Node Node)
    {
        // store every collider overlapping with boundCollider of the Node in the colliderBuffer array, and return number of overlaps (includes current Node)
        var hits = Physics.OverlapBoxNonAlloc(Node.transform.position + Node.boundsCollider.center, (Node.boundsCollider.size / 2) * .95f, colliderBuffer, Node.transform.rotation);
        if (hits > 0)
        {
            for (int i = 0; i < hits; i++)
            {
                var coll = colliderBuffer[i];
                // return false if bounding box overlaps with other Node
                if (coll != Node.boundsCollider && coll.CompareTag(GROUND_TAG) == false)
                {
                    return true;
                }
            }
        }
        return false;
    }

    /// <summary>
    /// Tries to place current Node (if not null) on the ground or other Node
    /// </summary>
    public void ProcessPlacing()
    {
        if(CurrentNode != null)
        {
            RaycastHit hit;
            if(IsRaycastingOver(out hit))
            {
                var otherNode = hit.collider.GetComponent<Node>();
                if(otherNode != null && otherNode != CurrentNode)
                {
                    ProcessPlacingOnTopOfOtherNode(hit, otherNode);
                }
                else
                {
                    ProcessPlacingOnGround(hit);
                }
            }
        }
    }
}
