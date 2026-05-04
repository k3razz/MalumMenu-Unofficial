using UnityEngine;
using System;
using System.Collections.Generic;

namespace MalumMenu;

public class TeleportTab : ITab
{
    public string name => "Teleport";

    private Vector2 scrollPosition;

    public void Draw()
    {
        GUILayout.BeginVertical(GUILayout.Width(MenuUI.windowWidth * 0.425f));
        
        DrawRoomsOnly();

        GUILayout.EndVertical();
    }

    private void DrawRoomsOnly()
    {
        var rooms = MapRooms.GetCurrentMapRooms();

        if (rooms != null)
        {
            scrollPosition = GUILayout.BeginScrollView(scrollPosition, GUILayout.Height(350f));
            
            int count = 0;
            foreach (var room in rooms)
            {
                if (count % 2 == 0) GUILayout.BeginHorizontal();

                if (GUILayout.Button(room.Key, GUIStylePreset.NormalButton, GUILayout.Width(130f)))
                {
                    TPToRoom(room.Value);
                }

                count++;
                if (count % 2 == 0) GUILayout.EndHorizontal();
            }
            
            if (count % 2 != 0) GUILayout.EndHorizontal();

            GUILayout.EndScrollView();
        }
        else
        {
            GUILayout.Label("Not in game or in lobby!");
        }
    }

    private void TPToRoom(Vector2 roomPos)
    {
        if (PlayerControl.LocalPlayer != null)
        {
            PlayerControl.LocalPlayer.NetTransform.RpcSnapTo(roomPos);
        }
    }
}

public static class MapRooms
{
    public static Dictionary<string, Vector2> Skeld = new() {
        { "Cafeteria", new Vector2(-5.91f, 0.68f) }, { "MedBay", new Vector2(-9.05f, -1.04f) }, 
        { "UpperEngine", new Vector2(-14.95f, 0.68f) }, { "Reactor", new Vector2(-19.22f, -5.69f) }, 
        { "Security", new Vector2(-14.26f, -5.69f) }, { "LowerEngine", new Vector2(-15.05f, -11.96f) }, 
        { "Electrical", new Vector2(-9.56f, -13.66f) }, { "Storage", new Vector2(-5.10f, -14.61f) }, 
        { "Storage 2", new Vector2(-0.73f, -9.34f) }, { "Admin", new Vector2(2.02f, -7.52f) }, 
        { "Comms", new Vector2(5.31f, -14.28f) }, { "Shields", new Vector2(6.97f, -12.66f) }, 
        { "Nav", new Vector2(15.46f, -5.21f) }, { "LifeSupp", new Vector2(7.61f, -3.88f) }, 
        { "Weapons", new Vector2(9.50f, -1.27f) }
    };

    public static Dictionary<string, Vector2> Mira = new() {
        { "Cafeteria", new Vector2(23.85f, 4.07f) }, { "Admin", new Vector2(19.23f, 18.51f) }, 
        { "Office", new Vector2(16.45f, 17.48f) }, { "Greenhouse", new Vector2(17.80f, 21.31f) }, 
        { "Storage", new Vector2(19.53f, 0.96f) }, { "Balcony", new Vector2(22.20f, -1.55f) }, 
        { "Comms", new Vector2(13.86f, 4.01f) }, { "LockerRoom", new Vector2(10.94f, 4.01f) }, 
        { "Decontamination", new Vector2(6.08f, 2.98f) }, { "Laboratory", new Vector2(7.35f, 13.58f) }, 
        { "Reactor", new Vector2(4.94f, 10.24f) }, { "MedBay", new Vector2(13.87f, -0.13f) }, 
        { "Launchpad", new Vector2(-4.25f, 0.01f) }
    };

