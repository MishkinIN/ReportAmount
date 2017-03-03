using Eplan.EplApi.ApplicationFramework;
using Eplan.EplApi.DataModel;
using System;

namespace Eplan.EplAddin.Bs.ReportAmount
{
    public class CostAmount : IEplAction
    {
        string objNames = null;
        private decimal totalCount;
        public bool Execute(ActionCallingContext oActionCallingContext)
        {

            decimal totalCost = Decimal.Zero;
            string strObjects = null;
            oActionCallingContext.GetParameter("objects", ref strObjects);
            if (strObjects != objNames)
            {
                objNames = strObjects;
                if (objNames != null)
                {
                    try
                    {
                        totalCost = Cost.GetCost(objNames);
                    }
                    catch (Exception)
                    {
                        return false;
                    }
                    oActionCallingContext.SetStrings(new string[] { totalCost.ToString("0.00", System.Globalization.CultureInfo.CurrentCulture) });
                }
                else
                {
                    return false;
                }
            }
            return true;
        }

        private static decimal GetCost(StorableObject obj)
        {
            return 1;
        }

        // Obsolete - игнорируем.
        public void GetActionProperties(ref ActionProperties actionProperties)
        {
            return;
        }

        public bool OnRegister(ref string Name, ref int Ordinal)
        {
            Name = "CostAmount";
            Ordinal = 20;
            return true;
        }
    }
}
