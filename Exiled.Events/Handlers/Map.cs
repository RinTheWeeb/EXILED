// -----------------------------------------------------------------------
// <copyright file="Map.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Handlers
{
    using Exiled.Events.EventArgs;
    using Exiled.Events.Extensions;

    using static Exiled.Events.Events;

    /// <summary>
    /// Map related events.
    /// </summary>
    public static class Map
    {
        /// <summary>
        /// Invoked before placing decals.
        /// </summary>
        public static event CustomEventHandler<PlacingDecalEventArgs> PlacingDecal;

        /// <summary>
        /// Invoked before placing bloods.
        /// </summary>
        public static event CustomEventHandler<PlacingBloodEventArgs> PlacingBlood;

        /// <summary>
        /// Invoked before announcing the light containment zone decontamination.
        /// </summary>
        public static event CustomEventHandler<AnnouncingDecontaminationEventArgs> AnnouncingDecontamination;

        /// <summary>
        /// Invoked before announcing an SCP termination.
        /// </summary>
        public static event CustomEventHandler<AnnouncingScpTerminationEventArgs> AnnouncingScpTermination;

        /// <summary>
        /// Invoked before announcing the NTF entrance.
        /// </summary>
        public static event CustomEventHandler<AnnouncingNtfEntranceEventArgs> AnnouncingNtfEntrance;

        /// <summary>
        /// Invoked after a <see cref="Generator079"/> has been activated.
        /// </summary>
        public static event CustomEventHandler<GeneratorActivatedEventArgs> GeneratorActivated;

        /// <summary>
        /// Invoked before decontaminating the light containment zone.
        /// </summary>
        public static event CustomEventHandler<DecontaminatingEventArgs> Decontaminating;

        /// <summary>
        /// Invoked before damaging a window.
        /// </summary>
        public static event CustomEventHandler<DamagingWindowEventArgs> DamagingWindow;

        /// <summary>
        /// Invoked before a grenade explodes.
        /// </summary>
        public static event CustomEventHandler<ExplodingGrenadeEventArgs> ExplodingGrenade;

        /// <summary>
        /// Invoked before an item is spawned.
        /// </summary>
        public static event CustomEventHandler<SpawningItemEventArgs> SpawningItem;

        /// <summary>
        /// Invoked after an item is spawned.
        /// </summary>
        public static event CustomEventHandler<SpawnedItemEventArgs> SpawnedItem;

        /// <summary>
        /// Called before placing a decal.
        /// </summary>
        /// <param name="ev">The <see cref="PlacingDecalEventArgs"/> instance.</param>
        public static void OnPlacingDecal(PlacingDecalEventArgs ev) => PlacingDecal.InvokeSafely(ev);

        /// <summary>
        /// Called before placing bloods.
        /// </summary>
        /// <param name="ev">The <see cref="PlacingDecalEventArgs"/> instance.</param>
        public static void OnPlacingBlood(PlacingBloodEventArgs ev) => PlacingBlood.InvokeSafely(ev);

        /// <summary>
        /// Called before announcing the light containment zone decontamination.
        /// </summary>
        /// <param name="ev">The <see cref="PlacingDecalEventArgs"/> instance.</param>
        public static void OnAnnouncingDecontamination(AnnouncingDecontaminationEventArgs ev) => AnnouncingDecontamination.InvokeSafely(ev);

        /// <summary>
        /// Called before announcing an SCP termination.
        /// </summary>
        /// <param name="ev">The <see cref="PlacingDecalEventArgs"/> instance.</param>
        public static void OnAnnouncingScpTermination(AnnouncingScpTerminationEventArgs ev) => AnnouncingScpTermination.InvokeSafely(ev);

        /// <summary>
        /// Called before announcing the NTF entrance.
        /// </summary>
        /// <param name="ev">The <see cref="PlacingDecalEventArgs"/> instance.</param>
        public static void OnAnnouncingNtfEntrance(AnnouncingNtfEntranceEventArgs ev) => AnnouncingNtfEntrance.InvokeSafely(ev);

        /// <summary>
        /// Called after a <see cref="Generator079"/> has been activated.
        /// </summary>
        /// <param name="ev">The <see cref="GeneratorActivatedEventArgs"/> instance.</param>
        public static void OnGeneratorActivated(GeneratorActivatedEventArgs ev) => GeneratorActivated.InvokeSafely(ev);

        /// <summary>
        /// Called before decontaminating the light containment zone.
        /// </summary>
        /// <param name="ev">The <see cref="DecontaminatingEventArgs"/> instance.</param>
        public static void OnDecontaminating(DecontaminatingEventArgs ev) => Decontaminating.InvokeSafely(ev);

        /// <summary>
        /// Called before damaging a window.
        /// </summary>
        /// <param name="ev">The <see cref="DamagingWindowEventArgs"/> instance.</param>
        public static void OnDamagingWindow(DamagingWindowEventArgs ev) => DamagingWindow.InvokeSafely(ev);

        /// <summary>
        /// Called before a grenade explodes.
        /// </summary>
        /// <param name="ev">The <see cref="ExplodingGrenadeEventArgs"/> instance.</param>
        public static void OnExplodingGrenade(ExplodingGrenadeEventArgs ev) => ExplodingGrenade.InvokeSafely(ev);

        /// <summary>
        /// Called before an item is spawned.
        /// </summary>
        /// <param name="ev">The <see cref="SpawningItemEventArgs"/> instance.</param>
        public static void OnSpawningItem(SpawningItemEventArgs ev) => SpawningItem.InvokeSafely(ev);

        /// <summary>
        /// Called after an item is spawned.
        /// </summary>
        /// <param name="ev">The <see cref="SpawnedItemEventArgs"/> instance.</param>
        public static void OnSpawnedItem(SpawnedItemEventArgs ev) => SpawnedItem.InvokeSafely(ev);
    }
}
