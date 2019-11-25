using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace RRYautja.ExtensionMethods
{
    /// <summary>
    /// Utility class for dealing with Pawn body parts.
    /// </summary>
    public static class XenomorphBodypartUtility
    {
        /// <summary>
        /// Gets the first matching bodypart in each bodypart tree node.
        /// </summary>
        /// <param name="pawn">Pawn to look in.</param>
        /// <param name="startingPart">Initial bodypart to start from.</param>
        /// <param name="hediffDef">HediffDef to look for.</param>
        /// <returns>Matching bodyparts.</returns>
        public static IEnumerable<BodyPartRecord> GetFirstMatchingBodyparts(this Pawn pawn, BodyPartRecord startingPart, HediffDef hediffDef)
        {
            //Hediff list from pawn.
            List<Hediff> hediffs = pawn.health.hediffSet.hediffs;

            //Working lists for recursive iteration.
            List<BodyPartRecord> currentSet = new List<BodyPartRecord>();
            List<BodyPartRecord> nextSet = new List<BodyPartRecord>();

            //Seed part.
            nextSet.Add(startingPart);

            do
            {
                //Initialize current set.
                currentSet.AddRange(nextSet);
                nextSet.Clear();

                foreach (BodyPartRecord part in currentSet)
                {
                    bool matchingPart = false;

                    //Iterate through all Hediffs.
                    for (int i = hediffs.Count - 1; i >= 0; i--)
                    {
                        //Current Hediff to inspect.
                        Hediff hediff = hediffs[i];

                        //Matching BodyPartRecord and Hediff
                        if (hediff.Part == part && hediff.def == hediffDef)
                        {
                            //Matching part.
                            matchingPart = true;

                            yield return part;
                        }
                    }

                    //If no matching parts found on this level, continue on next.
                    if (!matchingPart)
                    {
                        //Add next set.
                        for (int j = 0; j < part.parts.Count; j++)
                        {
                            nextSet.Add(part.parts[j]);
                        }
                    }
                }

                //Clear current set.
                currentSet.Clear();
            } while (nextSet.Count > 0); //Do while we got next set of parts to go through.
        }

        /// <summary>
        /// Gets the first matching bodypart in each bodypart tree node.
        /// </summary>
        /// <param name="pawn">Pawn to look in.</param>
        /// <param name="startingPart">Initial bodypart to start from.</param>
        /// <param name="hediffDef">HediffDef to look for.</param>
        /// <param name="hediffExceptionDef">HediffDef to stop at. Returns no bodypart.</param>
        /// <returns>Matching bodyparts.</returns>
        public static IEnumerable<BodyPartRecord> GetFirstMatchingBodyparts(this Pawn pawn, BodyPartRecord startingPart, HediffDef hediffDef, HediffDef hediffExceptionDef)
        {
            //Hediff list from pawn.
            List<Hediff> hediffs = pawn.health.hediffSet.hediffs;

            //Working lists for recursive iteration.
            List<BodyPartRecord> currentSet = new List<BodyPartRecord>();
            List<BodyPartRecord> nextSet = new List<BodyPartRecord>();

            //Seed part.
            nextSet.Add(startingPart);

            do
            {
                //Initialize current set.
                currentSet.AddRange(nextSet);
                nextSet.Clear();

                foreach (BodyPartRecord part in currentSet)
                {
                    bool matchingPart = false;

                    //Iterate through all Hediffs.
                    for (int i = hediffs.Count - 1; i >= 0; i--)
                    {
                        //Current Hediff to inspect.
                        Hediff hediff = hediffs[i];

                        //Matching BodyPartRecord and Hediff
                        if (hediff.Part == part)
                        {
                            if (hediff.def == hediffExceptionDef)
                            {
                                //Matching part. As an exception.
                                matchingPart = true;
                                break;
                            }
                            else if (hediff.def == hediffDef)
                            {
                                //Matching part.
                                matchingPart = true;

                                yield return part;
                                break;
                            }
                        }
                    }

                    //If no matching parts found on this level, continue on next.
                    if (!matchingPart)
                    {
                        //Add next set.
                        for (int j = 0; j < part.parts.Count; j++)
                        {
                            nextSet.Add(part.parts[j]);
                        }
                    }
                }

                //Clear current set.
                currentSet.Clear();
            } while (nextSet.Count > 0); //Do while we got next set of parts to go through.
        }

        /// <summary>
        /// Gets the first matching bodypart in each bodypart tree node.
        /// </summary>
        /// <param name="pawn">Pawn to look in.</param>
        /// <param name="startingPart">Initial bodypart to start from.</param>
        /// <param name="hediffDef">HediffDef to look for.</param>
        /// <param name="hediffExceptionDef">HediffDef to stop at. Returns no bodypart.</param>
        /// <param name="extraExceptionPredicate">Let you do an extra check to make an exception on a bodypart.</param>
        /// <returns>Matching bodyparts.</returns>
        public static IEnumerable<BodyPartRecord> GetFirstMatchingBodyparts(this Pawn pawn, BodyPartRecord startingPart, HediffDef hediffDef, HediffDef hediffExceptionDef, Predicate<Hediff> extraExceptionPredicate)
        {
            //Hediff list from pawn.
            List<Hediff> hediffs = pawn.health.hediffSet.hediffs;

            //Working lists for recursive iteration.
            List<BodyPartRecord> currentSet = new List<BodyPartRecord>();
            List<BodyPartRecord> nextSet = new List<BodyPartRecord>();

            //Seed part.
            nextSet.Add(startingPart);

            do
            {
                //Initialize current set.
                currentSet.AddRange(nextSet);
                nextSet.Clear();

                foreach (BodyPartRecord part in currentSet)
                {
                    bool matchingPart = false;

                    //Iterate through all Hediffs.
                    for (int i = hediffs.Count - 1; i >= 0; i--)
                    {
                        //Current Hediff to inspect.
                        Hediff hediff = hediffs[i];

                        //Matching BodyPartRecord and Hediff
                        if (hediff.Part == part)
                        {
                            if (hediff.def == hediffExceptionDef || extraExceptionPredicate(hediff))
                            {
                                //Matching part. As an exception.
                                matchingPart = true;
                                break;
                            }
                            else if (hediff.def == hediffDef)
                            {
                                //Matching part.
                                matchingPart = true;

                                yield return part;
                                break;
                            }
                        }
                    }

                    //If no matching parts found on this level, continue on next.
                    if (!matchingPart)
                    {
                        //Add next set.
                        for (int j = 0; j < part.parts.Count; j++)
                        {
                            nextSet.Add(part.parts[j]);
                        }
                    }
                }

                //Clear current set.
                currentSet.Clear();
            } while (nextSet.Count > 0); //Do while we got next set of parts to go through.
        }

        /// <summary>
        /// Gets the first matching bodypart in each bodypart tree node.
        /// </summary>
        /// <param name="pawn">Pawn to look in.</param>
        /// <param name="startingPart">Initial bodypart to start from.</param>
        /// <param name="hediffDef">HediffDef to look for.</param>
        /// <param name="hediffExceptionDefs">HediffDefs to stop at. Returns no bodypart.</param>
        /// <returns>Matching bodyparts.</returns>
        public static IEnumerable<BodyPartRecord> GetFirstMatchingBodyparts(this Pawn pawn, BodyPartRecord startingPart, HediffDef hediffDef, HediffDef[] hediffExceptionDefs)
        {
            //Hediff list from pawn.
            List<Hediff> hediffs = pawn.health.hediffSet.hediffs;

            //Working lists for recursive iteration.
            List<BodyPartRecord> currentSet = new List<BodyPartRecord>();
            List<BodyPartRecord> nextSet = new List<BodyPartRecord>();

            //Seed part.
            nextSet.Add(startingPart);

            do
            {
                //Initialize current set.
                currentSet.AddRange(nextSet);
                nextSet.Clear();

                foreach (BodyPartRecord part in currentSet)
                {
                    bool matchingPart = false;

                    //Iterate through all Hediffs.
                    for (int i = hediffs.Count - 1; i >= 0; i--)
                    {
                        //Current Hediff to inspect.
                        Hediff hediff = hediffs[i];

                        //Matching BodyPartRecord and Hediff
                        if (hediff.Part == part)
                        {
                            if (hediffExceptionDefs.Contains(hediff.def))
                            {
                                //Matching part. As an exception.
                                matchingPart = true;
                                break;
                            }
                            else if (hediff.def == hediffDef)
                            {
                                //Matching part.
                                matchingPart = true;

                                yield return part;
                                break;
                            }
                        }
                    }

                    //If no matching parts found on this level, continue on next.
                    if (!matchingPart)
                    {
                        //Add next set.
                        for (int j = 0; j < part.parts.Count; j++)
                        {
                            nextSet.Add(part.parts[j]);
                        }
                    }
                }

                //Clear current set.
                currentSet.Clear();
            } while (nextSet.Count > 0); //Do while we got next set of parts to go through.
        }

        /// <summary>
        /// Gets the first matching bodypart in each bodypart tree node.
        /// </summary>
        /// <param name="pawn">Pawn to look in.</param>
        /// <param name="startingPart">Initial bodypart to start from.</param>
        /// <param name="hediffDefs">HediffDefs to look for.</param>
        /// <returns>Matching bodyparts.</returns>
        public static IEnumerable<BodyPartRecord> GetFirstMatchingBodyparts(this Pawn pawn, BodyPartRecord startingPart, HediffDef[] hediffDefs)
        {
            //Hediff list from pawn.
            List<Hediff> hediffs = pawn.health.hediffSet.hediffs;

            //Working lists for recursive iteration.
            List<BodyPartRecord> currentSet = new List<BodyPartRecord>();
            List<BodyPartRecord> nextSet = new List<BodyPartRecord>();

            //Seed part.
            nextSet.Add(startingPart);

            do
            {
                //Initialize current set.
                currentSet.AddRange(nextSet);
                nextSet.Clear();

                foreach (BodyPartRecord part in currentSet)
                {
                    bool matchingPart = false;

                    //Iterate through all Hediffs.
                    for (int i = hediffs.Count - 1; i >= 0; i--)
                    {
                        //Current Hediff to inspect.
                        Hediff hediff = hediffs[i];

                        //Matching BodyPartRecord and Hediff
                        if (hediff.Part == part && hediffDefs.Contains(hediff.def))
                        {
                            //Matching part.
                            matchingPart = true;

                            yield return part;
                            break;
                        }
                    }

                    //If no matching parts found on this level, continue on next.
                    if (!matchingPart)
                    {
                        //Add next set.
                        for (int j = 0; j < part.parts.Count; j++)
                        {
                            nextSet.Add(part.parts[j]);
                        }
                    }
                }

                //Clear current set.
                currentSet.Clear();
            } while (nextSet.Count > 0); //Do while we got next set of parts to go through.
        }

        /// <summary>
        /// Replaces all Hediffs with another one at the same position starting from a bodypart.
        /// </summary>
        /// <param name="pawn">Pawn to look in.</param>
        /// <param name="startingPart">Initial bodypart to start from.</param>
        /// <param name="hediffDef">HediffDefs to look for.</param>
        /// <param name="replaceWithDef">HediffDef to replace with.</param>
        public static void ReplaceHediffFromBodypart(this Pawn pawn, BodyPartRecord startingPart, HediffDef hediffDef, HediffDef replaceWithDef)
        {
            //Hediff list from pawn.
            List<Hediff> hediffs = pawn.health.hediffSet.hediffs;

            //Working lists for recursive iteration.
            List<BodyPartRecord> currentSet = new List<BodyPartRecord>();
            List<BodyPartRecord> nextSet = new List<BodyPartRecord>();

            //Seed part.
            nextSet.Add(startingPart);

            do
            {
                //Initialize current set.
                currentSet.AddRange(nextSet);
                nextSet.Clear();

                foreach (BodyPartRecord part in currentSet)
                {
                    //Iterate through all Hediffs.
                    for (int i = hediffs.Count - 1; i >= 0; i--)
                    {
                        //Current Hediff to inspect.
                        Hediff hediff = hediffs[i];

                        //Matching BodyPartRecord and Hediff
                        if (hediff.Part == part && hediff.def == hediffDef)
                        {
                            //Remove Hediff.
                            Hediff hediff2 = hediffs[i];
                            hediffs.RemoveAt(i);
                            hediff2.PostRemoved();

                            //Add replacing Hediff.
                            Hediff newHeDiff = HediffMaker.MakeHediff(replaceWithDef, pawn, part);
                            hediffs.Insert(i, newHeDiff);
                        }
                    }

                    //Add next set.
                    for (int j = 0; j < part.parts.Count; j++)
                    {
                        nextSet.Add(part.parts[j]);
                    }
                }

                //Clear current set.
                currentSet.Clear();
            } while (nextSet.Count > 0); //Do while we got next set of parts to go through.
        }
    }
}
