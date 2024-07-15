using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using Avalonia.Threading;
using DynamicData;
using MCCArkiver.Models;

namespace MCCArkiver.Managers;

public class ArchiveManager
{
    private static DispatcherTimer _timer;
    private static DateTime _repTime = DateTime.MinValue;
    private static string _game = "";
    private static string _quarantine = "";

    public static string[] _gamesArr =
        { "haloce", "halo2c", "halo3", "halo4", "halo4", "halo3odst", "haloreach", "blam", "halo2a" };
    //       0          1        2        3        4         5             6          7        8

    private static int _copyCount;
    private static int _quarCount;
    private static int _failCount;

    public static void Timer()
    {
        _timer = new();
        _timer.Tick += TimerTick;
        _timer.Interval = new TimeSpan(0, 0, 5);
    }

    public static void TimerStart()
    {
        _timer.Start();
    }

    public static void TimerStop()
    {
        _timer.Stop();
    }

    private static async void TimerTick(object? sender, EventArgs e)
    {
        await CheckSource();
    }

    private static void ResetCounters()
    {
        _copyCount = 0;
        _quarCount = 0;
        _failCount = 0;
    }

    public static async Task CheckSource()
    {
        Shisno.Status("Scanning...");
        ResetCounters();
        CarnageProbe();
        string[] games = Directory.GetDirectories($"{Main.UserContent}/UserContent");

        foreach (var game in games)
        {
            await Runner(game);
        }

        Shisno.Status(CopyDone());

        if (_copyCount > 0)
        {
            // if (Main.IsLinux)
            //     Main.Notify(CopyDone());
            Main.SaveJson(GameManager.GameCollection, Main.Conf.ArchivePath);

            GameManager.SortGames();

        }
    }

    public static async void LegacyProbe(string dir)
    {
        Shisno.Status("Scanning old Ark...");
        ResetCounters();
        foreach (var type in Directory.GetDirectories(dir))
        {
            foreach (var game in Directory.GetDirectories(type))
            {
                await Runner(game);
            }
        }

        Shisno.Status(CopyDone());
    }

    private static string CopyDone()
    {
        string msg = $"{_copyCount} files copied to arkive";
        string check = "(check log for info)";
        if (_failCount > 0)
            msg += $" \n{_failCount} failed to copy {check}";
        if (_quarCount > 0)
            msg += $" \n{_quarCount} files was quarantined {check}";
        return msg;
    }

    private static async Task Runner(string game)
    {
        try
        {
            _game = Path.GetFileName(game);
            string[] files = Directory.GetFiles(game, ".", SearchOption.AllDirectories)
                .Where(f => !f.Contains("/read/"))
                .ToArray();

            Shisno.SetProg(files.Length);
            int i = 0;
            foreach (var file in files.OrderByDescending(f => File.GetCreationTime(f)))
            {
                string fileName = Path.GetFileName(file);
                try
                {
                    await Task.Run(() => CopyToArchive(file));
                }
                catch (Exception e)
                {
                    Shisno.Error($"Failed to copy {_game}/../{fileName}", e);
                    _failCount++;
                }

                Shisno.AddProg(i++, $"{_game}/{fileName}");
            }

            GameManager.SortLatest(_game.ToUpper());
        }
        catch (Exception e)
        {
            Shisno.Error($"Scan failed", e);
        }

        Shisno.SetProg(0);
    }

    public static async void GetGames()
    {
        Main.DirCheck(Main.Conf.ArchivePath);
        GameManager.ClearGames();
        foreach (var game in Directory.GetDirectories(Main.Conf.ArchivePath))
        {
            await Task.Run(() => GetMatches(game));
        }

        GameManager.SortGames();
    }

    private static async Task GetMatches(string game)
    {
        _game = Path.GetFileName(game);
        string[] files = Directory.GetFiles(game, ".", SearchOption.AllDirectories)
            .Where(f => f.EndsWith(".mov"))
            .ToArray();
        int fileCount = files.Length;
        if (fileCount > 0)
        {
            Shisno.SetProg(fileCount);
            int i = 0;
            foreach (var film in files.OrderByDescending(f => File.GetCreationTime(f)))
            {
                Shisno.AddProg(i++, $"{_game}/.../{Path.GetFileName(film)}");
                GameManager.AddMatch(_game.ToUpper(), GetInfo(film));
            }


            Shisno.SetProg(0);
        }
    }

    private static Match GetInfo(string file)
    {
        DateTime date = FilmTime(file);
        TimeSpan span = File.GetCreationTime(file) - date;
        // DateTime date = File.GetCreationTime(file);
        Match match = new()
        {
            Game = _game,
            Date = date.ToString("yy/MM/dd HH:mm"),
            Duration = span.ToString("hh\\:mm\\:ss"),
            Title = FilmName(file),
            FilePath = file
        };
        return match;
    }

