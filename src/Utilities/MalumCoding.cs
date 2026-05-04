using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using Il2CppInterop.Runtime.Injection;

namespace MalumMenu;

public static class MalumCoding
{
    public static readonly string CodePath = Path.Combine(BepInEx.Paths.ConfigPath, "MalumCoding.mlum");
    private static Dictionary<string, string> vars = new();
    private static string currentLib = "";
    private static List<string> scriptLines = new();
    private static int currentIndex = 0;
    private static float waitTimer = 0f;
    private static bool isRunning = false;
    private static bool typeRegistered = false;

    private static Stack<LoopData> loopStack = new();

    private class LoopData
    {
        public int StartIndex;
        public int EndIndex;
        public int RemainingCount;
    }

    public static void Execute()
    {
        if (!typeRegistered)
        {
            ClassInjector.RegisterTypeInIl2Cpp<MalumScriptUpdater>();
            typeRegistered = true;
        }

        if (!File.Exists(CodePath))
        {
            File.WriteAllText(CodePath, "var speed = 5.0\nplayer spd speed");
            return;
        }

        scriptLines = File.ReadAllLines(CodePath).Select(l => l.Trim()).ToList();
        vars.Clear();
        currentLib = "";
        currentIndex = 0;
        waitTimer = 0f;
        loopStack.Clear();
        isRunning = true;

        GameObject updater = GameObject.Find("MalumUpdater");
        if (updater == null)
        {
            updater = new GameObject("MalumUpdater");
            updater.hideFlags = HideFlags.HideAndDontSave;
            updater.AddComponent<MalumScriptUpdater>();
        }
    }

    public static void Tick()
    {
        if (!isRunning || currentIndex >= scriptLines.Count)
        {
            if (currentIndex >= scriptLines.Count && isRunning)
            {
                isRunning = false;
                ConsoleUI.Log("Script finished");
            }
            return;
        }

        if (waitTimer > 0)
        {
            waitTimer -= Time.deltaTime;
            return;
        }

        string line = scriptLines[currentIndex];

        if (string.IsNullOrEmpty(line) || line.StartsWith("//"))
        {
            currentIndex++;
            return;
        }

        if (line.StartsWith("var "))
        {
            HandleVariable(line);
            currentIndex++;
            return;
        }

        if (line.StartsWith("lib "))
        {
            currentLib = line.Substring(4).Trim().ToLower();
            currentIndex++;
            return;
        }

        if (line.StartsWith("wait "))
        {
            string val = ReplaceVars(line.Substring(5).Trim());
            if (float.TryParse(val, NumberStyles.Any, CultureInfo.InvariantCulture, out float sec))
            {
                waitTimer = sec;
            }
            currentIndex++;
            return;
        }

        if (line.StartsWith("loop "))
        {
            string countStr = ReplaceVars(line.Split(' ')[1]);
            if (int.TryParse(countStr, out int count))
            {
                int end = FindEndLoop(currentIndex);
                if (end != -1)
                {
                    loopStack.Push(new LoopData 
                    { 
                        StartIndex = currentIndex + 1, 
                        EndIndex = end, 
                        RemainingCount = count - 1 
                    });
                    currentIndex++;
                    return;
                }
            }
        }

        if (line == "endloop")
        {
            if (loopStack.Count > 0)
            {
                var loop = loopStack.Peek();
                if (loop.RemainingCount > 0)
                {
                    loop.RemainingCount--;
                    currentIndex = loop.StartIndex;
                }
                else
                {
                    loopStack.Pop();
                    currentIndex++;
                }
            }
            else
            {
                currentIndex++;
            }
            return;
        }

        ParseAndExecute(line);
        currentIndex++;
    }

    private static int FindEndLoop(int start)
    {
        int nest = 0;
        for (int i = start + 1; i < scriptLines.Count; i++)
        {
            if (scriptLines[i].StartsWith("loop ")) nest++;
            if (scriptLines[i] == "endloop")
            {
                if (nest == 0) return i;
                nest--;
            }
        }
        return -1;
    }

    private static void ParseAndExecute(string line)
    {
        string processed = ReplaceVars(line);
        string[] parts = processed.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length < 2 && string.IsNullOrEmpty(currentLib)) return;

        string lib = (parts.Length >= 2 && (parts[0] == "player" || parts[0] == "sabotage" || parts[0] == "anims" || parts[0] == "print")) ? parts[0].ToLower() : currentLib;
        string[] args = (parts.Length >= 2 && (parts[0] == "player" || parts[0] == "sabotage" || parts[0] == "anims" || parts[0] == "print")) ? parts.Skip(1).ToArray() : parts;

        try
        {
            switch (lib)
            {
                case "player": HandlePlayer(args); break;
                case "sabotage": HandleSabotage(args); break;
                case "anims": HandleAnims(args); break;
                case "print": ConsoleUI.Log(string.Join(" ", args)); break;
            }
        }
        catch (Exception ex)
        {
            ConsoleUI.Log($"Error: {ex.Message}");
        }
    }

    private static void HandleVariable(string line)
    {
        var parts = line.Substring(4).Split(new[] { '=' }, 2);
        if (parts.Length == 2) vars[parts[0].Trim()] = ReplaceVars(parts[1].Trim());
    }

    private static string ReplaceVars(string input)
    {
        foreach (var v in vars) input = input.Replace(v.Key, v.Value);
        return input;
    }

    private static void HandlePlayer(string[] args)
    {
        string cmd = args[0].ToLower();
        switch (cmd)
        {
            case "spd":
                if (args[1] == "default") PlayerControl.LocalPlayer.MyPhysics.Speed = Utils.DefaultSpeed;
                else if (float.TryParse(args[1], NumberStyles.Any, CultureInfo.InvariantCulture, out float s)) PlayerControl.LocalPlayer.MyPhysics.Speed = s;
                break;
            case "noclip":
                CheatToggles.noClip = bool.Parse(args[1]);
                break;
            case "meeting":
                if (args[1] == "call") CheatToggles.callMeeting = true;
                else if (args[1] == "close") CheatToggles.closeMeeting = true;
                break;
            case "vents":
                CheatToggles.unlockVents = bool.Parse(args[2]);
                break;
        }
    }

    private static void HandleSabotage(string[] args)
    {
        string target = args[0].ToLower();
        if (target == "doors")
        {
            CheatToggles.showDoorsMenu = bool.Parse(args[2]);
            return;
        }
        bool val = bool.Parse(args[1]);
        switch (target)
        {
            case "reactor": CheatToggles.reactorSab = val; break;
            case "oxygen": CheatToggles.oxygenSab = val; break;
            case "lights": CheatToggles.elecSab = val; break;
            case "comms": CheatToggles.commsSab = val; break;
            case "mushroom_mixup": CheatToggles.mushSab = val; break;
            case "triggerspores": CheatToggles.mushSpore = val; break;
        }
    }

    private static void HandleAnims(string[] args)
    {
        bool val = bool.Parse(args[1]);
        switch (args[0].ToLower())
        {
            case "med": CheatToggles.animMedScan = val; break;
            case "garbage": CheatToggles.animEmptyGarbage = val; break;
            case "cams": CheatToggles.animCamsInUse = val; break;
            case "asteroids": CheatToggles.animAsteroids = val; break;
            case "shields": CheatToggles.animShields = val; break;
        }
    }
}

public class MalumScriptUpdater : MonoBehaviour
{
    public MalumScriptUpdater(IntPtr ptr) : base(ptr) { }

    private void Update()
    {
        MalumCoding.Tick();
    }
}