using UnityEngine;

namespace MalumMenu;

public static class TracersHandler
{
    public static void DrawPlayerTracer(PlayerPhysics playerPhysics)
    {
        try
        {
            var color = Color.clear;

            if (!playerPhysics.myPlayer.Data.IsDead)
            {
                if (CheatToggles.tracersCrew && !playerPhysics.myPlayer.Data.Role.IsImpostor ||
                    CheatToggles.tracersImps && playerPhysics.myPlayer.Data.Role.IsImpostor)
                {
                    if (CheatToggles.colorBasedTracers)
                    {
                        color = playerPhysics.myPlayer.Data.Color;
                    }
                    else
                    {
                        color = playerPhysics.myPlayer.Data.Role.TeamColor;
                    }
                }
            }

            Utils.DrawTracer(playerPhysics.myPlayer.gameObject, PlayerControl.LocalPlayer.gameObject, color);
        } catch { }
    }

    public static void DrawBodyTracer(DeadBody deadBody)
    {
        var color = Color.clear;

        if (CheatToggles.tracersBodies)
        {
            if (CheatToggles.colorBasedTracers)
            {
                color = GameData.Instance.GetPlayerById(deadBody.ParentId).Color;
            }
            else
            {
                color = Color.yellow;
            }
        }

        Utils.DrawTracer(deadBody.gameObject, PlayerControl.LocalPlayer.gameObject, color);
    }
}