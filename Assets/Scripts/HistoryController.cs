using Assets.Scripts.Models;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class HistoryController : MonoBehaviour {

    private LinkedList<MoveState> moves;
    private LinkedListNode<MoveState> current;
    private TileGraph tileGraph;

    public Button stepBackButton;
	
	void Start () {
        moves = new LinkedList<MoveState>();
        current = null;
        tileGraph = GameObject.Find("TileGraph").GetComponent<TileGraph>();
        stepBackButton.enabled = false;

    }
	
    public MoveState AddMove(MoveState move)
    {
        if (!moves.Any() || current.Next == null)
        {
            moves.AddLast(move);
            current = moves.Last;
        }
        else
        {
            current.Next.Value.Start = move.Start;
            current.Next.Value.End = move.End;

            current.Next.Value.DestroyedAfterMove = move.DestroyedAfterMove;

            foreach (var ballState in current.Next.Value.Appeared)
            {
                if (tileGraph.tileNodes[ballState.x, ballState.y].ball != null)
                {
                    var newNode = tileGraph.GetRandomEmpty();
                    var nextState = current.Next.Value.Appeared.First(x => x.x == ballState.x && x.y == ballState.y);
                    nextState.x = newNode.x;
                    nextState.y = newNode.y;
                }
            }

            current = current.Next;
        }
        stepBackButton.enabled = true;
        return current.Value;
    }

    public bool hasPrevious()
    {
        return current != null && current.Previous != null;
    }

    
    public MoveState GetLastMove()
    {
        if (moves.Any() && current.Previous != null)
        {
            var result = current;
            current = hasPrevious() 
                ? current.Previous
                : current;
            stepBackButton.enabled = hasPrevious();
            return result.Value;
        }

        return null;
    }
	
    public MoveState GetNextMove()
    {
        if (moves.Any() && current.Next != null)
        {
            current = current.Next;
            return current.Value;
        }
        return null;
    }
}
