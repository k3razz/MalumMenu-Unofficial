using System;
using HarmonyLib;
using UnityEngine;

namespace MalumMenu;

[HarmonyPatch(typeof(PlayerPhysics), nameof(PlayerPhysics.LateUpdate))]
public static class PlayerPhysics_LateUpdate
{
    public static void Postfix(PlayerPhysics __instance)
    {
        MalumESP.PlayerNametags(__instance);
        MalumESP.SeeGhostsCheat(__instance);

        MalumCheats.NoClipCheat();
        MalumCheats.KillAllCheat();
        MalumCheats.KillAllCrewCheat();
        MalumCheats.KillAllImpsCheat();
        MalumCheats.ForceStartGameCheat();
        MalumCheats.TeleportCursorCheat();
        MalumCheats.CompleteMyTasksCheat();
        MalumCheats.PlayAnimationCheat();
        MalumCheats.PlayScannerCheat();

        MalumPPMCheats.EjectPlayerPPM();
        MalumPPMCheats.SpectatePPM();
        MalumPPMCheats.KillPlayerPPM();
        MalumPPMCheats.TelekillPlayerPPM();
        MalumPPMCheats.TeleportPlayerPPM();
        MalumPPMCheats.SetFakeRolePPM();
        // MalumPPMCheats.ForceRolePPM();

        TracersHandler.DrawPlayerTracer(__instance);

        GameObject[] bodyObjects = GameObject.FindGameObjectsWithTag("DeadBody");
        foreach(GameObject bodyObject in bodyObjects)
        {
            DeadBody deadBody = bodyObject.GetComponent<DeadBody>();

            if (!deadBody || deadBody.Reported) continue;
            TracersHandler.DrawBodyTracer(deadBody);
        }

        try
        {
            PlayerControl.LocalPlayer.MyPhysics.Speed = Mathf.Abs(PlayerControl.LocalPlayer.MyPhysics.Speed);
            PlayerControl.LocalPlayer.MyPhysics.GhostSpeed = Mathf.Abs(PlayerControl.LocalPlayer.MyPhysics.GhostSpeed);
        } catch (NullReferenceException) { }
    }
}
