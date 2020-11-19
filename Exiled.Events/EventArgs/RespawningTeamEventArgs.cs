// -----------------------------------------------------------------------
// <copyright file="RespawningTeamEventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs
{
    using System;
    using System.Collections.Generic;

    using Exiled.API.Features;

    using Respawning;

    using UnityEngine;

    /// <summary>
    /// Contains all informations before spawning a wave of <see cref="SpawnableTeamType.NineTailedFox"/> or <see cref="SpawnableTeamType.ChaosInsurgency"/>.
    /// </summary>
    public class RespawningTeamEventArgs : EventArgs
    {
        private SpawnableTeamType nextKnownTeam;

        /// <summary>
        /// Initializes a new instance of the <see cref="RespawningTeamEventArgs"/> class.
        /// </summary>
        /// <param name="players"><inheritdoc cref="Players"/></param>
        /// <param name="nextKnownTeam"><inheritdoc cref="NextKnownTeam"/></param>
        /// <param name="isAllowed"><inheritdoc cref="IsAllowed"/></param>
        public RespawningTeamEventArgs(List<Player> players, SpawnableTeamType nextKnownTeam, bool isAllowed = true)
        {
            Players = players;
            NextKnownTeam = nextKnownTeam;
            IsAllowed = isAllowed;
        }

        /// <summary>
        /// Gets the list of players that are going to be respawned.
        /// </summary>
        public List<Player> Players { get; }

        /// <summary>
        /// Gets or sets the maximum amount of respawnable players.
        /// </summary>
        public int MaximumRespawnAmount { get; set; }

        /// <summary>
        /// Gets or sets a value indicating what the next respawnable team is..
        /// </summary>
        public SpawnableTeamType NextKnownTeam
        {
            get => nextKnownTeam;
            set
            {
                nextKnownTeam = value;
                ReissueNextKnownTeam();
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the event can be executed or not.
        /// </summary>
        public bool IsAllowed { get; set; }

        /// <summary>
        /// Gets the current spawnable team.
        /// </summary>
        internal SpawnableTeam? SpawnableTeam => RespawnWaveGenerator.SpawnableTeams.TryGetValue(NextKnownTeam, out var team) ? (SpawnableTeam?)team : null;

        private void ReissueNextKnownTeam()
        {
            var team = SpawnableTeam;
            if (team != null)
            {
                // Refer to the game code
                int a = RespawnTickets.Singleton.GetAvailableTickets(NextKnownTeam);
                if (a == 0)
                {
                    a = RespawnTickets.DefaultTeamAmount;
                    RespawnTickets.Singleton.GrantTickets(RespawnTickets.DefaultTeam, RespawnTickets.DefaultTeamAmount, true);
                }

                MaximumRespawnAmount = Mathf.Min(a, team.Value.MaxWaveSize);
            }
        }
    }
}
