using CsvHelper.Configuration;

namespace SummitDataService
{
    public sealed class SummitElementCustomerMap : CsvClassMap<SummitElementCustomer>
    {
        public SummitElementCustomerMap()
        {
            Map(m => m.AcctNumber).Index(0);
            Map(m => m.Name).Index(1);
            Map(m => m.Region).Index(2);
            Map(m => m.BillTo1).Index(3);
            Map(m => m.BillTo2).Index(4);
            Map(m => m.BillTo3).Index(5);
            Map(m => m.ShipTo1).Index(6);
            Map(m => m.ShipTo2).Index(7);
            Map(m => m.ShipTo3).Index(8);
            Map(m => m.Phone).Index(9);
            Map(m => m.Balance).Index(10);

            
            Map(m => m.CurrentBalance).Index(11); /* build 0008 added */
            Map(m => m.v30DaysBalance).Index(12); /* build 0008 added */
            Map(m => m.v60DaysBalance).Index(13); /* build 0008 added */
            Map(m => m.v90DaysBalance).Index(14); /* build 0008 added */

            Map(m => m.SMNumber).Index(15);
            Map(m => m.Salesman).Index(16);
            Map(m => m.Contact).Index(17);
            Map(m => m.Email).Index(18);
            Map(m => m.ChineseName).Index(19);

            Map(m => m.RecordType).Index(20);

        }
    }
}