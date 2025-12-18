using System;
using System.Collections.Generic;
using System.Text;

namespace MadWin.Infrastructure.Convertors
{
    public static class NumberHelper
    {
        public static int RoundLastThreeDigitsToZero(decimal number)
        {
            return (int)(Math.Floor(number / 1000) * 1000);
        }
    }

}
