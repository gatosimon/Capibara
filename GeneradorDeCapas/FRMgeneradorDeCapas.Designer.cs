
namespace GeneradorDeCapas
{
    partial class FRMgeneradorDeCapas
    {
        /// <summary>
        /// Variable del diseñador necesaria.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Limpiar los recursos que se estén usando.
        /// </summary>
        /// <param name="disposing">true si los recursos administrados se deben desechar; false en caso contrario.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Código generado por el Diseñador de Windows Forms

        /// <summary>
        /// Método necesario para admitir el Diseñador. No se puede modificar
        /// el contenido de este método con el editor de código.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FRMgeneradorDeCapas));
            this.LBLservidor = new System.Windows.Forms.Label();
            this.BTNgenerar = new System.Windows.Forms.Button();
            this.CMBservidor = new System.Windows.Forms.ComboBox();
            this.CMBtablas = new System.Windows.Forms.ComboBox();
            this.LBLtablas = new System.Windows.Forms.Label();
            this.CMBbases = new System.Windows.Forms.ComboBox();
            this.LBLbases = new System.Windows.Forms.Label();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.LBLtablaSeleccionada = new System.Windows.Forms.Label();
            this.LSVcampos = new System.Windows.Forms.ListView();
            this.TXTclase = new System.Windows.Forms.TextBox();
            this.LBLespacioDeNombres = new System.Windows.Forms.Label();
            this.TXTespacioDeNombres = new System.Windows.Forms.TextBox();
            this.GPBgenerar = new System.Windows.Forms.GroupBox();
            this.CHKquitarEsquema = new System.Windows.Forms.CheckBox();
            this.CHKservice = new System.Windows.Forms.CheckBox();
            this.CHKrepositories = new System.Windows.Forms.CheckBox();
            this.CHKmodel = new System.Windows.Forms.CheckBox();
            this.CHKdto = new System.Windows.Forms.CheckBox();
            this.CHKcontrollers = new System.Windows.Forms.CheckBox();
            this.RDBsql = new System.Windows.Forms.RadioButton();
            this.RDBdb2 = new System.Windows.Forms.RadioButton();
            this.label1 = new System.Windows.Forms.Label();
            this.TXTpathCapas = new System.Windows.Forms.TextBox();
            this.BTNdirectorioCapas = new System.Windows.Forms.Button();
            this.FBDdirectorioCapas = new System.Windows.Forms.FolderBrowserDialog();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.GPBgenerar.SuspendLayout();
            this.SuspendLayout();
            // 
            // LBLservidor
            // 
            this.LBLservidor.AutoSize = true;
            this.LBLservidor.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LBLservidor.Location = new System.Drawing.Point(171, 23);
            this.LBLservidor.Name = "LBLservidor";
            this.LBLservidor.Size = new System.Drawing.Size(80, 16);
            this.LBLservidor.TabIndex = 0;
            this.LBLservidor.Text = "SERVIDOR:";
            // 
            // BTNgenerar
            // 
            this.BTNgenerar.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.BTNgenerar.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.BTNgenerar.Location = new System.Drawing.Point(1050, 19);
            this.BTNgenerar.Name = "BTNgenerar";
            this.BTNgenerar.Size = new System.Drawing.Size(75, 25);
            this.BTNgenerar.TabIndex = 9;
            this.BTNgenerar.Text = "&Generar";
            this.BTNgenerar.UseVisualStyleBackColor = true;
            this.BTNgenerar.Click += new System.EventHandler(this.BTNgenerar_Click);
            // 
            // CMBservidor
            // 
            this.CMBservidor.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.CMBservidor.FormattingEnabled = true;
            this.CMBservidor.Items.AddRange(new object[] {
            "133.123.120.120",
            "SERVER04",
            "SERVER01"});
            this.CMBservidor.Location = new System.Drawing.Point(260, 19);
            this.CMBservidor.Name = "CMBservidor";
            this.CMBservidor.Size = new System.Drawing.Size(137, 24);
            this.CMBservidor.TabIndex = 2;
            this.CMBservidor.SelectedIndexChanged += new System.EventHandler(this.CMBservidor_SelectedIndexChanged);
            // 
            // CMBtablas
            // 
            this.CMBtablas.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.CMBtablas.FormattingEnabled = true;
            this.CMBtablas.Location = new System.Drawing.Point(738, 19);
            this.CMBtablas.Name = "CMBtablas";
            this.CMBtablas.Size = new System.Drawing.Size(298, 24);
            this.CMBtablas.TabIndex = 4;
            this.CMBtablas.SelectedIndexChanged += new System.EventHandler(this.CMBtablas_SelectedIndexChanged);
            this.CMBtablas.TextUpdate += new System.EventHandler(this.CMBtablas_TextUpdate);
            // 
            // LBLtablas
            // 
            this.LBLtablas.AutoSize = true;
            this.LBLtablas.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LBLtablas.Location = new System.Drawing.Point(665, 23);
            this.LBLtablas.Name = "LBLtablas";
            this.LBLtablas.Size = new System.Drawing.Size(64, 16);
            this.LBLtablas.TabIndex = 4;
            this.LBLtablas.Text = "TABLAS:";
            // 
            // CMBbases
            // 
            this.CMBbases.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.CMBbases.FormattingEnabled = true;
            this.CMBbases.Items.AddRange(new object[] {
            "CONTABIL",
            "CONTAICD",
            "CONTAIMV",
            "CONTCBEL",
            "CONTIDS",
            "DOCUMENT",
            "GENERAL",
            "GIS",
            "HISTABM",
            "HISTORIC",
            "INFORMAT",
            "LICENCIA",
            "RRHH",
            "SISUS",
            "TRIBUTOS"});
            this.CMBbases.Location = new System.Drawing.Point(535, 19);
            this.CMBbases.Name = "CMBbases";
            this.CMBbases.Size = new System.Drawing.Size(121, 24);
            this.CMBbases.TabIndex = 3;
            this.CMBbases.SelectedIndexChanged += new System.EventHandler(this.CMBbases_SelectedIndexChanged);
            // 
            // LBLbases
            // 
            this.LBLbases.AutoSize = true;
            this.LBLbases.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LBLbases.Location = new System.Drawing.Point(406, 23);
            this.LBLbases.Name = "LBLbases";
            this.LBLbases.Size = new System.Drawing.Size(120, 16);
            this.LBLbases.TabIndex = 6;
            this.LBLbases.Text = "BASE DE DATOS:";
            // 
            // splitContainer1
            // 
            this.splitContainer1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.splitContainer1.Location = new System.Drawing.Point(15, 193);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.LBLtablaSeleccionada);
            this.splitContainer1.Panel1.Controls.Add(this.LSVcampos);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.TXTclase);
            this.splitContainer1.Size = new System.Drawing.Size(1110, 624);
            this.splitContainer1.SplitterDistance = 392;
            this.splitContainer1.TabIndex = 8;
            // 
            // LBLtablaSeleccionada
            // 
            this.LBLtablaSeleccionada.AutoSize = true;
            this.LBLtablaSeleccionada.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LBLtablaSeleccionada.Location = new System.Drawing.Point(8, 8);
            this.LBLtablaSeleccionada.Name = "LBLtablaSeleccionada";
            this.LBLtablaSeleccionada.Size = new System.Drawing.Size(0, 16);
            this.LBLtablaSeleccionada.TabIndex = 13;
            // 
            // LSVcampos
            // 
            this.LSVcampos.Activation = System.Windows.Forms.ItemActivation.OneClick;
            this.LSVcampos.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.LSVcampos.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LSVcampos.FullRowSelect = true;
            this.LSVcampos.HideSelection = false;
            this.LSVcampos.Location = new System.Drawing.Point(0, 27);
            this.LSVcampos.Name = "LSVcampos";
            this.LSVcampos.Size = new System.Drawing.Size(392, 630);
            this.LSVcampos.TabIndex = 0;
            this.LSVcampos.UseCompatibleStateImageBehavior = false;
            // 
            // TXTclase
            // 
            this.TXTclase.Dock = System.Windows.Forms.DockStyle.Fill;
            this.TXTclase.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.TXTclase.Location = new System.Drawing.Point(0, 0);
            this.TXTclase.Multiline = true;
            this.TXTclase.Name = "TXTclase";
            this.TXTclase.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.TXTclase.Size = new System.Drawing.Size(714, 624);
            this.TXTclase.TabIndex = 0;
            this.TXTclase.TabStop = false;
            // 
            // LBLespacioDeNombres
            // 
            this.LBLespacioDeNombres.AutoSize = true;
            this.LBLespacioDeNombres.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LBLespacioDeNombres.Location = new System.Drawing.Point(12, 65);
            this.LBLespacioDeNombres.Name = "LBLespacioDeNombres";
            this.LBLespacioDeNombres.Size = new System.Drawing.Size(162, 16);
            this.LBLespacioDeNombres.TabIndex = 13;
            this.LBLespacioDeNombres.Text = "ESPACIO DE NOMBRES:";
            // 
            // TXTespacioDeNombres
            // 
            this.TXTespacioDeNombres.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.TXTespacioDeNombres.Location = new System.Drawing.Point(180, 62);
            this.TXTespacioDeNombres.Name = "TXTespacioDeNombres";
            this.TXTespacioDeNombres.Size = new System.Drawing.Size(697, 23);
            this.TXTespacioDeNombres.TabIndex = 5;
            this.TXTespacioDeNombres.Text = "WebAPI";
            // 
            // GPBgenerar
            // 
            this.GPBgenerar.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.GPBgenerar.Controls.Add(this.CHKquitarEsquema);
            this.GPBgenerar.Controls.Add(this.CHKservice);
            this.GPBgenerar.Controls.Add(this.CHKrepositories);
            this.GPBgenerar.Controls.Add(this.CHKmodel);
            this.GPBgenerar.Controls.Add(this.CHKdto);
            this.GPBgenerar.Controls.Add(this.CHKcontrollers);
            this.GPBgenerar.Location = new System.Drawing.Point(15, 128);
            this.GPBgenerar.Name = "GPBgenerar";
            this.GPBgenerar.Size = new System.Drawing.Size(1110, 58);
            this.GPBgenerar.TabIndex = 7;
            this.GPBgenerar.TabStop = false;
            this.GPBgenerar.Text = "A generar";
            // 
            // CHKquitarEsquema
            // 
            this.CHKquitarEsquema.AutoSize = true;
            this.CHKquitarEsquema.Checked = true;
            this.CHKquitarEsquema.CheckState = System.Windows.Forms.CheckState.Checked;
            this.CHKquitarEsquema.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.CHKquitarEsquema.Location = new System.Drawing.Point(565, 24);
            this.CHKquitarEsquema.Name = "CHKquitarEsquema";
            this.CHKquitarEsquema.Size = new System.Drawing.Size(130, 20);
            this.CHKquitarEsquema.TabIndex = 5;
            this.CHKquitarEsquema.Text = "Quitar esquema";
            this.CHKquitarEsquema.UseVisualStyleBackColor = true;
            this.CHKquitarEsquema.Visible = false;
            // 
            // CHKservice
            // 
            this.CHKservice.AutoSize = true;
            this.CHKservice.Checked = true;
            this.CHKservice.CheckState = System.Windows.Forms.CheckState.Checked;
            this.CHKservice.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.CHKservice.Location = new System.Drawing.Point(457, 24);
            this.CHKservice.Name = "CHKservice";
            this.CHKservice.Size = new System.Drawing.Size(76, 20);
            this.CHKservice.TabIndex = 4;
            this.CHKservice.Text = "Service";
            this.CHKservice.UseVisualStyleBackColor = true;
            // 
            // CHKrepositories
            // 
            this.CHKrepositories.AutoSize = true;
            this.CHKrepositories.Checked = true;
            this.CHKrepositories.CheckState = System.Windows.Forms.CheckState.Checked;
            this.CHKrepositories.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.CHKrepositories.Location = new System.Drawing.Point(319, 24);
            this.CHKrepositories.Name = "CHKrepositories";
            this.CHKrepositories.Size = new System.Drawing.Size(106, 20);
            this.CHKrepositories.TabIndex = 3;
            this.CHKrepositories.Text = "Repositories";
            this.CHKrepositories.UseVisualStyleBackColor = true;
            // 
            // CHKmodel
            // 
            this.CHKmodel.AutoSize = true;
            this.CHKmodel.Checked = true;
            this.CHKmodel.CheckState = System.Windows.Forms.CheckState.Checked;
            this.CHKmodel.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.CHKmodel.Location = new System.Drawing.Point(222, 24);
            this.CHKmodel.Name = "CHKmodel";
            this.CHKmodel.Size = new System.Drawing.Size(65, 20);
            this.CHKmodel.TabIndex = 2;
            this.CHKmodel.Text = "Model";
            this.CHKmodel.UseVisualStyleBackColor = true;
            // 
            // CHKdto
            // 
            this.CHKdto.AutoSize = true;
            this.CHKdto.Checked = true;
            this.CHKdto.CheckState = System.Windows.Forms.CheckState.Checked;
            this.CHKdto.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.CHKdto.Location = new System.Drawing.Point(140, 24);
            this.CHKdto.Name = "CHKdto";
            this.CHKdto.Size = new System.Drawing.Size(50, 20);
            this.CHKdto.TabIndex = 1;
            this.CHKdto.Text = "Dto";
            this.CHKdto.UseVisualStyleBackColor = true;
            // 
            // CHKcontrollers
            // 
            this.CHKcontrollers.AutoSize = true;
            this.CHKcontrollers.Checked = true;
            this.CHKcontrollers.CheckState = System.Windows.Forms.CheckState.Checked;
            this.CHKcontrollers.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.CHKcontrollers.Location = new System.Drawing.Point(11, 24);
            this.CHKcontrollers.Name = "CHKcontrollers";
            this.CHKcontrollers.Size = new System.Drawing.Size(97, 20);
            this.CHKcontrollers.TabIndex = 0;
            this.CHKcontrollers.Text = "Controllers";
            this.CHKcontrollers.UseVisualStyleBackColor = true;
            // 
            // RDBsql
            // 
            this.RDBsql.AutoSize = true;
            this.RDBsql.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.RDBsql.Location = new System.Drawing.Point(15, 21);
            this.RDBsql.Name = "RDBsql";
            this.RDBsql.Size = new System.Drawing.Size(52, 20);
            this.RDBsql.TabIndex = 0;
            this.RDBsql.Text = "&SQL";
            this.RDBsql.UseVisualStyleBackColor = true;
            this.RDBsql.CheckedChanged += new System.EventHandler(this.RDBsql_CheckedChanged);
            // 
            // RDBdb2
            // 
            this.RDBdb2.AutoSize = true;
            this.RDBdb2.Checked = true;
            this.RDBdb2.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.RDBdb2.Location = new System.Drawing.Point(93, 21);
            this.RDBdb2.Name = "RDBdb2";
            this.RDBdb2.Size = new System.Drawing.Size(51, 20);
            this.RDBdb2.TabIndex = 1;
            this.RDBdb2.TabStop = true;
            this.RDBdb2.Text = "&DB2";
            this.RDBdb2.UseVisualStyleBackColor = true;
            this.RDBdb2.CheckedChanged += new System.EventHandler(this.RDBdb2_CheckedChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(12, 99);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(144, 16);
            this.label1.TabIndex = 18;
            this.label1.Text = "DIRECTORIO CAPAS:";
            // 
            // TXTpathCapas
            // 
            this.TXTpathCapas.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.TXTpathCapas.Location = new System.Drawing.Point(180, 96);
            this.TXTpathCapas.Name = "TXTpathCapas";
            this.TXTpathCapas.Size = new System.Drawing.Size(697, 23);
            this.TXTpathCapas.TabIndex = 6;
            this.TXTpathCapas.Text = "C:\\temp\\";
            // 
            // BTNdirectorioCapas
            // 
            this.BTNdirectorioCapas.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.BTNdirectorioCapas.Location = new System.Drawing.Point(883, 95);
            this.BTNdirectorioCapas.Name = "BTNdirectorioCapas";
            this.BTNdirectorioCapas.Size = new System.Drawing.Size(38, 25);
            this.BTNdirectorioCapas.TabIndex = 7;
            this.BTNdirectorioCapas.Text = "...";
            this.BTNdirectorioCapas.UseVisualStyleBackColor = true;
            this.BTNdirectorioCapas.Click += new System.EventHandler(this.BTNdirectorioCapas_Click);
            // 
            // FRMgeneradorDeCapas
            // 
            this.ClientSize = new System.Drawing.Size(1137, 829);
            this.Controls.Add(this.BTNdirectorioCapas);
            this.Controls.Add(this.TXTpathCapas);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.RDBdb2);
            this.Controls.Add(this.RDBsql);
            this.Controls.Add(this.GPBgenerar);
            this.Controls.Add(this.TXTespacioDeNombres);
            this.Controls.Add(this.LBLespacioDeNombres);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.CMBbases);
            this.Controls.Add(this.LBLbases);
            this.Controls.Add(this.CMBtablas);
            this.Controls.Add(this.LBLtablas);
            this.Controls.Add(this.CMBservidor);
            this.Controls.Add(this.BTNgenerar);
            this.Controls.Add(this.LBLservidor);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "FRMgeneradorDeCapas";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Load += new System.EventHandler(this.FRMgeneradorDeCapas_Load);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel1.PerformLayout();
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.GPBgenerar.ResumeLayout(false);
            this.GPBgenerar.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label LBLservidor;
        private System.Windows.Forms.Button BTNgenerar;
        private System.Windows.Forms.ComboBox CMBservidor;
        private System.Windows.Forms.ComboBox CMBtablas;
        private System.Windows.Forms.Label LBLtablas;
        private System.Windows.Forms.ComboBox CMBbases;
        private System.Windows.Forms.Label LBLbases;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.Label LBLtablaSeleccionada;
        private System.Windows.Forms.ListView LSVcampos;
        private System.Windows.Forms.TextBox TXTclase;
        private System.Windows.Forms.Label LBLespacioDeNombres;
        private System.Windows.Forms.TextBox TXTespacioDeNombres;
        private System.Windows.Forms.GroupBox GPBgenerar;
        private System.Windows.Forms.CheckBox CHKservice;
        private System.Windows.Forms.CheckBox CHKrepositories;
        private System.Windows.Forms.CheckBox CHKmodel;
        private System.Windows.Forms.CheckBox CHKdto;
        private System.Windows.Forms.CheckBox CHKcontrollers;
        private System.Windows.Forms.RadioButton RDBsql;
        private System.Windows.Forms.RadioButton RDBdb2;
        private System.Windows.Forms.CheckBox CHKquitarEsquema;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox TXTpathCapas;
        private System.Windows.Forms.Button BTNdirectorioCapas;
        private System.Windows.Forms.FolderBrowserDialog FBDdirectorioCapas;
    }
}

