using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Polygon : MonoBehaviour {

    LineRenderer lr;

    int n;
    public float radius { get; private set; }
    float edgeLength = 2;

	// Use this for initialization
	//void Start () {
	void Awake () {
        lr = GetComponent<LineRenderer>();
    }

    // Update is called once per frame
    void Update () {

        if(Input.GetKeyDown(KeyCode.KeypadPlus))
        {
            SetN(n + 1);
        }
        if (Input.GetKeyDown(KeyCode.KeypadMinus))
        {
            SetN(Mathf.Max(3, n - 1));
        }
    }

    public void SetN(int n)
    {
        Debug.Assert(n > 2);

        this.n = n;

        Vector3[] positions = new Vector3[n + 2];

        var tangent = Mathf.Tan(Mathf.PI * 2 / n / 2);
        var halfEdgeLength = edgeLength / 2;
        radius = halfEdgeLength / tangent;


        for (var i = 0; i < n; i++)
        {
            var angle = Mathf.PI * 2 / n * i;
            positions[i] = new Vector3(Mathf.Cos(angle) * radius, Mathf.Sin(angle) * radius);
        }
        for (var i = 0; i < 2; i++)
        {
            var angle = Mathf.PI * 2 / n * i;
            positions[n + i] = new Vector3(Mathf.Cos(angle) * radius, Mathf.Sin(angle) * radius);
        }

        lr.numPositions = positions.Length;
        lr.SetPositions(positions);
    }

    public Vector3 GetEdgePosition(int i)
    {
        var angle = Mathf.PI * 2 / n * i;
        var pos1 = new Vector3(Mathf.Cos(angle) * radius, Mathf.Sin(angle) * radius);

        angle = Mathf.PI * 2 / n * ((i + 1) % n);
        var pos2 = new Vector3(Mathf.Cos(angle) * radius, Mathf.Sin(angle) * radius);

        var pos = (pos1 + pos2) / 2;
        return pos;
    }

    public float GetEdgeAngle(int i)
    {
        var angle = Mathf.PI * 2 / (2*n) * (2*i+1);
        return angle;
    }

    public int GetEdgeNo(Vector3 pos)
    {
        pos.z = 0;
        pos = this.transform.worldToLocalMatrix * pos;

        if (pos.magnitude < radius)
        {
            return -1;
        }
        else
        {
            var angle = Mathf.Atan2(pos.y, pos.x);
            if (angle < 0) angle += Mathf.PI * 2;
            var perAngle = Mathf.PI * 2 / n;
            var edge = (int)(angle / perAngle);
            return edge;
        }
    }
}
