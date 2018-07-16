namespace ShatranjMain.Forms
{
    partial class ChessGame
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
            this.splitPanelMain = new System.Windows.Forms.SplitContainer();
            this.PnlGameInfo = new System.Windows.Forms.FlowLayoutPanel();
            this.splitPanelChessBoard = new System.Windows.Forms.SplitContainer();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.newToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.loadToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            ((System.ComponentModel.ISupportInitialize)(this.splitPanelMain)).BeginInit();
            this.splitPanelMain.Panel1.SuspendLayout();
            this.splitPanelMain.Panel2.SuspendLayout();
            this.splitPanelMain.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitPanelChessBoard)).BeginInit();
            this.splitPanelChessBoard.Panel1.SuspendLayout();
            this.splitPanelChessBoard.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitPanelMain
            // 
            this.splitPanelMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitPanelMain.Location = new System.Drawing.Point(0, 0);
            this.splitPanelMain.Name = "splitPanelMain";
            // 
            // splitPanelMain.Panel1
            // 
            this.splitPanelMain.Panel1.Controls.Add(this.splitPanelChessBoard);
            // 
            // splitPanelMain.Panel2
            // 
            this.splitPanelMain.Panel2.Controls.Add(this.PnlGameInfo);
            this.splitPanelMain.Size = new System.Drawing.Size(1178, 750);
            this.splitPanelMain.SplitterDistance = 750;
            this.splitPanelMain.TabIndex = 0;
            // 
            // PnlGameInfo
            // 
            this.PnlGameInfo.Dock = System.Windows.Forms.DockStyle.Fill;
            this.PnlGameInfo.Location = new System.Drawing.Point(0, 0);
            this.PnlGameInfo.Name = "PnlGameInfo";
            this.PnlGameInfo.Size = new System.Drawing.Size(424, 750);
            this.PnlGameInfo.TabIndex = 0;
            // 
            // splitPanelChessBoard
            // 
            this.splitPanelChessBoard.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitPanelChessBoard.Location = new System.Drawing.Point(0, 0);
            this.splitPanelChessBoard.Name = "splitPanelChessBoard";
            this.splitPanelChessBoard.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitPanelChessBoard.Panel1
            // 
            this.splitPanelChessBoard.Panel1.Controls.Add(this.menuStrip1);
            this.splitPanelChessBoard.Size = new System.Drawing.Size(750, 750);
            this.splitPanelChessBoard.SplitterDistance = 25;
            this.splitPanelChessBoard.TabIndex = 0;
            // 
            // menuStrip1
            // 
            this.menuStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.newToolStripMenuItem,
            this.saveToolStripMenuItem,
            this.loadToolStripMenuItem,
            this.exitToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(750, 28);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // newToolStripMenuItem
            // 
            this.newToolStripMenuItem.Name = "newToolStripMenuItem";
            this.newToolStripMenuItem.Size = new System.Drawing.Size(51, 24);
            this.newToolStripMenuItem.Text = "New";
            // 
            // saveToolStripMenuItem
            // 
            this.saveToolStripMenuItem.Name = "saveToolStripMenuItem";
            this.saveToolStripMenuItem.Size = new System.Drawing.Size(52, 24);
            this.saveToolStripMenuItem.Text = "Save";
            // 
            // loadToolStripMenuItem
            // 
            this.loadToolStripMenuItem.Name = "loadToolStripMenuItem";
            this.loadToolStripMenuItem.Size = new System.Drawing.Size(54, 24);
            this.loadToolStripMenuItem.Text = "Load";
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(45, 24);
            this.exitToolStripMenuItem.Text = "Exit";
            // 
            // ChessGame
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1178, 750);
            this.Controls.Add(this.splitPanelMain);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "ChessGame";
            this.Text = "ChessGame";
            this.splitPanelMain.Panel1.ResumeLayout(false);
            this.splitPanelMain.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitPanelMain)).EndInit();
            this.splitPanelMain.ResumeLayout(false);
            this.splitPanelChessBoard.Panel1.ResumeLayout(false);
            this.splitPanelChessBoard.Panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitPanelChessBoard)).EndInit();
            this.splitPanelChessBoard.ResumeLayout(false);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitPanelMain;
        private System.Windows.Forms.SplitContainer splitPanelChessBoard;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.FlowLayoutPanel PnlGameInfo;
        private System.Windows.Forms.ToolStripMenuItem newToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem loadToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
    }
}