    private static string FilmName(string film)
    {
        string text = File.ReadAllText(film).Substring(0, 2500);
        string endWord = ", ";
        string split = " on ";
        try
        {
            bool isSP = false;
            string mode = "";
            string map = "";
            int start = 70;

            switch (_game.ToLower())
            {
                case "halo3":
                    if (text.Contains(@"\maps\0") || text.Contains(@"\maps\1"))
                        isSP = true;
                    break;
                case "halo3odst":
                    if (!text.Contains(Main.Conf.GamerTag))
                        isSP = true;
                    break;
                default:
                    start = 185;
                    if (text.Contains(@"\maps\0") || text.Contains(@"\maps\1"))
                        isSP = true;
                    break;
            }

            text = text.Substring(start, text.IndexOf(endWord) - start).Replace("\0", "");
            // mode = text.Substring(0, text.IndexOf(split));

            int idx = text.IndexOf(split) + split.Length;
            map = text.Substring(idx, text.Length - idx);
            if (!isSP)
                mode = mode.Substring(mode.Length / 2);

            return $"{mode} on {map}";
        }
        catch (Exception e)
        {
            Shisno.Error("Failed to get map", e);
            Console.WriteLine(e);
        }

        return "blam!";
    }

    static void CarnageProbe()
    {
        foreach (string file in Directory.GetFiles(Main.UserContent))
        {
            DateTime date = File.GetCreationTime(file);
            if (date > _repTime)
            {
                CopyToArchive(file);
                _repTime = date;
            }
        }
    }

    static void CopyToArchive(string sourceFile)
    {
        string targetDir = $"{Main.Conf.ArchivePath}";
        string targetFile = "";
        if (sourceFile != _quarantine)
        {
            string time = Time(sourceFile);
            targetDir += $"/{_game.ToLower()}/{time}";
        }
        else
        {
            targetDir += $"/{_game.ToLower()}/quarantine";
            Shisno.Alert("Quarantined, failed to get film date", $"""
                                                                  File:      {sourceFile}
                                                                  Ark:       {targetDir}
                                                                  Game:      {_game}
                                                                  """);
        }

        targetFile = $"{targetDir}/{Path.GetFileName(sourceFile)}";

        if (!File.Exists(targetFile))
        {
            Main.DirCheck(targetDir);
            File.Copy(sourceFile, $"{targetFile}");
            Shisno.Good($"Copied to {targetFile}");
            _copyCount++;

            if (sourceFile.EndsWith(".mov"))
            {
                GameManager.AddMatch(_game, GetInfo(targetFile));
            }
        }
    }

    static string Time(string file)
    {
        string time = "";
        string suffix = file.Substring(file.LastIndexOf("."));

        switch (suffix)
        {
            case ".bin":
                return MapModeTime(file);
            case ".mov":
                return TimeFormat(FilmTime(file));
            case ".mvar":
                return MapModeTime(file);
            case ".xml":
                return TimeFormat(GetRepTime(file));
        }

        return time;
    }


    //  Reads the film file's text content and tries to parse the start date
    //  Quarantined if failed.
    static DateTime FilmTime(string file)
    {
        string text = "";
        try
        {
            string format = "MMMM dd, yyyy";
            if (_game.ToLower().Equals("halo3") || _game.ToLower().Equals("halo3odst"))
            {
                format = "MMMM d, yyyy";
            }

            DateTime dt = File.GetCreationTime(file);
            string? ds = dt.ToString(format);

            text = File.ReadAllText(file).Substring(100, 1000);
            text = text.Replace($"\0", "");

            if (text.Length > 100)
            {
                int idx = text.IndexOf(ds);
                if (idx < 0)
                {
                    dt -= TimeSpan.FromDays(1);
                    ds = dt.ToString(format);
                    idx = text.IndexOf(ds);
                }

                string parsed = text.Substring(idx, ds.Length + 6);
                DateTime date = DateTime.Parse(parsed);

                return date;
            }
        }
        catch (Exception e)
        {
            Shisno.Error($"AM-FilmTime: {file}", e);
        }

        _quarCount++;
        _quarantine = file;
        return File.GetCreationTime(file);
    }

    static DateTime GetRepTime(string xml)
    {
        DateTime repDate = File.GetCreationTime(xml);
        // TimeSpan min = new TimeSpan(0, 2, 0); // 2min threshold for edge cases when someone has a really slow connection
        TimeSpan time = new();

        XElement root = XDocument.Load(xml).Root;

        if (xml.Contains("mpc"))
        {
            List<XElement> players = root.Element("Players").Elements("Player").ToList();
            XElement? player = players.FirstOrDefault(p =>
                p.Attribute("mGamertagText").Value.ToLower() == Main.Conf.GamerTag.ToLower());

            if (player is null)
                player = players.First();

            time = TimeSpan.FromSeconds(int.Parse(player.Attribute("mSecondsPlayed").Value));
            // time = TimeSpan.FromSeconds(int.Parse(root.Element("Players")
            //     .Element("Player").Attribute("mSecondsPlayed").Value));
            // _game = Main.Translate(root.Element("GameEnum").Attribute("mGameEnum").Value, "games");
            _game = _gamesArr[int.Parse(root.Element("GameEnum").Attribute("mGameEnum").Value)];
        }
        else
        {
            time = TimeSpan.Parse(root.Element("GeneralData").Attribute("Time").Value);
            _game = _gamesArr[int.Parse(root.Element("GeneralData").Attribute("GameId").Value)];
        }

        repDate -= time + new TimeSpan(0, 0, 10);

        return repDate;
    }

    static string MapModeTime(string file)
    {
        return TimeFormat(File.GetCreationTime(file));
    }

    static string TimeFormat(DateTime time)
    {
        if (time != DateTime.MinValue)
            return time.ToString("yyyy/MM-dd/HH-mm");
        return "quarantined";
    }
}