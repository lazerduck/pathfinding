using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pathfinder
{
    class AiBotSimple : AiBotBase
    {
        public override void Setup(Level lvl, Player plr){}
        public AiBotSimple(int x, int y)
            : base(x, y)
        {

        }
        protected override void ChooseNextGridLocation(Level level, Player plr)
        {
            Coord2 playerPos = plr.GridPosition;
            Coord2 pos = GridPosition;
            bool ok = true;
            if (playerPos.X > pos.X)
            {
                pos.X++;
                ok = SetNextGridPosition(pos, level);
                if (ok != false)
                    return;
                pos.X--;
            }
            if (playerPos.X < pos.X)
            {
                pos.X--;
                ok = SetNextGridPosition(pos, level);
                if (ok != false)
                    return;
                pos.X++;
            }
            if (playerPos.Y > pos.Y)
            {
                pos.Y++;
                ok = SetNextGridPosition(pos, level);
                if (ok != false)
                    return;
                pos.Y--;
            }
            if (playerPos.Y < pos.Y)
            {
                pos.Y--;
                ok = SetNextGridPosition(pos, level);
                if (ok != false)
                    return;
                pos.Y++;
            }

        }
    }
}
