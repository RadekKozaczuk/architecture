﻿using System;
using Unity.Services.Vivox;
using UnityEngine;
using VivoxUnity;

namespace GameLogic.Systems
{
    public static class VoiceChatSystem
    {
        static ILoginSession _session;

        public static void JoinChannel(string channelName, ChannelType channelType, bool connectAudio, bool connectText,
            bool transmissionSwitch = true, Channel3DProperties properties = null)
        {
            if (_session.State == LoginState.LoggedIn)
            {
                var channel = new Channel(channelName, channelType, properties);

                IChannelSession channelSession = _session.GetChannelSession(channel);

                channelSession.BeginConnect(connectAudio, connectText, transmissionSwitch, channelSession.GetConnectToken(), ar =>
                {
                    try
                    {
                        channelSession.EndConnect(ar);
                    }
                    catch(Exception e)
                    {
                        Debug.LogError($"Could not connect to channel: {e.Message}");
                        return;
                    }
                });
            } else
            {
                Debug.LogError("Can't join a channel when not logged in.");
            }
        }

        internal static void Login(string displayName = null)
        {
            VivoxService.Instance.Initialize();
            var account = new Account(displayName);

            _session = VivoxService.Instance.Client.GetLoginSession(account);
            _session.PropertyChanged += LoginSession_PropertyChanged;

            _session.BeginLogin(_session.GetLoginToken(), SubscriptionMode.Accept,
                                null, null, null, ar =>
            {
                try
                {
                    _session.EndLogin(ar);
                }
                catch (Exception e)
                {
                    // Unbind any login session-related events you might be subscribed to.
                    // Handle error
                    return;
                }
                // At this point, we have successfully requested to login. 
                // When you are able to join channels, LoginSession.State will be set to LoginState.LoggedIn.
                // Reference LoginSession_PropertyChanged()
            });
        }

        // For this example, we immediately join a channel after LoginState changes to LoginState.LoggedIn.
        // In an actual game, when to join a channel will vary by implementation.
        static void LoginSession_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            var loginSession = (ILoginSession)sender;
            if (e.PropertyName == "State")
            {
                if (loginSession.State == LoginState.LoggedIn)
                {
                    bool connectAudio = true;
                    bool connectText = true;

                    // This puts you into an echo channel where you can hear yourself speaking.
                    // If you can hear yourself, then everything is working and you are ready to integrate Vivox into your project.
                    JoinChannel("TestChannel", ChannelType.Echo, connectAudio, connectText);
                    // To test with multiple users, try joining a non-positional channel.
                    // JoinChannel("MultipleUserTestChannel", ChannelType.NonPositional, connectAudio, connectText);
                }
            }
        }
    }
}