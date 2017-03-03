using Eplan.EplApi.ApplicationFramework;
using Eplan.EplApi.Base;
using Eplan.EplApi.DataModel;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;

namespace Eplan.EplAddin.Bs.ReportAmount
{
    public class Cost:IEplAction
    {
        private static string LastObjNames { get; set; }
        private static decimal LastCost { get; set; }

        public bool Execute(ActionCallingContext oActionCallingContext)
        {
            decimal totalCost = Decimal.Zero;
            string objNames = null;
            oActionCallingContext.GetParameter("objects", ref objNames);
            if (objNames != null)
            {

                try
                {
                    totalCost = GetCost(objNames);
                }
                catch (Exception)
                {

                    return false;
                }
                oActionCallingContext.SetStrings(new string[] { totalCost.ToString("0.00", System.Globalization.CultureInfo.CurrentCulture) });
                return true;
            }
            else
            {
                return false;
            }
        }
        internal static decimal GetCost(string objNames)
        {
            if (LastObjNames == objNames)
                return LastCost;
            decimal cost = decimal.Zero;
            decimal count = decimal.Zero;
            decimal total_PURCHASEPRICE_1 = decimal.Zero;
            string partNumber = "";
            string[] sObjs = objNames.Split(';');
            StorableObject obj = StorableObject.FromStringIdentifier(sObjs[sObjs.Length-1]);
            Eplan.EplApi.DataModel.MergedArticleReference partRef = (Eplan.EplApi.DataModel.MergedArticleReference)obj;
            Article article = partRef.GetMainArticleReference().Article;
            total_PURCHASEPRICE_1 = Decimal.Parse(article.Properties.ARTICLE_PACKAGINGPRICE_1.ToString(), CultureInfo.InvariantCulture);
            count = Count.GetCount_PACKAGINGQUANTITY(objNames);
            
#if DEBUG
            Debug.Print("{0} cost: {1}*{2}",
                partNumber,
                count.ToString("0.00"),
                total_PURCHASEPRICE_1.ToString("0.00")
                );
#endif
            cost = count * total_PURCHASEPRICE_1;
            LastCost = cost;
            return cost;
        }
        public void GetActionProperties(ref ActionProperties actionProperties)
        {
            return;
        }

        public bool OnRegister(ref string Name, ref int Ordinal)
        {
            Name = "Cost";
            Ordinal = 20;
            return true;
        }
    }
}
