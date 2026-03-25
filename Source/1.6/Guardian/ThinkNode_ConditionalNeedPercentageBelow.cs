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
            if (pawn.needs == null) return false;
            Need need = pawn.needs.TryGetNeed(this.need);
            return need != null && need.CurLevelPercentage < this.threshold;
        }

        private NeedDef need;

        private float threshold;
    }
}
