using ServiceUitls.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gfcaDataService
{
    class WholesaleConnection: DbHelperOdbc
    {

        public void SetConnectString(string connectString)
        {
            connectionString = connectString;
        }
    }
}
