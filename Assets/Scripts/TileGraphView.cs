using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(TileGraph))]
public class TileGraphView : MonoBehaviour {

    public GameObject tileViewPrefab;

    public void Init(TileGraph graph, float offset, Vector3 tileSize)
    {
        var parent = GameObject.Find("Tiles");
        foreach (var n in graph.tileNodes)
        {
            var tile = Instantiate(tileViewPrefab, Vector3.zero, Quaternion.identity, parent.transform);
            n.tile = tile.GetComponent<TileView>();
            tile.GetComponent<TileView>().Init(n, offset, tileSize);
        }
    }
}
