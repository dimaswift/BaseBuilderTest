using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Represents a building piece, with sockets and plugs.
/// Socket is the point where other nodes can be connected to.
/// Plug is the pivot point of specific node type
/// </summary>
public class Node : MonoBehaviour
{
    public bool canBePlacedOnTheGround;

    public NodeType type;

    Plug[] plugs;

    Socket[] sockets;

    BuildingState State
    {
        get
        {
            return _state;
        }
        set
        {
            if(_state != value)
            {
                _state = value;
                OnStateChanged(value);
            }
        }
    }

    int defaultLayer, ignoreAllLayer;

    public bool IsReadyToBuild
    {
        get { return State == readyToBuildState; }
    }

    public BoxCollider boundsCollider;

    BuildingState _state;

    public BuildingState regularState, readyToBuildState, notReadyToBuildState;

    MeshRenderer[] meshRenderers;

    private void Awake()
    {
        meshRenderers = GetComponentsInChildren<MeshRenderer>();
        plugs = GetComponentsInChildren<Plug>();
        sockets = GetComponentsInChildren<Socket>();
        ignoreAllLayer = LayerMask.NameToLayer("IgnoreEverything");
        defaultLayer = gameObject.layer;
    }

    public virtual void OnStateChanged(BuildingState state)
    {
        foreach (var r in meshRenderers)
        {
            r.sharedMaterial = state.material;
        }
    }

    public void IgnoreCollisions(bool ignore)
    {
        gameObject.layer = ignore ? ignoreAllLayer : defaultLayer;
    }

    [System.Serializable]
    public class BuildingState
    {
        public Material material;
    }

    public void SetReadyToBuild()
    {
        State = readyToBuildState;
    }

    public void SetNotReadyToBuild()
    {
        State = notReadyToBuildState;
    }

    public void SetToRegularState()
    {
        State = regularState;
    }

    /// <summary>
    /// Returns closest supported socket. If not found - returns null
    /// </summary>
    /// <param name="point"></param>
    /// <returns></returns>
    public Socket GetClosestSocket(Vector3 point, Node node, Plug plug)
    {
        
        var dist = float.MaxValue;

        Socket closest = null;
        foreach (var c in sockets)
        {
            var d = Vector3.SqrMagnitude(point - c.transform.position);
            if (d < dist && c.supportedTypes.Contains(node.type))
            {
                closest = c;
                dist = d;
            }
        }
        return closest;

    }


    public Plug GetPlug(NodeType type)
    {
        foreach (var p in plugs)
        {
            if (p.type == type)
                return p;
        }
        return null;
    }
}
