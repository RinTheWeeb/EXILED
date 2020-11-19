// -----------------------------------------------------------------------
// <copyright file="ChangingKnobSettingEventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs
{
    using System;

    using Exiled.API.Features;

    using global::Scp914;

    /// <summary>
    /// Contains all informations before a player changes the SCP-914 knob setting.
    /// </summary>
    public class ChangingKnobSettingEventArgs : EventArgs
    {
        private Scp914Knob knobSetting;

        /// <summary>
        /// Initializes a new instance of the <see cref="ChangingKnobSettingEventArgs"/> class.
        /// </summary>
        /// <param name="player"><inheritdoc cref="Player"/></param>
        /// <param name="knobSetting"><inheritdoc cref="KnobSetting"/></param>
        /// <param name="isAllowed"><inheritdoc cref="IsAllowed"/></param>
        public ChangingKnobSettingEventArgs(Player player, Scp914Knob knobSetting, bool isAllowed = true)
        {
            Player = player;
            KnobSetting = knobSetting;
            IsAllowed = isAllowed;
        }

        /// <summary>
        /// Gets the player who's changing the SCP-914 knob setting.
        /// </summary>
        public Player Player { get; }

        /// <summary>
        /// Gets or sets the SCP-914 knob setting.
        /// </summary>
        public Scp914Knob KnobSetting
        {
            get => knobSetting;
            set => knobSetting = value > Scp914Machine.knobStateMax ? Scp914Machine.knobStateMin : value;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the event can be executed or not.
        /// </summary>
        public bool IsAllowed { get; set; }
    }
}
