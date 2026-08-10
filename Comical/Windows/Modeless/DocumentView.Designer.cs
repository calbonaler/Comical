namespace Comical
{
	partial class DocumentView
	{
		/// <summary>
		/// 必要なデザイナー変数です。
		/// </summary>
		System.ComponentModel.IContainer components = null;

		/// <summary>
		/// 使用中のリソースをすべてクリーンアップします。
		/// </summary>
		/// <param name="disposing">マネージ リソースが破棄される場合 true、破棄されない場合は false です。</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				_comic.PropertyChanged -= OnComicPropertyChanged;
				if (components != null)
					components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows フォーム デザイナーで生成されたコード

		/// <summary>
		/// デザイナー サポートに必要なメソッドです。このメソッドの内容を
		/// コード エディターで変更しないでください。
		/// </summary>
		void InitializeComponent()
		{
			System.Windows.Forms.TableLayoutPanel Panel1TableLayoutPanel;
			var resources = new System.ComponentModel.ComponentResourceManager(typeof(DocumentView));
			System.Windows.Forms.Label BindingSideLabel;
			System.Windows.Forms.Label CultureDependingPublishedLabel;
			System.Windows.Forms.Label TitleLabel;
			System.Windows.Forms.Label PublishedLabel;
			System.Windows.Forms.Label AuthorLabel;
			System.Windows.Forms.Button SearchOnBrowserButton;
			System.Windows.Forms.TableLayoutPanel Panel2TableLayoutPanel;
			BindingSideComboBox = new System.Windows.Forms.ComboBox();
			CultureDependingPublishedTextBox = new System.Windows.Forms.TextBox();
			TitleTextBox = new System.Windows.Forms.TextBox();
			AuthorComboBox = new System.Windows.Forms.ComboBox();
			PublishedDateTimePicker = new System.Windows.Forms.DateTimePicker();
			ThumbnailLabel = new System.Windows.Forms.Label();
			ThumbnailEditOKButton = new System.Windows.Forms.Button();
			ThumbnailEditCancelButton = new System.Windows.Forms.Button();
			ThumbnailEditButton = new System.Windows.Forms.Button();
			ThumbnailClipper = new Comical.Controls.ImageClipper();
			MagnifyRatioNumericUpDown = new Comical.Controls.SuffixNumericUpDown();
			SizeLabel = new System.Windows.Forms.Label();
			ThumbnailMaximizeButton = new System.Windows.Forms.Button();
			MainSplitContainer = new System.Windows.Forms.SplitContainer();
			Panel1TableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
			BindingSideLabel = new System.Windows.Forms.Label();
			CultureDependingPublishedLabel = new System.Windows.Forms.Label();
			TitleLabel = new System.Windows.Forms.Label();
			PublishedLabel = new System.Windows.Forms.Label();
			AuthorLabel = new System.Windows.Forms.Label();
			SearchOnBrowserButton = new System.Windows.Forms.Button();
			Panel2TableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
			Panel1TableLayoutPanel.SuspendLayout();
			Panel2TableLayoutPanel.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)MagnifyRatioNumericUpDown).BeginInit();
			((System.ComponentModel.ISupportInitialize)MainSplitContainer).BeginInit();
			MainSplitContainer.Panel1.SuspendLayout();
			MainSplitContainer.Panel2.SuspendLayout();
			MainSplitContainer.SuspendLayout();
			SuspendLayout();
			// 
			// Panel1TableLayoutPanel
			// 
			resources.ApplyResources(Panel1TableLayoutPanel, "Panel1TableLayoutPanel");
			Panel1TableLayoutPanel.Controls.Add(BindingSideComboBox, 0, 9);
			Panel1TableLayoutPanel.Controls.Add(BindingSideLabel, 0, 8);
			Panel1TableLayoutPanel.Controls.Add(CultureDependingPublishedTextBox, 0, 7);
			Panel1TableLayoutPanel.Controls.Add(CultureDependingPublishedLabel, 0, 6);
			Panel1TableLayoutPanel.Controls.Add(TitleLabel, 0, 0);
			Panel1TableLayoutPanel.Controls.Add(TitleTextBox, 0, 1);
			Panel1TableLayoutPanel.Controls.Add(PublishedLabel, 0, 4);
			Panel1TableLayoutPanel.Controls.Add(AuthorLabel, 0, 2);
			Panel1TableLayoutPanel.Controls.Add(SearchOnBrowserButton, 1, 5);
			Panel1TableLayoutPanel.Controls.Add(AuthorComboBox, 0, 3);
			Panel1TableLayoutPanel.Controls.Add(PublishedDateTimePicker, 0, 5);
			Panel1TableLayoutPanel.Name = "Panel1TableLayoutPanel";
			// 
			// BindingSideComboBox
			// 
			resources.ApplyResources(BindingSideComboBox, "BindingSideComboBox");
			Panel1TableLayoutPanel.SetColumnSpan(BindingSideComboBox, 2);
			BindingSideComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			BindingSideComboBox.FormattingEnabled = true;
			BindingSideComboBox.Items.AddRange(new object[] { resources.GetString("BindingSideComboBox.Items"), resources.GetString("BindingSideComboBox.Items1"), resources.GetString("BindingSideComboBox.Items2") });
			BindingSideComboBox.Name = "BindingSideComboBox";
			BindingSideComboBox.SelectedIndexChanged += OnBindingSideComboBoxSelectedIndexChanged;
			// 
			// BindingSideLabel
			// 
			resources.ApplyResources(BindingSideLabel, "BindingSideLabel");
			Panel1TableLayoutPanel.SetColumnSpan(BindingSideLabel, 2);
			BindingSideLabel.Name = "BindingSideLabel";
			// 
			// CultureDependingPublishedTextBox
			// 
			resources.ApplyResources(CultureDependingPublishedTextBox, "CultureDependingPublishedTextBox");
			Panel1TableLayoutPanel.SetColumnSpan(CultureDependingPublishedTextBox, 2);
			CultureDependingPublishedTextBox.Name = "CultureDependingPublishedTextBox";
			CultureDependingPublishedTextBox.TextChanged += OnCultureDependingPublishedTextBoxTextChanged;
			// 
			// CultureDependingPublishedLabel
			// 
			resources.ApplyResources(CultureDependingPublishedLabel, "CultureDependingPublishedLabel");
			Panel1TableLayoutPanel.SetColumnSpan(CultureDependingPublishedLabel, 2);
			CultureDependingPublishedLabel.Name = "CultureDependingPublishedLabel";
			// 
			// TitleLabel
			// 
			resources.ApplyResources(TitleLabel, "TitleLabel");
			Panel1TableLayoutPanel.SetColumnSpan(TitleLabel, 2);
			TitleLabel.Name = "TitleLabel";
			// 
			// TitleTextBox
			// 
			resources.ApplyResources(TitleTextBox, "TitleTextBox");
			Panel1TableLayoutPanel.SetColumnSpan(TitleTextBox, 2);
			TitleTextBox.Name = "TitleTextBox";
			TitleTextBox.TextChanged += OnTitleTextBoxTextChanged;
			// 
			// PublishedLabel
			// 
			resources.ApplyResources(PublishedLabel, "PublishedLabel");
			Panel1TableLayoutPanel.SetColumnSpan(PublishedLabel, 2);
			PublishedLabel.Name = "PublishedLabel";
			// 
			// AuthorLabel
			// 
			resources.ApplyResources(AuthorLabel, "AuthorLabel");
			Panel1TableLayoutPanel.SetColumnSpan(AuthorLabel, 2);
			AuthorLabel.Name = "AuthorLabel";
			// 
			// SearchOnBrowserButton
			// 
			resources.ApplyResources(SearchOnBrowserButton, "SearchOnBrowserButton");
			SearchOnBrowserButton.Name = "SearchOnBrowserButton";
			SearchOnBrowserButton.UseVisualStyleBackColor = true;
			SearchOnBrowserButton.Click += OnSearchOnBrowserButtonClick;
			// 
			// AuthorComboBox
			// 
			resources.ApplyResources(AuthorComboBox, "AuthorComboBox");
			Panel1TableLayoutPanel.SetColumnSpan(AuthorComboBox, 2);
			AuthorComboBox.DropDownHeight = 256;
			AuthorComboBox.FormattingEnabled = true;
			AuthorComboBox.Name = "AuthorComboBox";
			AuthorComboBox.TextChanged += OnAuthorComboBoxTextChanged;
			// 
			// PublishedDateTimePicker
			// 
			resources.ApplyResources(PublishedDateTimePicker, "PublishedDateTimePicker");
			PublishedDateTimePicker.Checked = false;
			PublishedDateTimePicker.Name = "PublishedDateTimePicker";
			PublishedDateTimePicker.ShowCheckBox = true;
			PublishedDateTimePicker.ValueChanged += OnPublishedDateTimePickerValueChanged;
			// 
			// Panel2TableLayoutPanel
			// 
			resources.ApplyResources(Panel2TableLayoutPanel, "Panel2TableLayoutPanel");
			Panel2TableLayoutPanel.Controls.Add(ThumbnailLabel, 0, 0);
			Panel2TableLayoutPanel.Controls.Add(ThumbnailEditOKButton, 1, 4);
			Panel2TableLayoutPanel.Controls.Add(ThumbnailEditCancelButton, 2, 4);
			Panel2TableLayoutPanel.Controls.Add(ThumbnailEditButton, 2, 3);
			Panel2TableLayoutPanel.Controls.Add(ThumbnailClipper, 0, 1);
			Panel2TableLayoutPanel.Controls.Add(MagnifyRatioNumericUpDown, 2, 2);
			Panel2TableLayoutPanel.Controls.Add(SizeLabel, 0, 2);
			Panel2TableLayoutPanel.Controls.Add(ThumbnailMaximizeButton, 0, 3);
			Panel2TableLayoutPanel.Name = "Panel2TableLayoutPanel";
			// 
			// ThumbnailLabel
			// 
			resources.ApplyResources(ThumbnailLabel, "ThumbnailLabel");
			Panel2TableLayoutPanel.SetColumnSpan(ThumbnailLabel, 3);
			ThumbnailLabel.Name = "ThumbnailLabel";
			// 
			// ThumbnailEditOKButton
			// 
			resources.ApplyResources(ThumbnailEditOKButton, "ThumbnailEditOKButton");
			ThumbnailEditOKButton.Name = "ThumbnailEditOKButton";
			ThumbnailEditOKButton.UseVisualStyleBackColor = true;
			ThumbnailEditOKButton.Click += OnOKButtonClick;
			// 
			// ThumbnailEditCancelButton
			// 
			resources.ApplyResources(ThumbnailEditCancelButton, "ThumbnailEditCancelButton");
			ThumbnailEditCancelButton.Name = "ThumbnailEditCancelButton";
			ThumbnailEditCancelButton.UseVisualStyleBackColor = true;
			ThumbnailEditCancelButton.Click += OnCancelButtonClick;
			// 
			// ThumbnailEditButton
			// 
			resources.ApplyResources(ThumbnailEditButton, "ThumbnailEditButton");
			ThumbnailEditButton.Name = "ThumbnailEditButton";
			ThumbnailEditButton.UseVisualStyleBackColor = true;
			ThumbnailEditButton.Click += OnEditButtonClick;
			// 
			// ThumbnailClipper
			// 
			ThumbnailClipper.AllowDrop = true;
			Panel2TableLayoutPanel.SetColumnSpan(ThumbnailClipper, 3);
			resources.ApplyResources(ThumbnailClipper, "ThumbnailClipper");
			ThumbnailClipper.Name = "ThumbnailClipper";
			ThumbnailClipper.PropertyChanged += OnThumbnailClipperPropertyChanged;
			ThumbnailClipper.DragDrop += OnThumbnailClipperDragDrop;
			ThumbnailClipper.DragEnter += OnThumbnailClipperDragEnter;
			// 
			// MagnifyRatioNumericUpDown
			// 
			resources.ApplyResources(MagnifyRatioNumericUpDown, "MagnifyRatioNumericUpDown");
			MagnifyRatioNumericUpDown.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
			MagnifyRatioNumericUpDown.Name = "MagnifyRatioNumericUpDown";
			MagnifyRatioNumericUpDown.Suffix = " %";
			MagnifyRatioNumericUpDown.Value = new decimal(new int[] { 100, 0, 0, 0 });
			MagnifyRatioNumericUpDown.ValueChanged += OnMagnifyRatioNumericUpDownValueChanged;
			// 
			// SizeLabel
			// 
			resources.ApplyResources(SizeLabel, "SizeLabel");
			Panel2TableLayoutPanel.SetColumnSpan(SizeLabel, 2);
			SizeLabel.Name = "SizeLabel";
			// 
			// ThumbnailMaximizeButton
			// 
			resources.ApplyResources(ThumbnailMaximizeButton, "ThumbnailMaximizeButton");
			ThumbnailMaximizeButton.Name = "ThumbnailMaximizeButton";
			Panel2TableLayoutPanel.SetRowSpan(ThumbnailMaximizeButton, 2);
			ThumbnailMaximizeButton.UseVisualStyleBackColor = true;
			ThumbnailMaximizeButton.Click += OnThumbnailMaximizeButtonClick;
			// 
			// MainSplitContainer
			// 
			resources.ApplyResources(MainSplitContainer, "MainSplitContainer");
			MainSplitContainer.Name = "MainSplitContainer";
			// 
			// MainSplitContainer.Panel1
			// 
			MainSplitContainer.Panel1.Controls.Add(Panel1TableLayoutPanel);
			// 
			// MainSplitContainer.Panel2
			// 
			MainSplitContainer.Panel2.Controls.Add(Panel2TableLayoutPanel);
			// 
			// DocumentView
			// 
			resources.ApplyResources(this, "$this");
			AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
			Controls.Add(MainSplitContainer);
			HideOnClose = true;
			Name = "DocumentView";
			Panel1TableLayoutPanel.ResumeLayout(false);
			Panel1TableLayoutPanel.PerformLayout();
			Panel2TableLayoutPanel.ResumeLayout(false);
			Panel2TableLayoutPanel.PerformLayout();
			((System.ComponentModel.ISupportInitialize)MagnifyRatioNumericUpDown).EndInit();
			MainSplitContainer.Panel1.ResumeLayout(false);
			MainSplitContainer.Panel2.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)MainSplitContainer).EndInit();
			MainSplitContainer.ResumeLayout(false);
			ResumeLayout(false);
		}

		#endregion

		System.Windows.Forms.TextBox TitleTextBox;
		System.Windows.Forms.ComboBox AuthorComboBox;
		System.Windows.Forms.DateTimePicker PublishedDateTimePicker;
		System.Windows.Forms.Label SizeLabel;
		private System.Windows.Forms.ComboBox BindingSideComboBox;
		private System.Windows.Forms.Label ThumbnailLabel;
		private System.Windows.Forms.TextBox CultureDependingPublishedTextBox;
		private System.Windows.Forms.Button ThumbnailEditButton;
		private System.Windows.Forms.SplitContainer MainSplitContainer;
		private System.Windows.Forms.Button ThumbnailEditOKButton;
		private Controls.SuffixNumericUpDown MagnifyRatioNumericUpDown;
		private System.Windows.Forms.Button ThumbnailEditCancelButton;
		private Controls.ImageClipper ThumbnailClipper;
		private System.Windows.Forms.Button ThumbnailMaximizeButton;
	}
}