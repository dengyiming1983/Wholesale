using System;
using System.Text;

namespace SummitDataService
{
    public class SummitElementCustomer
    {
        //Acct# 
        public string AcctNumber { get; set; }

        //Name 
        //public string Name { get; set; }
        private string _Name;
        public string Name
        {

            get { return _Name; }

            set { _Name = value.Trim(); }

        }

        //Region 
        public string Region { get; set; }

        //Bill to 1 
        public string BillTo1 { get; set; }

        //      2 
        public string BillTo2 { get; set; }

        //      3 
        public string BillTo3 { get; set; }

        //Ship to 1 
        public string ShipTo1 { get; set; }

        //     2 
        public string ShipTo2 { get; set; }

        //     3 
        public string ShipTo3 { get; set; }

        //Phone 
        public string Phone { get; set; }

        //Balance 
        //public string Balance { get; set; }
        private string _Balance;
        public string Balance {
            get { return _Balance; }

            set
            {
                string v = value.Replace(" ", "");

                decimal res = 0;
                if (decimal.TryParse(v, out res))
                {
                    res = res / 100;
                    if (res < 0)
                        res = 0;
                }
                else
                {
                    res = 0;
                }
                _Balance = Convert.ToString(res);
            }
        }

        //CurrentBalance
        private string _CurrentBalance;
        public string CurrentBalance
        {
            get { return _CurrentBalance; }

            set
            {
                string v = value.Replace(" ", "");

                decimal res = 0;
                if (decimal.TryParse(v, out res))
                {
                    res = res / 100;
                    if (res < 0)
                        res = 0;
                }
                else
                {
                    res = 0;
                }
                _CurrentBalance = Convert.ToString(res);
            }
        }

        // 30 days balance
        private string _v30DaysBalance;
        public string v30DaysBalance
        {
            get { return _v30DaysBalance; }

            set
            {
                string v = value.Replace(" ", "");

                decimal res = 0;
                if (decimal.TryParse(v, out res))
                {
                    res = res / 100;
                    if (res < 0)
                        res = 0;
                }
                else
                {
                    res = 0;
                }
                _v30DaysBalance = Convert.ToString(res);
            }
        }

        // 60 days balance
        private string _v60DaysBalance;
        public string v60DaysBalance
        {
            get { return _v60DaysBalance; }

            set
            {
                string v = value.Replace(" ", "");

                decimal res = 0;
                if (decimal.TryParse(v, out res))
                {
                    res = res / 100;
                    if (res < 0)
                        res = 0;
                }
                else
                {
                    res = 0;
                }
                _v60DaysBalance = Convert.ToString(res);
            }
        }

        // 90 days balance
        private string _v90DaysBalance;
        public string v90DaysBalance
        {
            get { return _v90DaysBalance; }

            set
            {
                string v = value.Replace(" ", "");

                decimal res = 0;
                if (decimal.TryParse(v, out res))
                {
                    res = res / 100;
                    if (res < 0)
                        res = 0;
                }
                else
                {
                    res = 0;
                }
                _v90DaysBalance = Convert.ToString(res);
            }
        }

        //SM# 
        //public string SMNumber { get; set; }
        private string _SMNumber;
        public string SMNumber {

            get { return _SMNumber; }

            set
            {
                string v = value.Replace(" ", "");

                Int32 res = 0;
                if (Int32.TryParse(v, out res))
                {
                    //res = res / 100;
                    //if (res < 0)
                    //    res = 0;
                    _SMNumber = Convert.ToString(res);
                }
                else
                {
                    _SMNumber = "";
                }
               
            }

        }


        //Salesman 
        //public string Salesman { get; set; }
        private string _Salesman;
        public string Salesman
        {

            get { return _Salesman; }

            set { _Salesman = value.Trim(); }

        }

        //Contact 
        //public string Contact { get; set; }
        private string _Contact;
        public string Contact
        {

            get { return _Contact; }

            set { _Contact = value.Trim(); }

        }

        //email 
        //public string Email { get; set; }
        private string _Email;
        public string Email
        {

            get { return _Email; }

            set { _Email = value.Trim(); }

        }

        //Chinese Name - 
        //public string ChineseName { get; set; }
        private string _ChineseName;
        /// <summary>
        /// 注意: 此处乱码问题解决，由于导入内容字符编码为big5, 需要转换big5编码进行解析才能正确显示
        /// </summary>
        public string ChineseName
        {
            get { return _ChineseName; }

            set
            {
                string v = value.Trim();

                byte[] src = Encoding.Default.GetBytes(v);

                // build 0006
                _ChineseName = Encoding.GetEncoding("big5").GetString(src);

                // build 0007
                //_ChineseName = _encoding.GetString(src);
            }
        }


        public string RecordType { get; set; }


        //private Encoding _encoding = Encoding.Default;
        //internal void SetEncoding(Encoding fileEncoding)
        //{
        //    _encoding = fileEncoding;
        //}
    }
}