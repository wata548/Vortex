using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Drawing;
using System;
using System.IO;


public class MakeEnviroment : MonoBehaviour
{
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

        int countX = size.x;
        int countY = size.y;

        float indexY = startCoor.y;

        for(int i = 0; i < countY; i++, indexY += interval.y) {

            float indexX = startCoor.x;
            for(int j = 0; j < countX; j++, indexX += interval.x) {

                float per = perlinNoiseGenerator.PerlinNoise2D(new Vector3(indexX, indexY), octave);
                int result =  (int)( per * incresePoint2D);

                for(int k = 0; k < result; k++) {

                    GameObject tmep = Instantiate(prefab);
                    tmep.transform.position = new(transform.position.x + i, transform.position.y + k, transform.position.z + j);
                }
            }
        }

    }

    void MakeEnviroment3D() {
        int countX = size.x;
        int countY = size.y;
        int countZ = size.z;

        float indexZ = startCoor.z;

        for (int i = 0; i < countZ; i++, indexZ += interval.z) {

            float indexY = startCoor.y;
            for(int j = 0; j < countY; j++, indexY += interval.y) {

                float indexX = startCoor.x;
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

        float indexX = startCoor.x;

        //List<GameObject> newList = new();
        for(int i = 0; i < countX; i++, indexX += interval.x) {

            float indexZ = startCoor.z;
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
