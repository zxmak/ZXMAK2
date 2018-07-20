namespace ZXMAK2.Hardware.WinForms
{
   partial class dbgWD1793
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
          this.components = new System.ComponentModel.Container();
          this.label1 = new System.Windows.Forms.Label();
          this.timerUpdate = new System.Windows.Forms.Timer(this.components);
          this.SuspendLayout();
          // 
          // label1
          // 
          this.label1.AutoSize = true;
          this.label1.Location = new System.Drawing.Point(36, 39);
          this.label1.Name = "label1";
          this.label1.Size = new System.Drawing.Size(35, 13);
          this.label1.TabIndex = 0;
          this.label1.Text = "label1";
          // 
          // timerUpdate
          // 
          this.timerUpdate.Enabled = true;
          this.timerUpdate.Interval = 300;
          this.timerUpdate.Tick += new System.EventHandler(this.timerUpdate_Tick);
          // 
          // dbgWD1793
          // 
          this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
          this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
          this.ClientSize = new System.Drawing.Size(292, 371);
          this.Controls.Add(this.label1);
          this.Name = "dbgWD1793";
          this.ShowIcon = false;
          this.ShowInTaskbar = false;
          this.Text = "dbgWD1793";
          this.TopMost = true;
          this.ResumeLayout(false);
          this.PerformLayout();

      }

      #endregion

      private System.Windows.Forms.Label label1;
      private System.Windows.Forms.Timer timerUpdate;
   }
}