    public static Dictionary<string, Vector2> Polus = new() {
        { "Office", new Vector2(22.06f, -18.75f) }, { "Admin", new Vector2(20.90f, -20.64f) }, 
        { "Weapons", new Vector2(12.99f, -21.25f) }, { "Comms", new Vector2(11.05f, -18.48f) }, 
        { "LifeSupp", new Vector2(6.14f, -22.96f) }, { "BoilerRoom", new Vector2(3.41f, -23.38f) }, 
        { "Electrical", new Vector2(5.51f, -14.02f) }, { "Security", new Vector2(2.52f, -11.22f) }, 
        { "Dropship", new Vector2(16.55f, -6.40f) }, { "Storage", new Vector2(18.58f, -11.23f) }, 
        { "Laboratory", new Vector2(24.78f, -9.84f) }, { "Decontamation2", new Vector2(38.31f, -10.20f) }, 
        { "Specimens", new Vector2(38.52f, -19.54f) }, { "Decontamation3", new Vector2(23.80f, -24.26f) }
    };

    public static Dictionary<string, Vector2> Airship = new() {
        { "Cockpit", new Vector2(-16.45f, -1.14f) }, { "Kitchen", new Vector2(-8.47f, -12.49f) }, 
        { "ViewingDeck", new Vector2(-8.91f, -12.49f) }, { "HallOfPortraits", new Vector2(-1.30f, -12.70f) }, 
        { "Security", new Vector2(4.68f, -12.70f) }, { "Electrical", new Vector2(9.64f, -10.79f) }, 
        { "Showers", new Vector2(17.85f, -0.39f) }, { "MainHall", new Vector2(17.26f, -0.39f) }, 
        { "Engine", new Vector2(4.14f, -0.39f) }, { "Armory", new Vector2(-10.73f, -3.02f) }, 
        { "Comms", new Vector2(-13.00f, -0.35f) }, { "Medical", new Vector2(21.47f, -9.05f) }, 
        { "CargoBay", new Vector2(32.47f, -4.77f) }, { "Ventilation", new Vector2(29.38f, -1.80f) }, 
        { "Lounge", new Vector2(34.05f, 3.95f) }, { "Records", new Vector2(23.61f, 8.72f) }, 
        { "VaultRoom", new Vector2(-4.44f, 8.14f) }, { "GapRoom 1", new Vector2(3.00f, 8.14f) }, 
        { "GapRoom 2", new Vector2(16.17f, 8.91f) }, { "Brig", new Vector2(-3.57f, 8.14f) }, 
        { "MeetingRoom", new Vector2(4.48f, 14.33f) }
    };

    public static Dictionary<string, Vector2> Fungle = new() {
        { "MeetingRoom", new Vector2(-4.85f, -0.51f) }, { "RecRoom", new Vector2(-13.99f, -0.80f) }, 
        { "Cafeteria", new Vector2(-19.57f, 5.69f) }, { "Dropship", new Vector2(-5.96f, 7.67f) }, 
        { "Storage", new Vector2(-1.72f, 4.01f) }, { "Mining Pit_Down", new Vector2(11.69f, 9.54f) }, 
        { "Mining Pit_Up 1", new Vector2(19.75f, 7.15f) }, { "Mining Pit_Up 2", new Vector2(21.52f, 9.85f) }, 
        { "Comms", new Vector2(18.40f, 12.98f) }, { "Lookout", new Vector2(10.85f, 2.82f) }, 
        { "UpperEngine", new Vector2(20.77f, 2.78f) }, { "Reactor", new Vector2(19.40f, -7.52f) }, 
        { "Greenhouse", new Vector2(11.46f, -10.00f) }, { "Greenhouse 2", new Vector2(11.47f, -12.35f) }, 
        { "Laboratory", new Vector2(-4.35f, -8.56f) }, { "SleepingQuarters", new Vector2(0.21f, -1.90f) }, 
        { "Kitchen", new Vector2(-15.68f, -6.24f) }, { "FishingDock", new Vector2(-19.68f, -7.90f) }
    };

    public static Dictionary<string, Vector2> GetCurrentMapRooms()
    {
        if (Utils.isSkeldMap) return Skeld;
        if (Utils.isMiraHQMap) return Mira;
        if (Utils.isPolusMap) return Polus;
        if (Utils.isAirshipMap) return Airship;
        if (Utils.isFungleMap) return Fungle;
        return null;
    }
}