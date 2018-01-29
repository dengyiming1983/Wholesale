using CsvHelper.Configuration;

namespace SummitDataService
{
    public class SummitElementStateMap : CsvClassMap<SummitElementState>
    {

        public SummitElementStateMap()
        {
            Map(m => m.Cmr).Index(0);
            Map(m => m.cs_tran_no).Index(1);
            Map(m => m.cs_date).Index(2);
            Map(m => m.cs_due_date).Index(3);
            Map(m => m.total).Index(4);
            Map(m => m.paid).Index(5);
            Map(m => m.discount).Index(6);
            Map(m => m.balance).Index(7); 

        }
    }
}