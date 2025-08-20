
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
            this.CMBservidor = new System.Windows.Forms.ComboBox();
            this.CMBtablas = new System.Windows.Forms.ComboBox();
            this.LBLtablas = new System.Windows.Forms.Label();
            this.CMBbases = new System.Windows.Forms.ComboBox();
            this.LBLbases = new System.Windows.Forms.Label();
            this.LBLespacioDeNombres = new System.Windows.Forms.Label();
            this.TXTespacioDeNombres = new System.Windows.Forms.TextBox();
            this.RDBsql = new System.Windows.Forms.RadioButton();
            this.RDBdb2 = new System.Windows.Forms.RadioButton();
            this.LBLdirectorioCapas = new System.Windows.Forms.Label();
            this.TXTpathCapas = new System.Windows.Forms.TextBox();
            this.BTNdirectorioCapas = new System.Windows.Forms.Button();
            this.FBDdirectorioCapas = new System.Windows.Forms.FolderBrowserDialog();
            this.BTNbuscarSolucion = new System.Windows.Forms.Button();
            this.CMBnamespaces = new System.Windows.Forms.ComboBox();
            this.OFDlistarDeSolucion = new System.Windows.Forms.OpenFileDialog();
            this.SPCcapas = new System.Windows.Forms.SplitContainer();
            this.GPBback = new System.Windows.Forms.GroupBox();
            this.CHKtryOrIf = new System.Windows.Forms.CheckBox();
            this.CHKquitarEsquema = new System.Windows.Forms.CheckBox();
            this.CHKservice = new System.Windows.Forms.CheckBox();
            this.CHKrepositories = new System.Windows.Forms.CheckBox();
            this.CHKmodel = new System.Windows.Forms.CheckBox();
            this.CHKdto = new System.Windows.Forms.CheckBox();
            this.CHKcontrollers = new System.Windows.Forms.CheckBox();
            this.GPBFront = new System.Windows.Forms.GroupBox();
            this.CHKtypeScript = new System.Windows.Forms.CheckBox();
            this.BTNgenerarDesdeTabla = new System.Windows.Forms.Button();
            this.SPCseparador = new System.Windows.Forms.SplitContainer();
            this.GBPaPartirDeConsulta = new System.Windows.Forms.GroupBox();
            this.BTNgenerarDesdeConsulta = new System.Windows.Forms.Button();
            this.BNTobtenerEstructura = new System.Windows.Forms.Button();
            this.TXTgenerarAPartirDeConsulta = new System.Windows.Forms.TextBox();
            this.SPCclase = new System.Windows.Forms.SplitContainer();
            this.LBLtablaSeleccionada = new System.Windows.Forms.Label();
            this.LSVcampos = new System.Windows.Forms.ListView();
            this.TXTclase = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.SPCcapas)).BeginInit();
            this.SPCcapas.Panel1.SuspendLayout();
            this.SPCcapas.Panel2.SuspendLayout();
            this.SPCcapas.SuspendLayout();
            this.GPBback.SuspendLayout();
            this.GPBFront.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.SPCseparador)).BeginInit();
            this.SPCseparador.Panel1.SuspendLayout();
            this.SPCseparador.Panel2.SuspendLayout();
            this.SPCseparador.SuspendLayout();
            this.GBPaPartirDeConsulta.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.SPCclase)).BeginInit();
            this.SPCclase.Panel1.SuspendLayout();
            this.SPCclase.Panel2.SuspendLayout();
            this.SPCclase.SuspendLayout();
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
            this.TXTespacioDeNombres.Size = new System.Drawing.Size(561, 23);
            this.TXTespacioDeNombres.TabIndex = 5;
            this.TXTespacioDeNombres.Text = "WebAPI";
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
            // LBLdirectorioCapas
            // 
            this.LBLdirectorioCapas.AutoSize = true;
            this.LBLdirectorioCapas.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LBLdirectorioCapas.Location = new System.Drawing.Point(12, 99);
            this.LBLdirectorioCapas.Name = "LBLdirectorioCapas";
            this.LBLdirectorioCapas.Size = new System.Drawing.Size(144, 16);
            this.LBLdirectorioCapas.TabIndex = 18;
            this.LBLdirectorioCapas.Text = "DIRECTORIO CAPAS:";
            // 
            // TXTpathCapas
            // 
            this.TXTpathCapas.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.TXTpathCapas.Location = new System.Drawing.Point(180, 96);
            this.TXTpathCapas.Name = "TXTpathCapas";
            this.TXTpathCapas.Size = new System.Drawing.Size(561, 23);
            this.TXTpathCapas.TabIndex = 8;
            this.TXTpathCapas.Text = "C:\\temp\\";
            // 
            // BTNdirectorioCapas
            // 
            this.BTNdirectorioCapas.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.BTNdirectorioCapas.Location = new System.Drawing.Point(747, 95);
            this.BTNdirectorioCapas.Name = "BTNdirectorioCapas";
            this.BTNdirectorioCapas.Size = new System.Drawing.Size(38, 25);
            this.BTNdirectorioCapas.TabIndex = 9;
            this.BTNdirectorioCapas.Text = "...";
            this.BTNdirectorioCapas.UseVisualStyleBackColor = true;
            this.BTNdirectorioCapas.Click += new System.EventHandler(this.BTNdirectorioCapas_Click);
            // 
            // BTNbuscarSolucion
            // 
            this.BTNbuscarSolucion.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.BTNbuscarSolucion.Location = new System.Drawing.Point(747, 61);
            this.BTNbuscarSolucion.Name = "BTNbuscarSolucion";
            this.BTNbuscarSolucion.Size = new System.Drawing.Size(161, 25);
            this.BTNbuscarSolucion.TabIndex = 6;
            this.BTNbuscarSolucion.Text = "Listar desde solución";
            this.BTNbuscarSolucion.UseVisualStyleBackColor = true;
            this.BTNbuscarSolucion.Click += new System.EventHandler(this.BTNbuscarSolucion_Click);
            // 
            // CMBnamespaces
            // 
            this.CMBnamespaces.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.CMBnamespaces.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.CMBnamespaces.FormattingEnabled = true;
            this.CMBnamespaces.Location = new System.Drawing.Point(914, 61);
            this.CMBnamespaces.Name = "CMBnamespaces";
            this.CMBnamespaces.Size = new System.Drawing.Size(254, 24);
            this.CMBnamespaces.TabIndex = 7;
            this.CMBnamespaces.SelectedIndexChanged += new System.EventHandler(this.CMBnamespaces_SelectedIndexChanged);
            // 
            // OFDlistarDeSolucion
            // 
            this.OFDlistarDeSolucion.Filter = "Solución .Net|*.sln";
            // 
            // SPCcapas
            // 
            this.SPCcapas.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.SPCcapas.IsSplitterFixed = true;
            this.SPCcapas.Location = new System.Drawing.Point(12, 125);
            this.SPCcapas.Name = "SPCcapas";
            this.SPCcapas.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // SPCcapas.Panel1
            // 
            this.SPCcapas.Panel1.Controls.Add(this.GPBback);
            // 
            // SPCcapas.Panel2
            // 
            this.SPCcapas.Panel2.Controls.Add(this.GPBFront);
            this.SPCcapas.Size = new System.Drawing.Size(1047, 142);
            this.SPCcapas.SplitterDistance = 69;
            this.SPCcapas.TabIndex = 10;
            // 
            // GPBback
            // 
            this.GPBback.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.GPBback.Controls.Add(this.CHKtryOrIf);
            this.GPBback.Controls.Add(this.CHKquitarEsquema);
            this.GPBback.Controls.Add(this.CHKservice);
            this.GPBback.Controls.Add(this.CHKrepositories);
            this.GPBback.Controls.Add(this.CHKmodel);
            this.GPBback.Controls.Add(this.CHKdto);
            this.GPBback.Controls.Add(this.CHKcontrollers);
            this.GPBback.Dock = System.Windows.Forms.DockStyle.Fill;
            this.GPBback.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.GPBback.Location = new System.Drawing.Point(0, 0);
            this.GPBback.Name = "GPBback";
            this.GPBback.Size = new System.Drawing.Size(1047, 69);
            this.GPBback.TabIndex = 0;
            this.GPBback.TabStop = false;
            this.GPBback.Text = "BACK END";
            // 
            // CHKtryOrIf
            // 
            this.CHKtryOrIf.AutoSize = true;
            this.CHKtryOrIf.Checked = true;
            this.CHKtryOrIf.CheckState = System.Windows.Forms.CheckState.Checked;
            this.CHKtryOrIf.Dock = System.Windows.Forms.DockStyle.Left;
            this.CHKtryOrIf.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.CHKtryOrIf.Location = new System.Drawing.Point(617, 19);
            this.CHKtryOrIf.Name = "CHKtryOrIf";
            this.CHKtryOrIf.Size = new System.Drawing.Size(148, 47);
            this.CHKtryOrIf.TabIndex = 6;
            this.CHKtryOrIf.Text = "Usar Try en DB2   ";
            this.CHKtryOrIf.UseVisualStyleBackColor = true;
            // 
            // CHKquitarEsquema
            // 
            this.CHKquitarEsquema.AutoSize = true;
            this.CHKquitarEsquema.Checked = true;
            this.CHKquitarEsquema.CheckState = System.Windows.Forms.CheckState.Checked;
            this.CHKquitarEsquema.Dock = System.Windows.Forms.DockStyle.Left;
            this.CHKquitarEsquema.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.CHKquitarEsquema.Location = new System.Drawing.Point(472, 19);
            this.CHKquitarEsquema.Name = "CHKquitarEsquema";
            this.CHKquitarEsquema.Size = new System.Drawing.Size(145, 47);
            this.CHKquitarEsquema.TabIndex = 5;
            this.CHKquitarEsquema.Text = "Quitar esquema   ";
            this.CHKquitarEsquema.UseVisualStyleBackColor = true;
            this.CHKquitarEsquema.Visible = false;
            // 
            // CHKservice
            // 
            this.CHKservice.AutoSize = true;
            this.CHKservice.Checked = true;
            this.CHKservice.CheckState = System.Windows.Forms.CheckState.Checked;
            this.CHKservice.Dock = System.Windows.Forms.DockStyle.Left;
            this.CHKservice.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.CHKservice.Location = new System.Drawing.Point(381, 19);
            this.CHKservice.Name = "CHKservice";
            this.CHKservice.Size = new System.Drawing.Size(91, 47);
            this.CHKservice.TabIndex = 4;
            this.CHKservice.Text = "Service   ";
            this.CHKservice.UseVisualStyleBackColor = true;
            // 
            // CHKrepositories
            // 
            this.CHKrepositories.AutoSize = true;
            this.CHKrepositories.Checked = true;
            this.CHKrepositories.CheckState = System.Windows.Forms.CheckState.Checked;
            this.CHKrepositories.Dock = System.Windows.Forms.DockStyle.Left;
            this.CHKrepositories.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.CHKrepositories.Location = new System.Drawing.Point(260, 19);
            this.CHKrepositories.Name = "CHKrepositories";
            this.CHKrepositories.Size = new System.Drawing.Size(121, 47);
            this.CHKrepositories.TabIndex = 3;
            this.CHKrepositories.Text = "Repositories   ";
            this.CHKrepositories.UseVisualStyleBackColor = true;
            // 
            // CHKmodel
            // 
            this.CHKmodel.AutoSize = true;
            this.CHKmodel.Checked = true;
            this.CHKmodel.CheckState = System.Windows.Forms.CheckState.Checked;
            this.CHKmodel.Dock = System.Windows.Forms.DockStyle.Left;
            this.CHKmodel.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.CHKmodel.Location = new System.Drawing.Point(180, 19);
            this.CHKmodel.Name = "CHKmodel";
            this.CHKmodel.Size = new System.Drawing.Size(80, 47);
            this.CHKmodel.TabIndex = 2;
            this.CHKmodel.Text = "Model   ";
            this.CHKmodel.UseVisualStyleBackColor = true;
            // 
            // CHKdto
            // 
            this.CHKdto.AutoSize = true;
            this.CHKdto.Checked = true;
            this.CHKdto.CheckState = System.Windows.Forms.CheckState.Checked;
            this.CHKdto.Dock = System.Windows.Forms.DockStyle.Left;
            this.CHKdto.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.CHKdto.Location = new System.Drawing.Point(115, 19);
            this.CHKdto.Name = "CHKdto";
            this.CHKdto.Size = new System.Drawing.Size(65, 47);
            this.CHKdto.TabIndex = 1;
            this.CHKdto.Text = "Dto   ";
            this.CHKdto.UseVisualStyleBackColor = true;
            // 
            // CHKcontrollers
            // 
            this.CHKcontrollers.AutoSize = true;
            this.CHKcontrollers.Checked = true;
            this.CHKcontrollers.CheckState = System.Windows.Forms.CheckState.Checked;
            this.CHKcontrollers.Dock = System.Windows.Forms.DockStyle.Left;
            this.CHKcontrollers.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.CHKcontrollers.Location = new System.Drawing.Point(3, 19);
            this.CHKcontrollers.Name = "CHKcontrollers";
            this.CHKcontrollers.Size = new System.Drawing.Size(112, 47);
            this.CHKcontrollers.TabIndex = 0;
            this.CHKcontrollers.Text = "Controllers   ";
            this.CHKcontrollers.UseVisualStyleBackColor = true;
            // 
            // GPBFront
            // 
            this.GPBFront.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.GPBFront.Controls.Add(this.CHKtypeScript);
            this.GPBFront.Dock = System.Windows.Forms.DockStyle.Fill;
            this.GPBFront.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.GPBFront.Location = new System.Drawing.Point(0, 0);
            this.GPBFront.Name = "GPBFront";
            this.GPBFront.Size = new System.Drawing.Size(1047, 69);
            this.GPBFront.TabIndex = 0;
            this.GPBFront.TabStop = false;
            this.GPBFront.Text = "FRONT END";
            // 
            // CHKtypeScript
            // 
            this.CHKtypeScript.AutoSize = true;
            this.CHKtypeScript.Checked = true;
            this.CHKtypeScript.CheckState = System.Windows.Forms.CheckState.Checked;
            this.CHKtypeScript.Dock = System.Windows.Forms.DockStyle.Left;
            this.CHKtypeScript.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.CHKtypeScript.Location = new System.Drawing.Point(3, 19);
            this.CHKtypeScript.Name = "CHKtypeScript";
            this.CHKtypeScript.Size = new System.Drawing.Size(98, 47);
            this.CHKtypeScript.TabIndex = 0;
            this.CHKtypeScript.Text = "TypeScript";
            this.CHKtypeScript.UseVisualStyleBackColor = true;
            // 
            // BTNgenerarDesdeTabla
            // 
            this.BTNgenerarDesdeTabla.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.BTNgenerarDesdeTabla.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.BTNgenerarDesdeTabla.Location = new System.Drawing.Point(1065, 128);
            this.BTNgenerarDesdeTabla.Name = "BTNgenerarDesdeTabla";
            this.BTNgenerarDesdeTabla.Size = new System.Drawing.Size(97, 139);
            this.BTNgenerarDesdeTabla.TabIndex = 12;
            this.BTNgenerarDesdeTabla.Text = "&Generar";
            this.BTNgenerarDesdeTabla.UseVisualStyleBackColor = true;
            this.BTNgenerarDesdeTabla.Click += new System.EventHandler(this.BTNgenerarDesdeTabla_Click);
            // 
            // SPCseparador
            // 
            this.SPCseparador.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.SPCseparador.Location = new System.Drawing.Point(12, 273);
            this.SPCseparador.Name = "SPCseparador";
            this.SPCseparador.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // SPCseparador.Panel1
            // 
            this.SPCseparador.Panel1.Controls.Add(this.GBPaPartirDeConsulta);
            // 
            // SPCseparador.Panel2
            // 
            this.SPCseparador.Panel2.Controls.Add(this.SPCclase);
            this.SPCseparador.Size = new System.Drawing.Size(1156, 544);
            this.SPCseparador.SplitterDistance = 159;
            this.SPCseparador.TabIndex = 11;
            // 
            // GBPaPartirDeConsulta
            // 
            this.GBPaPartirDeConsulta.Controls.Add(this.BTNgenerarDesdeConsulta);
            this.GBPaPartirDeConsulta.Controls.Add(this.BNTobtenerEstructura);
            this.GBPaPartirDeConsulta.Controls.Add(this.TXTgenerarAPartirDeConsulta);
            this.GBPaPartirDeConsulta.Dock = System.Windows.Forms.DockStyle.Fill;
            this.GBPaPartirDeConsulta.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.GBPaPartirDeConsulta.Location = new System.Drawing.Point(0, 0);
            this.GBPaPartirDeConsulta.Name = "GBPaPartirDeConsulta";
            this.GBPaPartirDeConsulta.Size = new System.Drawing.Size(1156, 159);
            this.GBPaPartirDeConsulta.TabIndex = 12;
            this.GBPaPartirDeConsulta.TabStop = false;
            this.GBPaPartirDeConsulta.Text = "Generar a partir de consulta";
            // 
            // BTNgenerarDesdeConsulta
            // 
            this.BTNgenerarDesdeConsulta.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.BTNgenerarDesdeConsulta.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.BTNgenerarDesdeConsulta.Location = new System.Drawing.Point(1053, 69);
            this.BTNgenerarDesdeConsulta.Name = "BTNgenerarDesdeConsulta";
            this.BTNgenerarDesdeConsulta.Size = new System.Drawing.Size(97, 88);
            this.BTNgenerarDesdeConsulta.TabIndex = 2;
            this.BTNgenerarDesdeConsulta.Text = "Ge&nerar";
            this.BTNgenerarDesdeConsulta.UseVisualStyleBackColor = true;
            this.BTNgenerarDesdeConsulta.Click += new System.EventHandler(this.BTNgenerarDesdeConsulta_Click);
            // 
            // BNTobtenerEstructura
            // 
            this.BNTobtenerEstructura.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.BNTobtenerEstructura.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.BNTobtenerEstructura.Location = new System.Drawing.Point(1053, 19);
            this.BNTobtenerEstructura.Name = "BNTobtenerEstructura";
            this.BNTobtenerEstructura.Size = new System.Drawing.Size(97, 44);
            this.BNTobtenerEstructura.TabIndex = 1;
            this.BNTobtenerEstructura.Text = "&Obtener estructura";
            this.BNTobtenerEstructura.UseVisualStyleBackColor = true;
            this.BNTobtenerEstructura.Click += new System.EventHandler(this.BNTobtenerEstructura_Click);
            // 
            // TXTgenerarAPartirDeConsulta
            // 
            this.TXTgenerarAPartirDeConsulta.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.TXTgenerarAPartirDeConsulta.Location = new System.Drawing.Point(3, 19);
            this.TXTgenerarAPartirDeConsulta.Multiline = true;
            this.TXTgenerarAPartirDeConsulta.Name = "TXTgenerarAPartirDeConsulta";
            this.TXTgenerarAPartirDeConsulta.Size = new System.Drawing.Size(1044, 137);
            this.TXTgenerarAPartirDeConsulta.TabIndex = 0;
            // 
            // SPCclase
            // 
            this.SPCclase.Dock = System.Windows.Forms.DockStyle.Fill;
            this.SPCclase.Location = new System.Drawing.Point(0, 0);
            this.SPCclase.Name = "SPCclase";
            // 
            // SPCclase.Panel1
            // 
            this.SPCclase.Panel1.Controls.Add(this.LBLtablaSeleccionada);
            this.SPCclase.Panel1.Controls.Add(this.LSVcampos);
            // 
            // SPCclase.Panel2
            // 
            this.SPCclase.Panel2.Controls.Add(this.TXTclase);
            this.SPCclase.Size = new System.Drawing.Size(1156, 381);
            this.SPCclase.SplitterDistance = 312;
            this.SPCclase.TabIndex = 13;
            this.SPCclase.SplitterMoved += new System.Windows.Forms.SplitterEventHandler(this.SPCclase_SplitterMoved);
            // 
            // LBLtablaSeleccionada
            // 
            this.LBLtablaSeleccionada.AutoSize = true;
            this.LBLtablaSeleccionada.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LBLtablaSeleccionada.Location = new System.Drawing.Point(8, 8);
            this.LBLtablaSeleccionada.Name = "LBLtablaSeleccionada";
            this.LBLtablaSeleccionada.Size = new System.Drawing.Size(0, 16);
            this.LBLtablaSeleccionada.TabIndex = 13;
            this.LBLtablaSeleccionada.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
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
            this.LSVcampos.Size = new System.Drawing.Size(311, 354);
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
            this.TXTclase.Size = new System.Drawing.Size(840, 381);
            this.TXTclase.TabIndex = 0;
            this.TXTclase.TabStop = false;
            // 
            // FRMgeneradorDeCapas
            // 
            this.ClientSize = new System.Drawing.Size(1186, 829);
            this.Controls.Add(this.SPCseparador);
            this.Controls.Add(this.BTNgenerarDesdeTabla);
            this.Controls.Add(this.SPCcapas);
            this.Controls.Add(this.CMBnamespaces);
            this.Controls.Add(this.BTNbuscarSolucion);
            this.Controls.Add(this.BTNdirectorioCapas);
            this.Controls.Add(this.TXTpathCapas);
            this.Controls.Add(this.LBLdirectorioCapas);
            this.Controls.Add(this.RDBdb2);
            this.Controls.Add(this.RDBsql);
            this.Controls.Add(this.TXTespacioDeNombres);
            this.Controls.Add(this.LBLespacioDeNombres);
            this.Controls.Add(this.CMBbases);
            this.Controls.Add(this.LBLbases);
            this.Controls.Add(this.CMBtablas);
            this.Controls.Add(this.LBLtablas);
            this.Controls.Add(this.CMBservidor);
            this.Controls.Add(this.LBLservidor);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "FRMgeneradorDeCapas";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Capificator";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.Load += new System.EventHandler(this.FRMgeneradorDeCapas_Load);
            this.Resize += new System.EventHandler(this.FRMgeneradorDeCapas_Resize);
            this.SPCcapas.Panel1.ResumeLayout(false);
            this.SPCcapas.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.SPCcapas)).EndInit();
            this.SPCcapas.ResumeLayout(false);
            this.GPBback.ResumeLayout(false);
            this.GPBback.PerformLayout();
            this.GPBFront.ResumeLayout(false);
            this.GPBFront.PerformLayout();
            this.SPCseparador.Panel1.ResumeLayout(false);
            this.SPCseparador.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.SPCseparador)).EndInit();
            this.SPCseparador.ResumeLayout(false);
            this.GBPaPartirDeConsulta.ResumeLayout(false);
            this.GBPaPartirDeConsulta.PerformLayout();
            this.SPCclase.Panel1.ResumeLayout(false);
            this.SPCclase.Panel1.PerformLayout();
            this.SPCclase.Panel2.ResumeLayout(false);
            this.SPCclase.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.SPCclase)).EndInit();
            this.SPCclase.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label LBLservidor;
        private System.Windows.Forms.ComboBox CMBservidor;
        private System.Windows.Forms.ComboBox CMBtablas;
        private System.Windows.Forms.Label LBLtablas;
        private System.Windows.Forms.ComboBox CMBbases;
        private System.Windows.Forms.Label LBLbases;
        private System.Windows.Forms.Label LBLespacioDeNombres;
        private System.Windows.Forms.TextBox TXTespacioDeNombres;
        private System.Windows.Forms.RadioButton RDBsql;
        private System.Windows.Forms.RadioButton RDBdb2;
        private System.Windows.Forms.Label LBLdirectorioCapas;
        private System.Windows.Forms.TextBox TXTpathCapas;
        private System.Windows.Forms.Button BTNdirectorioCapas;
        private System.Windows.Forms.FolderBrowserDialog FBDdirectorioCapas;
        private System.Windows.Forms.Button BTNbuscarSolucion;
        private System.Windows.Forms.ComboBox CMBnamespaces;
        private System.Windows.Forms.OpenFileDialog OFDlistarDeSolucion;
        private System.Windows.Forms.SplitContainer SPCcapas;
        private System.Windows.Forms.GroupBox GPBback;
        private System.Windows.Forms.CheckBox CHKtryOrIf;
        private System.Windows.Forms.CheckBox CHKquitarEsquema;
        private System.Windows.Forms.CheckBox CHKservice;
        private System.Windows.Forms.CheckBox CHKrepositories;
        private System.Windows.Forms.CheckBox CHKmodel;
        private System.Windows.Forms.CheckBox CHKdto;
        private System.Windows.Forms.CheckBox CHKcontrollers;
        private System.Windows.Forms.Button BTNgenerarDesdeTabla;
        private System.Windows.Forms.GroupBox GPBFront;
        private System.Windows.Forms.CheckBox CHKtypeScript;
        private System.Windows.Forms.SplitContainer SPCseparador;
        private System.Windows.Forms.GroupBox GBPaPartirDeConsulta;
        private System.Windows.Forms.TextBox TXTgenerarAPartirDeConsulta;
        private System.Windows.Forms.SplitContainer SPCclase;
        private System.Windows.Forms.Label LBLtablaSeleccionada;
        private System.Windows.Forms.ListView LSVcampos;
        private System.Windows.Forms.TextBox TXTclase;
        private System.Windows.Forms.Button BNTobtenerEstructura;
        private System.Windows.Forms.Button BTNgenerarDesdeConsulta;
    }
}

