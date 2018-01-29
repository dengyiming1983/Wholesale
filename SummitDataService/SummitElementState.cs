using System;

namespace SummitDataService
{
    public class SummitElementState
    {
        public string Cmr { get; set; }
        public string cs_tran_no { get; set; }
       
        public string cs_date { get; set; }
        public string cs_due_date { get; set; }

        private string _cs_total;
        public string total {

            get { return _cs_total; }

            set
            {
                if (value.Trim() == string.Empty)
                {
                    _cs_total = "0";
                }
                else
                {
                    string v = value.Replace(" ", "");

                    decimal num = 1;
                    if (v.EndsWith("-"))
                    {
                        num *= -1;
                        v = value.Replace("-", "");
                    }

                    decimal res = 0;
                    if (decimal.TryParse(v, out res))
                    {
                        res = res / 100;
                        res *= num;
                    }
                    _cs_total = Convert.ToString(res);
                }
                
            }
        }

        private string _paid { get; set; }
        public string paid
        {
            get { return _paid; }

            set
            {
                if (value.Trim() == string.Empty)
                {
                    _paid = "0";
                }
                else
                {
                    string v = value.Replace(" ", "");

                    decimal num = 1;
                    if (v.EndsWith("-"))
                    {
                        num *= -1;
                        v = value.Replace("-", "");
                    }

                    decimal res = 0;
                    if (decimal.TryParse(v, out res))
                    {
                        res = res / 100;
                        res *= num;
                    }

                    _paid = Convert.ToString(res);
                }

            }
        }

        private string _discount { get; set; }
        public string discount {
            get { return _discount; }

            set
            {
                if (value.Trim() == string.Empty)
                {
                    _discount = "0";
                }
                else
                {
                    string v = value.Replace(" ", "");

                    decimal num = 1;
                    if (v.EndsWith("-"))
                    {
                        num *= -1;
                        v = value.Replace("-", "");
                    }

                    decimal res = 0;
                    if (decimal.TryParse(v, out res))
                    {
                        res = res / 100;
                        res *= num;
                    }

                    _discount = Convert.ToString(res);
                }

            }
        }
        private string _balance { get; set; }

        /// <summary>
        /// balance绝对是正数
        /// </summary>
        public string balance
        {
            get { return _balance; }

            set
            {
                if (value.Trim() == string.Empty)
                {
                    _balance = "0";
                }
                else
                {
                    string v = value.Replace(" ", "");

                    decimal num = 1;
                    if (v.EndsWith("-"))
                    {
                        num *= -1;
                        v = value.Replace("-", "");
                    }

                    decimal res = 0;
                    if (decimal.TryParse(v, out res))
                    {
                        res = res / 100;
                        res *= num;
                    }

                    _balance = Convert.ToString(res);
                }
                
            }
        }

    }
}