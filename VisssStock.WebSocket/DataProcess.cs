using NCalc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VissStockApp
{
    internal class DataProcess
    {
        public static bool CheckFormula(string expression)
        {
            try
            {
                var exp = new Expression(expression);
                var result = exp.Evaluate();
                return Convert.ToBoolean(result);
            }
            catch (Exception ex)
            {
                throw new Exception("Error when check formula", ex);
            }
        }
    }
}
