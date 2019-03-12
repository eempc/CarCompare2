using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CarCompareDesktop {
    public partial class Form1 : Form {
        public Form1() {
            InitializeComponent();
        }

        string connectionString = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=CarCompareContext-d1a204cc-6cb2-4983-b6ca-8e135f56615c;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";

        private void buttonDisplayAll_Click(object sender, EventArgs e) {

        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e) => Application.Exit();
    }
}
