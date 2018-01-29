using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SummitDataService
{
    public class SummitElement
    {

        // Item #
        public string ItemNumber { get; set; }

        // description
        public string Description { get; set; }

        // brand
        public string Brand { get; set; }

        // pack
        public string Pack { get; set; }

        // quantity on hand
        public string QuantityOnHand { get; set; }

        // profit current month
        public string ProfitCurrentMonth { get; set; }

        // profit one month ago
        public string ProfitOneMonthAgo { get; set; }

        // profit two months ago
        public string ProfitTwoMonthsAgo { get; set; }

        // profit three months ago
        public string ProfitThreeMonthsAgo { get; set; }

        // quantity committed
        public string QuantityCommitted { get; set; }

        // quantity on order
        public string QuantityOnOrder { get; set; }

        // order date
        public string OrderDate { get; set; }

        // quantity shipped
        public string QuantityShipped { get; set; }

        // ship date
        public string ShipDate { get; set; }

        // D O #
        public string DONumber { get; set; }

        // D O date
        public string DODate { get; set; }

        // gross weight
        public string GrossWeight { get; set; }

        // new D O #
        public string NewDONumber { get; set; }

        // quantity discount
        public string QuantityDiscount { get; set; }

        // quantity price
        public string QuantityPrice { get; set; }

        // price A
        public string PriceA { get; set; }

        // price B
        public string PriceB { get; set; }

        // price C
        public string PriceC { get; set; }

        // FOB
        public string FOB { get; set; }

        // duty
        public string Duty { get; set; }

        // cost
        public string Cost { get; set; }

        // location
        public string Location { get; set; }

        // vendor #
        public string VendorNumber { get; set; }

        // memo
        public string Memo { get; set; }

        // container
        public string Container { get; set; }

        // switches
        public string Switches { get; set; }

        // chinese description
        public string ChineseDescription { get; set; }

        // chinese brand
        public string ChineseBrand { get; set; }

        // new item #
        public string NewItem  { get; set; }

        // file type
        public string FileType { get; set; }
    }
}
