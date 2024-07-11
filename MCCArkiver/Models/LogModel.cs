using System;

namespace MCCArkiver.Models;

public class LogModel
{
    public long Timestamp { get; set; } = DateTimeOffset.Now.ToUnixTimeMilliseconds();
    public string? Time { get; set; } = $"[{DateTime.Now.ToString("HH:mm:ss")}]";
    public string? Title { get; set; }
    public string? Message { get; set; }
    public bool IsError { get; set; }
    public bool IsAlert { get; set; }
    public bool IsSoftAlert { get; set; }
    public bool IsNote { get; set; }
    public bool IsUpdate { get; set; }
    public bool IsGood { get; set; }
    public bool IsDebug { get; set; }
    public int Level { get; set; } = 2;
}