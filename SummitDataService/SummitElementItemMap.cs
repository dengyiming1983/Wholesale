using CsvHelper.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SummitDataService
{
     public sealed  class SummitElementItemMap :CsvClassMap<SummitElementItem>
    {
        public SummitElementItemMap()
        {
            Map(m => m.ItemNumber).Index(0);
            Map(m => m.Description).Index(1);
            Map(m => m.Brand).Index(2);
            Map(m => m.Pack).Index(3);
            Map(m => m.QuantityOnHand).Index(4);
            Map(m => m.GrossWeight).Index(5);
            Map(m => m.PriceA).Index(6);
            Map(m => m.PriceB).Index(7);
            Map(m => m.PriceC).Index(8);
            Map(m => m.FOB).Index(9);
            Map(m => m.Cost).Index(10);
            Map(m => m.VendorNumber).Index(11);
            Map(m => m.Memo).Index(12);
            Map(m => m.ChineseDescription).Index(13);
            Map(m => m.UPCCode).Index(14);
            Map(m => m.RelatedItemCode).Index(15);
            Map(m => m.BrandName).Index(16);
            Map(m => m.RecordType).Index(17);

        }
    }
}
