using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Platform.Storage;
using Avalonia.Threading;
using MCCArkiver.Models;

namespace MCCArkiver.Managers;

public class Main
{
    public static Window Win;
    public static Conf Conf { get; set; } = new();
    public static string UserContent = "";
    public static string ConfPath = "";
    public static bool IsLinux = CheckIfLinux();
    public static string DesktopFile = $"/home/{Environment.UserName}/.local/share/applications/mccarkiver.desktop";

    public static bool HasDesktop { get; set; }

    public static string[] _difficulties = { "Easy", "Normal", "Heroic", "Legendary" };

    public static readonly string Version = GetVersion();

    private static string _mccTemp = "AppData/LocalLow/MCC/Temporary";
    private static string _ark = "mccarkiver";

    public static void Init()
    {
        string name = Environment.UserName;
        if (IsLinux)
        {
            ConfPath = $"/home/{name}/.config/{_ark}";
            DirCheck(ConfPath);
            Conf = ReadConf();
            if (string.IsNullOrEmpty(Conf.Prefix))
            {
                string prefix = $"/home/{name}/.local/share/Steam/steamapps/compatdata";
                Conf.Prefix = prefix;
                Shisno.SoftAlert($"Using default prefix ({prefix}).");
            }


            if (!Directory.Exists($"{Conf.Prefix}/976730"))
                Shisno.Alert("Please confirm Proton prefix location!",
                    $"Ark was unable to find Halo: MCC's prefix (\"976730\") in the given prefix folder (\"{Conf.Prefix}\").\n" +
                    "If the game is installed on a separate drive you need to specify that Steam library's \"compatdata\" folder.\n" +
                    "It should be in \"/path/to/mount/steamapps/compatdata\"\n" +
                    "If it is correct, please ensure that the game has been launched at least once and created the prefix.");
            else
            {
                UserContent = $"{Conf.Prefix}/976730/pfx/drive_c/users/steamuser/{_mccTemp}";
                Shisno.Good("MCC prefix found!");
            }

            Conf.IsInstalled = File.Exists(DesktopFile);
        }
        else
        {
            string user = $@"C:\Users\{name}";
            ConfPath = $@"{user}\AppData\Local\{_ark}";
            DirCheck(ConfPath);
            Conf = ReadConf();
            UserContent = $@"{user}\{_mccTemp}";
        }

        if (string.IsNullOrEmpty(Conf.ArchivePath))
        {
            Shisno.Alert("No archive directory given",
                "Ark needs a place to store the files, please enter a location in the settings!");
        }

        ArchiveManager.Timer();
        GameManager.ReadGames();
    }

    public static void DirCheck(string dir)
    {
        if (!Directory.Exists(dir))
            Directory.CreateDirectory(dir);
    }

    static bool CheckIfLinux()
    {
        if (OperatingSystem.IsLinux())
            return true;
        return false;
    }

    static string GetVersion()
    {
        string v = Assembly.GetExecutingAssembly().GetName().Version.ToString();
        return $"v{v.Substring(0, v.LastIndexOf("."))}";
    }

    public static void Notify(string note)
    {
        RunAsync("notify-send", $"-t 3000 -i {ConfPath}/mccarkiver.png \"MCC Arkiver\" \"{note}\"");
    }
    
    public static async Task RunAsync(string app, string args)
    {
        try
        {

            ProcessStartInfo psi = new()
            {
                FileName = $"/bin/{app}",
                Arguments = $"{args}",
                UseShellExecute = false,
                CreateNoWindow = true,
                RedirectStandardOutput = true
            };

            Process proc = new();
            proc.StartInfo = psi;
            proc.Start();
        }
        catch (Exception e)
        {
            Shisno.Error($"Failed to launch {app}", e);
        }
    }

    public static void SaveJson(object obj, string path)
    {
        string name = ObjectName(obj);
        JsonSerializerOptions opts = new JsonSerializerOptions() { WriteIndented = true };
        string json = JsonSerializer.Serialize(obj, opts);
        string file = $"{path}/{name}.json";
        File.WriteAllText(file, json);
        // Shisno.Good($"Saved to {file}");
    }

    public static object ReadJson(object obj, string path)
    {
        string name = ObjectName(obj);
        string file = $"{path}/{name}.json";
        if (File.Exists(file))
        {
            string json = File.ReadAllText(file);
            switch (name)
            {
                case "conf":
                    return JsonSerializer.Deserialize<Conf>(json);
                case "gamecollection":
                    return JsonSerializer.Deserialize<GameCollection>(json);
            }
        }

        return obj;
    }

    private static string ObjectName(object obj)
    {
        return obj.GetType().Name.ToLower();
    }

    public static void SaveConf(Conf conf)
    {
        JsonSerializerOptions options = new JsonSerializerOptions() { WriteIndented = true };
        string json = JsonSerializer.Serialize(conf, options);
        string file = $"{ConfPath}/conf.json";

        File.WriteAllText(file, json);
        Init();
    }

    public static Conf ReadConf()
    {
        string confFile = $"{ConfPath}/conf.json";
        if (File.Exists(confFile))
        {
            string json = File.ReadAllText(confFile);
            Conf conf = JsonSerializer.Deserialize<Conf>(json);
            return conf;
        }

        Shisno.Note($"Failed to load {confFile}");
        return new Conf();
    }

    private static string Home()
    {
        string name = Environment.UserName;
        if (IsLinux)
            return $"/home/{name}";
        return $@"C:\Users\{name}";
    }

    public static async Task<string>? DirPicker(string? dir)
    {
        string? uri = dir;
        if (string.IsNullOrEmpty(dir))
            uri = Home();
        TopLevel top = TopLevel.GetTopLevel(Win);

        FolderPickerOpenOptions opts = new() { AllowMultiple = false };
        opts.SuggestedStartLocation = await top.StorageProvider.TryGetFolderFromPathAsync(new Uri(uri));

        var folder = await top.StorageProvider.OpenFolderPickerAsync(opts);

        if (folder.Count > 0)
        {
            return folder[0].Path.ToString().Replace("file://", "");
        }

        return dir;
    }

    public static void CreateDesktop()
    {
        string appsDir = $"/home/{Environment.UserName}/.local/share/applications";
        string app = "mccarkiver";
        string dir = Directory.GetCurrentDirectory();
        string appName = $"{Directory.GetFiles(dir).FirstOrDefault(a => a.ToLower().Contains($"{app}.app"))}";

        DirCheck(appsDir);

        string content = $"""
                          [Desktop Entry]
                          Encoding=UTF-8
                          Version={Version}
                          Type=Application
                          Terminal=false
                          Exec={appName}
                          Icon={ConfPath}/{app}.png
                          Name=MCC Arkiver
                          Description=File keeper for Halo: MCC
                          """;
        File.WriteAllText(DesktopFile, content);
        Shisno.Good($"Added .desktop file to {appsDir}");
    }

    public static string Translate(string raw, string file)
    {
        file = $"{ConfPath}/Library/{file.ToLower()}.txt";
        if (File.Exists(file))
        {
            try
            {
                foreach (string line in File.ReadLines(file))
                {
                    if (raw.Contains(line.Split(',')[0]))
                    {
                        raw = line.Split(',')[1];
                        break;
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        return raw;
    }
}