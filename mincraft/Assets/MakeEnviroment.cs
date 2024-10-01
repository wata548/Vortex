using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Drawing;
using System;
using System.IO;
using static UnityEditor.Experimental.AssetDatabaseExperimental.AssetDatabaseCounters;


public class MakeEnviroment : MonoBehaviour
{
    [SerializeField] Material grass1;
    [SerializeField] Material grass2;
    [SerializeField] Material stone;

    [SerializeField] GameObject         prefab;
    [SerializeField] int                seed                 = 14;
    [SerializeField] Vector3            startCoor            = new(0,0,0);
    [SerializeField] Vector3            interval             = new(0.1f,0.1f,0.1f);
    [SerializeField] Vector3Int         size                 = new(30, 10, 30);
    [SerializeField] bool               flat                 = false;
    [SerializeField] bool               twoD                 = false;
    [SerializeField] int                octave               = 1;
                     List<GameObject>   objects              = new();
                     PerLinNoise        perlinNoiseGenerator = null;

    [SerializeField] int                incresePoint2D       = 20;
    [SerializeField] int                foundation           = 30;

    Dictionary<Vector3, GameObject>     map                  = new();

    void Awake()
    {
        perlinNoiseGenerator = new(seed);

        if(twoD) {

            MakeEnviroment2D();
        }

        else {

            if (flat) {

                StartCoroutine(MakeEnviroment2DAnimation());
            }

            else {
                MakeEnviroment3D();
            }
        }
    }

    
    void MakeEnviroment2D() {

        MakeFoundation();

        int countX = size.x;
        int countZ = size.z;

        float indexZ = startCoor.y + transform.position.y;

        for(int i = 0; i < countZ; i++, indexZ += interval.z) {

            float indexX = startCoor.x + transform.position.x;
            for(int j = 0; j < countX; j++, indexX += interval.x) {

                float per = perlinNoiseGenerator.PerlinNoise2D(new Vector3(indexX, indexZ), octave);
                int result =  (int)( per * incresePoint2D);

                for(int k = 0; k < result; k++) {

                    GameObject tmep = Instantiate(prefab);
                    tmep.transform.position = new(transform.position.x + i, transform.position.y + k + foundation, transform.position.z + j);
                    tmep.GetComponent<MeshRenderer>().sharedMaterial = (UnityEngine.Random.Range(0, 2) == 0 ? grass1 : grass2);

                    map.Add(tmep.transform.position, tmep);
                }
            }
        }
        FixEnviroment2D();

        void MakeFoundation() {

            int countX = size.x;
            int countZ = size.z;

            float indexZ = startCoor.z;

            for (int i = 0; i < countZ; i++, indexZ += interval.z) {

                float indexX = startCoor.x;
                for (int j = 0; j < countX; j++, indexX += interval.x) {

                    for (int k = 0; k < foundation; k++) {

                        GameObject tmep = Instantiate(prefab);
                        tmep.transform.position = new(transform.position.x + i, transform.position.y + k, transform.position.z + j);
                        tmep.GetComponent<MeshRenderer>().sharedMaterial = stone;

                        map.Add(tmep.transform.position, tmep);
                    }
                }
            }
        }
        void FixEnviroment2D() {
            int countX = size.x;
            int countY = size.y;
            int countZ = size.z;

            float indexZ = startCoor.z + transform.position.z;

            for (int i = 0; i < countZ; i++, indexZ += interval.z) {

                float indexY = startCoor.y + transform.position.y;
                for (int j = 0; j < incresePoint2D * 2 + foundation; j++, indexY += interval.y) {

                    float indexX = startCoor.x + transform.position.x;
                    for (int k = 0; k < countX; k++, indexX += interval.x) {

                        float result = perlinNoiseGenerator.PerlinNoise3D(new Vector3(indexX, indexY, indexZ), octave);

                        Vector3 position = new(transform.position.x + k, transform.position.y + j, transform.position.z + i);
                        if (result < -0.25f && map.ContainsKey(position)) {

                            Destroy(map[position]);
                            map.Remove(position);
                        }
                    }
                }
            }
        }
    }

    void MakeEnviroment3D() {
        int countX = size.x;
        int countY = size.y;
        int countZ = size.z;

        float indexZ = startCoor.z + transform.position.z;

        for (int i = 0; i < countZ; i++, indexZ += interval.z) {

            float indexY = startCoor.y + transform.position.y;
            for(int j = 0; j < countY; j++, indexY += interval.y) {

                float indexX = startCoor.x + transform.position.x;
                for(int k = 0; k < countX; k++, indexX += interval.x) {

                    float result = perlinNoiseGenerator.PerlinNoise3D(new Vector3(indexX, indexY, indexZ), octave);

                    if (result > 0) {

                        GameObject tmep = Instantiate(prefab);
                        tmep.transform.position = new(transform.position.x + k, transform.position.y + j, transform.position.z + i);
                    }
                }
            }
        }
    }

    IEnumerator MakeEnviroment2DAnimation(float coorY = 0) {
        yield return new WaitForSeconds(0.01f);

        while(objects.Count != 0) {
            Destroy(objects[0]);
            objects.RemoveAt(0);
        }

        int countX = size.x;
        int countZ = size.z;

        float indexX = startCoor.x + transform.position.x;

        //List<GameObject> newList = new();
        for(int i = 0; i < countX; i++, indexX += interval.x) {

            float indexZ = startCoor.z + transform.position.z;
            for(int j = 0; j < countZ; j++, indexZ += interval.z) {

                float d = perlinNoiseGenerator.PerlinNoise3D(new Vector3(indexX, coorY + startCoor.y, indexZ), octave);

                if (d > 0) {

                    GameObject tmep = Instantiate(prefab);
                    tmep.transform.position = new(transform.position.x + j, transform.position.y + i, transform.position.z + 0);
                    objects.Add(tmep);
                }
            }
        }

        if(coorY < size.y) {

            StartCoroutine(MakeEnviroment2DAnimation(coorY + interval.y));
        }
    }

    void Update()
    {
        
    }
}
