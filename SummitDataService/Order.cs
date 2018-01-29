namespace SummitDataService
{
    public class Order
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
        public string RefNumber { get; set; }
        public string InvoiceNumber { get; set; }
        public string Salesman { get; set; }
        public string Commission { get; set; }
        public string Discount { get; set; }

        public string OrderTotal { get; set; }

        public string ShipVia { get; set; }
        public string OrderDate { get; set; }

        public string DeliveryDate { get; set; }
        public string Message { get; set; }

        public string ShiptoAdrs { get; set; }

        public string Source { get; set; }

    }
}