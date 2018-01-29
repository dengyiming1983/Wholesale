using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SummitDataService
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string pack = textBox1.Text;
            var r = Regex.Matches(pack, @"\((.+?)\)");
            foreach (Match x in r)
                Console.WriteLine(x.Groups[1].Value);

            foreach (Match x in r)
            {
                pack = pack.Replace(x.Groups[0].Value, " ");
            }

            //容器，例如瓶子
            string pack_case = "";
            string pack_info = "";
            string measure = "";
            string netWt = "1";
            string package = "1";
            string layer = "1";


            var vals0 = pack.Split(' ');

            if (vals0.Length > 0)
            {
                pack_info = vals0[0];

                var vals = pack_info.ToUpper().Split('X');

                if (vals.Length > 0)
                {
                    if (vals.Length - 1 >= 0)
                    {
                        netWt = vals[vals.Length - 1];
                       // netWt = netWt.Substring(0, netWt.IndexOf(' '));
                    }

                    if (vals.Length - 2 >= 0)
                    {
                        package = vals[vals.Length - 2];
                    }

                    if (vals.Length - 3 >= 0)
                    {
                        layer = vals[vals.Length - 3];
                    }

                }

                if (vals0.Length > 1)
                {
                    measure = vals0[1];
                }

                //measure
                var re = Regex.Matches(pack, @"([a-zA-Z]+)\.");
                foreach (Match x in re)
                    Console.WriteLine(x.Groups[1].Value);

                var re2 = Regex.Matches(pack, @"\/([a-zA-Z]+)");
                foreach (Match x in re2)
                {
                    pack_case = x.Groups[1].Value;
                }
                    


            }



            MessageBox.Show(string.Format("Layer: {0} \r\n Package: {1}\r\n Net WT: {2}\r\n  measure: {3}\r\n  pack_case: {4}", layer,package, netWt, measure, pack_case));
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // textBox1.Text = "12x6.7 FL.OZ(8.8OZ) ";
            textBox1.Text = "12X14.7 FL.OZ./PET B";
        }
    }
}
