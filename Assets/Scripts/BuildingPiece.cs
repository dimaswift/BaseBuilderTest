using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BuildingPiece : MonoBehaviour
{

    public BuildingState State
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

    BuildingState _state;

    public BuildingState regularState, canBeBuiltState, cannotBeBuiltState;

    public GameObject gizmo;

    MeshRenderer[] meshRenderers;

    public abstract bool CanBePlaced(Vector3 groundPoint, out Vector3 finalPosition);

    public virtual void OnStateChanged(BuildingState state)
    {
        foreach (var r in meshRenderers)
        {
            r.sharedMaterial = state.material;
        }
    }

    [System.Serializable]
    public class BuildingState
    {
        public string name;
        public Material material;
    }

}
