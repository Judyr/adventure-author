/*
 * User: Keiron Nicholson
 * Date: 10/04/2007
 * Time: 11:31
 */
namespace AdventureAuthor.UI.Forms
{
	partial class RunAdventure_Form
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
			this.button_RunAdventure = new System.Windows.Forms.Button();
			this.button_CancelRunAdventure = new System.Windows.Forms.Button();
			this.radioButton_RunFromStart = new System.Windows.Forms.RadioButton();
			this.radioButton_RunFromWaypoint = new System.Windows.Forms.RadioButton();
			this.comboBox_WaypointsToRunAdventureFrom = new System.Windows.Forms.ComboBox();
			this.checkBox_CanLaunchDebugWindow = new System.Windows.Forms.CheckBox();
			this.checkBox_PlayerIsInvincible = new System.Windows.Forms.CheckBox();
			this.groupBox_RunFrom = new System.Windows.Forms.GroupBox();
			this.groupBox_RunFrom.SuspendLayout();
			this.SuspendLayout();
			// 
			// button_RunAdventure
			// 
			this.button_RunAdventure.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.button_RunAdventure.Location = new System.Drawing.Point(279, 185);
			this.button_RunAdventure.Name = "button_RunAdventure";
			this.button_RunAdventure.Size = new System.Drawing.Size(88, 35);
			this.button_RunAdventure.TabIndex = 1;
			this.button_RunAdventure.Text = "Run";
			this.button_RunAdventure.UseVisualStyleBackColor = true;
			this.button_RunAdventure.Click += new System.EventHandler(this.Button_RunAdventureClick);
			// 
			// button_CancelRunAdventure
			// 
			this.button_CancelRunAdventure.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.button_CancelRunAdventure.Location = new System.Drawing.Point(185, 185);
			this.button_CancelRunAdventure.Name = "button_CancelRunAdventure";
			this.button_CancelRunAdventure.Size = new System.Drawing.Size(88, 35);
			this.button_CancelRunAdventure.TabIndex = 2;
			this.button_CancelRunAdventure.Text = "Cancel";
			this.button_CancelRunAdventure.UseVisualStyleBackColor = true;
			this.button_CancelRunAdventure.Click += new System.EventHandler(this.Button_CancelRunAdventureClick);
			// 
			// radioButton_RunFromStart
			// 
			this.radioButton_RunFromStart.Checked = true;
			this.radioButton_RunFromStart.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.radioButton_RunFromStart.Location = new System.Drawing.Point(15, 19);
			this.radioButton_RunFromStart.Name = "radioButton_RunFromStart";
			this.radioButton_RunFromStart.Size = new System.Drawing.Size(145, 24);
			this.radioButton_RunFromStart.TabIndex = 3;
			this.radioButton_RunFromStart.TabStop = true;
			this.radioButton_RunFromStart.Text = "Run from Start";
			this.radioButton_RunFromStart.UseVisualStyleBackColor = true;
			this.radioButton_RunFromStart.CheckedChanged += new System.EventHandler(this.RadioButton_RunFromStartCheckedChanged);
			// 
			// radioButton_RunFromWaypoint
			// 
			this.radioButton_RunFromWaypoint.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.radioButton_RunFromWaypoint.Location = new System.Drawing.Point(15, 49);
			this.radioButton_RunFromWaypoint.Name = "radioButton_RunFromWaypoint";
			this.radioButton_RunFromWaypoint.Size = new System.Drawing.Size(145, 24);
			this.radioButton_RunFromWaypoint.TabIndex = 4;
			this.radioButton_RunFromWaypoint.Text = "Run from Waypoint";
			this.radioButton_RunFromWaypoint.UseVisualStyleBackColor = true;
			this.radioButton_RunFromWaypoint.CheckedChanged += new System.EventHandler(this.RadioButton_RunFromWaypointCheckedChanged);
			// 
			// comboBox_WaypointsToRunAdventureFrom
			// 
			this.comboBox_WaypointsToRunAdventureFrom.DisplayMember = "OutsideTheCave";
			this.comboBox_WaypointsToRunAdventureFrom.Enabled = false;
			this.comboBox_WaypointsToRunAdventureFrom.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.comboBox_WaypointsToRunAdventureFrom.FormattingEnabled = true;
			this.comboBox_WaypointsToRunAdventureFrom.Items.AddRange(new object[] {
									"This is currently functionless",
									"OutsideTheCave",
									"AtTheMagicFountain",
									"This is currently functionless"});
			this.comboBox_WaypointsToRunAdventureFrom.Location = new System.Drawing.Point(166, 49);
			this.comboBox_WaypointsToRunAdventureFrom.Name = "comboBox_WaypointsToRunAdventureFrom";
			this.comboBox_WaypointsToRunAdventureFrom.Size = new System.Drawing.Size(175, 24);
			this.comboBox_WaypointsToRunAdventureFrom.TabIndex = 5;
			this.comboBox_WaypointsToRunAdventureFrom.Text = "Choose a waypoint...";
			// 
			// checkBox_CanLaunchDebugWindow
			// 
			this.checkBox_CanLaunchDebugWindow.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.checkBox_CanLaunchDebugWindow.Location = new System.Drawing.Point(27, 108);
			this.checkBox_CanLaunchDebugWindow.Name = "checkBox_CanLaunchDebugWindow";
			this.checkBox_CanLaunchDebugWindow.Size = new System.Drawing.Size(341, 24);
			this.checkBox_CanLaunchDebugWindow.TabIndex = 6;
			this.checkBox_CanLaunchDebugWindow.Text = "Player can launch Debug window during gameplay";
			this.checkBox_CanLaunchDebugWindow.UseVisualStyleBackColor = true;
			// 
			// checkBox_PlayerIsInvincible
			// 
			this.checkBox_PlayerIsInvincible.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.checkBox_PlayerIsInvincible.Location = new System.Drawing.Point(27, 138);
			this.checkBox_PlayerIsInvincible.Name = "checkBox_PlayerIsInvincible";
			this.checkBox_PlayerIsInvincible.Size = new System.Drawing.Size(341, 24);
			this.checkBox_PlayerIsInvincible.TabIndex = 7;
			this.checkBox_PlayerIsInvincible.Text = "Player cannot be harmed";
			this.checkBox_PlayerIsInvincible.UseVisualStyleBackColor = true;
			// 
			// groupBox_RunFrom
			// 
			this.groupBox_RunFrom.Controls.Add(this.radioButton_RunFromStart);
			this.groupBox_RunFrom.Controls.Add(this.radioButton_RunFromWaypoint);
			this.groupBox_RunFrom.Controls.Add(this.comboBox_WaypointsToRunAdventureFrom);
			this.groupBox_RunFrom.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.groupBox_RunFrom.Location = new System.Drawing.Point(12, 12);
			this.groupBox_RunFrom.Name = "groupBox_RunFrom";
			this.groupBox_RunFrom.Size = new System.Drawing.Size(355, 90);
			this.groupBox_RunFrom.TabIndex = 8;
			this.groupBox_RunFrom.TabStop = false;
			this.groupBox_RunFrom.Text = "Run from...";
			// 
			// RunAdventure_Form
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(378, 232);
			this.Controls.Add(this.groupBox_RunFrom);
			this.Controls.Add(this.checkBox_PlayerIsInvincible);
			this.Controls.Add(this.checkBox_CanLaunchDebugWindow);
			this.Controls.Add(this.button_CancelRunAdventure);
			this.Controls.Add(this.button_RunAdventure);
			this.Name = "RunAdventure_Form";
			this.Text = "Run Adventure";
			this.groupBox_RunFrom.ResumeLayout(false);
			this.ResumeLayout(false);
		}
		private System.Windows.Forms.GroupBox groupBox_RunFrom;
		private System.Windows.Forms.CheckBox checkBox_PlayerIsInvincible;
		private System.Windows.Forms.CheckBox checkBox_CanLaunchDebugWindow;
		private System.Windows.Forms.ComboBox comboBox_WaypointsToRunAdventureFrom;
		private System.Windows.Forms.RadioButton radioButton_RunFromWaypoint;
		private System.Windows.Forms.RadioButton radioButton_RunFromStart;
		private System.Windows.Forms.Button button_CancelRunAdventure;
		private System.Windows.Forms.Button button_RunAdventure;
	}
}
