using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using MCCArkiver.Models;

namespace MCCArkiver.Managers;

public class GameManager
{
    public static GameCollection GameCollection = new();

    public static void ReadGames()
    {
        GameCollection.Games.Clear();
        GameCollection gc = (GameCollection)Main.ReadJson(GameCollection, Main.Conf.ArchivePath);
        foreach (var game in gc.Games)
        {
            AddGame(game);
            // SortLatest(game.Name);
            foreach (var match in game.Matches)
            {
                match.GameCheck();
            }
        }
        SortGames();
    }

    public static void ClearGames()
    {
        GameCollection.Games.Clear();
    }

    public static void AddGame(Game game)
    {
        GameCollection.Games.Add(game);
    }

    public static void AddMatch(string gameName, Match match)
    {
        if (!GameCollection.Games.Any(g => g.Name == gameName.ToUpper()))
            AddGame(new Game() { Name = gameName.ToUpper() });
        Game game = GameCollection.Games.FirstOrDefault(g => g.Name == gameName.ToUpper());
        match.GameCheck();
        if (!game.Matches.Any(m => m.FilePath == match.FilePath))
        {
            game.Matches.Add(match);
        }
    }

    public static void SortGames()
    {
        List<Game> games = GameCollection.Games.OrderBy(g => g.Name).ToList();
        ClearGames();
        foreach (var game in games)
        {
            AddGame(game);
        }
    }

    public static void SortLatest(string name)
    {
        try
        {
            if (GameCollection.Games.Any(g => g.Name == name.ToUpper()))
            {
                Game game = GameCollection.Games.FirstOrDefault(g => g.Name == name);
                List<Match> matches = game.Matches.OrderByDescending(m => m.Date).ToList();
                
                game.Matches.Clear();
                foreach (var match in matches)
                {
                    game.Matches.Add(match);
                }
            }
        }
        catch (Exception e)
        {
            Shisno.Error("Failed to sort games", e);
        }
    }

    public static void GetGames()
    {
        ObservableCollection<Game> emptyList = new();
        string[] lines = File.ReadAllLines($"{Main.ConfPath}/Library/games.txt");
        foreach (string line in lines)
        {
            emptyList.Add(new Game() { Name = line.Substring(line.IndexOf(',') + 1), Matches = new() });
        }

        GameCollection.Games = emptyList;
    }

    public static void GetArkGames(string path)
    {
        foreach (string dir in Directory.GetDirectories(path))
        {
            GameCollection.Games.Add(new Game()
            {
                Name = Path.GetFileName(dir),
                Matches = new()
            });
        }
    }
}