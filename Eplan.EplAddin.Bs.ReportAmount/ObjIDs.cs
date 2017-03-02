using Eplan.EplApi.ApplicationFramework;
using Eplan.EplApi.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Eplan.EplAddin.Bs.ReportAmount
{
    public class ObjIDs : IEplAction
    {
        public bool Execute(ActionCallingContext oActionCallingContext)
        {
            string strObjects =null;
            oActionCallingContext.GetParameter("objects", ref strObjects);
                oActionCallingContext.SetStrings(new string[] { strObjects });
            return true;
        }

        public void GetActionProperties(ref ActionProperties actionProperties)
        {
            return;
        }

        public bool OnRegister(ref string Name, ref int Ordinal)
        {
            Name = "ObjIDs";
            Ordinal = 20;
            return true;
        }
    }
}
