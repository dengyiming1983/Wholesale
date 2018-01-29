using CsvHelper.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SummitDataService
{
     public sealed  class SummitElementMap :CsvClassMap<SummitElement>
    {
        public SummitElementMap()
        {
            Map(m => m.ItemNumber).Index(0);
            Map(m => m.Description).Index(1);
            Map(m => m.Brand).Index(2);
            Map(m => m.Pack).Index(3);
            Map(m => m.QuantityOnHand).Index(4);
            Map(m => m.ProfitCurrentMonth).Index(5);
            Map(m => m.ProfitOneMonthAgo).Index(6);
            Map(m => m.ProfitTwoMonthsAgo).Index(7);
            Map(m => m.ProfitThreeMonthsAgo).Index(8);
            Map(m => m.QuantityCommitted).Index(9);
            Map(m => m.QuantityOnOrder).Index(10);
            Map(m => m.OrderDate).Index(11);
            Map(m => m.QuantityShipped).Index(12);
            Map(m => m.ShipDate).Index(13);
            Map(m => m.DONumber).Index(14);
            Map(m => m.DODate).Index(15);
            Map(m => m.GrossWeight).Index(16);
            Map(m => m.NewDONumber).Index(17);
            Map(m => m.QuantityDiscount).Index(18);
            Map(m => m.QuantityPrice).Index(19);
            Map(m => m.PriceA).Index(20);
            Map(m => m.PriceB).Index(21);
            Map(m => m.PriceC).Index(22);
            Map(m => m.FOB).Index(23);
            Map(m => m.Duty).Index(24);
            Map(m => m.Cost).Index(25);
            Map(m => m.Location).Index(26);
            Map(m => m.VendorNumber).Index(27);
            Map(m => m.Memo).Index(28);
            Map(m => m.Container).Index(29);
            Map(m => m.Switches).Index(30);
            Map(m => m.ChineseDescription).Index(31);
            Map(m => m.ChineseBrand).Index(32);
            Map(m => m.NewItem).Index(33);
            Map(m => m.FileType).Index(34);


        }
    }
}
