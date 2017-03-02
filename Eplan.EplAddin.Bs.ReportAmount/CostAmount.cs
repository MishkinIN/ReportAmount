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
            
            //int parameterCount = oActionCallingContext.GetParameterCount();
            string firstParameter = null;
            oActionCallingContext.GetParameter("0", ref firstParameter);
            //string[] paramNames = oActionCallingContext.GetParameters();

            if (!String.IsNullOrWhiteSpace(firstParameter))
            {
                switch (firstParameter.ToLower())
                {
                    case "init":
                        totalCount = 0;
                        return true;
                    case "total":
                        oActionCallingContext.SetStrings(new string[] {"total: "+ totalCount.ToString("0,0.00",System.Globalization.CultureInfo.CurrentCulture)});
                        return true;
                    case "cost":
                        break;
                    case "section":
                    default:
                        oActionCallingContext.SetStrings(new string[] { "Допустимые значения первого параметров 'init', 'cost', 'total'" });//'section имяСекции', 
                        return true;
                }
            }
            // Вызов без параметров - из области данных.

            string strObjects =null;
            oActionCallingContext.GetParameter("objects", ref strObjects);
            if (strObjects!= objNames)
            {
                objNames = strObjects;
                string[] objIDs = objNames.Split(';');
                foreach (string objId in objIDs)
                {
                    try
                    {
                        StorableObject obj = StorableObject.FromStringIdentifier(objId);
                        totalCount += GetCost(obj);
                        oActionCallingContext.SetStrings(new string[] { "row " + objNames + ":" + totalCount.ToString("0,0.00", System.Globalization.CultureInfo.CurrentCulture) });//'section имяСекции', 
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
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
