using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataObjects;

namespace LogicLayer
{
    public class DieRoller
    {
        private static Random _rand = new Random();
        private static int _minDieRoll = 1;
        public static int RollDie(Dice ds)
        {
            return (_rand.Next(_minDieRoll, (int)ds));
        }
    }
}
