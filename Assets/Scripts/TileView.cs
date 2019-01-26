using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileView : MonoBehaviour {
    public Node node;
    public float spacing = 0.1f;

    public void Init(Node node, float offset, Vector3 tileSize)
    {
        this.node = node;
        transform.position = new Vector2(node.x * (tileSize.x + spacing) - offset, node.y * (tileSize.y + spacing) - offset);
    }
}
