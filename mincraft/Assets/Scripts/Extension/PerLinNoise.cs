using System.Collections.Generic;
using UnityEngine;
using System;
using Random = System.Random;

public class PerLinNoise
{
    
   //==================================================||Fields 
    private readonly int SEED = 3667;
    
   //==================================================||Constructors 
    public PerLinNoise(int pSeed = 12234324) {
        SEED = pSeed;
    }

   //==================================================||Mehods 
    public float Get(float pX, float pY, int pOctave = 1) =>
        Get(new Vector2(pX, pY), pOctave);

    public float Get(float pX, float pY, float pZ, int pOctave = 1) =>
        Get(new Vector3(pX, pY, pZ), pOctave);
    
    public float Get(Vector2 pPoint, int pOctave = 1) {
        
        float result = 0;
        int frequency = 1;

        while (pOctave-- > 0) {
            result += PerlinNoise2D(pPoint * frequency) / frequency;
            frequency <<= 1;
        }

        return result;
    }

    public float Get(Vector3 pPoint, int pOctave = 1) {

        float result = 0;
        int frequency = 1;

        while (pOctave-- > 0) {

            result += PerlinNoise3D(pPoint * frequency) / frequency;
            frequency <<= 1;
        }

        return result;
    }

    private float PerlinNoise2D(Vector2 pPoint) {

        var floor = new Func<float, int>(Mathf.FloorToInt)!;
        var grid = new Vector2Int(floor(pPoint.x), floor(pPoint.y));

        var     interval    = SetInterval();

        float   leftUp      = RandomDotProduction2D(grid.x,     grid.y,     pPoint); 
        float   rightUp     = RandomDotProduction2D(grid.x + 1, grid.y,     pPoint); 
        float   leftDown    = RandomDotProduction2D(grid.x,     grid.y + 1, pPoint); 
        float   rightDown   = RandomDotProduction2D(grid.x + 1, grid.y + 1, pPoint);

        float   lerpX1      = Lerp(interval.Item1, leftUp,      rightUp);
        float   lerpX2      = Lerp(interval.Item1, leftDown,    rightDown);
        float   result      = Lerp(interval.Item2, lerpX1,      lerpX2);

        return result;

        (float, float) SetInterval() {

            float intervalX = Smooth(pPoint.x - grid.x);
            float intervalY = Smooth(pPoint.y - grid.y);

            return (intervalX, intervalY);
        }

        float RandomDotProduction2D(int gridX, int gridY, Vector2 coor) {

            float degree = (GetSeed() % 10000 / 10000f * 2 * Mathf.PI);
            float deltaX = coor.x - gridX;
            float deltaY = coor.y - gridY;

            if(deltaX == 0 && deltaY == 0) {

                deltaX = 0.01f;
            }

            float dotProductionX = deltaX * Mathf.Cos(degree);
            float dotProductionY = deltaY * Mathf.Sin(degree);

            return dotProductionX + dotProductionY;

            int GetSeed() {

                int seed = 0;

                int[] RandomMultiple = { 13453, 8535};
                int[] RandomIncrese = { 74243, 23647};

                seed ^= gridX * RandomMultiple[0] + RandomIncrese[0];
                seed ^= gridY * RandomMultiple[1] + RandomIncrese[1];
                seed += SEED;

                return seed;
            }
        }
    }

    private float PerlinNoise3D(Vector3 pPoint) {

        int[] checkRangeX = { 0, 1, 0, 1, 0, 1, 0, 1 };
        int[] checkRangeY = { 0, 0, 1, 1, 0, 0, 1, 1 };
        int[] checkRangeZ = { 0, 0, 0, 0, 1, 1, 1, 1 };

        var floor = new Func<float, int>(Mathf.FloorToInt)!;
        var grid = new Vector3Int(
            floor(pPoint.x),
            floor(pPoint.y),
            floor(pPoint.z)
        );

        List<float> list = new();

        for(int i = 0; i < 8; i++) {

            int currentGridX = grid.x + checkRangeX[i];
            int currentGridY = grid.y + checkRangeY[i];
            int currentGridZ = grid.z + checkRangeZ[i];

            list.Add(RandomDotProduct3D(currentGridX, currentGridY, currentGridZ, pPoint));
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

            float intervalX = Smooth(pPoint.x - grid.x);
            float intervalY = Smooth(pPoint.y - grid.y);
            float intervalZ = Smooth(pPoint.z - grid.z);

            return (intervalX, intervalY, intervalZ);
        }

        float RandomDotProduct3D(int gridX, int gridY, int gridZ, Vector3 coor) {

            var r = new Random(GetSeed());

            float degreeFlat = (float)r.NextDouble() * 2 * Mathf.PI;
            float degreeHeight = (float)r.NextDouble() * 2 * Mathf.PI;

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

            int GetSeed() {
                int seed = 0;

                int[] RandomMultiple = { 66513, 13, 733 };
                int[] RandomIncrese = { 13415, 143, -73 };

                seed ^= gridX * RandomMultiple[0] + RandomIncrese[0];
                seed ^= gridY * RandomMultiple[1] + RandomIncrese[1];
                seed ^= gridZ * RandomMultiple[2] + RandomIncrese[2];
                seed += SEED;

                return seed;
            }
        }
    }

    private float Smooth(float pX) {
        return pX * pX * (3 - 2 * pX);
    }

    private float Lerp(float pT, float pX1, float pX2) {

        return (1 - pT) * pX1 + pT * pX2;
    }
}