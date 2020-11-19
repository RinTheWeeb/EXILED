// -----------------------------------------------------------------------
// <copyright file="UnlockingGeneratorEventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs
{
    using System;

    using Exiled.API.Features;

    /// <summary>
    /// Contains all informations before a generator is unlocked.
    /// </summary>
    public class UnlockingGeneratorEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UnlockingGeneratorEventArgs"/> class.
        /// </summary>
        /// <param name="player"><inheritdoc cref="Player"/></param>
        /// <param name="generator"><inheritdoc cref="Generator"/></param>
        /// <param name="isAllowed"><inheritdoc cref="IsAllowed"/></param>
        public UnlockingGeneratorEventArgs(Player player, Generator079 generator, bool isAllowed = true)
        {
            Player = player;
            Generator = generator;
            IsAllowed = isAllowed;
        }

        /// <summary>
        /// Gets the player who's unlocking the generator.
        /// </summary>
        public Player Player { get; }

        /// <summary>
        /// Gets the generator that is going to be unlocked.
        /// </summary>
        public Generator079 Generator { get; }

        /// <summary>
        /// Gets or sets a value indicating whether the event can be executed or not.
        /// </summary>
        public bool IsAllowed { get; set; }
    }
}
