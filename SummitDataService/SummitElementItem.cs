using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SummitDataService
{
    public class SummitElementItem
    {

        // Item #
        private string _ItemNumber;
        public string ItemNumber
        {

            get { return _ItemNumber; }

            set { _ItemNumber = value.Replace(" ", ""); }

        }
        //public string ItemNumber { get; set; }

        // Description
        //public string Description { get; set; }
        private string _Description;
        public string Description
        {

            get { return _Description; }

            set { _Description = value.Trim(); }

        }

        // Brand
        public string Brand { get; set; }

        // Pack
        public string Pack { get; set; }

        // Qty on hand
        //public string QuantityOnHand { get; set; }
        private string _QuantityOnHand;
        public string QuantityOnHand
        {

            get { return _QuantityOnHand; }

            set
            {
                string v = value.Replace(" ", "");
  
                double res = 0;
                if (double.TryParse(v, out res))
                {
                    //res = res / 100;
                    if (res < 0)
                        res = 0;
                }
                _QuantityOnHand = Convert.ToString(res);
            }

        }

        // Gross weight
        public string GrossWeight { get; set; }

        // Price A
        //public string PriceA { get; set; }
        private string _PriceA;
        public string PriceA
        {

            get { return _PriceA; }

            set {
                string v = value.Replace(" ", "");
                double res = 0;
                if (double.TryParse(v,out res))
                {
                    res = res / 100;
                }
                _PriceA = Convert.ToString(res) ;
            }

        }

        // Price B
        //public string PriceB { get; set; }
        private string _PriceB;
        public string PriceB
        {

            get { return _PriceB; }

            set
            {
                string v = value.Replace(" ", "");
                double res = 0;
                if (double.TryParse(v, out res))
                {
                    res = res / 100;
                }
                _PriceB = Convert.ToString(res);
            }

        }


        // Price C
        // public string PriceC { get; set; }
        private string _PriceC;
        public string PriceC
        {

            get { return _PriceC; }

            set
            {
                string v = value.Replace(" ", "");
                double res = 0;
                if (double.TryParse(v, out res))
                {
                    res = res / 100;
                }
                _PriceC = Convert.ToString(res);
            }

        }

        // FOB
        //public string FOB { get; set; }
        private string _FOB { get; set; }
        public string FOB
        {

            get { return _FOB; }

            set
            {
                string v = value.Replace(" ", "");
                double res = 0;
                if (double.TryParse(v, out res))
                {
                    res = res / 100;
                }
                _FOB = Convert.ToString(res);
            }

        }


        // Cost
        //public string Cost { get; set; }
        private string _Cost;
        public string Cost
        {

            get { return _Cost; }

            set
            {
                string v = value.Replace(" ", "");
                double res = 0;
                if (double.TryParse(v, out res))
                {
                    res = res / 100;
                }
                _Cost = Convert.ToString(res);
            }

        }

        // Vendor #
        public string VendorNumber { get; set; }

        // Memo
        //public string Memo { get; set; }
        private string _Memo { get; set; }
        public string Memo
        {

            get { return _Memo; }

            set { _Memo = value.Trim(); }

        }

        // Chinese Description
        //public string ChineseDescription { get; set; }
        private string _ChineseDescription;

        public string ChineseDescription
        {

            get { return _ChineseDescription; }

            // build 0006
            //set { _ChineseDescription = value.Trim(); }

            // build 0007
            set
            {
                string v = value.Trim();

                byte[] src = Encoding.Default.GetBytes(v);

                //_ChineseDescription = _encoding.GetString(src);
                _ChineseDescription =  Encoding.GetEncoding("gb2312").GetString(src);

            }

        }


        private string _UPCCode;
        // UPC code
        public string UPCCode
        {

            get { return _UPCCode; }

            set { _UPCCode = value.Replace(" ", ""); }

        }
        //public string UPCCode { get; set; }


        // Related Item Code
        //public string RelatedItemCode { get; set; }

        private string _RelatedItemCode;
        public string RelatedItemCode
        {

            get { return _RelatedItemCode; }

            set { _RelatedItemCode =  value.Replace(" ", ""); }

        }

        // Record type
        public string RecordType { get; set; }


        //private Encoding _encoding = Encoding.Default;
        //internal void SetEncoding(Encoding fileEncoding)
        //{
        //    _encoding = fileEncoding;
        //}

      //  public string BrandName { get; set; }
        private string _BrandName = "";
        public string BrandName
        {

            get { return _BrandName; }

            set { _BrandName = value.Trim(); }

        }
    }
}
