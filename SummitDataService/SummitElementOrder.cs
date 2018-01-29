using System;

namespace SummitDataService
{
    public class SummitElementOrder
    {
        /*
           1. Type			"H"
           2. Account #		"01168"	
           3. PO #			""
           4. Total line		"20"
           5. Status		"0"  (0-New;1-Packed;2-Invoiced;9-Canceled)
           6. Ref #		"4"
           7. Invoice #		""
           8. Salesman		"Whiting Wu"
           9. Commission		"0"
           10. Discount		"0"
           11. Order Total		"170850"
           12. Ship Via		""
           13. Order Date		"2017-11-06"
           14. Delivery Date	"2017-11-08"
           15. Message		""
           16. Ship to Adrs	"1339 CENTERNIAL AVE PISCATAWAY NJ 08854"
           17. Source		"2"  (2-From MoleQ)
       */

        public string Type { get; set; }
        public string AccountNumber { get; set; }
        public string PONumber { get; set; }
        public string TotalLine { get; set; }
        public string Status { get; set; }

        private string _RefNumber;
        public string RefNumber
        {
            get { return _RefNumber; }

            set { _RefNumber = value.Trim(); }
        }

        private string _InvoiceNumber;
        public string InvoiceNumber {
            get { return _InvoiceNumber; }

            set { _InvoiceNumber = value.Trim(); }
        }
        public string Salesman { get; set; }
        public string Commission { get; set; }
        public string Discount { get; set; }

        private string _OrderTotal;
        public string OrderTotal {
            get { return _OrderTotal; }

            set
            {
                string v = value.Replace(" ", "");
                double res = 0;
                if (double.TryParse(v, out res))
                {
                    res = res / 100;
                }
                _OrderTotal = Convert.ToString(res);
            }
        }


        public string ShipVia { get; set; }
        public string OrderDate { get; set; }
        public string DeliveryDate { get; set; }
        public string Message { get; set; }
        public string ShiptoAdrs { get; set; }
        public string Source { get; set; }
    }
}