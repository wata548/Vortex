using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Drawing;
using System;
using System.IO;
using UnityEngine.Serialization;
using static UnityEditor.Experimental.AssetDatabaseExperimental.AssetDatabaseCounters;


public class MakeEnviroment : MonoBehaviour
{
    [SerializeField] Material grass1;
    [SerializeField] Material grass2;
    [SerializeField] Material stone;

    [SerializeField] GameObject _prefab;
    [SerializeField] int _seed = 14;
    [SerializeField] Vector3 _origin = new(0,0,0);
    [SerializeField] Vector3 _perlinInterval = new(0.1f,0.1f,0.1f);
    [SerializeField] Vector3Int _chunkSize = new(10, 10, 10);
    [SerializeField] bool _isAnimate = false;
    [SerializeField] bool _hasHole = false;
    [SerializeField] int _octave = 1;
    List<GameObject>   _animationPool              = new();
    PerLinNoise        _perlinNoise = null;

    [SerializeField] int                incresePoint2D       = 20;
    [SerializeField] int                foundation           = 30;

    Dictionary<Vector3, GameObject>     map                  = new();

    void Awake()
    {
        _perlinNoise = new(_seed);

        if (_isAnimate) {
            StartCoroutine(MakeEnv2DAnimation());
            return;
        }
        if (_hasHole)
            MakeEnv3D();
        else 
            MakeEnv2D();
    }

    
    void MakeEnv3D() {

        MakeFoundation();

        int countX = _chunkSize.x;
        int countZ = _chunkSize.z;

        float indexZ = _origin.y + transform.position.y;

        for(int i = 0; i < countZ; i++, indexZ += _perlinInterval.z) {

            float indexX = _origin.x + transform.position.x;
            for(int j = 0; j < countX; j++, indexX += _perlinInterval.x) {

                float per = _perlinNoise.Get(indexX, indexZ, _octave);
                int result =  (int)( per * incresePoint2D);

                for(int k = 0; k < result; k++) {

                    GameObject tmep = Instantiate(_prefab);
                    tmep.transform.position = new(transform.position.x + i, transform.position.y + k + foundation, transform.position.z + j);
                    tmep.GetComponent<MeshRenderer>().sharedMaterial = (UnityEngine.Random.Range(0, 2) == 0 ? grass1 : grass2);

                    map.Add(tmep.transform.position, tmep);
                }
            }
        }
        FixEnv3D();

        void MakeFoundation() {

            int countX = _chunkSize.x;
            int countZ = _chunkSize.z;

            float indexZ = _origin.z;

            for (int i = 0; i < countZ; i++, indexZ += _perlinInterval.z) {

                float indexX = _origin.x;
                for (int j = 0; j < countX; j++, indexX += _perlinInterval.x) {

                    for (int k = 0; k < foundation; k++) {

                        GameObject temp = Instantiate(_prefab);
                        temp.transform.position = new(transform.position.x + i, transform.position.y + k, transform.position.z + j);
                        temp.GetComponent<MeshRenderer>().sharedMaterial = stone;

                        map.Add(temp.transform.position, temp);
                    }
                }
            }
        }
        void FixEnv3D() {

            int countX = _chunkSize.x;
            int countY = _chunkSize.y;
            int countZ = _chunkSize.z;

            float indexZ = _origin.z + transform.position.z;

            for (int i = 0; i < countZ; i++, indexZ += _perlinInterval.z) {

                float indexY = _origin.y + transform.position.y;
                for (int j = 0; j < incresePoint2D * 2 + foundation; j++, indexY += _perlinInterval.y) {

                    float indexX = _origin.x + transform.position.x;
                    for (int k = 0; k < countX; k++, indexX += _perlinInterval.x) {

                        float result = _perlinNoise.Get(new Vector3(indexX, indexY, indexZ), _octave);

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

    void MakeEnv2D() {

        int countX = _chunkSize.x;
        int countY = _chunkSize.y;
        int countZ = _chunkSize.z;

        float indexZ = _origin.z + transform.position.z;

        for (int i = 0; i < countZ; i++, indexZ += _perlinInterval.z) {

            float indexY = _origin.y + transform.position.y;
            for(int j = 0; j < countY; j++, indexY += _perlinInterval.y) {

                float indexX = _origin.x + transform.position.x;
                for(int k = 0; k < countX; k++, indexX += _perlinInterval.x) {

                    float result = _perlinNoise.Get(new Vector3(indexX, indexY, indexZ), _octave);

                    if (result > 0) {

                        var temp = Instantiate(_prefab);
                        temp.transform.position = new(transform.position.x + k, transform.position.y + j, transform.position.z + i);
                    }
                }
            }
        }
    }

    IEnumerator MakeEnv2DAnimation(float coorY = 0) {

        
        yield return new WaitForSeconds(0.1f);

        while(_animationPool.Count != 0) {
            Destroy(_animationPool[0]);
            _animationPool.RemoveAt(0);
        }

        int countX = _chunkSize.x;
        int countZ = _chunkSize.z;

        float indexX = _origin.x + transform.position.x;

        //List<GameObject> newList = new();
        for(int i = 0; i < countX; i++, indexX += _perlinInterval.x) {

            float indexZ = _origin.z + transform.position.z;
            for(int j = 0; j < countZ; j++, indexZ += _perlinInterval.z) {

                float d = _perlinNoise.Get(new Vector3(indexX, coorY + _origin.y, indexZ), _octave);

                if (d > 0) {

                    GameObject tmep = Instantiate(_prefab);
                    tmep.transform.position = new(transform.position.x + j, transform.position.y + i, transform.position.z + 0);
                    _animationPool.Add(tmep);
                }
            }
        }

        if(coorY < _chunkSize.y) {

            StartCoroutine(MakeEnv2DAnimation(coorY + _perlinInterval.y));
        }
    }
}