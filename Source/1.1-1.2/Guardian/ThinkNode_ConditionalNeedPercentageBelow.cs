using System;
using Verse;
using RimWorld;
using Verse.AI;

namespace aRandomKiwi.GFM
{
    public class ThinkNode_ConditionalNeedPercentageBelow : ThinkNode_Conditional
    {
        public override ThinkNode DeepCopy(bool resolve = true)
        {
            ThinkNode_ConditionalNeedPercentageBelow thinkNode_ConditionalNeedPercentageAbove = (ThinkNode_ConditionalNeedPercentageBelow)base.DeepCopy(resolve);
            thinkNode_ConditionalNeedPercentageAbove.need = this.need;
            thinkNode_ConditionalNeedPercentageAbove.threshold = this.threshold;
            return thinkNode_ConditionalNeedPercentageAbove;
        }

        protected override bool Satisfied(Pawn pawn)
        {
            return pawn.needs.TryGetNeed(this.need).CurLevelPercentage < this.threshold;
        }

        private NeedDef need;

        private float threshold;
    }
}
