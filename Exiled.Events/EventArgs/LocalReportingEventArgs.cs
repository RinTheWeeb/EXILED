// -----------------------------------------------------------------------
// <copyright file="LocalReportingEventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs
{
    using System;

    using Exiled.API.Features;

    /// <summary>
    /// Contains informations before a report is sent to local administrators.
    /// </summary>
    public class LocalReportingEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LocalReportingEventArgs"/> class.
        /// </summary>
        /// <param name="issuer"><inheritdoc cref="Issuer"/></param>
        /// <param name="target"><inheritdoc cref="Target"/></param>
        /// <param name="reason"><inheritdoc cref="Reason"/></param>
        /// <param name="isAllowed"><inheritdoc cref="IsAllowed"/></param>
        public LocalReportingEventArgs(Player issuer, Player target, string reason, bool isAllowed = true)
        {
            Issuer = issuer;
            Target = target;
            Reason = reason;
            IsAllowed = isAllowed;
        }

        /// <summary>
        /// Gets the report issuer.
        /// </summary>
        public Player Issuer { get; }

        /// <summary>
        /// Gets the report target.
        /// </summary>
        public Player Target { get; }

        /// <summary>
        /// Gets or sets the report reason.
        /// </summary>
        public string Reason { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the process can be processed or not.
        /// </summary>
        public bool IsAllowed { get; set; }
    }
}
