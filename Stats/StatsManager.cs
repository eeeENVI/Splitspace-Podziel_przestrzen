using System;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization; 

namespace Splitspace_Podziel_przestrzen.Stats;
public class GameStats
{
    // Statystyki Sumaryczne
    public int TotalGamesPlayed { get; set; }
    public int TotalLevelsCompleted { get; set; }
    public float TotalTimePlayed { get; set; }
    public int TotalCutsPerformed { get; set; }
    public int HighestLevelReached { get; set; }
    public float WinLossRatio { get; set; }

    // Średnie 
    public float AverageAccuracy { get; set; }
    public float AverageTimePerLevel { get; set; }
    public float AverageCutsPerLevel { get; set; }

    public void UpdateWithLevel(float levelAccuracy, float levelTime, int levelCuts, int currentLevel)
    {
        TotalGamesPlayed++;
        TotalTimePlayed += levelTime;
        TotalCutsPerformed += levelCuts;

        if (currentLevel > HighestLevelReached) 
            HighestLevelReached = currentLevel;

        // Średnia krocząca
        if(TotalLevelsCompleted > 0)
        {
            AverageAccuracy = ((AverageAccuracy * (TotalLevelsCompleted - 1)) + levelAccuracy) / TotalLevelsCompleted;
            
            AverageTimePerLevel = TotalTimePlayed / TotalLevelsCompleted;
        
            AverageCutsPerLevel = (float)TotalCutsPerformed / TotalLevelsCompleted;

            WinLossRatio = (float)TotalLevelsCompleted / TotalGamesPlayed;
        }
    }
}

public static class StatsService
{
    private static string _filePath = "stats.json";

    public static void Save(GameStats stats)
    {
        var options = new JsonSerializerOptions 
        { 
            WriteIndented = true,
            NumberHandling = JsonNumberHandling.AllowNamedFloatingPointLiterals 
        };

        string jsonString = JsonSerializer.Serialize(stats, options);
        File.WriteAllText(_filePath, jsonString);
    }

    public static GameStats Load()
    {
        if (!File.Exists(_filePath)) 
            return new GameStats();

        try
        {
            string jsonString = File.ReadAllText(_filePath);
            return JsonSerializer.Deserialize<GameStats>(jsonString);
        }
        catch
        {
            return new GameStats(); // W razie błędu pliku zwraca puste staty
        }
    }
}