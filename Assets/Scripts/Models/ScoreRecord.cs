[System.Serializable]
public class ScoreRecord 
{
    public string PlayerName { get; set; }
    public int HighScore { get; set; }

    public ScoreRecord(string PlayerName, int HighScore)
    {
        this.PlayerName = PlayerName;
        this.HighScore = HighScore;
    }
}
