using System;
using System.ComponentModel;
using Unity.Services.Vivox;
using UnityEngine;
using VivoxUnity;

namespace GameLogic.Systems
{
    static class VoiceChatSystem
    {
        static ILoginSession _session;
        static IChannelSession _channel;
        static readonly VivoxUnity.Client _client = new Client();

        static Action callbackToRunWhenLogin;

        internal static void Login(Action callback,string displayName = null) {
            callbackToRunWhenLogin = callback;
            var account = new Account(displayName);

            _session = VivoxService.Instance.Client.GetLoginSession(account);
            _session.PropertyChanged += LoginSession_PropertyChanged;

            _session.BeginLogin(_session.GetLoginToken(), SubscriptionMode.Accept, null, null, null, ar =>
            {
                try
                {
                    _session.EndLogin(ar);
                }
                catch (Exception e)
                {
                    // Unbind any login session-related events you might be subscribed to.
                    // Handle error
                    Debug.LogError($"Could not login: {e.Message}");
                }
                // At this point, we have successfully requested to login. 
                // When you are able to join channels, LoginSession.State will be set to LoginState.LoggedIn.
                // Reference LoginSession_PropertyChanged()
            });
        }

        internal static void JoinChannel(string channelName, ChannelType channelType, bool connectAudio, bool connectText, bool transmissionSwitch = true,
            Channel3DProperties properties = null)
        {
            if (_session.State == LoginState.LoggedIn)
            {
                var channel = new Channel(channelName, channelType, properties);

                IChannelSession channelSession = _session.GetChannelSession(channel);

                _channel = channelSession;

                channelSession.BeginConnect(connectAudio, connectText, transmissionSwitch, channelSession.GetConnectToken(), ar =>
                {
                    try
                    {
                        channelSession.EndConnect(ar);
                    }
                    catch (Exception e)
                    {
                        Debug.LogError($"Could not connect to channel: {e.Message}");
                    }
                });
            }
            else
                Debug.LogError("Can't join a channel when not logged in.");
        }

        internal static void LeaveCurrentChannel() {
            if (_channel != null)
                // Disconnect from channel
                _channel.Disconnect();
        }

        // For this example, we immediately join a channel after LoginState changes to LoginState.LoggedIn.
        // In an actual game, when to join a channel will vary by implementation.
        static void LoginSession_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var loginSession = (ILoginSession)sender;
            if (e.PropertyName == "State")
                if (loginSession.State == LoginState.LoggedIn)
                {
                    callbackToRunWhenLogin?.Invoke();
                    callbackToRunWhenLogin = null;
                    bool connectAudio = true;
                    bool connectText = true;

                    // This puts you into an echo channel where you can hear yourself speaking.
                    // If you can hear yourself, then everything is working and you are ready to integrate Vivox into your project.
                    JoinChannel("TestChannel", ChannelType.Echo, connectAudio, connectText);
                    // To test with multiple users, try joining a non-positional channel.
                    // JoinChannel("MultipleUserTestChannel", ChannelType.NonPositional, connectAudio, connectText);
                }
        }

        internal static void ToggleMuteInput(bool mute) {
            _client.AudioInputDevices.Muted = mute;
            /*
            _session.SetTransmissionMode(mute ? TransmissionMode.None : TransmissionMode.All);
            Debug.LogError(_session.TransmissionType);
            */
        }
    }
}