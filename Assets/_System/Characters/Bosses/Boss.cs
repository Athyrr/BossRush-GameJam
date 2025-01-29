using System.Collections.Generic;
using UnityEngine;

public class Boss : MonoBehaviour
{
    public Transform Target { get; private set; }

    [HideInInspector] public BossMvt Mvt;
    [HideInInspector] public Boss01AttackBehaviour AttackBehaviour;

    private void Awake()
    {
        Target = GameObject.Find("target").transform; //faut faire autrement
        Mvt = GetComponent<BossMvt>();
        AttackBehaviour = GetComponent<Boss01AttackBehaviour>();
    }

    public Vector3 GetGroundNormal(bool walls)
    {
        List<Vector3> normals = new() { Vector3.down };
        if (walls)
            normals.AddRange(new Vector3[] { Vector3.right, Vector3.left, Vector3.forward, Vector3.back });

        foreach (var dir in normals)
        {
            if (Physics.Raycast(transform.position, dir, 2.1f)) //2.1f est arbitraire, faudra changer
                return -dir;
        }
        return Vector3.zero;
    }

    public bool IsGrounded(bool walls)
    {
        return GetGroundNormal(walls) != Vector3.zero;
    }
}
