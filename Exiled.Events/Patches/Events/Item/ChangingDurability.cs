// -----------------------------------------------------------------------
// <copyright file="ChangingDurability.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Item
{
#pragma warning disable SA1118

    using System.Collections.Generic;
    using System.Reflection.Emit;

    using Exiled.Events.EventArgs;

    using HarmonyLib;

    using Mirror;

    using NorthwoodLib.Pools;

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Patches <see cref="Inventory.SyncListItemInfo.ModifyDuration(int, float)"/>.
    /// Adds the <see cref="Handlers.Item.ChangingDurability"/> event.
    /// </summary>
    [HarmonyPatch(typeof(Inventory.SyncListItemInfo), nameof(Inventory.SyncListItemInfo.ModifyDuration))]
    internal static class ChangingDurability
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            var newInstructions = ListPool<CodeInstruction>.Shared.Rent(instructions);

            // The index offset.
            const int offset = 0;

            // Search for the last "ldloca.s".
            var index = newInstructions.FindLastIndex(instruction => instruction.opcode == OpCodes.Ldloca_S) + offset;

            // Set the return label to the last instruction.
            var returnLabel = generator.DefineLabel();

            // Declare ChangingDurabilityEventArgs, to be able to store its instance with "stloc.1".
            var ev = generator.DeclareLocal(typeof(ChangingDurabilityEventArgs));

            // var ev = new ChangingDurabilityEventArgs(item, newDurability, true);
            //
            // Handlers.Item.OnChangingDurability(ev);
            //
            // if (!ev.IsAllowed)
            //   return;
            //
            // base[index] = ev.Item;
            //
            // return;
            newInstructions.InsertRange(index, new[]
            {
                 new CodeInstruction(OpCodes.Ldloc_S, 0),
                 new CodeInstruction(OpCodes.Ldarg_2),
                 new CodeInstruction(OpCodes.Ldc_I4_1),
                 new CodeInstruction(OpCodes.Newobj, GetDeclaredConstructors(typeof(ChangingDurabilityEventArgs))[0]),
                 new CodeInstruction(OpCodes.Dup),
                 new CodeInstruction(OpCodes.Dup),
                 new CodeInstruction(OpCodes.Stloc, ev.LocalIndex),
                 new CodeInstruction(OpCodes.Call, Method(typeof(Handlers.Item), nameof(Handlers.Item.OnChangingDurability))),
                 new CodeInstruction(OpCodes.Callvirt, PropertyGetter(typeof(ChangingDurabilityEventArgs), nameof(ChangingDurabilityEventArgs.IsAllowed))),
                 new CodeInstruction(OpCodes.Brfalse_S, returnLabel),
                 new CodeInstruction(OpCodes.Ldarg_0),
                 new CodeInstruction(OpCodes.Ldarg_1),
                 new CodeInstruction(OpCodes.Ldloc, ev.LocalIndex),
                 new CodeInstruction(OpCodes.Callvirt, PropertyGetter(typeof(ChangingDurabilityEventArgs), nameof(ChangingDurabilityEventArgs.NewItem))),
                 new CodeInstruction(OpCodes.Call, Method(typeof(SyncList<Inventory.SyncItemInfo>), "set_Item")),
                 new CodeInstruction(OpCodes.Ret).WithLabels(returnLabel),
            });

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Shared.Return(newInstructions);
        }
    }
}
