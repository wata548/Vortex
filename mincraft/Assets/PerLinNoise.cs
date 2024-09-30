using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using UnityEditor;
using System.Drawing;

public class PerLinNoise
{
    
    int seed2D = 3667;
    int seed3D = 3667;
    
    public PerLinNoise(int seed2D = 32434324, int seed3D = 324324) {

        this.seed2D = seed2D;
        this.seed3D = seed3D;
    }

    public float PerlinNoise2D(Vector2 coor, int octave = 1) {
        
        float result = 0;

        int   frequency = 1;

        while (octave-- > 0) {

            result += PerlinNoise2D(coor * frequency) / frequency;
            frequency <<= 1;
        }

        return result;
    }

    public float PerlinNoise3D(Vector3 coor, int octave = 1) {

        float result = 0;

        int frequency = 1;

        while (octave-- > 0) {

            result += PerlinNoise3D(coor * frequency) / frequency;
            frequency <<= 1;
        }

        return result;
    }

    private float PerlinNoise2D(Vector2 coor) {

        Vector2Int grid        = new(SetGrid(coor.x), SetGrid(coor.y));

        var     interval    = SetInterval();

        float   leftUp      = RandomDotProduction2D(grid.x,     grid.y,     coor); 
        float   rightUp     = RandomDotProduction2D(grid.x + 1, grid.y,     coor); 
        float   leftDown    = RandomDotProduction2D(grid.x,     grid.y + 1, coor); 
        float   rightDown   = RandomDotProduction2D(grid.x + 1, grid.y + 1, coor);

        float   lerpX1      = Lerp(interval.Item1, leftUp,      rightUp);
        float   lerpX2      = Lerp(interval.Item1, leftDown,    rightDown);
        float   result      = Lerp(interval.Item2, lerpX1,      lerpX2);

        return result + 1;

        (float, float) SetInterval() {

            float intervalX = Smooth(coor.x - grid.x);
            float intervalY = Smooth(coor.y - grid.y);

            return (intervalX, intervalY);
        }

        float RandomDotProduction2D(int gridX, int gridY, Vector2 coor) {

            UnityEngine.Random.InitState(SetSeed());

            float degree = UnityEngine.Random.Range(0, 2 * Mathf.PI);

            float deltaX = coor.x - gridX;
            float deltaY = coor.y - gridY;

            if(deltaX == 0 && deltaY == 0) {

                deltaX = 0.01f;
            }

            float dotProductionX = deltaX * Mathf.Cos(degree);
            float dotProductionY = deltaY * Mathf.Sin(degree);

            return dotProductionX + dotProductionY;

            int SetSeed() {

                int seed = 0;

                int[] RandomMultiple = { 13453, 8535};
                int[] RandomIncrese = { 7442243, 2364257};

                seed ^= gridX * RandomMultiple[0] + RandomIncrese[0];
                seed ^= gridY * RandomMultiple[1] + RandomIncrese[1];
                seed += this.seed2D;

                return seed;
            }
        }
    }

    private float PerlinNoise3D(Vector3 coor) {

        int[] checkRangeX = { 0, 1, 0, 1, 0, 1, 0, 1 };
        int[] checkRangeY = { 0, 0, 1, 1, 0, 0, 1, 1 };
        int[] checkRangeZ = { 0, 0, 0, 0, 1, 1, 1, 1 };

        Vector3Int grid = new(SetGrid(coor.x), SetGrid(coor.y), SetGrid(coor.z));

        List<float> list = new();

        for(int i = 0; i < 8; i++) {

            int currentGridX = grid.x + checkRangeX[i];
            int currentGridY = grid.y + checkRangeY[i];
            int currentGridZ = grid.z + checkRangeZ[i];

            list.Add(RandomDotProduct3D(currentGridX, currentGridY, currentGridZ, coor));
        }

        var interval = SetInterval();

        float lerpX1 = Lerp(interval.Item1, list[0], list[1]);
        float lerpX2 = Lerp(interval.Item1, list[2], list[3]);
        float lerpX3 = Lerp(interval.Item1, list[4], list[5]);
        float lerpX4 = Lerp(interval.Item1, list[6], list[7]);

        float lerpY1 = Lerp(interval.Item2, lerpX1, lerpX2);
        float lerpY2 = Lerp(interval.Item2, lerpX3, lerpX4);

        float result = Lerp(interval.Item3, lerpY1, lerpY2);
        
        return result;

        (float, float, float ) SetInterval() {

            float intervalX = Smooth(coor.x - grid.x);
            float intervalY = Smooth(coor.y - grid.y);
            float intervalZ = Smooth(coor.z - grid.z);

            return (intervalX, intervalY, intervalZ);
        }

        float RandomDotProduct3D(int gridX, int gridY, int gridZ, Vector3 coor) {

            UnityEngine.Random.InitState(SetSeed());

            float degreeFlat = UnityEngine.Random.Range(0, 2 * Mathf.PI);
            float degreeHeight = UnityEngine.Random.Range(0, 2 * Mathf.PI);

            float deltaX = coor.x - gridX;
            float deltaY = coor.y - gridY;
            float deltaZ = coor.z - gridZ;

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
                seed += this.seed3D;

                return seed;
            }
        }
    }

    float Smooth(float x) {
        return x * x * (3 - 2 * x);
    }

    float Lerp(float t, float x1, float x2) {

        return (1 - t) * x1 + t * x2;
    }

    int SetGrid(float coor) {
        int grid = (int)coor;

        if (coor < 0 && coor - grid != 0) {
            grid--;
        }

        return grid;
    }
}
