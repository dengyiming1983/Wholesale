using System;

namespace SummitDataService
{
    public class SummitElementOrderLineItem
    {
        /*
            1. Type			"L"
            2. Ref #		"4"
            3. Invoice #		""
            4. Item #		"40750"
            5. Price		"5350"
            6. Quantity		"10"
            7. Line #		"1"
        */

        public string Type { get; set; }

        private string _RefNumber;
        public string RefNumber
        {
            get { return _RefNumber; }

            set { _RefNumber = value.Trim(); }
        }

        private string _InvoiceNumber;
        public string InvoiceNumber
        {
            get { return _InvoiceNumber; }

            set { _InvoiceNumber = value.Trim(); }
        }

        public string ItemNumber { get; set; }
        //public string Price { get; set; }


        private string _Price;
        public string Price
        {

            get { return _Price; }

            set
            {
                string v = value.Replace(" ", "");
                double res = 0;
                if (double.TryParse(v, out res))
                {
                    res = res / 100;
                }
                _Price = Convert.ToString(res);
            }

        }


        public string Quantity { get; set; }
        public string LineNumber { get; set; }
    }
}