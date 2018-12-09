using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Linq;

public class FileController {

    private const string ScoreFile = "scoreFile.bl";
    private const string SettingsFile = "settings.bl";
    private const int MaxScoreRecords = 10;

	public static int GetHighScore()
    {
        string highScoreFile = string.Format("{0}/{1}", Application.persistentDataPath, ScoreFile);
        if (!File.Exists(highScoreFile))
        {
            return 0;
        }

        if (File.ReadAllBytes(highScoreFile).Length == 0)
        {
            return 0;
        }

        var formatter = new BinaryFormatter();
        using (var fileStream = new FileStream(highScoreFile, FileMode.Open))
        {
            var scoreList = formatter.Deserialize(fileStream) as List<ScoreRecord>;
            int result = 0;
            if (scoreList != null)
            {
                result = scoreList.Any()
                    ? scoreList.FirstOrDefault().HighScore
                    : 0;
            }

            fileStream.Close();
            return result;
        }
    }

    public static List<ScoreRecord> GetAllRecords()
    {
        string highScoreFile = string.Format("{0}/{1}", Application.persistentDataPath, ScoreFile);
        if (!File.Exists(highScoreFile))
        {
            return new List<ScoreRecord>();
        }

        if (File.ReadAllBytes(highScoreFile).Length == 0)
        {
            return new List<ScoreRecord>();
        }

        var formatter = new BinaryFormatter();
        using (var fileStream = new FileStream(highScoreFile, FileMode.Open))
        {
            var result = formatter.Deserialize(fileStream) as List<ScoreRecord>;
            
            fileStream.Close();
            return result;
        }
    }

    public static void AddRecord(ScoreRecord record)
    {
        string highScoreFile = string.Format("{0}/{1}", Application.persistentDataPath, ScoreFile);
        var formatter = new BinaryFormatter();

        bool fileEmpty = !File.Exists(highScoreFile) || File.ReadAllBytes(highScoreFile).Length == 0;
        using (var fileStream = new FileStream(highScoreFile, FileMode.OpenOrCreate, FileAccess.ReadWrite))
        {
            List<ScoreRecord> list = new List<ScoreRecord>();

            if (fileEmpty)
            {
                list.Add(record);
            }
            else
            {
                list = formatter.Deserialize(fileStream) as List<ScoreRecord>;
                list.Insert(0, record);
                if (list.Count > MaxScoreRecords)
                {
                    list.RemoveAt(MaxScoreRecords);
                }
            }
            formatter.Serialize(fileStream, list);
            fileStream.Close();
        }
    }

    public static Settings GetSettings()
    {
        string settingsFile = string.Format("{0}/{1}", Application.persistentDataPath, SettingsFile);

        if (!File.Exists(settingsFile))
        {
            using (var fileStream = new FileStream(settingsFile, FileMode.Create, FileAccess.Write))
            {
                var formatter = new BinaryFormatter();
                var defaultSettings = new Settings
                {
                    BubbleVolume = 1f,
                    MusicVolume = 1f
                };

                formatter.Serialize(fileStream, defaultSettings);
                fileStream.Close();
                return defaultSettings;
            }
        }

        using (var fileStream = new FileStream(settingsFile, FileMode.Open, FileAccess.Read))
        {
            var formatter = new BinaryFormatter();
            var settings = formatter.Deserialize(fileStream) as Settings;
            fileStream.Close();
            return settings;
        }
    }

    public static void SetSettings(Settings newSettings)
    {
        string settingsFile = string.Format("{0}/{1}", Application.persistentDataPath, SettingsFile);
        using (var fileStream = new FileStream(settingsFile, FileMode.OpenOrCreate, FileAccess.Write))
        {
            var formatter = new BinaryFormatter();
            formatter.Serialize(fileStream, newSettings);
            fileStream.Close();
        }
    }
}
