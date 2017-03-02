using Eplan.EplApi.ApplicationFramework;
using Eplan.EplApi.DataModel;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace Eplan.EplAddin.Bs.ReportAmount
{
    public class Cost:IEplAction
    {
        string objNames = null;
        public bool Execute(ActionCallingContext oActionCallingContext)
        {
            decimal totalCount=Decimal.Zero;
            string strObjects = null;
            oActionCallingContext.GetParameter("objects", ref strObjects);
            if (strObjects != objNames)
            {
                objNames = strObjects;
                string[] objIDs = objNames.Split(';');
                if (objIDs.Length>0)
                {
                    //foreach (string objId in objIDs)
                    //{
                        try
                        {
                            StorableObject obj = StorableObject.FromStringIdentifier(objIDs[0]);
                            totalCount += GetCost(obj);
                        }
                        catch (Exception ex)
                        {
                            ;
                        }
                    //}
                    oActionCallingContext.SetStrings(new string[] { totalCount.ToString(System.Globalization.CultureInfo.CurrentCulture) }); 
                }
                else
                {
                    ;
                }
            }
            return true;
        }
        internal static decimal GetCount(Project project, MergedArticleReference partRef)
        {
            MergedArticleReferencePropertyList properties = partRef.Properties;
            string partNumbetr = properties.ARTICLEREF_PARTNO;
            Article article = project.Articles.FirstOrDefault(a => a.PartNr == partNumbetr);
            decimal article_PACKAGINGQUANTITY = article.Properties.ARTICLE_PACKAGINGQUANTITY;
            decimal article_PRICEUNIT = article.Properties.ARTICLE_PRICEUNIT;

            if (properties.ARTICLEREF_CABLE_LENGTH_SUM > 0)
            {
                // Calculate as Cable
                return Decimal.Ceiling( properties.ARTICLEREF_CABLE_LENGTH_SUM * article_PRICEUNIT / article_PACKAGINGQUANTITY);
            }
            else
            {
                // Calculate as item
                return Decimal.Ceiling(properties.ARTICLEREF_COUNT_TOTAL * article_PRICEUNIT / article_PACKAGINGQUANTITY);
            }
        }
        internal static decimal GetCost(StorableObject obj)
        {
            Eplan.EplApi.DataModel.MergedArticleReference partRef = (Eplan.EplApi.DataModel.MergedArticleReference)obj;
            MergedArticleReferencePropertyList properties = partRef.Properties;
            decimal count = 0;
            try
            {
                count = GetCount(obj.Project, partRef);
            }
            catch (Exception)
            {
                count = 5000000;
            }
            decimal price = (decimal)properties.ARTICLEREF_TOTALPURCHASEPRICE_1.ToDouble();
            decimal result= count * price;
            return result;
        }

        // Obsolete - игнорируем.
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
