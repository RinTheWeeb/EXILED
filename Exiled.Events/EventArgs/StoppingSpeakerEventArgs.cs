// -----------------------------------------------------------------------
// <copyright file="StoppingSpeakerEventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs
{
    using System;

    using Exiled.API.Features;

    /// <summary>
    /// Contains all informations before SCP-079 finishes using a speaker.
    /// </summary>
    public class StoppingSpeakerEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StoppingSpeakerEventArgs"/> class.
        /// </summary>
        /// <param name="player"><inheritdoc cref="Player"/></param>
        /// <param name="room"><inheritdoc cref="Room"/></param>
        /// <param name="isAllowed"><inheritdoc cref="IsAllowed"/></param>
        public StoppingSpeakerEventArgs(Player player, Room room, bool isAllowed = true)
        {
            Player = player;
            Room = room;
            IsAllowed = isAllowed;
        }

        /// <summary>
        /// Gets the player who's stopping the speaker through SCP-079.
        /// </summary>
        public Player Player { get; }

        /// <summary>
        /// Gets the room where the camera is located, that SCP-079 is stopping.
        /// </summary>
        public Room Room { get; }

        /// <summary>
        /// Gets or sets a value indicating whether the event can be executed or not.
        /// </summary>
        public bool IsAllowed { get; set; }
    }
}
