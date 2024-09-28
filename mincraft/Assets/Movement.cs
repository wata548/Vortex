using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    public float h = 0, v = 90;
    float degree;
    Vector3 coor;
    public GameObject prefab;

    Vector3 ViewPointVector(float h, float v) {
        Vector3 view = new(Mathf.Cos(v) * Mathf.Cos(h), Mathf.Sin(v), Mathf.Cos(v) * Mathf.Sin(h));
        return view;
    }

    private void Awake() {
        coor = transform.position;
        degree = Mathf.PI / 180;
    }

    void FixedUpdate()
    {
        v += degree * 20;

        if (v >= 2 * Mathf.PI) {
            v = 0;
            h += degree * 20;
        }
        if(h >= 2 * Mathf.PI) {
            Destroy(this);
        }
            

        Instantiate(prefab).transform.position = coor + ViewPointVector(h, v) * 5;
        this.transform.position = coor + ViewPointVector(h, v) * 5;
    }

}
