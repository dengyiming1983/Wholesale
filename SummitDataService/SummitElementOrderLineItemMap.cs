using CsvHelper.Configuration;

namespace SummitDataService
{
    public sealed class SummitElementOrderLineItemMap : CsvClassMap<SummitElementOrderLineItem>
    {
        public SummitElementOrderLineItemMap()
        {
            Map(m => m.Type)            .Index(0);
            Map(m => m.RefNumber)       .Index(1);
            Map(m => m.InvoiceNumber)   .Index(2);
            Map(m => m.ItemNumber)      .Index(3);
            Map(m => m.Price)           .Index(4);
            Map(m => m.Quantity)        .Index(5);
            Map(m => m.LineNumber)      .Index(6);
        }
    }
}