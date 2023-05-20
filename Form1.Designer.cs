
namespace Aula5
{
    partial class Form1
    {
        /// <summary>
        /// Variável de designer necessária.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Limpar os recursos que estão sendo usados.
        /// </summary>
        /// <param name="disposing">true se for necessário descartar os recursos gerenciados; caso contrário, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Código gerado pelo Windows Form Designer

        /// <summary>
        /// Método necessário para suporte ao Designer - não modifique 
        /// o conteúdo deste método com o editor de código.
        /// </summary>
        private void InitializeComponent()
        {
            this.btnPDP = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btnPDP
            // 
            this.btnPDP.Location = new System.Drawing.Point(209, 97);
            this.btnPDP.Name = "btnPDP";
            this.btnPDP.Size = new System.Drawing.Size(75, 23);
            this.btnPDP.TabIndex = 0;
            this.btnPDP.Text = "PDP";
            this.btnPDP.UseVisualStyleBackColor = true;
            this.btnPDP.Click += new System.EventHandler(this.btnPDP_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.btnPDP);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnPDP;
    }
}

