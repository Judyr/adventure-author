/*
 * User: Keiron Nicholson
 * Date: 12/04/2007
 * Time: 11:27
 */
namespace AdventureAuthor.Core.UI
{
	partial class NewAdventure_Form
	{
		/// <summary>
		/// Designer variable used to keep track of non-visual components.
		/// </summary>
		private System.ComponentModel.IContainer components = null;
		
		/// <summary>
		/// Disposes resources used by the form.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing) {
				if (components != null) {
					components.Dispose();
				}
			}
			base.Dispose(disposing);
		}
		
		/// <summary>
		/// This method is required for Windows Forms designer support.
		/// Do not change the method contents inside the source code editor. The Forms designer might
		/// not be able to load this method if it was changed manually.
		/// </summary>
		private void InitializeComponent()
		{
			this.button_Cancel_NewAdventure = new System.Windows.Forms.Button();
			this.button_OK_NewAdventure = new System.Windows.Forms.Button();
			this.label_CurrentUser = new System.Windows.Forms.Label();
			this.label_CreateANewAdventureCalled = new System.Windows.Forms.Label();
			this.textBox_NameOfNewAdventure = new System.Windows.Forms.TextBox();
			this.SuspendLayout();
			// 
			// button_Cancel_NewAdventure
			// 
			this.button_Cancel_NewAdventure.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.button_Cancel_NewAdventure.Location = new System.Drawing.Point(285, 111);
			this.button_Cancel_NewAdventure.Name = "button_Cancel_NewAdventure";
			this.button_Cancel_NewAdventure.Size = new System.Drawing.Size(88, 35);
			this.button_Cancel_NewAdventure.TabIndex = 2;
			this.button_Cancel_NewAdventure.Text = "Cancel";
			this.button_Cancel_NewAdventure.UseVisualStyleBackColor = true;
			this.button_Cancel_NewAdventure.Click += new System.EventHandler(this.Button_Cancel_NewAdventureClick);
			// 
			// button_OK_NewAdventure
			// 
			this.button_OK_NewAdventure.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.button_OK_NewAdventure.Location = new System.Drawing.Point(379, 111);
			this.button_OK_NewAdventure.Name = "button_OK_NewAdventure";
			this.button_OK_NewAdventure.Size = new System.Drawing.Size(88, 35);
			this.button_OK_NewAdventure.TabIndex = 3;
			this.button_OK_NewAdventure.Text = "OK";
			this.button_OK_NewAdventure.UseVisualStyleBackColor = true;
			this.button_OK_NewAdventure.Click += new System.EventHandler(this.Button_OK_NewAdventureClick);
			// 
			// label_CurrentUser
			// 
			this.label_CurrentUser.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label_CurrentUser.Location = new System.Drawing.Point(12, 19);
			this.label_CurrentUser.Name = "label_CurrentUser";
			this.label_CurrentUser.Size = new System.Drawing.Size(346, 21);
			this.label_CurrentUser.TabIndex = 14;
			this.label_CurrentUser.Text = "Current user:";
			// 
			// label_CreateANewAdventureCalled
			// 
			this.label_CreateANewAdventureCalled.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label_CreateANewAdventureCalled.Location = new System.Drawing.Point(12, 62);
			this.label_CreateANewAdventureCalled.Name = "label_CreateANewAdventureCalled";
			this.label_CreateANewAdventureCalled.Size = new System.Drawing.Size(205, 21);
			this.label_CreateANewAdventureCalled.TabIndex = 15;
			this.label_CreateANewAdventureCalled.Text = "Create a new Adventure called:";
			// 
			// textBox_NameOfNewAdventure
			// 
			this.textBox_NameOfNewAdventure.Location = new System.Drawing.Point(223, 61);
			this.textBox_NameOfNewAdventure.MaxLength = 32;
			this.textBox_NameOfNewAdventure.Name = "textBox_NameOfNewAdventure";
			this.textBox_NameOfNewAdventure.Size = new System.Drawing.Size(243, 20);
			this.textBox_NameOfNewAdventure.TabIndex = 1;
			// 
			// NewAdventure_Form
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(479, 158);
			this.Controls.Add(this.textBox_NameOfNewAdventure);
			this.Controls.Add(this.label_CreateANewAdventureCalled);
			this.Controls.Add(this.label_CurrentUser);
			this.Controls.Add(this.button_Cancel_NewAdventure);
			this.Controls.Add(this.button_OK_NewAdventure);
			this.Name = "NewAdventure_Form";
			this.Text = "Create New Adventure";
			this.ResumeLayout(false);
			this.PerformLayout();
		}
		private System.Windows.Forms.Button button_Cancel_NewAdventure;
		private System.Windows.Forms.TextBox textBox_NameOfNewAdventure;
		private System.Windows.Forms.Label label_CreateANewAdventureCalled;
		private System.Windows.Forms.Button button_OK_NewAdventure;
		private System.Windows.Forms.Label label_CurrentUser;
	}
}
