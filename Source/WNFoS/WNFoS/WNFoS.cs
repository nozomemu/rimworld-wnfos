using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using Verse;
using TaranMagicFramework;

namespace WNFoS
{
    [StaticConstructorOnStartup]
    public class WNFoS : Mod
    {
        public WNFoS(ModContentPack content) : base(content)
        {
            Harmony harmony = new Harmony("NozoMeMu.WorldOfNaruto.FleeOnSight");
            harmony.PatchAll(Assembly.GetExecutingAssembly());
        }
    }

    [DefOf]
    public static class WNFoS_DefOf
    {
        public static WNFoS_OrderDef WNFoS_Order_Custom;
        public static WNFoS_OrderDef WNFoS_Order_FleeOnSight;

        public static WNFoS_OrderReasonDef WNFoS_OrderReason_DeadlyJutsu;
        public static WNFoS_OrderReasonDef WNFoS_OrderReason_DeadlyNinjaI;
        public static WNFoS_OrderReasonDef WNFoS_OrderReason_DeadlyNinjaII;
        public static WNFoS_OrderReasonDef WNFoS_OrderReason_DeadlyNinjaIII;
        public static WNFoS_OrderReasonDef WNFoS_OrderReason_DeadlyNinjaIV;
        public static WNFoS_OrderReasonDef WNFoS_OrderReason_DeadlyNinjaV;

        // Core defs
        public static RecordDef KillsHumanlikes;
        public static RecordDef PawnsDowned;
        public static RecordDef PeopleCaptured;
    }
}
