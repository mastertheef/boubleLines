using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileMatrix : MonoBehaviour {

    public int width = 10;
    public int height = 10;

    public int startingCloseTiles = 3;

	public int[,] MakeStartMap()
    {
        int[,] map = new int[width, height];
        for (int w = 0; w < width; w++)
        {
            for (int h = 0; h< height; h++)
            {
                map[w, h] = 0;
            }
        }

        for (int i = 0; i<startingCloseTiles; i++)
        {
            map[Random.Range(0, width), Random.Range(0, height)] = 1;
        }

        return map;
    }
}
