using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

public class PerLinNoise
{
    
    int seed = 3667;
    
    (float, float, float) interval = (0,0,0);

    public PerLinNoise(int seed) {

        this.seed = seed;
    }

    public float PerLinNoise3D(Vector3 coor) {

        int[] checkRangeX = { 0, 1, 0, 1, 0, 1, 0, 1 };
        int[] checkRangeY = { 0, 0, 1, 1, 0, 0, 1, 1 };
        int[] checkRangeZ = { 0, 0, 0, 0, 1, 1, 1, 1 };

        Vector3 grid = FixVectorToGrid(coor);

        SetInterval();

        List<float> list = new();

        for(int i = 0; i < 8; i++) {

            int currentGridX = (int)grid.x + checkRangeX[i];
            int currentGridY = (int)grid.y + checkRangeY[i];
            int currentGridZ = (int)grid.z + checkRangeZ[i];

            list.Add(RandomDotProduct(currentGridX, currentGridY, currentGridZ, coor));
        }

        float lerpX1 = Lerp(interval.Item1, list[0], list[1]);
        float lerpX2 = Lerp(interval.Item1, list[2], list[3]);
        float lerpX3 = Lerp(interval.Item1, list[4], list[5]);
        float lerpX4 = Lerp(interval.Item1, list[6], list[7]);

        float lerpY1 = Lerp(interval.Item2, lerpX1, lerpX2);
        float lerpY2 = Lerp(interval.Item2, lerpX3, lerpX4);

        float result = Lerp(interval.Item3, lerpY1, lerpY2);
        
        return result;

        Vector3 FixVectorToGrid(Vector3 coor) {

            int      x    = (int)coor.x;
            int      y    = (int)coor.y;
            int      z    = (int)coor.z;
            Vector3  grid = new(x, y, z);

            return grid;
        }

        void SetInterval() {

            float x = Smrp(coor.x - grid.x);
            float y = Smrp(coor.y - grid.y);
            float z = Smrp(coor.z - grid.z);

            interval = (x, y, z);
        }
    }

    float Smrp(float x) {
        return x * x * (3 - 2 * x);
    }

    float Lerp(float t, float x1, float x2) {

        return (1 - t) * x1 + t * x2;
    }

    private float RandomDotProduct(int gridX, int gridY, int gridZ, Vector3 coor) {

        UnityEngine.Random.InitState(SetSeed());

        float degreeFlat    = UnityEngine.Random.Range(0, 2 * Mathf.PI);
        float degreeHeight  = UnityEngine.Random.Range(0, 2 * Mathf.PI);

        float deltaX        = coor.x - gridX;
        float deltaY        = coor.y - gridY;
        float deltaZ        = coor.z - gridZ;

        if (deltaX == 0 && deltaY == 0 && deltaZ == 0) {

            deltaX = 0.01f;
        }

        float dotProductionX = deltaX * Mathf.Cos(degreeHeight) * Mathf.Cos(degreeFlat);
        float dotProductionY = deltaY * Mathf.Sin(degreeHeight);
        float dotProductionZ = deltaZ * Mathf.Cos(degreeHeight) * Mathf.Sin(degreeFlat);

        return dotProductionX + dotProductionY + dotProductionZ;

        int SetSeed() {
            int seed = 0;

            int[] RandomMultiple = { 66513, 13, 733 };
            int[] RandomIncrese = { 13415, 143, -73 };

            seed ^= gridX * RandomMultiple[0] + RandomIncrese[0];
            seed ^= gridY * RandomMultiple[1] + RandomIncrese[1];
            seed ^= gridZ * RandomMultiple[2] + RandomIncrese[2];
            seed += this.seed;

            return seed;
        }
    }
}
