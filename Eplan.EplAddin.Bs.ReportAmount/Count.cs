using Eplan.EplApi.ApplicationFramework;
using Eplan.EplApi.DataModel;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;

namespace Eplan.EplAddin.Bs.ReportAmount
{
    public class Count : IEplAction
    {
        private static string LastObjNames { get;  set; }
        private static decimal LastCount { get;  set; }
        private decimal totalCount;
        public bool Execute(ActionCallingContext oActionCallingContext)
        {

            decimal count = Decimal.Zero;
            string objNames = null;
            oActionCallingContext.GetParameter("objects", ref objNames);
            if (objNames != null)
            {

                try
                {
                    count = GetCount_PACKAGINGQUANTITY(objNames);
                }
                catch (Exception)
                {
                    return false;
                }
                oActionCallingContext.SetStrings(new string[] { count.ToString("0.##", System.Globalization.CultureInfo.CurrentCulture) });
                return true;
            }
            else
            {
                return false;
            }
        }

        internal static decimal GetCount_PACKAGINGQUANTITY(string objNames)
        {
            if (LastObjNames == objNames)
                return LastCount;
            decimal count = decimal.Zero;
            decimal article_PRICEUNIT = decimal.Zero;
            decimal article_PACKAGINGQUANTITY = decimal.Zero;
            decimal total_PURCHASEPRICE_1 = decimal.Zero;
            string partNumber = "";
            string[] sObjs = objNames.Split(';');
            foreach (var sObj in sObjs)
            {
                StorableObject obj = StorableObject.FromStringIdentifier(sObj);
                Eplan.EplApi.DataModel.MergedArticleReference partRef = (Eplan.EplApi.DataModel.MergedArticleReference)obj;
                ArticleReference[] allArticleReferences = partRef.GetArticleReferences();
                Article article = partRef.GetMainArticleReference().Article;
                article_PACKAGINGQUANTITY = Decimal.Parse(article.Properties.ARTICLE_PACKAGINGQUANTITY.ToString(), CultureInfo.InvariantCulture);
                article_PRICEUNIT = article.Properties.ARTICLE_PRICEUNIT.ToInt();
                int productGroup = article.Properties.ARTICLE_PRODUCTGROUP;
                int productSubgroup = article.Properties.ARTICLE_PRODUCTSUBGROUP;
                total_PURCHASEPRICE_1 = Decimal.Parse(article.Properties.ARTICLE_PACKAGINGPRICE_1.ToString(), CultureInfo.InvariantCulture);
                partNumber = partRef.Properties.ARTICLEREF_PARTNO;
                if (productGroup == 29 & productSubgroup == 1)
                {
                    // Выводим длинну кабеля
                    foreach (var articleReference in allArticleReferences)
                    {
#if DEBUG
                        Debug.Print("{0}({1}) articleReference.Properties.ARTICLE_PARTIAL_LENGTH_VALUE: {2}",
                            partNumber,
                            productGroup,
                            articleReference.Properties.ARTICLE_PARTIAL_LENGTH_VALUE.ToString()
                            );
#endif
                        count += Decimal.Parse(articleReference.Properties.ARTICLE_PARTIAL_LENGTH_VALUE.ToString(), CultureInfo.InvariantCulture);
                    }
                }
                else
                {
                    // выводим общее количество
                    foreach (var articleReference in allArticleReferences)
                    {
#if DEBUG
                        Debug.Print("{0}({1}) articleReference.Properties.ARTICLEREF_COUNT: {2}",
                            partNumber,
                            productGroup,
                            articleReference.Properties.ARTICLEREF_COUNT.ToInt()
                            );
#endif
                        count += articleReference.Properties.ARTICLEREF_COUNT.ToInt();
                    }
                }
#if DEBUG
                Debug.Print("{0} count: {1}",
                    partNumber,
                    count.ToString("0.00")
                    );
#endif
            }
            count = Decimal.Ceiling(count * article_PRICEUNIT / article_PACKAGINGQUANTITY);
            LastCount=count;
            return count;
        }


        // Obsolete - игнорируем.
        public void GetActionProperties(ref ActionProperties actionProperties)
        {
            return;
        }

        public bool OnRegister(ref string Name, ref int Ordinal)
        {
            Name = "Count_PACKAGINGQUANTITY";
            Ordinal = 20;
            return true;
        }
    }
}
