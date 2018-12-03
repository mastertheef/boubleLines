using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.Models
{
    public class BallState
    {
        public int x { get; set; }
        public int y { get; set; }
        public Color Color { get; set; }

        public BallState(Node node)
        {
            x = node.x;
            y = node.y;
            Color = node.ball.GetComponentInChildren<Ball>().Color;
        }
    }

    public class MoveState
    {
        public Node Start { get; set; }
        public Node End { get; set; }
        public List<BallState> DestroyedAfterMove { get; set; }
        public List<BallState> Appeared { get; set; }
        public List<BallState> DestroyedAfterAppear { get; set; }
        public int ScoreAdded { get; set; }

        public MoveState()
        {
            DestroyedAfterMove = new List<BallState>();
            DestroyedAfterAppear = new List<BallState>();
            Appeared = new List<BallState>();
        }

        public void AddDestroyedAfterMove(List<Node> nodes)
        {
            foreach (var node in nodes)
            {
                DestroyedAfterMove.Add(new BallState(node));
            }
        }

        public void AddDestroyedAfterAppear(List<Node> nodes)
        {
            foreach (var node in nodes)
            {
                DestroyedAfterAppear.Add(new BallState(node));
            }
        }

    }

}
