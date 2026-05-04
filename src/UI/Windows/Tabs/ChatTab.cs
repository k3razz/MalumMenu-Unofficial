using UnityEngine;
using Hazel;
using InnerNet;

namespace MalumMenu;

public class ChatTab : ITab
{
    public string name => "Chat";

    private static bool _textSpam = false; 
    private static bool _quickSpam = false; 
    private static float _lastsent = 0f;
    private static float _lastQuickSent = 0f;

    private static void SendQuickChat(byte msg1, ushort msg2, byte msg3, byte msg4, ushort msg7)
    {
        try
        {
            MessageWriter messageWriter = AmongUsClient.Instance.StartRpcImmediately(((InnerNetObject)PlayerControl.LocalPlayer).NetId, 33, SendOption.None);
            messageWriter.Write(msg1);
            messageWriter.Write(msg2);
            messageWriter.Write(msg3);
            messageWriter.Write(msg4);
            messageWriter.Write(msg7);
            AmongUsClient.Instance.FinishRpcImmediately(messageWriter);
            
            MessageWriter messageWriter2 = AmongUsClient.Instance.StartRpcImmediately(((InnerNetObject)PlayerControl.LocalPlayer).NetId, 33, SendOption.None, ((InnerNetObject)PlayerControl.LocalPlayer).OwnerId);
            messageWriter2.Write(msg1);
            messageWriter2.Write(msg2);
            messageWriter2.Write(msg3);
            messageWriter2.Write(msg4);
            messageWriter2.Write(msg7);
            AmongUsClient.Instance.FinishRpcImmediately(messageWriter2);
        }
        catch { }
    }

    public void Draw()
    {
        GUILayout.BeginVertical(GUILayout.Width(MenuUI.windowWidth * 0.425f));

        DrawGeneral();
        GUILayout.Space(15);
        DrawTextbox();
        GUILayout.Space(15);
        DrawSpammerUI();

        GUILayout.EndVertical();
    }

    private void DrawGeneral()
    {
        CheatToggles.enableChat = GUILayout.Toggle(CheatToggles.enableChat, " Enable Chat");
        CheatToggles.bypassUrlBlock = GUILayout.Toggle(CheatToggles.bypassUrlBlock, " Bypass URL Block");
        CheatToggles.lowerRateLimits = GUILayout.Toggle(CheatToggles.lowerRateLimits, " Lower Rate Limits");
    }

    private void DrawTextbox()
    {
        GUILayout.Label("Textbox", GUIStylePreset.TabSubtitle);
        CheatToggles.unlockCharacters = GUILayout.Toggle(CheatToggles.unlockCharacters, " Unlock Extra Characters");
        CheatToggles.longerMessages = GUILayout.Toggle(CheatToggles.longerMessages, " Allow Longer Messages");
        CheatToggles.unlockClipboard = GUILayout.Toggle(CheatToggles.unlockClipboard, " Unlock Clipboard");
    }

    private void DrawSpammerUI()
    {
        GUILayout.Label("Spammer", GUIStylePreset.TabSubtitle);
        
        _textSpam = GUILayout.Toggle(_textSpam, " Enable Text Spam");
        _quickSpam = GUILayout.Toggle(_quickSpam, " Enable QuickChat Spam");
        GUILayout.Label("Thanks for QuickChat spam to todyaramello!");
        GUILayout.Label("The QuickChat spam kicks, be careful using it");

        GUILayout.Space(10);
        
        string textStatus;
        if (_textSpam)
        {
            float textInterval = MalumMenu.spamInterval.Value; 
            float textTimeLeft = Mathf.Max(0, (_lastsent + textInterval) - Time.time);
            textStatus = $"{textTimeLeft:F1}s";
        }
        else
        {
            textStatus = "Disabled";
        }
        GUILayout.Label($"Next TextChat spam message: {textStatus}");

        string quickStatus;
        if (_quickSpam)
        {
            float quickTimeLeft = Mathf.Max(0, (_lastQuickSent + 0.5f) - Time.time);
            quickStatus = $"{quickTimeLeft:F1}s";
        }
        else
        {
            quickStatus = "Disabled";
        }
        GUILayout.Label($"Next QuickChat spam message: {quickStatus}");

        Spammer();
    }

    private void Spammer()
    {
        if (PlayerControl.LocalPlayer == null) return;

        if (_textSpam && Time.time > _lastsent + MalumMenu.spamInterval.Value) 
        {
            try 
            { 
                PlayerControl.LocalPlayer.RpcSendChat(MalumMenu.spamMessage.Value); 
                _lastsent = Time.time;
            } 
            catch { }
        }

        if (_quickSpam && Time.time > _lastQuickSent + 0.5f)
        {
            SendQuickChat(3, 78, 1, 2, (ushort)MalumMenu.quickChatSpamID.Value); 
            _lastQuickSent = Time.time;
        }
    }
}