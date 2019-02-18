using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOverPopup : PopupBase
{
    private GameOverController gameOverController;
    public override void ClosePopup()
    {
        base.ClosePopup();
        gameOverController = GameObject.Find("GameOverController").GetComponent<GameOverController>();
        gameOverController.StartGameOverRoutine();
    }
}
