using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Aula5
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void btnPDP_Click(object sender, EventArgs e)
        {

            PDP MeuPDP = new PDP();
            MeuPDP.LerRequisicoes(@"C:\Users\User\Documents\Mestrado\PMI\Aula02\Aula5\Requisicoes.txt");
            MeuPDP.LerVeiculos(@"C:\Users\User\Documents\Mestrado\PMI\Aula02\Aula5\Veiculos.txt");
            MeuPDP.CriarModelo();
        }
    }
}
