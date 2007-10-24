/*
 * User: Keiron Nicholson
 * Date: 08/03/2007
 * Time: 12:59
 */
namespace AdventureAuthor.Core.UI
{
	partial class CreateChapter_Form
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
			this.tabPage_CreateChapter_Story = new System.Windows.Forms.TabPage();
			this.button_Cancel_StoryPanel = new System.Windows.Forms.Button();
			this.button_OK_StoryPanel = new System.Windows.Forms.Button();
			this.textBox_ChapterName = new System.Windows.Forms.TextBox();
			this.radioButton_Outdoors = new System.Windows.Forms.RadioButton();
			this.radioButton_Indoors = new System.Windows.Forms.RadioButton();
			this.label_WhereDoesItTakePlace = new System.Windows.Forms.Label();
			this.label_WhatIsTheNameOfThisChapter = new System.Windows.Forms.Label();
			this.tabControl_CreateChapter = new System.Windows.Forms.TabControl();
			this.tabPage_CreateChapter_Story.SuspendLayout();
			this.tabControl_CreateChapter.SuspendLayout();
			this.SuspendLayout();
			// 
			// tabPage_CreateChapter_Story
			// 
			this.tabPage_CreateChapter_Story.Controls.Add(this.button_Cancel_StoryPanel);
			this.tabPage_CreateChapter_Story.Controls.Add(this.button_OK_StoryPanel);
			this.tabPage_CreateChapter_Story.Controls.Add(this.textBox_ChapterName);
			this.tabPage_CreateChapter_Story.Controls.Add(this.radioButton_Outdoors);
			this.tabPage_CreateChapter_Story.Controls.Add(this.radioButton_Indoors);
			this.tabPage_CreateChapter_Story.Controls.Add(this.label_WhereDoesItTakePlace);
			this.tabPage_CreateChapter_Story.Controls.Add(this.label_WhatIsTheNameOfThisChapter);
			this.tabPage_CreateChapter_Story.Location = new System.Drawing.Point(4, 22);
			this.tabPage_CreateChapter_Story.Name = "tabPage_CreateChapter_Story";
			this.tabPage_CreateChapter_Story.Padding = new System.Windows.Forms.Padding(3);
			this.tabPage_CreateChapter_Story.Size = new System.Drawing.Size(692, 440);
			this.tabPage_CreateChapter_Story.TabIndex = 0;
			this.tabPage_CreateChapter_Story.Text = "Chapter settings";
			this.tabPage_CreateChapter_Story.UseVisualStyleBackColor = true;
			// 
			// button_Cancel_StoryPanel
			// 
			this.button_Cancel_StoryPanel.Font = new System.Drawing.Font("Comic Sans MS", 10F);
			this.button_Cancel_StoryPanel.Location = new System.Drawing.Point(165, 233);
			this.button_Cancel_StoryPanel.Name = "button_Cancel_StoryPanel";
			this.button_Cancel_StoryPanel.Size = new System.Drawing.Size(115, 37);
			this.button_Cancel_StoryPanel.TabIndex = 15;
			this.button_Cancel_StoryPanel.Text = "Cancel";
			this.button_Cancel_StoryPanel.UseVisualStyleBackColor = true;
			this.button_Cancel_StoryPanel.Click += new System.EventHandler(this.CreateChapter_Cancel);
			// 
			// button_OK_StoryPanel
			// 
			this.button_OK_StoryPanel.Font = new System.Drawing.Font("Comic Sans MS", 10F);
			this.button_OK_StoryPanel.Location = new System.Drawing.Point(286, 233);
			this.button_OK_StoryPanel.Name = "button_OK_StoryPanel";
			this.button_OK_StoryPanel.Size = new System.Drawing.Size(115, 37);
			this.button_OK_StoryPanel.TabIndex = 14;
			this.button_OK_StoryPanel.Text = "OK";
			this.button_OK_StoryPanel.UseVisualStyleBackColor = true;
			this.button_OK_StoryPanel.Click += new System.EventHandler(this.CreateChapter_OK);
			// 
			// textBox_ChapterName
			// 
			this.textBox_ChapterName.Font = new System.Drawing.Font("Comic Sans MS", 10F);
			this.textBox_ChapterName.Location = new System.Drawing.Point(19, 41);
			this.textBox_ChapterName.Name = "textBox_ChapterName";
			this.textBox_ChapterName.Size = new System.Drawing.Size(361, 26);
			this.textBox_ChapterName.TabIndex = 0;
			// 
			// radioButton_Outdoors
			// 
			this.radioButton_Outdoors.Font = new System.Drawing.Font("Comic Sans MS", 10F);
			this.radioButton_Outdoors.Location = new System.Drawing.Point(19, 111);
			this.radioButton_Outdoors.Name = "radioButton_Outdoors";
			this.radioButton_Outdoors.Size = new System.Drawing.Size(104, 24);
			this.radioButton_Outdoors.TabIndex = 4;
			this.radioButton_Outdoors.TabStop = true;
			this.radioButton_Outdoors.Text = "Outdoors";
			this.radioButton_Outdoors.UseVisualStyleBackColor = true;
			// 
			// radioButton_Indoors
			// 
			this.radioButton_Indoors.Font = new System.Drawing.Font("Comic Sans MS", 10F);
			this.radioButton_Indoors.Location = new System.Drawing.Point(129, 111);
			this.radioButton_Indoors.Name = "radioButton_Indoors";
			this.radioButton_Indoors.Size = new System.Drawing.Size(104, 24);
			this.radioButton_Indoors.TabIndex = 3;
			this.radioButton_Indoors.TabStop = true;
			this.radioButton_Indoors.Text = "Indoors";
			this.radioButton_Indoors.UseVisualStyleBackColor = true;
			// 
			// label_WhereDoesItTakePlace
			// 
			this.label_WhereDoesItTakePlace.Font = new System.Drawing.Font("Comic Sans MS", 10F);
			this.label_WhereDoesItTakePlace.Location = new System.Drawing.Point(19, 85);
			this.label_WhereDoesItTakePlace.Name = "label_WhereDoesItTakePlace";
			this.label_WhereDoesItTakePlace.Size = new System.Drawing.Size(196, 23);
			this.label_WhereDoesItTakePlace.TabIndex = 2;
			this.label_WhereDoesItTakePlace.Text = "Where does it take place?";
			// 
			// label_WhatIsTheNameOfThisChapter
			// 
			this.label_WhatIsTheNameOfThisChapter.Font = new System.Drawing.Font("Comic Sans MS", 10F);
			this.label_WhatIsTheNameOfThisChapter.Location = new System.Drawing.Point(19, 15);
			this.label_WhatIsTheNameOfThisChapter.Name = "label_WhatIsTheNameOfThisChapter";
			this.label_WhatIsTheNameOfThisChapter.Size = new System.Drawing.Size(245, 23);
			this.label_WhatIsTheNameOfThisChapter.TabIndex = 1;
			this.label_WhatIsTheNameOfThisChapter.Text = "What is the name of this chapter?";
			// 
			// tabControl_CreateChapter
			// 
			this.tabControl_CreateChapter.Controls.Add(this.tabPage_CreateChapter_Story);
			this.tabControl_CreateChapter.Location = new System.Drawing.Point(0, 0);
			this.tabControl_CreateChapter.Multiline = true;
			this.tabControl_CreateChapter.Name = "tabControl_CreateChapter";
			this.tabControl_CreateChapter.SelectedIndex = 0;
			this.tabControl_CreateChapter.Size = new System.Drawing.Size(700, 466);
			this.tabControl_CreateChapter.TabIndex = 0;
			// 
			// CreateChapter_Form
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(418, 304);
			this.Controls.Add(this.tabControl_CreateChapter);
			this.Name = "CreateChapter_Form";
			this.Text = "Create a New Chapter";
			this.tabPage_CreateChapter_Story.ResumeLayout(false);
			this.tabPage_CreateChapter_Story.PerformLayout();
			this.tabControl_CreateChapter.ResumeLayout(false);
			this.ResumeLayout(false);
		}
		private System.Windows.Forms.Button button_OK_StoryPanel;
		private System.Windows.Forms.Button button_Cancel_StoryPanel;
		private System.Windows.Forms.TextBox textBox_ChapterName;
		private System.Windows.Forms.Label label_WhatIsTheNameOfThisChapter;
		private System.Windows.Forms.Label label_WhereDoesItTakePlace;
		private System.Windows.Forms.RadioButton radioButton_Indoors;
		private System.Windows.Forms.RadioButton radioButton_Outdoors;
		private System.Windows.Forms.TabPage tabPage_CreateChapter_Story;
		private System.Windows.Forms.TabControl tabControl_CreateChapter;
	}
}
