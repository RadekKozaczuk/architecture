#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
using System;
using Core;
using Unity.Services.Vivox;
using UnityEngine;

namespace GameLogic.Systems
{
    static class VoiceChatSystem
    {
        static Action _callbackToRunWhenLogin;

        internal static bool IsMuted => VivoxService.Instance != null && VivoxService.Instance.IsInputDeviceMuted;

        internal static async void Login(Action callback)
        {
            _callbackToRunWhenLogin = callback;

            try
            {
                await VivoxService.Instance.LoginAsync();
                _callbackToRunWhenLogin?.Invoke();
                _callbackToRunWhenLogin = null!;

            }
            catch (Exception e)
            {
                Debug.LogError($"Could not login: {e.Message}");
            }
        }

        internal static async void JoinChannel(string channelName, bool connectAudio, bool connectText, bool transmissionSwitch = true)
        {
            try
            {
                ChatCapability chatCapability = ChatCapability.TextOnly;
                if (connectAudio && connectText)
                    chatCapability = ChatCapability.TextAndAudio;
                else if (connectAudio)
                    chatCapability = ChatCapability.AudioOnly;
                else if (connectText)
                    chatCapability = ChatCapability.TextOnly;

                await VivoxService.Instance.JoinGroupChannelAsync(channelName, chatCapability);
            }
            catch (Exception e)
            {
                Debug.LogError($"Could not connect to channel: {e.Message}");
            }
        }

        internal static async void LeaveChannel()
        {
            if (VivoxService.Instance.ActiveChannels.Count > 0)
            {
                try
                {
                    await VivoxService.Instance.LeaveAllChannelsAsync();
                }
                catch (Exception e)
                {
                    Debug.LogError($"Could not leave channels: {e.Message}");
                }
            }
            else
                Debug.LogWarning("Attempted to leave channels, but no active channels are found.");
        }

        internal static void ToggleMuteInput()
        {
            if (IsMuted)
                VivoxService.Instance.UnmuteInputDevice();
            else
                VivoxService.Instance.MuteInputDevice();

            Signals.ToggleMuteVoiceChat();
        }
    }
}
