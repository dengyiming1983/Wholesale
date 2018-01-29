using CsvHelper.Configuration;

namespace SummitDataService
{
    internal class SummitElementOrderMap : CsvClassMap<SummitElementOrder>
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
        -------------------------------------------------------------------
        1. Type			"L"
        2. Ref #		"4"
        3. Invoice #		""
        4. Item #		"40750"
        5. Price		"5350"
        6. Quantity		"10"
        7. Line #		"1"
        */

        public SummitElementOrderMap()
        {
            Map(m => m.Type).Index(0);
            Map(m => m.AccountNumber).Index(1);
            Map(m => m.PONumber).Index(2);
            Map(m => m.TotalLine).Index(3);
            Map(m => m.Status).Index(4);
            Map(m => m.RefNumber).Index(5);
            Map(m => m.InvoiceNumber).Index(6);
            Map(m => m.Salesman).Index(7);
            Map(m => m.Commission).Index(8);
            Map(m => m.Discount).Index(9);
            Map(m => m.OrderTotal).Index(10);
            Map(m => m.ShipVia).Index(11);
            Map(m => m.OrderDate).Index(12);
            Map(m => m.DeliveryDate).Index(13);
            Map(m => m.Message).Index(14);
            Map(m => m.ShiptoAdrs).Index(15);
            Map(m => m.Source).Index(16);
        }
    }
}