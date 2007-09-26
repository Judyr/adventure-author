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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CreateChapter_Form));
			this.tabControl_CreateChapter = new System.Windows.Forms.TabControl();
			this.tabPage_CreateChapter_Story = new System.Windows.Forms.TabPage();
			this.button_Cancel_StoryPanel = new System.Windows.Forms.Button();
			this.button_OK_StoryPanel = new System.Windows.Forms.Button();
			this.textBox_IntroductionToChapter = new System.Windows.Forms.TextBox();
			this.label_WriteYourIntroductionToThisChapter = new System.Windows.Forms.Label();
			this.label_HowDoYouWantThePlayerToFeelDuringThisChapter = new System.Windows.Forms.Label();
			this.label_WhatStoryEventsTakePlaceInThisChapter = new System.Windows.Forms.Label();
			this.listBox_Emotions = new System.Windows.Forms.ListBox();
			this.button_AddNewStoryEvent = new System.Windows.Forms.Button();
			this.button_RemoveStoryEvent = new System.Windows.Forms.Button();
			this.listBox_StoryEvents = new System.Windows.Forms.ListBox();
			this.radioButton_Outdoors = new System.Windows.Forms.RadioButton();
			this.radioButton_Indoors = new System.Windows.Forms.RadioButton();
			this.label_WhereDoesItTakePlace = new System.Windows.Forms.Label();
			this.label_WhatIsTheNameOfThisChapter = new System.Windows.Forms.Label();
			this.textBox_ChapterName = new System.Windows.Forms.TextBox();
			this.tabPage_CreateChapter_Gameplay = new System.Windows.Forms.TabPage();
			this.button_Cancel_GameplayPanel = new System.Windows.Forms.Button();
			this.button_OK_GameplayPanel = new System.Windows.Forms.Button();
			this.label_AdventureAuthorThinks_DifficultyOfChapter = new System.Windows.Forms.Label();
			this.groupBox_StartingEvents = new System.Windows.Forms.GroupBox();
			this.radioButton1 = new System.Windows.Forms.RadioButton();
			this.radioButton3 = new System.Windows.Forms.RadioButton();
			this.radioButton2 = new System.Windows.Forms.RadioButton();
			this.label_Average = new System.Windows.Forms.Label();
			this.label_VeryHard = new System.Windows.Forms.Label();
			this.label_VeryEasy = new System.Windows.Forms.Label();
			this.trackBar_DifficultyOfChapter = new System.Windows.Forms.TrackBar();
			this.label1 = new System.Windows.Forms.Label();
			this.groupBox_GameplayStyleExplanation = new System.Windows.Forms.GroupBox();
			this.richTextBox_GameplayStyleExplanation = new System.Windows.Forms.RichTextBox();
			this.comboBox_GameplayStyles = new System.Windows.Forms.ComboBox();
			this.label_WhatGameplayStyleDoesThisChapterMainlyUse = new System.Windows.Forms.Label();
			this.tabPage_CreateChapter_Art = new System.Windows.Forms.TabPage();
			this.groupBox_MusicAndSound = new System.Windows.Forms.GroupBox();
			this.label7 = new System.Windows.Forms.Label();
			this.trackBar1 = new System.Windows.Forms.TrackBar();
			this.comboBox3 = new System.Windows.Forms.ComboBox();
			this.label6 = new System.Windows.Forms.Label();
			this.label5 = new System.Windows.Forms.Label();
			this.comboBox1 = new System.Windows.Forms.ComboBox();
			this.comboBox2 = new System.Windows.Forms.ComboBox();
			this.label4 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.groupBox_Visuals = new System.Windows.Forms.GroupBox();
			this.label8 = new System.Windows.Forms.Label();
			this.comboBox4 = new System.Windows.Forms.ComboBox();
			this.radioButton5 = new System.Windows.Forms.RadioButton();
			this.radioButton4 = new System.Windows.Forms.RadioButton();
			this.button_Cancel_AudioVisualPanel = new System.Windows.Forms.Button();
			this.button_OK_AudioVisualPanel = new System.Windows.Forms.Button();
			this.tabControl_CreateChapter.SuspendLayout();
			this.tabPage_CreateChapter_Story.SuspendLayout();
			this.tabPage_CreateChapter_Gameplay.SuspendLayout();
			this.groupBox_StartingEvents.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.trackBar_DifficultyOfChapter)).BeginInit();
			this.groupBox_GameplayStyleExplanation.SuspendLayout();
			this.tabPage_CreateChapter_Art.SuspendLayout();
			this.groupBox_MusicAndSound.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.trackBar1)).BeginInit();
			this.groupBox_Visuals.SuspendLayout();
			this.SuspendLayout();
			// 
			// tabControl_CreateChapter
			// 
			this.tabControl_CreateChapter.Controls.Add(this.tabPage_CreateChapter_Story);
			this.tabControl_CreateChapter.Controls.Add(this.tabPage_CreateChapter_Gameplay);
			this.tabControl_CreateChapter.Controls.Add(this.tabPage_CreateChapter_Art);
			this.tabControl_CreateChapter.Location = new System.Drawing.Point(0, 0);
			this.tabControl_CreateChapter.Multiline = true;
			this.tabControl_CreateChapter.Name = "tabControl_CreateChapter";
			this.tabControl_CreateChapter.SelectedIndex = 0;
			this.tabControl_CreateChapter.Size = new System.Drawing.Size(700, 466);
			this.tabControl_CreateChapter.TabIndex = 0;
			// 
			// tabPage_CreateChapter_Story
			// 
			this.tabPage_CreateChapter_Story.Controls.Add(this.button_Cancel_StoryPanel);
			this.tabPage_CreateChapter_Story.Controls.Add(this.button_OK_StoryPanel);
			this.tabPage_CreateChapter_Story.Controls.Add(this.textBox_IntroductionToChapter);
			this.tabPage_CreateChapter_Story.Controls.Add(this.label_WriteYourIntroductionToThisChapter);
			this.tabPage_CreateChapter_Story.Controls.Add(this.label_HowDoYouWantThePlayerToFeelDuringThisChapter);
			this.tabPage_CreateChapter_Story.Controls.Add(this.label_WhatStoryEventsTakePlaceInThisChapter);
			this.tabPage_CreateChapter_Story.Controls.Add(this.listBox_Emotions);
			this.tabPage_CreateChapter_Story.Controls.Add(this.button_AddNewStoryEvent);
			this.tabPage_CreateChapter_Story.Controls.Add(this.button_RemoveStoryEvent);
			this.tabPage_CreateChapter_Story.Controls.Add(this.listBox_StoryEvents);
			this.tabPage_CreateChapter_Story.Controls.Add(this.radioButton_Outdoors);
			this.tabPage_CreateChapter_Story.Controls.Add(this.radioButton_Indoors);
			this.tabPage_CreateChapter_Story.Controls.Add(this.label_WhereDoesItTakePlace);
			this.tabPage_CreateChapter_Story.Controls.Add(this.label_WhatIsTheNameOfThisChapter);
			this.tabPage_CreateChapter_Story.Controls.Add(this.textBox_ChapterName);
			this.tabPage_CreateChapter_Story.Location = new System.Drawing.Point(4, 22);
			this.tabPage_CreateChapter_Story.Name = "tabPage_CreateChapter_Story";
			this.tabPage_CreateChapter_Story.Padding = new System.Windows.Forms.Padding(3);
			this.tabPage_CreateChapter_Story.Size = new System.Drawing.Size(692, 440);
			this.tabPage_CreateChapter_Story.TabIndex = 0;
			this.tabPage_CreateChapter_Story.Text = "Story design";
			this.tabPage_CreateChapter_Story.UseVisualStyleBackColor = true;
			// 
			// button_Cancel_StoryPanel
			// 
			this.button_Cancel_StoryPanel.Font = new System.Drawing.Font("Comic Sans MS", 10F);
			this.button_Cancel_StoryPanel.Location = new System.Drawing.Point(431, 379);
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
			this.button_OK_StoryPanel.Location = new System.Drawing.Point(561, 379);
			this.button_OK_StoryPanel.Name = "button_OK_StoryPanel";
			this.button_OK_StoryPanel.Size = new System.Drawing.Size(115, 37);
			this.button_OK_StoryPanel.TabIndex = 14;
			this.button_OK_StoryPanel.Text = "OK";
			this.button_OK_StoryPanel.UseVisualStyleBackColor = true;
			this.button_OK_StoryPanel.Click += new System.EventHandler(this.CreateChapter_OK);
			// 
			// textBox_IntroductionToChapter
			// 
			this.textBox_IntroductionToChapter.Font = new System.Drawing.Font("Comic Sans MS", 10F);
			this.textBox_IntroductionToChapter.Location = new System.Drawing.Point(19, 299);
			this.textBox_IntroductionToChapter.Multiline = true;
			this.textBox_IntroductionToChapter.Name = "textBox_IntroductionToChapter";
			this.textBox_IntroductionToChapter.Size = new System.Drawing.Size(361, 117);
			this.textBox_IntroductionToChapter.TabIndex = 12;
			// 
			// label_WriteYourIntroductionToThisChapter
			// 
			this.label_WriteYourIntroductionToThisChapter.Font = new System.Drawing.Font("Comic Sans MS", 10F);
			this.label_WriteYourIntroductionToThisChapter.Location = new System.Drawing.Point(19, 273);
			this.label_WriteYourIntroductionToThisChapter.Name = "label_WriteYourIntroductionToThisChapter";
			this.label_WriteYourIntroductionToThisChapter.Size = new System.Drawing.Size(307, 23);
			this.label_WriteYourIntroductionToThisChapter.TabIndex = 11;
			this.label_WriteYourIntroductionToThisChapter.Text = "Write your introduction to this chapter.";
			// 
			// label_HowDoYouWantThePlayerToFeelDuringThisChapter
			// 
			this.label_HowDoYouWantThePlayerToFeelDuringThisChapter.Font = new System.Drawing.Font("Comic Sans MS", 10F);
			this.label_HowDoYouWantThePlayerToFeelDuringThisChapter.Location = new System.Drawing.Point(423, 143);
			this.label_HowDoYouWantThePlayerToFeelDuringThisChapter.Name = "label_HowDoYouWantThePlayerToFeelDuringThisChapter";
			this.label_HowDoYouWantThePlayerToFeelDuringThisChapter.Size = new System.Drawing.Size(146, 85);
			this.label_HowDoYouWantThePlayerToFeelDuringThisChapter.TabIndex = 10;
			this.label_HowDoYouWantThePlayerToFeelDuringThisChapter.Text = "What kind of atmosphere do you want this chapter to have?";
			// 
			// label_WhatStoryEventsTakePlaceInThisChapter
			// 
			this.label_WhatStoryEventsTakePlaceInThisChapter.Font = new System.Drawing.Font("Comic Sans MS", 10F);
			this.label_WhatStoryEventsTakePlaceInThisChapter.Location = new System.Drawing.Point(19, 88);
			this.label_WhatStoryEventsTakePlaceInThisChapter.Name = "label_WhatStoryEventsTakePlaceInThisChapter";
			this.label_WhatStoryEventsTakePlaceInThisChapter.Size = new System.Drawing.Size(342, 23);
			this.label_WhatStoryEventsTakePlaceInThisChapter.TabIndex = 9;
			this.label_WhatStoryEventsTakePlaceInThisChapter.Text = "What story events take place in this chapter?";
			// 
			// listBox_Emotions
			// 
			this.listBox_Emotions.Font = new System.Drawing.Font("Comic Sans MS", 10F);
			this.listBox_Emotions.FormattingEnabled = true;
			this.listBox_Emotions.ItemHeight = 18;
			this.listBox_Emotions.Items.AddRange(new object[] {
									"Pleasant",
									"Creepy",
									"Funny",
									"Mysterious",
									"Exciting"});
			this.listBox_Emotions.Location = new System.Drawing.Point(575, 114);
			this.listBox_Emotions.Name = "listBox_Emotions";
			this.listBox_Emotions.Size = new System.Drawing.Size(79, 130);
			this.listBox_Emotions.TabIndex = 8;
			// 
			// button_AddNewStoryEvent
			// 
			this.button_AddNewStoryEvent.Font = new System.Drawing.Font("Comic Sans MS", 10F);
			this.button_AddNewStoryEvent.Location = new System.Drawing.Point(226, 215);
			this.button_AddNewStoryEvent.Name = "button_AddNewStoryEvent";
			this.button_AddNewStoryEvent.Size = new System.Drawing.Size(154, 31);
			this.button_AddNewStoryEvent.TabIndex = 7;
			this.button_AddNewStoryEvent.Text = "Add new story event";
			this.button_AddNewStoryEvent.UseVisualStyleBackColor = true;
			// 
			// button_RemoveStoryEvent
			// 
			this.button_RemoveStoryEvent.Font = new System.Drawing.Font("Comic Sans MS", 10F);
			this.button_RemoveStoryEvent.Location = new System.Drawing.Point(71, 215);
			this.button_RemoveStoryEvent.Name = "button_RemoveStoryEvent";
			this.button_RemoveStoryEvent.Size = new System.Drawing.Size(149, 31);
			this.button_RemoveStoryEvent.TabIndex = 6;
			this.button_RemoveStoryEvent.Text = "Remove story event";
			this.button_RemoveStoryEvent.UseVisualStyleBackColor = true;
			// 
			// listBox_StoryEvents
			// 
			this.listBox_StoryEvents.Font = new System.Drawing.Font("Comic Sans MS", 10F);
			this.listBox_StoryEvents.FormattingEnabled = true;
			this.listBox_StoryEvents.ItemHeight = 18;
			this.listBox_StoryEvents.Items.AddRange(new object[] {
									"* The player gets a quest from Merlin to find a ring.",
									"* Sir Lancelot joins the player\'s party to help him. ",
									"* The player finds out that Lancelot has a secret."});
			this.listBox_StoryEvents.Location = new System.Drawing.Point(19, 114);
			this.listBox_StoryEvents.Name = "listBox_StoryEvents";
			this.listBox_StoryEvents.Size = new System.Drawing.Size(361, 94);
			this.listBox_StoryEvents.TabIndex = 5;
			// 
			// radioButton_Outdoors
			// 
			this.radioButton_Outdoors.Font = new System.Drawing.Font("Comic Sans MS", 10F);
			this.radioButton_Outdoors.Location = new System.Drawing.Point(431, 37);
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
			this.radioButton_Indoors.Location = new System.Drawing.Point(541, 37);
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
			this.label_WhereDoesItTakePlace.Location = new System.Drawing.Point(431, 15);
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
			// textBox_ChapterName
			// 
			this.textBox_ChapterName.Font = new System.Drawing.Font("Comic Sans MS", 10F);
			this.textBox_ChapterName.Location = new System.Drawing.Point(19, 41);
			this.textBox_ChapterName.Name = "textBox_ChapterName";
			this.textBox_ChapterName.Size = new System.Drawing.Size(361, 26);
			this.textBox_ChapterName.TabIndex = 0;
			// 
			// tabPage_CreateChapter_Gameplay
			// 
			this.tabPage_CreateChapter_Gameplay.Controls.Add(this.button_Cancel_GameplayPanel);
			this.tabPage_CreateChapter_Gameplay.Controls.Add(this.button_OK_GameplayPanel);
			this.tabPage_CreateChapter_Gameplay.Controls.Add(this.label_AdventureAuthorThinks_DifficultyOfChapter);
			this.tabPage_CreateChapter_Gameplay.Controls.Add(this.groupBox_StartingEvents);
			this.tabPage_CreateChapter_Gameplay.Controls.Add(this.label_Average);
			this.tabPage_CreateChapter_Gameplay.Controls.Add(this.label_VeryHard);
			this.tabPage_CreateChapter_Gameplay.Controls.Add(this.label_VeryEasy);
			this.tabPage_CreateChapter_Gameplay.Controls.Add(this.trackBar_DifficultyOfChapter);
			this.tabPage_CreateChapter_Gameplay.Controls.Add(this.label1);
			this.tabPage_CreateChapter_Gameplay.Controls.Add(this.groupBox_GameplayStyleExplanation);
			this.tabPage_CreateChapter_Gameplay.Controls.Add(this.comboBox_GameplayStyles);
			this.tabPage_CreateChapter_Gameplay.Controls.Add(this.label_WhatGameplayStyleDoesThisChapterMainlyUse);
			this.tabPage_CreateChapter_Gameplay.Location = new System.Drawing.Point(4, 22);
			this.tabPage_CreateChapter_Gameplay.Name = "tabPage_CreateChapter_Gameplay";
			this.tabPage_CreateChapter_Gameplay.Padding = new System.Windows.Forms.Padding(3);
			this.tabPage_CreateChapter_Gameplay.Size = new System.Drawing.Size(692, 440);
			this.tabPage_CreateChapter_Gameplay.TabIndex = 1;
			this.tabPage_CreateChapter_Gameplay.Text = "Gameplay design";
			this.tabPage_CreateChapter_Gameplay.UseVisualStyleBackColor = true;
			// 
			// button_Cancel_GameplayPanel
			// 
			this.button_Cancel_GameplayPanel.Font = new System.Drawing.Font("Comic Sans MS", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.button_Cancel_GameplayPanel.Location = new System.Drawing.Point(431, 379);
			this.button_Cancel_GameplayPanel.Name = "button_Cancel_GameplayPanel";
			this.button_Cancel_GameplayPanel.Size = new System.Drawing.Size(115, 37);
			this.button_Cancel_GameplayPanel.TabIndex = 18;
			this.button_Cancel_GameplayPanel.Text = "Cancel";
			this.button_Cancel_GameplayPanel.UseVisualStyleBackColor = true;
			this.button_Cancel_GameplayPanel.Click += new System.EventHandler(this.CreateChapter_Cancel);
			// 
			// button_OK_GameplayPanel
			// 
			this.button_OK_GameplayPanel.Font = new System.Drawing.Font("Comic Sans MS", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.button_OK_GameplayPanel.Location = new System.Drawing.Point(561, 379);
			this.button_OK_GameplayPanel.Name = "button_OK_GameplayPanel";
			this.button_OK_GameplayPanel.Size = new System.Drawing.Size(115, 37);
			this.button_OK_GameplayPanel.TabIndex = 17;
			this.button_OK_GameplayPanel.Text = "OK";
			this.button_OK_GameplayPanel.UseVisualStyleBackColor = true;
			this.button_OK_GameplayPanel.Click += new System.EventHandler(this.CreateChapter_OK);
			// 
			// label_AdventureAuthorThinks_DifficultyOfChapter
			// 
			this.label_AdventureAuthorThinks_DifficultyOfChapter.Enabled = false;
			this.label_AdventureAuthorThinks_DifficultyOfChapter.Font = new System.Drawing.Font("Comic Sans MS", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label_AdventureAuthorThinks_DifficultyOfChapter.Location = new System.Drawing.Point(19, 350);
			this.label_AdventureAuthorThinks_DifficultyOfChapter.Name = "label_AdventureAuthorThinks_DifficultyOfChapter";
			this.label_AdventureAuthorThinks_DifficultyOfChapter.Size = new System.Drawing.Size(229, 66);
			this.label_AdventureAuthorThinks_DifficultyOfChapter.TabIndex = 16;
			this.label_AdventureAuthorThinks_DifficultyOfChapter.Text = "Adventure Author thinks this chapter is Impossible, based on the difficulty level" +
			" of the monsters.";
			// 
			// groupBox_StartingEvents
			// 
			this.groupBox_StartingEvents.Controls.Add(this.radioButton1);
			this.groupBox_StartingEvents.Controls.Add(this.radioButton3);
			this.groupBox_StartingEvents.Controls.Add(this.radioButton2);
			this.groupBox_StartingEvents.Font = new System.Drawing.Font("Comic Sans MS", 10F);
			this.groupBox_StartingEvents.Location = new System.Drawing.Point(310, 226);
			this.groupBox_StartingEvents.Name = "groupBox_StartingEvents";
			this.groupBox_StartingEvents.Size = new System.Drawing.Size(366, 147);
			this.groupBox_StartingEvents.TabIndex = 15;
			this.groupBox_StartingEvents.TabStop = false;
			this.groupBox_StartingEvents.Text = "At the start of the chapter";
			// 
			// radioButton1
			// 
			this.radioButton1.Location = new System.Drawing.Point(16, 19);
			this.radioButton1.Name = "radioButton1";
			this.radioButton1.Size = new System.Drawing.Size(344, 45);
			this.radioButton1.TabIndex = 12;
			this.radioButton1.TabStop = true;
			this.radioButton1.Text = "I want this chapter to start with a party conversation";
			this.radioButton1.UseVisualStyleBackColor = true;
			// 
			// radioButton3
			// 
			this.radioButton3.Location = new System.Drawing.Point(16, 111);
			this.radioButton3.Name = "radioButton3";
			this.radioButton3.Size = new System.Drawing.Size(344, 30);
			this.radioButton3.TabIndex = 14;
			this.radioButton3.TabStop = true;
			this.radioButton3.Text = "I don\'t want anything to happen at the start";
			this.radioButton3.UseVisualStyleBackColor = true;
			// 
			// radioButton2
			// 
			this.radioButton2.Location = new System.Drawing.Point(16, 65);
			this.radioButton2.Name = "radioButton2";
			this.radioButton2.Size = new System.Drawing.Size(360, 40);
			this.radioButton2.TabIndex = 13;
			this.radioButton2.TabStop = true;
			this.radioButton2.Text = "I want this chapter to start with a scripted event";
			this.radioButton2.UseVisualStyleBackColor = true;
			// 
			// label_Average
			// 
			this.label_Average.Font = new System.Drawing.Font("Comic Sans MS", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label_Average.Location = new System.Drawing.Point(38, 260);
			this.label_Average.Name = "label_Average";
			this.label_Average.Size = new System.Drawing.Size(79, 30);
			this.label_Average.TabIndex = 11;
			this.label_Average.Text = "Average";
			// 
			// label_VeryHard
			// 
			this.label_VeryHard.Font = new System.Drawing.Font("Comic Sans MS", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label_VeryHard.Location = new System.Drawing.Point(38, 210);
			this.label_VeryHard.Name = "label_VeryHard";
			this.label_VeryHard.Size = new System.Drawing.Size(79, 26);
			this.label_VeryHard.TabIndex = 10;
			this.label_VeryHard.Text = "Very Hard";
			// 
			// label_VeryEasy
			// 
			this.label_VeryEasy.Font = new System.Drawing.Font("Comic Sans MS", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label_VeryEasy.Location = new System.Drawing.Point(38, 309);
			this.label_VeryEasy.Name = "label_VeryEasy";
			this.label_VeryEasy.Size = new System.Drawing.Size(79, 27);
			this.label_VeryEasy.TabIndex = 6;
			this.label_VeryEasy.Text = "Very Easy";
			// 
			// trackBar_DifficultyOfChapter
			// 
			this.trackBar_DifficultyOfChapter.LargeChange = 0;
			this.trackBar_DifficultyOfChapter.Location = new System.Drawing.Point(134, 201);
			this.trackBar_DifficultyOfChapter.Maximum = 4;
			this.trackBar_DifficultyOfChapter.Name = "trackBar_DifficultyOfChapter";
			this.trackBar_DifficultyOfChapter.Orientation = System.Windows.Forms.Orientation.Vertical;
			this.trackBar_DifficultyOfChapter.Size = new System.Drawing.Size(45, 135);
			this.trackBar_DifficultyOfChapter.TabIndex = 5;
			this.trackBar_DifficultyOfChapter.TickStyle = System.Windows.Forms.TickStyle.Both;
			// 
			// label1
			// 
			this.label1.Font = new System.Drawing.Font("Comic Sans MS", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label1.Location = new System.Drawing.Point(19, 155);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(213, 43);
			this.label1.TabIndex = 3;
			this.label1.Text = "How difficult do you think this chapter should be?";
			// 
			// groupBox_GameplayStyleExplanation
			// 
			this.groupBox_GameplayStyleExplanation.Controls.Add(this.richTextBox_GameplayStyleExplanation);
			this.groupBox_GameplayStyleExplanation.Font = new System.Drawing.Font("Comic Sans MS", 10F);
			this.groupBox_GameplayStyleExplanation.Location = new System.Drawing.Point(310, 15);
			this.groupBox_GameplayStyleExplanation.Name = "groupBox_GameplayStyleExplanation";
			this.groupBox_GameplayStyleExplanation.Size = new System.Drawing.Size(366, 183);
			this.groupBox_GameplayStyleExplanation.TabIndex = 2;
			this.groupBox_GameplayStyleExplanation.TabStop = false;
			this.groupBox_GameplayStyleExplanation.Text = "Gameplay Style";
			// 
			// richTextBox_GameplayStyleExplanation
			// 
			this.richTextBox_GameplayStyleExplanation.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.richTextBox_GameplayStyleExplanation.Enabled = false;
			this.richTextBox_GameplayStyleExplanation.Location = new System.Drawing.Point(26, 35);
			this.richTextBox_GameplayStyleExplanation.Name = "richTextBox_GameplayStyleExplanation";
			this.richTextBox_GameplayStyleExplanation.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.None;
			this.richTextBox_GameplayStyleExplanation.Size = new System.Drawing.Size(310, 128);
			this.richTextBox_GameplayStyleExplanation.TabIndex = 0;
			this.richTextBox_GameplayStyleExplanation.Text = resources.GetString("richTextBox_GameplayStyleExplanation.Text");
			// 
			// comboBox_GameplayStyles
			// 
			this.comboBox_GameplayStyles.Font = new System.Drawing.Font("Comic Sans MS", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.comboBox_GameplayStyles.FormattingEnabled = true;
			this.comboBox_GameplayStyles.Items.AddRange(new object[] {
									"Exploration",
									"Detective work",
									"Treasure hunt",
									"Chase",
									"Siege",
									"Boss Fight",
									"Protection",
									"Puzzle-solving",
									"Other"});
			this.comboBox_GameplayStyles.Location = new System.Drawing.Point(19, 65);
			this.comboBox_GameplayStyles.MaxDropDownItems = 12;
			this.comboBox_GameplayStyles.Name = "comboBox_GameplayStyles";
			this.comboBox_GameplayStyles.Size = new System.Drawing.Size(147, 26);
			this.comboBox_GameplayStyles.TabIndex = 1;
			this.comboBox_GameplayStyles.Text = "Exploration";
			// 
			// label_WhatGameplayStyleDoesThisChapterMainlyUse
			// 
			this.label_WhatGameplayStyleDoesThisChapterMainlyUse.Font = new System.Drawing.Font("Comic Sans MS", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label_WhatGameplayStyleDoesThisChapterMainlyUse.Location = new System.Drawing.Point(19, 15);
			this.label_WhatGameplayStyleDoesThisChapterMainlyUse.Name = "label_WhatGameplayStyleDoesThisChapterMainlyUse";
			this.label_WhatGameplayStyleDoesThisChapterMainlyUse.Size = new System.Drawing.Size(247, 47);
			this.label_WhatGameplayStyleDoesThisChapterMainlyUse.TabIndex = 0;
			this.label_WhatGameplayStyleDoesThisChapterMainlyUse.Text = "What gameplay style does this chapter mainly use?";
			// 
			// tabPage_CreateChapter_Art
			// 
			this.tabPage_CreateChapter_Art.Controls.Add(this.groupBox_MusicAndSound);
			this.tabPage_CreateChapter_Art.Controls.Add(this.groupBox_Visuals);
			this.tabPage_CreateChapter_Art.Controls.Add(this.button_Cancel_AudioVisualPanel);
			this.tabPage_CreateChapter_Art.Controls.Add(this.button_OK_AudioVisualPanel);
			this.tabPage_CreateChapter_Art.Location = new System.Drawing.Point(4, 22);
			this.tabPage_CreateChapter_Art.Name = "tabPage_CreateChapter_Art";
			this.tabPage_CreateChapter_Art.Padding = new System.Windows.Forms.Padding(3);
			this.tabPage_CreateChapter_Art.Size = new System.Drawing.Size(692, 440);
			this.tabPage_CreateChapter_Art.TabIndex = 2;
			this.tabPage_CreateChapter_Art.Text = "Audio and Visual design";
			this.tabPage_CreateChapter_Art.UseVisualStyleBackColor = true;
			// 
			// groupBox_MusicAndSound
			// 
			this.groupBox_MusicAndSound.Controls.Add(this.label7);
			this.groupBox_MusicAndSound.Controls.Add(this.trackBar1);
			this.groupBox_MusicAndSound.Controls.Add(this.comboBox3);
			this.groupBox_MusicAndSound.Controls.Add(this.label6);
			this.groupBox_MusicAndSound.Controls.Add(this.label5);
			this.groupBox_MusicAndSound.Controls.Add(this.comboBox1);
			this.groupBox_MusicAndSound.Controls.Add(this.comboBox2);
			this.groupBox_MusicAndSound.Controls.Add(this.label4);
			this.groupBox_MusicAndSound.Controls.Add(this.label3);
			this.groupBox_MusicAndSound.Controls.Add(this.label2);
			this.groupBox_MusicAndSound.Font = new System.Drawing.Font("Comic Sans MS", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.groupBox_MusicAndSound.Location = new System.Drawing.Point(351, 16);
			this.groupBox_MusicAndSound.Name = "groupBox_MusicAndSound";
			this.groupBox_MusicAndSound.Size = new System.Drawing.Size(314, 341);
			this.groupBox_MusicAndSound.TabIndex = 22;
			this.groupBox_MusicAndSound.TabStop = false;
			this.groupBox_MusicAndSound.Text = "Music and Sound";
			// 
			// label7
			// 
			this.label7.Location = new System.Drawing.Point(15, 272);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(115, 49);
			this.label7.TabIndex = 12;
			this.label7.Text = "How loud should they be?";
			this.label7.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// trackBar1
			// 
			this.trackBar1.LargeChange = 0;
			this.trackBar1.Location = new System.Drawing.Point(151, 265);
			this.trackBar1.Maximum = 4;
			this.trackBar1.Name = "trackBar1";
			this.trackBar1.Size = new System.Drawing.Size(135, 45);
			this.trackBar1.TabIndex = 11;
			this.trackBar1.TickStyle = System.Windows.Forms.TickStyle.Both;
			// 
			// comboBox3
			// 
			this.comboBox3.FormattingEnabled = true;
			this.comboBox3.Items.AddRange(new object[] {
									"Riot Outside",
									"Riot Inside",
									"Talking, Laughter",
									"Wind and Rain"});
			this.comboBox3.Location = new System.Drawing.Point(151, 215);
			this.comboBox3.Name = "comboBox3";
			this.comboBox3.Size = new System.Drawing.Size(144, 26);
			this.comboBox3.TabIndex = 10;
			this.comboBox3.Text = "Riot Outside";
			// 
			// label6
			// 
			this.label6.Location = new System.Drawing.Point(25, 215);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(105, 48);
			this.label6.TabIndex = 9;
			this.label6.Text = "Ambient Sound Effects";
			this.label6.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// label5
			// 
			this.label5.Location = new System.Drawing.Point(7, 156);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(279, 45);
			this.label5.TabIndex = 8;
			this.label5.Text = "What sound effects do you want to be played in the background?";
			// 
			// comboBox1
			// 
			this.comboBox1.FormattingEnabled = true;
			this.comboBox1.Location = new System.Drawing.Point(129, 110);
			this.comboBox1.Name = "comboBox1";
			this.comboBox1.Size = new System.Drawing.Size(166, 26);
			this.comboBox1.TabIndex = 7;
			this.comboBox1.Text = "Dragon Combat";
			// 
			// comboBox2
			// 
			this.comboBox2.DisplayMember = "Swamp Theme";
			this.comboBox2.FormattingEnabled = true;
			this.comboBox2.Items.AddRange(new object[] {
									"Dungeon Theme",
									"Back Alley Theme",
									"City Theme",
									"Swamp Theme"});
			this.comboBox2.Location = new System.Drawing.Point(129, 67);
			this.comboBox2.Name = "comboBox2";
			this.comboBox2.Size = new System.Drawing.Size(166, 26);
			this.comboBox2.TabIndex = 6;
			this.comboBox2.Text = "Swamp Theme";
			// 
			// label4
			// 
			this.label4.Location = new System.Drawing.Point(25, 110);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(96, 26);
			this.label4.TabIndex = 4;
			this.label4.Text = "Battle Music";
			this.label4.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(19, 70);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(104, 26);
			this.label3.TabIndex = 3;
			this.label3.Text = "Normal Music";
			this.label3.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(6, 20);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(302, 44);
			this.label2.TabIndex = 2;
			this.label2.Text = "What music do you want to be played during this chapter?";
			// 
			// groupBox_Visuals
			// 
			this.groupBox_Visuals.Controls.Add(this.label8);
			this.groupBox_Visuals.Controls.Add(this.comboBox4);
			this.groupBox_Visuals.Controls.Add(this.radioButton5);
			this.groupBox_Visuals.Controls.Add(this.radioButton4);
			this.groupBox_Visuals.Font = new System.Drawing.Font("Comic Sans MS", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.groupBox_Visuals.Location = new System.Drawing.Point(21, 16);
			this.groupBox_Visuals.Name = "groupBox_Visuals";
			this.groupBox_Visuals.Size = new System.Drawing.Size(314, 341);
			this.groupBox_Visuals.TabIndex = 21;
			this.groupBox_Visuals.TabStop = false;
			this.groupBox_Visuals.Text = "Visuals";
			// 
			// label8
			// 
			this.label8.Location = new System.Drawing.Point(52, 245);
			this.label8.Name = "label8";
			this.label8.Size = new System.Drawing.Size(241, 38);
			this.label8.TabIndex = 13;
			this.label8.Text = "Which picture do you want to use for the sky background? ";
			this.label8.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// comboBox4
			// 
			this.comboBox4.FormattingEnabled = true;
			this.comboBox4.Items.AddRange(new object[] {
									"Hilly Landscape",
									"City",
									"Swamp",
									"Rocky Landscape"});
			this.comboBox4.Location = new System.Drawing.Point(127, 299);
			this.comboBox4.Name = "comboBox4";
			this.comboBox4.Size = new System.Drawing.Size(166, 26);
			this.comboBox4.TabIndex = 13;
			this.comboBox4.Text = "Hilly Landscape";
			// 
			// radioButton5
			// 
			this.radioButton5.Location = new System.Drawing.Point(161, 19);
			this.radioButton5.Name = "radioButton5";
			this.radioButton5.Size = new System.Drawing.Size(132, 50);
			this.radioButton5.TabIndex = 14;
			this.radioButton5.TabStop = true;
			this.radioButton5.Text = "Use special lighting";
			this.radioButton5.UseVisualStyleBackColor = true;
			// 
			// radioButton4
			// 
			this.radioButton4.Location = new System.Drawing.Point(22, 19);
			this.radioButton4.Name = "radioButton4";
			this.radioButton4.Size = new System.Drawing.Size(133, 50);
			this.radioButton4.TabIndex = 13;
			this.radioButton4.TabStop = true;
			this.radioButton4.Text = "Use standard lighting";
			this.radioButton4.UseVisualStyleBackColor = true;
			// 
			// button_Cancel_AudioVisualPanel
			// 
			this.button_Cancel_AudioVisualPanel.Font = new System.Drawing.Font("Comic Sans MS", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.button_Cancel_AudioVisualPanel.Location = new System.Drawing.Point(431, 379);
			this.button_Cancel_AudioVisualPanel.Name = "button_Cancel_AudioVisualPanel";
			this.button_Cancel_AudioVisualPanel.Size = new System.Drawing.Size(115, 37);
			this.button_Cancel_AudioVisualPanel.TabIndex = 20;
			this.button_Cancel_AudioVisualPanel.Text = "Cancel";
			this.button_Cancel_AudioVisualPanel.UseVisualStyleBackColor = true;
			this.button_Cancel_AudioVisualPanel.Click += new System.EventHandler(this.CreateChapter_Cancel);
			// 
			// button_OK_AudioVisualPanel
			// 
			this.button_OK_AudioVisualPanel.Font = new System.Drawing.Font("Comic Sans MS", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.button_OK_AudioVisualPanel.Location = new System.Drawing.Point(561, 379);
			this.button_OK_AudioVisualPanel.Name = "button_OK_AudioVisualPanel";
			this.button_OK_AudioVisualPanel.Size = new System.Drawing.Size(115, 37);
			this.button_OK_AudioVisualPanel.TabIndex = 19;
			this.button_OK_AudioVisualPanel.Text = "OK";
			this.button_OK_AudioVisualPanel.UseVisualStyleBackColor = true;
			this.button_OK_AudioVisualPanel.Click += new System.EventHandler(this.CreateChapter_OK);
			// 
			// CreateChapter_Form
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(692, 466);
			this.Controls.Add(this.tabControl_CreateChapter);
			this.Name = "CreateChapter_Form";
			this.Text = "Create a New Chapter";
			this.tabControl_CreateChapter.ResumeLayout(false);
			this.tabPage_CreateChapter_Story.ResumeLayout(false);
			this.tabPage_CreateChapter_Story.PerformLayout();
			this.tabPage_CreateChapter_Gameplay.ResumeLayout(false);
			this.tabPage_CreateChapter_Gameplay.PerformLayout();
			this.groupBox_StartingEvents.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.trackBar_DifficultyOfChapter)).EndInit();
			this.groupBox_GameplayStyleExplanation.ResumeLayout(false);
			this.tabPage_CreateChapter_Art.ResumeLayout(false);
			this.groupBox_MusicAndSound.ResumeLayout(false);
			this.groupBox_MusicAndSound.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.trackBar1)).EndInit();
			this.groupBox_Visuals.ResumeLayout(false);
			this.ResumeLayout(false);
		}
		private System.Windows.Forms.Button button_OK_AudioVisualPanel;
		private System.Windows.Forms.Button button_Cancel_AudioVisualPanel;
		private System.Windows.Forms.RadioButton radioButton4;
		private System.Windows.Forms.RadioButton radioButton5;
		private System.Windows.Forms.ComboBox comboBox4;
		private System.Windows.Forms.Label label8;
		private System.Windows.Forms.ComboBox comboBox3;
		private System.Windows.Forms.TrackBar trackBar1;
		private System.Windows.Forms.Label label7;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.ComboBox comboBox2;
		private System.Windows.Forms.ComboBox comboBox1;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.GroupBox groupBox_MusicAndSound;
		private System.Windows.Forms.GroupBox groupBox_Visuals;
		private System.Windows.Forms.TabPage tabPage_CreateChapter_Art;
		private System.Windows.Forms.Button button_OK_GameplayPanel;
		private System.Windows.Forms.Button button_Cancel_GameplayPanel;
		private System.Windows.Forms.Button button_OK_StoryPanel;
		private System.Windows.Forms.Button button_Cancel_StoryPanel;
		private System.Windows.Forms.Label label_AdventureAuthorThinks_DifficultyOfChapter;
		private System.Windows.Forms.GroupBox groupBox_StartingEvents;
		private System.Windows.Forms.RadioButton radioButton1;
		private System.Windows.Forms.RadioButton radioButton2;
		private System.Windows.Forms.RadioButton radioButton3;
		private System.Windows.Forms.Label label_VeryHard;
		private System.Windows.Forms.Label label_Average;
		private System.Windows.Forms.TrackBar trackBar_DifficultyOfChapter;
		private System.Windows.Forms.Label label_VeryEasy;
		private System.Windows.Forms.RichTextBox richTextBox_GameplayStyleExplanation;
		private System.Windows.Forms.Label label_WhatGameplayStyleDoesThisChapterMainlyUse;
		private System.Windows.Forms.GroupBox groupBox_GameplayStyleExplanation;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.ComboBox comboBox_GameplayStyles;
		private System.Windows.Forms.Label label_WriteYourIntroductionToThisChapter;
		private System.Windows.Forms.TextBox textBox_IntroductionToChapter;
		private System.Windows.Forms.TextBox textBox_ChapterName;
		private System.Windows.Forms.Label label_WhatIsTheNameOfThisChapter;
		private System.Windows.Forms.Label label_WhereDoesItTakePlace;
		private System.Windows.Forms.RadioButton radioButton_Indoors;
		private System.Windows.Forms.RadioButton radioButton_Outdoors;
		private System.Windows.Forms.ListBox listBox_StoryEvents;
		private System.Windows.Forms.Button button_RemoveStoryEvent;
		private System.Windows.Forms.Button button_AddNewStoryEvent;
		private System.Windows.Forms.ListBox listBox_Emotions;
		private System.Windows.Forms.Label label_WhatStoryEventsTakePlaceInThisChapter;
		private System.Windows.Forms.Label label_HowDoYouWantThePlayerToFeelDuringThisChapter;
		private System.Windows.Forms.TabPage tabPage_CreateChapter_Gameplay;
		private System.Windows.Forms.TabPage tabPage_CreateChapter_Story;
		private System.Windows.Forms.TabControl tabControl_CreateChapter;
	}
}
