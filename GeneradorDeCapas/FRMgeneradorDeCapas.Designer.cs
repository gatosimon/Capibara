
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
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
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
            this.SPCseparador = new System.Windows.Forms.SplitContainer();
            this.GBPaPartirDeConsulta = new System.Windows.Forms.GroupBox();
            this.BNTobtenerEstructura = new System.Windows.Forms.Button();
            this.TXTgenerarAPartirDeConsulta = new System.Windows.Forms.TextBox();
            this.SPCclase = new System.Windows.Forms.SplitContainer();
            this.LBLtablaSeleccionada = new System.Windows.Forms.Label();
            this.LSVcampos = new System.Windows.Forms.ListView();
            this.SPCparametros = new System.Windows.Forms.SplitContainer();
            this.BTNquitarCampo = new System.Windows.Forms.Button();
            this.BTNagregarCampo = new System.Windows.Forms.Button();
            this.LBLcamposABM = new System.Windows.Forms.Label();
            this.TBCcamposABM = new System.Windows.Forms.TabControl();
            this.TBPbaja = new System.Windows.Forms.TabPage();
            this.DGVbaja = new System.Windows.Forms.DataGridView();
            this.CampoBaja = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.VinculoBaja = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.TBPmodificacion = new System.Windows.Forms.TabPage();
            this.DGVmodificacion = new System.Windows.Forms.DataGridView();
            this.CampoModificacion = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.VinculoModificacion = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.TBPrecuperacion = new System.Windows.Forms.TabPage();
            this.DGVrecuperacion = new System.Windows.Forms.DataGridView();
            this.dataGridViewTextBoxColumn1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewComboBoxColumn1 = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.LBLclasesGeneradas = new System.Windows.Forms.Label();
            this.TXTclase = new System.Windows.Forms.TextBox();
            this.TBCbackFront = new System.Windows.Forms.TabControl();
            this.TBPback = new System.Windows.Forms.TabPage();
            this.SPCback1 = new System.Windows.Forms.SplitContainer();
            this.GPBback = new System.Windows.Forms.GroupBox();
            this.CHKservice = new System.Windows.Forms.CheckBox();
            this.CHKrepositories = new System.Windows.Forms.CheckBox();
            this.CHKmodel = new System.Windows.Forms.CheckBox();
            this.CHKdto = new System.Windows.Forms.CheckBox();
            this.CHKcontrollers = new System.Windows.Forms.CheckBox();
            this.SPCbak2 = new System.Windows.Forms.SplitContainer();
            this.GPBajustes = new System.Windows.Forms.GroupBox();
            this.CHKtryOrIf = new System.Windows.Forms.CheckBox();
            this.CHKquitarEsquema = new System.Windows.Forms.CheckBox();
            this.GPBmetodos = new System.Windows.Forms.GroupBox();
            this.CHKrecuperacion = new System.Windows.Forms.CheckBox();
            this.CHKtodos = new System.Windows.Forms.CheckBox();
            this.CHKobtenerPorId = new System.Windows.Forms.CheckBox();
            this.CHKmodificacion = new System.Windows.Forms.CheckBox();
            this.CHKbaja = new System.Windows.Forms.CheckBox();
            this.CHKalta = new System.Windows.Forms.CheckBox();
            this.TBPfront = new System.Windows.Forms.TabPage();
            this.GPBFront = new System.Windows.Forms.GroupBox();
            this.CHKtypeScript = new System.Windows.Forms.CheckBox();
            this.TTPusarTryOrIf = new System.Windows.Forms.ToolTip(this.components);
            this.BTNgenerarDesdeTabla = new GeneradorDeCapas.SplitButton();
            ((System.ComponentModel.ISupportInitialize)(this.SPCseparador)).BeginInit();
            this.SPCseparador.Panel1.SuspendLayout();
            this.SPCseparador.Panel2.SuspendLayout();
            this.SPCseparador.SuspendLayout();
            this.GBPaPartirDeConsulta.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.SPCclase)).BeginInit();
            this.SPCclase.Panel1.SuspendLayout();
            this.SPCclase.Panel2.SuspendLayout();
            this.SPCclase.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.SPCparametros)).BeginInit();
            this.SPCparametros.Panel1.SuspendLayout();
            this.SPCparametros.Panel2.SuspendLayout();
            this.SPCparametros.SuspendLayout();
            this.TBCcamposABM.SuspendLayout();
            this.TBPbaja.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.DGVbaja)).BeginInit();
            this.TBPmodificacion.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.DGVmodificacion)).BeginInit();
            this.TBPrecuperacion.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.DGVrecuperacion)).BeginInit();
            this.TBCbackFront.SuspendLayout();
            this.TBPback.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.SPCback1)).BeginInit();
            this.SPCback1.Panel1.SuspendLayout();
            this.SPCback1.Panel2.SuspendLayout();
            this.SPCback1.SuspendLayout();
            this.GPBback.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.SPCbak2)).BeginInit();
            this.SPCbak2.Panel1.SuspendLayout();
            this.SPCbak2.Panel2.SuspendLayout();
            this.SPCbak2.SuspendLayout();
            this.GPBajustes.SuspendLayout();
            this.GPBmetodos.SuspendLayout();
            this.TBPfront.SuspendLayout();
            this.GPBFront.SuspendLayout();
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
            this.CMBtablas.KeyDown += new System.Windows.Forms.KeyEventHandler(this.CMBtablas_KeyDown);
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
            this.SPCseparador.Size = new System.Drawing.Size(1156, 464);
            this.SPCseparador.SplitterDistance = 135;
            this.SPCseparador.TabIndex = 11;
            this.SPCseparador.TabStop = false;
            // 
            // GBPaPartirDeConsulta
            // 
            this.GBPaPartirDeConsulta.Controls.Add(this.BNTobtenerEstructura);
            this.GBPaPartirDeConsulta.Controls.Add(this.TXTgenerarAPartirDeConsulta);
            this.GBPaPartirDeConsulta.Dock = System.Windows.Forms.DockStyle.Fill;
            this.GBPaPartirDeConsulta.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.GBPaPartirDeConsulta.Location = new System.Drawing.Point(0, 0);
            this.GBPaPartirDeConsulta.Name = "GBPaPartirDeConsulta";
            this.GBPaPartirDeConsulta.Size = new System.Drawing.Size(1156, 135);
            this.GBPaPartirDeConsulta.TabIndex = 0;
            this.GBPaPartirDeConsulta.TabStop = false;
            this.GBPaPartirDeConsulta.Text = "GENERAR A PARITR DE CONSULTA";
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
            this.TXTgenerarAPartirDeConsulta.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.TXTgenerarAPartirDeConsulta.Location = new System.Drawing.Point(3, 19);
            this.TXTgenerarAPartirDeConsulta.Multiline = true;
            this.TXTgenerarAPartirDeConsulta.Name = "TXTgenerarAPartirDeConsulta";
            this.TXTgenerarAPartirDeConsulta.Size = new System.Drawing.Size(1044, 113);
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
            this.SPCclase.Panel2.Controls.Add(this.SPCparametros);
            this.SPCclase.Size = new System.Drawing.Size(1156, 325);
            this.SPCclase.SplitterDistance = 312;
            this.SPCclase.TabIndex = 13;
            this.SPCclase.TabStop = false;
            this.SPCclase.SplitterMoved += new System.Windows.Forms.SplitterEventHandler(this.SPCclase_SplitterMoved);
            // 
            // LBLtablaSeleccionada
            // 
            this.LBLtablaSeleccionada.Dock = System.Windows.Forms.DockStyle.Top;
            this.LBLtablaSeleccionada.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LBLtablaSeleccionada.Location = new System.Drawing.Point(0, 0);
            this.LBLtablaSeleccionada.Name = "LBLtablaSeleccionada";
            this.LBLtablaSeleccionada.Size = new System.Drawing.Size(312, 20);
            this.LBLtablaSeleccionada.TabIndex = 0;
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
            this.LSVcampos.Size = new System.Drawing.Size(311, 298);
            this.LSVcampos.TabIndex = 1;
            this.LSVcampos.UseCompatibleStateImageBehavior = false;
            // 
            // SPCparametros
            // 
            this.SPCparametros.Dock = System.Windows.Forms.DockStyle.Fill;
            this.SPCparametros.Location = new System.Drawing.Point(0, 0);
            this.SPCparametros.Name = "SPCparametros";
            // 
            // SPCparametros.Panel1
            // 
            this.SPCparametros.Panel1.Controls.Add(this.BTNquitarCampo);
            this.SPCparametros.Panel1.Controls.Add(this.BTNagregarCampo);
            this.SPCparametros.Panel1.Controls.Add(this.LBLcamposABM);
            this.SPCparametros.Panel1.Controls.Add(this.TBCcamposABM);
            // 
            // SPCparametros.Panel2
            // 
            this.SPCparametros.Panel2.Controls.Add(this.LBLclasesGeneradas);
            this.SPCparametros.Panel2.Controls.Add(this.TXTclase);
            this.SPCparametros.Size = new System.Drawing.Size(840, 325);
            this.SPCparametros.SplitterDistance = 374;
            this.SPCparametros.TabIndex = 1;
            this.SPCparametros.TabStop = false;
            // 
            // BTNquitarCampo
            // 
            this.BTNquitarCampo.Font = new System.Drawing.Font("Verdana", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.BTNquitarCampo.Location = new System.Drawing.Point(6, 215);
            this.BTNquitarCampo.Name = "BTNquitarCampo";
            this.BTNquitarCampo.Size = new System.Drawing.Size(55, 55);
            this.BTNquitarCampo.TabIndex = 2;
            this.BTNquitarCampo.Text = "-";
            this.BTNquitarCampo.UseVisualStyleBackColor = true;
            this.BTNquitarCampo.Click += new System.EventHandler(this.BTNquitarCampo_Click);
            // 
            // BTNagregarCampo
            // 
            this.BTNagregarCampo.Font = new System.Drawing.Font("Verdana", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.BTNagregarCampo.Location = new System.Drawing.Point(8, 144);
            this.BTNagregarCampo.Name = "BTNagregarCampo";
            this.BTNagregarCampo.Size = new System.Drawing.Size(55, 55);
            this.BTNagregarCampo.TabIndex = 1;
            this.BTNagregarCampo.Text = "+";
            this.BTNagregarCampo.UseVisualStyleBackColor = true;
            this.BTNagregarCampo.Click += new System.EventHandler(this.BTNagregarCampo_Click);
            // 
            // LBLcamposABM
            // 
            this.LBLcamposABM.Dock = System.Windows.Forms.DockStyle.Top;
            this.LBLcamposABM.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LBLcamposABM.Location = new System.Drawing.Point(0, 0);
            this.LBLcamposABM.Name = "LBLcamposABM";
            this.LBLcamposABM.Size = new System.Drawing.Size(374, 20);
            this.LBLcamposABM.TabIndex = 0;
            this.LBLcamposABM.Text = "CAMPOS ABM";
            this.LBLcamposABM.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // TBCcamposABM
            // 
            this.TBCcamposABM.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.TBCcamposABM.Controls.Add(this.TBPbaja);
            this.TBCcamposABM.Controls.Add(this.TBPmodificacion);
            this.TBCcamposABM.Controls.Add(this.TBPrecuperacion);
            this.TBCcamposABM.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.TBCcamposABM.Location = new System.Drawing.Point(65, 27);
            this.TBCcamposABM.Name = "TBCcamposABM";
            this.TBCcamposABM.SelectedIndex = 0;
            this.TBCcamposABM.Size = new System.Drawing.Size(306, 298);
            this.TBCcamposABM.TabIndex = 3;
            // 
            // TBPbaja
            // 
            this.TBPbaja.Controls.Add(this.DGVbaja);
            this.TBPbaja.Location = new System.Drawing.Point(4, 25);
            this.TBPbaja.Name = "TBPbaja";
            this.TBPbaja.Padding = new System.Windows.Forms.Padding(3);
            this.TBPbaja.Size = new System.Drawing.Size(298, 269);
            this.TBPbaja.TabIndex = 1;
            this.TBPbaja.Text = "  BAJA  ";
            this.TBPbaja.UseVisualStyleBackColor = true;
            // 
            // DGVbaja
            // 
            this.DGVbaja.AllowUserToAddRows = false;
            this.DGVbaja.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.DGVbaja.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.CampoBaja,
            this.VinculoBaja});
            this.DGVbaja.Dock = System.Windows.Forms.DockStyle.Fill;
            this.DGVbaja.Location = new System.Drawing.Point(3, 3);
            this.DGVbaja.Name = "DGVbaja";
            this.DGVbaja.RowHeadersVisible = false;
            this.DGVbaja.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.DGVbaja.Size = new System.Drawing.Size(292, 263);
            this.DGVbaja.TabIndex = 0;
            // 
            // CampoBaja
            // 
            this.CampoBaja.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            this.CampoBaja.DefaultCellStyle = dataGridViewCellStyle1;
            this.CampoBaja.HeaderText = "CAMPO";
            this.CampoBaja.Name = "CampoBaja";
            // 
            // VinculoBaja
            // 
            this.VinculoBaja.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            this.VinculoBaja.DefaultCellStyle = dataGridViewCellStyle2;
            this.VinculoBaja.HeaderText = "VINCULAR";
            this.VinculoBaja.Name = "VinculoBaja";
            // 
            // TBPmodificacion
            // 
            this.TBPmodificacion.Controls.Add(this.DGVmodificacion);
            this.TBPmodificacion.Location = new System.Drawing.Point(4, 25);
            this.TBPmodificacion.Name = "TBPmodificacion";
            this.TBPmodificacion.Padding = new System.Windows.Forms.Padding(3);
            this.TBPmodificacion.Size = new System.Drawing.Size(298, 269);
            this.TBPmodificacion.TabIndex = 2;
            this.TBPmodificacion.Text = "  MODIFICACION  ";
            this.TBPmodificacion.UseVisualStyleBackColor = true;
            // 
            // DGVmodificacion
            // 
            this.DGVmodificacion.AllowUserToAddRows = false;
            this.DGVmodificacion.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.DGVmodificacion.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.CampoModificacion,
            this.VinculoModificacion});
            this.DGVmodificacion.Dock = System.Windows.Forms.DockStyle.Fill;
            this.DGVmodificacion.Location = new System.Drawing.Point(3, 3);
            this.DGVmodificacion.Name = "DGVmodificacion";
            this.DGVmodificacion.RowHeadersVisible = false;
            this.DGVmodificacion.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.DGVmodificacion.Size = new System.Drawing.Size(292, 263);
            this.DGVmodificacion.TabIndex = 1;
            // 
            // CampoModificacion
            // 
            this.CampoModificacion.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.CampoModificacion.HeaderText = "CAMPO";
            this.CampoModificacion.Name = "CampoModificacion";
            // 
            // VinculoModificacion
            // 
            this.VinculoModificacion.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.VinculoModificacion.HeaderText = "VINCULAR";
            this.VinculoModificacion.Name = "VinculoModificacion";
            // 
            // TBPrecuperacion
            // 
            this.TBPrecuperacion.Controls.Add(this.DGVrecuperacion);
            this.TBPrecuperacion.Location = new System.Drawing.Point(4, 25);
            this.TBPrecuperacion.Name = "TBPrecuperacion";
            this.TBPrecuperacion.Padding = new System.Windows.Forms.Padding(3);
            this.TBPrecuperacion.Size = new System.Drawing.Size(298, 269);
            this.TBPrecuperacion.TabIndex = 3;
            this.TBPrecuperacion.Text = "  RECUPERACION  ";
            this.TBPrecuperacion.UseVisualStyleBackColor = true;
            // 
            // DGVrecuperacion
            // 
            this.DGVrecuperacion.AllowUserToAddRows = false;
            this.DGVrecuperacion.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.DGVrecuperacion.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.dataGridViewTextBoxColumn1,
            this.dataGridViewComboBoxColumn1});
            this.DGVrecuperacion.Dock = System.Windows.Forms.DockStyle.Fill;
            this.DGVrecuperacion.Location = new System.Drawing.Point(3, 3);
            this.DGVrecuperacion.Name = "DGVrecuperacion";
            this.DGVrecuperacion.RowHeadersVisible = false;
            this.DGVrecuperacion.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.DGVrecuperacion.Size = new System.Drawing.Size(292, 263);
            this.DGVrecuperacion.TabIndex = 1;
            // 
            // dataGridViewTextBoxColumn1
            // 
            this.dataGridViewTextBoxColumn1.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.dataGridViewTextBoxColumn1.HeaderText = "CAMPO";
            this.dataGridViewTextBoxColumn1.Name = "dataGridViewTextBoxColumn1";
            // 
            // dataGridViewComboBoxColumn1
            // 
            this.dataGridViewComboBoxColumn1.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.dataGridViewComboBoxColumn1.HeaderText = "VINCULAR";
            this.dataGridViewComboBoxColumn1.Name = "dataGridViewComboBoxColumn1";
            // 
            // LBLclasesGeneradas
            // 
            this.LBLclasesGeneradas.Dock = System.Windows.Forms.DockStyle.Top;
            this.LBLclasesGeneradas.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LBLclasesGeneradas.Location = new System.Drawing.Point(0, 0);
            this.LBLclasesGeneradas.Margin = new System.Windows.Forms.Padding(3, 0, 3, 5);
            this.LBLclasesGeneradas.Name = "LBLclasesGeneradas";
            this.LBLclasesGeneradas.Size = new System.Drawing.Size(462, 20);
            this.LBLclasesGeneradas.TabIndex = 0;
            this.LBLclasesGeneradas.Text = "CLASES GENERADAS";
            this.LBLclasesGeneradas.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // TXTclase
            // 
            this.TXTclase.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.TXTclase.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.TXTclase.Location = new System.Drawing.Point(0, 27);
            this.TXTclase.Multiline = true;
            this.TXTclase.Name = "TXTclase";
            this.TXTclase.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.TXTclase.Size = new System.Drawing.Size(462, 305);
            this.TXTclase.TabIndex = 1;
            // 
            // TBCbackFront
            // 
            this.TBCbackFront.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.TBCbackFront.Controls.Add(this.TBPback);
            this.TBCbackFront.Controls.Add(this.TBPfront);
            this.TBCbackFront.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.TBCbackFront.Location = new System.Drawing.Point(12, 125);
            this.TBCbackFront.Name = "TBCbackFront";
            this.TBCbackFront.SelectedIndex = 0;
            this.TBCbackFront.Size = new System.Drawing.Size(1047, 142);
            this.TBCbackFront.TabIndex = 10;
            // 
            // TBPback
            // 
            this.TBPback.Controls.Add(this.SPCback1);
            this.TBPback.Location = new System.Drawing.Point(4, 25);
            this.TBPback.Name = "TBPback";
            this.TBPback.Padding = new System.Windows.Forms.Padding(3);
            this.TBPback.Size = new System.Drawing.Size(1039, 113);
            this.TBPback.TabIndex = 0;
            this.TBPback.Text = "  BACK  ";
            this.TBPback.UseVisualStyleBackColor = true;
            // 
            // SPCback1
            // 
            this.SPCback1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.SPCback1.IsSplitterFixed = true;
            this.SPCback1.Location = new System.Drawing.Point(3, 3);
            this.SPCback1.Name = "SPCback1";
            // 
            // SPCback1.Panel1
            // 
            this.SPCback1.Panel1.Controls.Add(this.GPBback);
            // 
            // SPCback1.Panel2
            // 
            this.SPCback1.Panel2.Controls.Add(this.SPCbak2);
            this.SPCback1.Size = new System.Drawing.Size(1033, 107);
            this.SPCback1.SplitterDistance = 259;
            this.SPCback1.TabIndex = 4;
            // 
            // GPBback
            // 
            this.GPBback.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.GPBback.Controls.Add(this.CHKservice);
            this.GPBback.Controls.Add(this.CHKrepositories);
            this.GPBback.Controls.Add(this.CHKmodel);
            this.GPBback.Controls.Add(this.CHKdto);
            this.GPBback.Controls.Add(this.CHKcontrollers);
            this.GPBback.Dock = System.Windows.Forms.DockStyle.Fill;
            this.GPBback.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.GPBback.Location = new System.Drawing.Point(0, 0);
            this.GPBback.Name = "GPBback";
            this.GPBback.Size = new System.Drawing.Size(259, 107);
            this.GPBback.TabIndex = 1;
            this.GPBback.TabStop = false;
            this.GPBback.Text = "CAPAS A GENERAR";
            // 
            // CHKservice
            // 
            this.CHKservice.AutoSize = true;
            this.CHKservice.Checked = true;
            this.CHKservice.CheckState = System.Windows.Forms.CheckState.Checked;
            this.CHKservice.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.CHKservice.Location = new System.Drawing.Point(132, 51);
            this.CHKservice.Name = "CHKservice";
            this.CHKservice.Size = new System.Drawing.Size(91, 20);
            this.CHKservice.TabIndex = 4;
            this.CHKservice.Text = "Service   ";
            this.CHKservice.UseVisualStyleBackColor = true;
            // 
            // CHKrepositories
            // 
            this.CHKrepositories.AutoSize = true;
            this.CHKrepositories.Checked = true;
            this.CHKrepositories.CheckState = System.Windows.Forms.CheckState.Checked;
            this.CHKrepositories.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.CHKrepositories.Location = new System.Drawing.Point(132, 23);
            this.CHKrepositories.Name = "CHKrepositories";
            this.CHKrepositories.Size = new System.Drawing.Size(121, 20);
            this.CHKrepositories.TabIndex = 3;
            this.CHKrepositories.Text = "Repositories   ";
            this.CHKrepositories.UseVisualStyleBackColor = true;
            // 
            // CHKmodel
            // 
            this.CHKmodel.AutoSize = true;
            this.CHKmodel.Checked = true;
            this.CHKmodel.CheckState = System.Windows.Forms.CheckState.Checked;
            this.CHKmodel.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.CHKmodel.Location = new System.Drawing.Point(3, 79);
            this.CHKmodel.Name = "CHKmodel";
            this.CHKmodel.Size = new System.Drawing.Size(80, 20);
            this.CHKmodel.TabIndex = 2;
            this.CHKmodel.Text = "Model   ";
            this.CHKmodel.UseVisualStyleBackColor = true;
            // 
            // CHKdto
            // 
            this.CHKdto.AutoSize = true;
            this.CHKdto.Checked = true;
            this.CHKdto.CheckState = System.Windows.Forms.CheckState.Checked;
            this.CHKdto.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.CHKdto.Location = new System.Drawing.Point(3, 51);
            this.CHKdto.Name = "CHKdto";
            this.CHKdto.Size = new System.Drawing.Size(65, 20);
            this.CHKdto.TabIndex = 1;
            this.CHKdto.Text = "Dto   ";
            this.CHKdto.UseVisualStyleBackColor = true;
            // 
            // CHKcontrollers
            // 
            this.CHKcontrollers.AutoSize = true;
            this.CHKcontrollers.Checked = true;
            this.CHKcontrollers.CheckState = System.Windows.Forms.CheckState.Checked;
            this.CHKcontrollers.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.CHKcontrollers.Location = new System.Drawing.Point(3, 23);
            this.CHKcontrollers.Name = "CHKcontrollers";
            this.CHKcontrollers.Size = new System.Drawing.Size(112, 20);
            this.CHKcontrollers.TabIndex = 0;
            this.CHKcontrollers.Text = "Controllers   ";
            this.CHKcontrollers.UseVisualStyleBackColor = true;
            // 
            // SPCbak2
            // 
            this.SPCbak2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.SPCbak2.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.SPCbak2.IsSplitterFixed = true;
            this.SPCbak2.Location = new System.Drawing.Point(0, 0);
            this.SPCbak2.Name = "SPCbak2";
            // 
            // SPCbak2.Panel1
            // 
            this.SPCbak2.Panel1.Controls.Add(this.GPBajustes);
            // 
            // SPCbak2.Panel2
            // 
            this.SPCbak2.Panel2.Controls.Add(this.GPBmetodos);
            this.SPCbak2.Size = new System.Drawing.Size(770, 107);
            this.SPCbak2.SplitterDistance = 170;
            this.SPCbak2.TabIndex = 4;
            // 
            // GPBajustes
            // 
            this.GPBajustes.Controls.Add(this.CHKtryOrIf);
            this.GPBajustes.Controls.Add(this.CHKquitarEsquema);
            this.GPBajustes.Dock = System.Windows.Forms.DockStyle.Fill;
            this.GPBajustes.Location = new System.Drawing.Point(0, 0);
            this.GPBajustes.Margin = new System.Windows.Forms.Padding(20, 3, 3, 3);
            this.GPBajustes.Name = "GPBajustes";
            this.GPBajustes.Size = new System.Drawing.Size(170, 107);
            this.GPBajustes.TabIndex = 2;
            this.GPBajustes.TabStop = false;
            this.GPBajustes.Text = "AJUSTES";
            // 
            // CHKtryOrIf
            // 
            this.CHKtryOrIf.AutoSize = true;
            this.CHKtryOrIf.Checked = true;
            this.CHKtryOrIf.CheckState = System.Windows.Forms.CheckState.Checked;
            this.CHKtryOrIf.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.CHKtryOrIf.Location = new System.Drawing.Point(8, 23);
            this.CHKtryOrIf.Name = "CHKtryOrIf";
            this.CHKtryOrIf.Size = new System.Drawing.Size(148, 20);
            this.CHKtryOrIf.TabIndex = 1;
            this.CHKtryOrIf.Text = "Usar Try en DB2   ";
            this.TTPusarTryOrIf.SetToolTip(this.CHKtryOrIf, "TRY/CATCH para usar la capa de datos actual de SistemaMunicipalGeneral. IF para u" +
        "sar una capa futura con correcciones y mejoras sobre las consultas a la base de " +
        "datos");
            this.CHKtryOrIf.UseVisualStyleBackColor = true;
            // 
            // CHKquitarEsquema
            // 
            this.CHKquitarEsquema.AutoSize = true;
            this.CHKquitarEsquema.Checked = true;
            this.CHKquitarEsquema.CheckState = System.Windows.Forms.CheckState.Checked;
            this.CHKquitarEsquema.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.CHKquitarEsquema.Location = new System.Drawing.Point(8, 51);
            this.CHKquitarEsquema.Name = "CHKquitarEsquema";
            this.CHKquitarEsquema.Size = new System.Drawing.Size(145, 20);
            this.CHKquitarEsquema.TabIndex = 0;
            this.CHKquitarEsquema.Text = "Quitar esquema   ";
            this.CHKquitarEsquema.UseVisualStyleBackColor = true;
            this.CHKquitarEsquema.Visible = false;
            // 
            // GPBmetodos
            // 
            this.GPBmetodos.Controls.Add(this.CHKrecuperacion);
            this.GPBmetodos.Controls.Add(this.CHKtodos);
            this.GPBmetodos.Controls.Add(this.CHKobtenerPorId);
            this.GPBmetodos.Controls.Add(this.CHKmodificacion);
            this.GPBmetodos.Controls.Add(this.CHKbaja);
            this.GPBmetodos.Controls.Add(this.CHKalta);
            this.GPBmetodos.Dock = System.Windows.Forms.DockStyle.Fill;
            this.GPBmetodos.Location = new System.Drawing.Point(0, 0);
            this.GPBmetodos.Margin = new System.Windows.Forms.Padding(20, 3, 3, 3);
            this.GPBmetodos.Name = "GPBmetodos";
            this.GPBmetodos.Size = new System.Drawing.Size(596, 107);
            this.GPBmetodos.TabIndex = 3;
            this.GPBmetodos.TabStop = false;
            this.GPBmetodos.Text = "METODOS";
            // 
            // CHKrecuperacion
            // 
            this.CHKrecuperacion.AutoSize = true;
            this.CHKrecuperacion.Checked = true;
            this.CHKrecuperacion.CheckState = System.Windows.Forms.CheckState.Checked;
            this.CHKrecuperacion.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.CHKrecuperacion.Location = new System.Drawing.Point(147, 79);
            this.CHKrecuperacion.Name = "CHKrecuperacion";
            this.CHKrecuperacion.Size = new System.Drawing.Size(115, 20);
            this.CHKrecuperacion.TabIndex = 5;
            this.CHKrecuperacion.Text = "Recuperación";
            this.CHKrecuperacion.UseVisualStyleBackColor = true;
            // 
            // CHKtodos
            // 
            this.CHKtodos.AutoSize = true;
            this.CHKtodos.Checked = true;
            this.CHKtodos.CheckState = System.Windows.Forms.CheckState.Checked;
            this.CHKtodos.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.CHKtodos.Location = new System.Drawing.Point(147, 51);
            this.CHKtodos.Name = "CHKtodos";
            this.CHKtodos.Size = new System.Drawing.Size(122, 20);
            this.CHKtodos.TabIndex = 4;
            this.CHKtodos.Text = "Obtener todos";
            this.CHKtodos.UseVisualStyleBackColor = true;
            // 
            // CHKobtenerPorId
            // 
            this.CHKobtenerPorId.AutoSize = true;
            this.CHKobtenerPorId.Checked = true;
            this.CHKobtenerPorId.CheckState = System.Windows.Forms.CheckState.Checked;
            this.CHKobtenerPorId.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.CHKobtenerPorId.Location = new System.Drawing.Point(147, 23);
            this.CHKobtenerPorId.Name = "CHKobtenerPorId";
            this.CHKobtenerPorId.Size = new System.Drawing.Size(125, 20);
            this.CHKobtenerPorId.TabIndex = 3;
            this.CHKobtenerPorId.Text = "Obtener por ID";
            this.CHKobtenerPorId.UseVisualStyleBackColor = true;
            // 
            // CHKmodificacion
            // 
            this.CHKmodificacion.AutoSize = true;
            this.CHKmodificacion.Checked = true;
            this.CHKmodificacion.CheckState = System.Windows.Forms.CheckState.Checked;
            this.CHKmodificacion.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.CHKmodificacion.Location = new System.Drawing.Point(8, 79);
            this.CHKmodificacion.Name = "CHKmodificacion";
            this.CHKmodificacion.Size = new System.Drawing.Size(108, 20);
            this.CHKmodificacion.TabIndex = 2;
            this.CHKmodificacion.Text = "Modificación";
            this.CHKmodificacion.UseVisualStyleBackColor = true;
            // 
            // CHKbaja
            // 
            this.CHKbaja.AutoSize = true;
            this.CHKbaja.Checked = true;
            this.CHKbaja.CheckState = System.Windows.Forms.CheckState.Checked;
            this.CHKbaja.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.CHKbaja.Location = new System.Drawing.Point(8, 51);
            this.CHKbaja.Name = "CHKbaja";
            this.CHKbaja.Size = new System.Drawing.Size(55, 20);
            this.CHKbaja.TabIndex = 1;
            this.CHKbaja.Text = "Baja";
            this.CHKbaja.UseVisualStyleBackColor = true;
            // 
            // CHKalta
            // 
            this.CHKalta.AutoSize = true;
            this.CHKalta.Checked = true;
            this.CHKalta.CheckState = System.Windows.Forms.CheckState.Checked;
            this.CHKalta.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.CHKalta.Location = new System.Drawing.Point(8, 23);
            this.CHKalta.Name = "CHKalta";
            this.CHKalta.Size = new System.Drawing.Size(53, 20);
            this.CHKalta.TabIndex = 0;
            this.CHKalta.Text = "Alta";
            this.CHKalta.UseVisualStyleBackColor = true;
            // 
            // TBPfront
            // 
            this.TBPfront.Controls.Add(this.GPBFront);
            this.TBPfront.Location = new System.Drawing.Point(4, 25);
            this.TBPfront.Name = "TBPfront";
            this.TBPfront.Padding = new System.Windows.Forms.Padding(3);
            this.TBPfront.Size = new System.Drawing.Size(1039, 113);
            this.TBPfront.TabIndex = 1;
            this.TBPfront.Text = "  FRONT  ";
            this.TBPfront.UseVisualStyleBackColor = true;
            // 
            // GPBFront
            // 
            this.GPBFront.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.GPBFront.Controls.Add(this.CHKtypeScript);
            this.GPBFront.Dock = System.Windows.Forms.DockStyle.Fill;
            this.GPBFront.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.GPBFront.Location = new System.Drawing.Point(3, 3);
            this.GPBFront.Name = "GPBFront";
            this.GPBFront.Size = new System.Drawing.Size(1033, 107);
            this.GPBFront.TabIndex = 1;
            this.GPBFront.TabStop = false;
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
            this.CHKtypeScript.Size = new System.Drawing.Size(98, 85);
            this.CHKtypeScript.TabIndex = 0;
            this.CHKtypeScript.Text = "TypeScript";
            this.CHKtypeScript.UseVisualStyleBackColor = true;
            // 
            // TTPusarTryOrIf
            // 
            this.TTPusarTryOrIf.ToolTipIcon = System.Windows.Forms.ToolTipIcon.Info;
            this.TTPusarTryOrIf.ToolTipTitle = "Información";
            // 
            // BTNgenerarDesdeTabla
            // 
            this.BTNgenerarDesdeTabla.AlwaysDropDown = true;
            this.BTNgenerarDesdeTabla.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.BTNgenerarDesdeTabla.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.BTNgenerarDesdeTabla.Image = global::GeneradorDeCapas.Properties.Resources.Capibara50x50;
            this.BTNgenerarDesdeTabla.Location = new System.Drawing.Point(1065, 150);
            this.BTNgenerarDesdeTabla.Menu = null;
            this.BTNgenerarDesdeTabla.Name = "BTNgenerarDesdeTabla";
            this.BTNgenerarDesdeTabla.Size = new System.Drawing.Size(97, 113);
            this.BTNgenerarDesdeTabla.TabIndex = 12;
            this.BTNgenerarDesdeTabla.Text = "&Capibarar";
            this.BTNgenerarDesdeTabla.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.BTNgenerarDesdeTabla.UseVisualStyleBackColor = true;
            // 
            // FRMgeneradorDeCapas
            // 
            this.ClientSize = new System.Drawing.Size(1186, 749);
            this.Controls.Add(this.TBCbackFront);
            this.Controls.Add(this.SPCseparador);
            this.Controls.Add(this.BTNgenerarDesdeTabla);
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
            this.Text = "CAPIBARA";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.Load += new System.EventHandler(this.FRMgeneradorDeCapas_Load);
            this.Resize += new System.EventHandler(this.FRMgeneradorDeCapas_Resize);
            this.SPCseparador.Panel1.ResumeLayout(false);
            this.SPCseparador.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.SPCseparador)).EndInit();
            this.SPCseparador.ResumeLayout(false);
            this.GBPaPartirDeConsulta.ResumeLayout(false);
            this.GBPaPartirDeConsulta.PerformLayout();
            this.SPCclase.Panel1.ResumeLayout(false);
            this.SPCclase.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.SPCclase)).EndInit();
            this.SPCclase.ResumeLayout(false);
            this.SPCparametros.Panel1.ResumeLayout(false);
            this.SPCparametros.Panel2.ResumeLayout(false);
            this.SPCparametros.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.SPCparametros)).EndInit();
            this.SPCparametros.ResumeLayout(false);
            this.TBCcamposABM.ResumeLayout(false);
            this.TBPbaja.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.DGVbaja)).EndInit();
            this.TBPmodificacion.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.DGVmodificacion)).EndInit();
            this.TBPrecuperacion.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.DGVrecuperacion)).EndInit();
            this.TBCbackFront.ResumeLayout(false);
            this.TBPback.ResumeLayout(false);
            this.SPCback1.Panel1.ResumeLayout(false);
            this.SPCback1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.SPCback1)).EndInit();
            this.SPCback1.ResumeLayout(false);
            this.GPBback.ResumeLayout(false);
            this.GPBback.PerformLayout();
            this.SPCbak2.Panel1.ResumeLayout(false);
            this.SPCbak2.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.SPCbak2)).EndInit();
            this.SPCbak2.ResumeLayout(false);
            this.GPBajustes.ResumeLayout(false);
            this.GPBajustes.PerformLayout();
            this.GPBmetodos.ResumeLayout(false);
            this.GPBmetodos.PerformLayout();
            this.TBPfront.ResumeLayout(false);
            this.GPBFront.ResumeLayout(false);
            this.GPBFront.PerformLayout();
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
        private GeneradorDeCapas.SplitButton BTNgenerarDesdeTabla;
        private System.Windows.Forms.SplitContainer SPCseparador;
        private System.Windows.Forms.GroupBox GBPaPartirDeConsulta;
        private System.Windows.Forms.TextBox TXTgenerarAPartirDeConsulta;
        private System.Windows.Forms.SplitContainer SPCclase;
        private System.Windows.Forms.Label LBLtablaSeleccionada;
        private System.Windows.Forms.ListView LSVcampos;
        private System.Windows.Forms.Button BNTobtenerEstructura;
        private System.Windows.Forms.SplitContainer SPCparametros;
        private System.Windows.Forms.TextBox TXTclase;
        private System.Windows.Forms.Label LBLclasesGeneradas;
        private System.Windows.Forms.Button BTNquitarCampo;
        private System.Windows.Forms.Button BTNagregarCampo;
        private System.Windows.Forms.Label LBLcamposABM;
        private System.Windows.Forms.TabControl TBCcamposABM;
        private System.Windows.Forms.TabPage TBPmodificacion;
        private System.Windows.Forms.TabPage TBPbaja;
        private System.Windows.Forms.DataGridView DGVbaja;
        private System.Windows.Forms.DataGridView DGVmodificacion;
        private System.Windows.Forms.DataGridViewTextBoxColumn CampoModificacion;
        private System.Windows.Forms.DataGridViewComboBoxColumn VinculoModificacion;
        private System.Windows.Forms.TabPage TBPrecuperacion;
        private System.Windows.Forms.DataGridView DGVrecuperacion;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn1;
        private System.Windows.Forms.DataGridViewComboBoxColumn dataGridViewComboBoxColumn1;
        private System.Windows.Forms.TabControl TBCbackFront;
        private System.Windows.Forms.TabPage TBPback;
        private System.Windows.Forms.TabPage TBPfront;
        private System.Windows.Forms.GroupBox GPBFront;
        private System.Windows.Forms.CheckBox CHKtypeScript;
        private System.Windows.Forms.DataGridViewTextBoxColumn CampoBaja;
        private System.Windows.Forms.DataGridViewComboBoxColumn VinculoBaja;
        private System.Windows.Forms.ToolTip TTPusarTryOrIf;
        private System.Windows.Forms.SplitContainer SPCback1;
        private System.Windows.Forms.GroupBox GPBback;
        private System.Windows.Forms.CheckBox CHKservice;
        private System.Windows.Forms.CheckBox CHKrepositories;
        private System.Windows.Forms.CheckBox CHKmodel;
        private System.Windows.Forms.CheckBox CHKdto;
        private System.Windows.Forms.CheckBox CHKcontrollers;
        private System.Windows.Forms.SplitContainer SPCbak2;
        private System.Windows.Forms.GroupBox GPBajustes;
        private System.Windows.Forms.CheckBox CHKtryOrIf;
        private System.Windows.Forms.CheckBox CHKquitarEsquema;
        private System.Windows.Forms.GroupBox GPBmetodos;
        private System.Windows.Forms.CheckBox CHKrecuperacion;
        private System.Windows.Forms.CheckBox CHKtodos;
        private System.Windows.Forms.CheckBox CHKobtenerPorId;
        private System.Windows.Forms.CheckBox CHKmodificacion;
        private System.Windows.Forms.CheckBox CHKbaja;
        private System.Windows.Forms.CheckBox CHKalta;
    }
}

