﻿using System;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using RimWorld;
using RocketMan;
using Verse;
using Verse.AI;

namespace Soyuz
{
    public static partial class SoyuzSettingsUtility
    {
        private static JobDef[] fullyThrottledJobs = null;

        private static JobDef[] partiallyThrottledJobs = null;

        private static JobDef[] notThrottledJobs = null;

        private static bool initialized = false;

        private static Assembly vanilla;

        private static void PreparePresets()
        {
            vanilla = typeof(Find).Assembly;

            if (fullyThrottledJobs == null)
            {
                fullyThrottledJobs = new JobDef[]
                {
                    JobDefOf.HaulToCell,
                    JobDefOf.Goto,
                    JobDefOf.LayDown,
                    JobDefOf.GotoWander,
                    JobDefOf.Wait_Wander,
                };
            }
            if (partiallyThrottledJobs == null)
            {
                partiallyThrottledJobs = new JobDef[]
                {
                    JobDefOf.Wait,
                    JobDefOf.Sow,
                    JobDefOf.Clean,
                    JobDefOf.CutPlant,
                    JobDefOf.CutPlantDesignated,
                    JobDefOf.Harvest,
                    JobDefOf.HarvestDesignated,
                    JobDefOf.Follow,
                    JobDefOf.FollowClose,
                };
            }
            if (notThrottledJobs == null)
            {
                notThrottledJobs = new JobDef[]
                {
                    JobDefOf.DeliverFood,
                    JobDefOf.FeedPatient,
                    JobDefOf.Mine,
                    JobDefOf.SocialRelax,
                    JobDefOf.FinishFrame,
                    JobDefOf.Refuel,
                    JobDefOf.DoBill,
                    JobDefOf.Ingest,
                    JobDefOf.TakeInventory,
                    JobDefOf.Capture,
                    JobDefOf.Repair,
                    JobDefOf.AttackMelee,
                    JobDefOf.AttackStatic,
                    JobDefOf.CastAbilityOnThing,
                    JobDefOf.CastAbilityOnWorldTile,
                    JobDefOf.CastJump,
                    JobDefOf.BeatFire,
                    JobDefOf.Rescue,
                    JobDefOf.Mate,
                };
            }
        }

        [Main.OnWorldLoaded]
        public static void SetRecommendedJobConfig()
        {
            if (!initialized)
            {
                PreparePresets();
                initialized = true;
            }
            Assembly vanilla = typeof(Find).Assembly;

            foreach (JobDef def in fullyThrottledJobs)
            {
                if (def != null && Context.JobDilationByDef.TryGetValue(def, out JobSettings settings))
                {
                    //if (!IsModifiedJob(def))
                    //{
                    settings.throttleFilter = JobThrottleFilter.All;
                    settings.throttleMode = JobThrottleMode.Full;
                    //}
                    //else
                    //{
                    //    settings.throttleFilter = JobThrottleFilter.Animals;
                    //    settings.throttleMode = JobThrottleMode.Full;

                    //    Log.Message($"SOYUZ: Blacklisted job {settings.def.defName}");
                    //}
                }
                else
                {
                    Log.Warning($"SOYUZ: Job {def?.defName}:{def?.label} settings not found while setting presets!");
                }
            }

            foreach (JobDef def in partiallyThrottledJobs)
            {
                if (def != null && Context.JobDilationByDef.TryGetValue(def, out JobSettings settings))
                {
                    //if (!IsModifiedJob(def))
                    //{
                    settings.throttleFilter = JobThrottleFilter.All;
                    settings.throttleMode = JobThrottleMode.Partial;
                    //}
                }
            }
            foreach (JobDef def in notThrottledJobs)
            {
                if (def != null && Context.JobDilationByDef.TryGetValue(def, out JobSettings settings))
                {
                    settings.throttleFilter = JobThrottleFilter.All;
                    settings.throttleMode = JobThrottleMode.None;
                }
                else
                {
                    Log.Warning($"SOYUZ: Job {def?.defName}:{def?.label} settings not found while setting presets!");
                }
            }
            foreach (JobSettings settings in DefDatabase<JobDef>.AllDefs
                .Select(d => Context.JobDilationByDef.TryGetValue(d, fallback: null))
                .Where(p => p != null && !partiallyThrottledJobs.Contains(p.def) && !fullyThrottledJobs.Contains(p.def) && !notThrottledJobs.Contains(p.def)))
            {
                settings.throttleFilter = JobThrottleFilter.Animals;
                settings.throttleMode = JobThrottleMode.Full;
            }
            Log.Message("SOYUZ: Preset loaded!");
        }

        private static string harmonyId = Finder.HarmonyID + ".Soyuz";

        // TODO: redo this
        // private static bool IsModifiedJob(JobDef def)
        // {
        //    if (vanilla == def.driverClass.GetType().Assembly)
        //    {
        //        foreach (MethodBase method in def.driverClass.GetMethods())
        //        {
        //            if (!method.IsValidTarget())
        //                continue;
        //            var info = Harmony.GetPatchInfo(method);
        //            if (info.Postfixes.Any(p => p.owner != harmonyId))
        //                return true;
        //            if (info.Prefixes.Any(p => p.owner != harmonyId))
        //                return true;
        //            if (info.Transpilers.Any(p => p.owner != harmonyId))
        //                return true;
        //            if (info.Finalizers.Any(p => p.owner != harmonyId))
        //                return true;
        //        }
        //    }
        //    return false;
        // }
    }
}
