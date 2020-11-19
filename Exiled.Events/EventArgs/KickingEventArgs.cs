// -----------------------------------------------------------------------
// <copyright file="KickingEventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs
{
    using System;
    using System.Reflection;

    using Exiled.API.Features;
    using Exiled.Events;

    /// <summary>
    /// Contains all informations before kicking a player from the server.
    /// </summary>
    public class KickingEventArgs : EventArgs
    {
        private Player target;
        private Player issuer;
        private bool isAllowed;

        /// <summary>
        /// Initializes a new instance of the <see cref="KickingEventArgs"/> class.
        /// </summary>
        /// <param name="target"><inheritdoc cref="Target"/></param>
        /// <param name="issuer"><inheritdoc cref="Issuer"/></param>
        /// <param name="reason"><inheritdoc cref="Reason"/></param>
        /// <param name="fullMessage"><inheritdoc cref="FullMessage"/></param>
        /// <param name="isAllowed"><inheritdoc cref="IsAllowed"/></param>
        public KickingEventArgs(Player target, Player issuer, string reason, string fullMessage, bool isAllowed = true)
        {
            Target = target;
            Issuer = issuer;
            Reason = reason;
            FullMessage = fullMessage;
            IsAllowed = isAllowed;
        }

        /// <summary>
        /// Gets or sets the ban target.
        /// </summary>
        public Player Target
        {
            get => target;
            set
            {
                if (value == null || target == value)
                    return;

                if (Events.Instance.Config.ShouldLogBans && target != null)
                {
                    LogBanChange(Assembly.GetCallingAssembly().GetName().Name
                    + $" changed the banned player from user {target.Nickname} ({target.UserId}) to {value.Nickname} ({value.UserId})");
                }

                target = value;
            }
        }

        /// <summary>
        /// Gets or sets the ban issuer.
        /// </summary>
        public Player Issuer
        {
            get => issuer;
            set
            {
                if (value == null || issuer == value)
                    return;

                if (Events.Instance.Config.ShouldLogBans && issuer != null)
                {
                    LogBanChange(Assembly.GetCallingAssembly().GetName().Name
                                   + $" changed the ban issuer from user {issuer.Nickname} ({issuer.UserId}) to {value.Nickname} ({value.UserId})");
                }

                issuer = value;
            }
        }

        /// <summary>
        /// Gets or sets the kick reason.
        /// </summary>
        public string Reason { get; set; }

        /// <summary>
        /// Gets or sets the full kick message.
        /// </summary>
        public string FullMessage { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the event can be executed or not.
        /// </summary>
        public bool IsAllowed
        {
            get => isAllowed;
            set
            {
                if (isAllowed == value)
                    return;

                if (Events.Instance.Config.ShouldLogBans)
                    LogBanChange(Assembly.GetCallingAssembly().GetName().Name + $" {(value ? "allowed" : "denied")} banning user with ID: {Target.UserId}");

                isAllowed = value;
            }
        }

        /// <summary>
        /// Logs the kick, anti-backdoor protection from malicious plugins.
        /// </summary>
        /// <param name="message">The message to be logged.</param>
        protected void LogBanChange(string message)
        {
            lock (ServerLogs.LockObject)
            {
                Log.Warn($"[ANTI-BACKDOOR]: {message} - {TimeBehaviour.FormatTime("yyyy-MM-dd HH:mm:ss.fff zzz")}");
            }

            ServerLogs._state = ServerLogs.LoggingState.Write;
        }
    }
}
