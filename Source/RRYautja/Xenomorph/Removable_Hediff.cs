using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace AvP
{
    /// <summary>
    /// Temporary Hediff for replacing Hediffs like MissingPartHeDiff.
    /// </summary>
    public class Removable_Hediff : Hediff
    {
        public override bool ShouldRemove => true;
    }
}
