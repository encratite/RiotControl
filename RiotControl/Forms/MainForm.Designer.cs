namespace RiotControl
{
	partial class MainForm
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
			this.mainTab = new System.Windows.Forms.TabControl();
			this.outputPage = new System.Windows.Forms.TabPage();
			this.outputTextBox = new System.Windows.Forms.RichTextBox();
			this.regionPage = new System.Windows.Forms.TabPage();
			this.regionLabel = new System.Windows.Forms.Label();
			this.editRegionButton = new System.Windows.Forms.Button();
			this.regionListBox = new System.Windows.Forms.ListBox();
			this.mainTab.SuspendLayout();
			this.outputPage.SuspendLayout();
			this.regionPage.SuspendLayout();
			this.SuspendLayout();
			// 
			// mainTab
			// 
			this.mainTab.Controls.Add(this.outputPage);
			this.mainTab.Controls.Add(this.regionPage);
			this.mainTab.Dock = System.Windows.Forms.DockStyle.Fill;
			this.mainTab.Location = new System.Drawing.Point(0, 0);
			this.mainTab.Name = "mainTab";
			this.mainTab.SelectedIndex = 0;
			this.mainTab.Size = new System.Drawing.Size(512, 206);
			this.mainTab.TabIndex = 0;
			// 
			// outputPage
			// 
			this.outputPage.Controls.Add(this.outputTextBox);
			this.outputPage.Location = new System.Drawing.Point(4, 22);
			this.outputPage.Name = "outputPage";
			this.outputPage.Padding = new System.Windows.Forms.Padding(3);
			this.outputPage.Size = new System.Drawing.Size(504, 180);
			this.outputPage.TabIndex = 0;
			this.outputPage.Text = "Output";
			this.outputPage.UseVisualStyleBackColor = true;
			// 
			// outputTextBox
			// 
			this.outputTextBox.BackColor = System.Drawing.SystemColors.ControlLightLight;
			this.outputTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
			this.outputTextBox.Location = new System.Drawing.Point(3, 3);
			this.outputTextBox.Name = "outputTextBox";
			this.outputTextBox.ReadOnly = true;
			this.outputTextBox.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
			this.outputTextBox.Size = new System.Drawing.Size(498, 174);
			this.outputTextBox.TabIndex = 0;
			this.outputTextBox.Text = "";
			// 
			// regionPage
			// 
			this.regionPage.Controls.Add(this.regionLabel);
			this.regionPage.Controls.Add(this.editRegionButton);
			this.regionPage.Controls.Add(this.regionListBox);
			this.regionPage.Location = new System.Drawing.Point(4, 22);
			this.regionPage.Name = "regionPage";
			this.regionPage.Padding = new System.Windows.Forms.Padding(3);
			this.regionPage.Size = new System.Drawing.Size(504, 180);
			this.regionPage.TabIndex = 1;
			this.regionPage.Text = "Logins";
			this.regionPage.UseVisualStyleBackColor = true;
			// 
			// regionLabel
			// 
			this.regionLabel.AutoSize = true;
			this.regionLabel.Location = new System.Drawing.Point(3, 4);
			this.regionLabel.Name = "regionLabel";
			this.regionLabel.Size = new System.Drawing.Size(94, 13);
			this.regionLabel.TabIndex = 2;
			this.regionLabel.Text = "Regions available:";
			// 
			// editRegionButton
			// 
			this.editRegionButton.Enabled = false;
			this.editRegionButton.Location = new System.Drawing.Point(145, 69);
			this.editRegionButton.Name = "editRegionButton";
			this.editRegionButton.Size = new System.Drawing.Size(64, 23);
			this.editRegionButton.TabIndex = 1;
			this.editRegionButton.Text = "Edit login";
			this.editRegionButton.UseVisualStyleBackColor = true;
			// 
			// regionListBox
			// 
			this.regionListBox.FormattingEnabled = true;
			this.regionListBox.Location = new System.Drawing.Point(6, 20);
			this.regionListBox.Name = "regionListBox";
			this.regionListBox.Size = new System.Drawing.Size(203, 43);
			this.regionListBox.TabIndex = 0;
			this.regionListBox.SelectedValueChanged += new System.EventHandler(this.regionListBoxSelectedValueChanged);
			// 
			// MainForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(512, 206);
			this.Controls.Add(this.mainTab);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "MainForm";
			this.Text = "Riot Control";
			this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.MainFormFormClosed);
			this.mainTab.ResumeLayout(false);
			this.outputPage.ResumeLayout(false);
			this.regionPage.ResumeLayout(false);
			this.regionPage.PerformLayout();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.TabControl mainTab;
		private System.Windows.Forms.TabPage outputPage;
		private System.Windows.Forms.RichTextBox outputTextBox;
		private System.Windows.Forms.TabPage regionPage;
		private System.Windows.Forms.Label regionLabel;
		private System.Windows.Forms.Button editRegionButton;
		private System.Windows.Forms.ListBox regionListBox;
	}
}