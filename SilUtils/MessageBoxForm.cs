using System;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
#if !MONO

#endif

namespace SIL.Utils
{
	/* The code in this file is based on:
	* FlexibleMessageBox – A flexible replacement for the .NET MessageBox
	* 
	*  Author:         Jörg Reichert (public@jreichert.de)
	*  Contributors:   Thanks to: David Hall, Roink
	*  Version:        1.3
	*  Published at:   http://www.codeproject.com/Articles/601900/FlexibleMessageBox
	*  
	************************************************************************************************************
	* Features:
	*  - It is small, only one source file, which could be added easily to each solution 
	*  - It can be resized and the content is correctly word-wrapped
	*  - It tries to auto-size the width to show the longest text row
	*  - It never exceeds the current desktop working area
	*  - It displays a vertical scrollbar when needed
	*  - It does support hyperlinks in text
	* 
	*  Because the interface is identical to MessageBox, you can add this single source file to your project 
	*  and use the FlexibleMessageBox almost everywhere you use a standard MessageBox. 
	*  The goal was NOT to produce as many features as possible but to provide a simple replacement to fit my 
	*  own needs. Feel free to add additional features on your own, but please leave my credits in this class.
	* 
	************************************************************************************************************
	*  THE SOFTWARE IS PROVIDED BY THE AUTHOR "AS IS", WITHOUT WARRANTY
	*  OF ANY KIND, EXPRESS OR IMPLIED. IN NO EVENT SHALL THE AUTHOR BE
	*  LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY ARISING FROM,
	*  OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OF THIS
	*  SOFTWARE.
	*  
	************************************************************************************************************
	* History:
	*  Version 1.3 - 19.December 2014
	*  - Added refactoring function GetButtonText()
	*  - Used CurrentUICulture instead of InstalledUICulture
	*  - Added more button localizations. Supported languages are now: ENGLISH, GERMAN, SPANISH, ITALIAN
	*  - Added standard MessageBox handling for "copy to clipboard" with <Ctrl> + <C> and <Ctrl> + <Insert>
	*  - Tab handling is now corrected (only tabbing over the visible buttons)
	*  - Added standard MessageBox handling for ALT-Keyboard shortcuts
	*  - SetDialogSizes: Refactored completely: Corrected sizing and added caption driven sizing
	* 
	*  Version 1.2 - 10.August 2013
	*   - Do not ShowInTaskbar anymore (original MessageBox is also hidden in taskbar)
	*   - Added handling for Escape-Button
	*   - Adapted top right close button (red X) to behave like MessageBox (but hidden instead of deactivated)
	* 
	*  Version 1.1 - 14.June 2013
	*   - Some Refactoring
	*   - Added internal form class
	*   - Added missing code comments, etc.
	*  
	*  Version 1.0 - 15.April 2013
	*   - Initial Version
	*/

	public class MessageBoxForm : Form
	{       
		#region Public statics

		/// <summary>
		/// Defines the maximum width for all instances in percent of the working area.
		/// 
		/// Allowed values are 0.2 - 1.0 where: 
		/// 0.2 means:  The FlexibleMessageBox can be at most 20 percent of the working width.
		/// 1.0 means:  The FlexibleMessageBox can be as wide as the working area.
		/// 
		/// Default is: 30% of the working area width.
		/// </summary>
		public static double MAX_WIDTH_FACTOR = 0.3;

		/// <summary>
		/// Defines the maximum height for all FlexibleMessageBox instances in percent of the working area.
		/// 
		/// Allowed values are 0.2 - 1.0 where: 
		/// 0.2 means:  The FlexibleMessageBox can be at most 20 percent of the working height.
		/// 1.0 means:  The FlexibleMessageBox can be as high as the working area.
		/// 
		/// Default is: 50% of the working area height.
		/// </summary>
		public static double MAX_HEIGHT_FACTOR = 0.5;

		/// <summary>
		/// Defines the font for all FlexibleMessageBox instances.
		/// 
		/// Default is: SystemFonts.MessageBoxFont
		/// </summary>
		public static Font FONT = SystemFonts.MessageBoxFont;

		#endregion

		#region MessageBoxStrings stuff
#if !MONO
		[DllImport("user32.dll", CharSet = CharSet.Auto)]
		static extern int LoadString(IntPtr hInstance, uint uID, StringBuilder lpBuffer, int nBufferMax);
		[DllImport("kernel32")]
		static extern IntPtr LoadLibrary(string lpFileName);
#endif

		private const uint OK_CAPTION = 800;
		private const uint CANCEL_CAPTION = 801;
		private const uint ABORT_CAPTION = 802;
		private const uint RETRY_CAPTION = 803;
		private const uint IGNORE_CAPTION = 804;
		private const uint YES_CAPTION = 805;
		private const uint NO_CAPTION = 806;

		
		/// <summary>
		/// Gets the button text for the CurrentUICulture language.
		/// Note: The fallback language is English
		/// </summary>
		/// <param name="buttonId">The ID of the button.</param>
		/// <returns>The button text</returns>
		private string GetButtonText(ButtonID buttonId)
		{
			uint button;
			switch (buttonId)
			{
				case ButtonID.OK: button = OK_CAPTION; break;
				case ButtonID.CANCEL: button = CANCEL_CAPTION; break;
				case ButtonID.YES: button = YES_CAPTION; break;
				case ButtonID.NO: button = NO_CAPTION; break;
				case ButtonID.ABORT: button = ABORT_CAPTION; break;
				case ButtonID.RETRY: button = RETRY_CAPTION; break;
				case ButtonID.IGNORE: button = IGNORE_CAPTION; break;
				default:
					throw new ArgumentOutOfRangeException(nameof(buttonId), buttonId, null);
			}

#if !MONO // TODO: If we ever come up with a Mono version of Glyssen, need to figure out the right way to get the MessageBox button labels.
			StringBuilder sb = new StringBuilder(256);

			try
			{
				IntPtr user32 = LoadLibrary(Environment.SystemDirectory + "\\User32.dll");
				LoadString(user32, button, sb, sb.Capacity);
			}
			catch (Exception e)
			{
				Debug.Fail(e.Message);
				// In production, if the call to load the DLL or get the button's label fail,
				// we'll just take the hard-coded (English) versions from the switch below.
			}
			if (sb.Length > 0 && sb[0] == '&')
				sb.Remove(0, 1);
			if (sb.Length > 0)
				return sb.ToString();
#endif

			var buttonTextArrayIndex = Convert.ToInt32(buttonId);

			// Try to evaluate the language. If this fails, the fallback language English will be used
			if (!Enum.TryParse(CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, out TwoLetterISOLanguageID languageId))
				languageId = TwoLetterISOLanguageID.en;

			switch (languageId)
			{
				case TwoLetterISOLanguageID.de: return BUTTON_TEXTS_GERMAN_DE[buttonTextArrayIndex];
				case TwoLetterISOLanguageID.es: return BUTTON_TEXTS_SPANISH_ES[buttonTextArrayIndex];
				case TwoLetterISOLanguageID.it: return BUTTON_TEXTS_ITALIAN_IT[buttonTextArrayIndex];

				default: return BUTTON_TEXTS_ENGLISH_EN[buttonTextArrayIndex];
			}
		}
		#endregion

		#region Form-Designer generated code

		/// <summary>
		/// Erforderliche Designervariable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Verwendete Ressourcen bereinigen.
		/// </summary>
		/// <param name="disposing">True, wenn verwaltete Ressourcen gelöscht werden sollen; andernfalls False.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}

			base.Dispose(disposing);
		}

		/// <summary>
		/// Erforderliche Methode für die Designerunterstützung.
		/// Der Inhalt der Methode darf nicht mit dem Code-Editor geändert werden.
		/// </summary>
		private void InitializeComponent()
		{
			components = new System.ComponentModel.Container();
			button1 = new System.Windows.Forms.Button();
			richTextBoxMessage = new System.Windows.Forms.RichTextBox();
			MessageBoxFormBindingSource = new System.Windows.Forms.BindingSource(components);
			panel1 = new System.Windows.Forms.Panel();
			pictureBoxForIcon = new System.Windows.Forms.PictureBox();
			button2 = new System.Windows.Forms.Button();
			button3 = new System.Windows.Forms.Button();
			((System.ComponentModel.ISupportInitialize)(MessageBoxFormBindingSource)).BeginInit();
			panel1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(pictureBoxForIcon)).BeginInit();
			SuspendLayout();
			// 
			// button1
			// 
			button1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			button1.AutoSize = true;
			button1.DialogResult = System.Windows.Forms.DialogResult.OK;
			button1.Location = new System.Drawing.Point(11, 67);
			button1.MinimumSize = new System.Drawing.Size(0, 24);
			button1.Name = "button1";
			button1.Size = new System.Drawing.Size(75, 24);
			button1.TabIndex = 2;
			button1.Text = "OK";
			button1.UseVisualStyleBackColor = true;
			button1.Visible = false;
			// 
			// richTextBoxMessage
			// 
			richTextBoxMessage.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
					| System.Windows.Forms.AnchorStyles.Left)
				| System.Windows.Forms.AnchorStyles.Right)));
			richTextBoxMessage.BackColor = System.Drawing.Color.White;
			richTextBoxMessage.BorderStyle = System.Windows.Forms.BorderStyle.None;
			richTextBoxMessage.DataBindings.Add(new System.Windows.Forms.Binding("Text", MessageBoxFormBindingSource, "MessageText", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
			richTextBoxMessage.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			richTextBoxMessage.Location = new System.Drawing.Point(50, 26);
			richTextBoxMessage.Margin = new System.Windows.Forms.Padding(0);
			richTextBoxMessage.Name = "richTextBoxMessage";
			richTextBoxMessage.ReadOnly = true;
			richTextBoxMessage.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
			richTextBoxMessage.Size = new System.Drawing.Size(200, 20);
			richTextBoxMessage.TabIndex = 0;
			richTextBoxMessage.TabStop = false;
			richTextBoxMessage.Text = "<Message>";
			richTextBoxMessage.LinkClicked += new System.Windows.Forms.LinkClickedEventHandler(richTextBoxMessage_LinkClicked);
			// 
			// panel1
			// 
			panel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
					| System.Windows.Forms.AnchorStyles.Left)
				| System.Windows.Forms.AnchorStyles.Right)));
			panel1.BackColor = System.Drawing.Color.White;
			panel1.Controls.Add(pictureBoxForIcon);
			panel1.Controls.Add(richTextBoxMessage);
			panel1.Location = new System.Drawing.Point(-3, -4);
			panel1.Name = "panel1";
			panel1.Size = new System.Drawing.Size(268, 59);
			panel1.TabIndex = 1;
			// 
			// pictureBoxForIcon
			// 
			pictureBoxForIcon.BackColor = System.Drawing.Color.Transparent;
			pictureBoxForIcon.Location = new System.Drawing.Point(15, 19);
			pictureBoxForIcon.Name = "pictureBoxForIcon";
			pictureBoxForIcon.Size = new System.Drawing.Size(32, 32);
			pictureBoxForIcon.TabIndex = 8;
			pictureBoxForIcon.TabStop = false;
			// 
			// button2
			// 
			button2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			button2.DialogResult = System.Windows.Forms.DialogResult.OK;
			button2.Location = new System.Drawing.Point(92, 67);
			button2.MinimumSize = new System.Drawing.Size(0, 24);
			button2.Name = "button2";
			button2.Size = new System.Drawing.Size(75, 24);
			button2.TabIndex = 3;
			button2.Text = "OK";
			button2.UseVisualStyleBackColor = true;
			button2.Visible = false;
			// 
			// button3
			// 
			button3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			button3.AutoSize = true;
			button3.DialogResult = System.Windows.Forms.DialogResult.OK;
			button3.Location = new System.Drawing.Point(173, 67);
			button3.MinimumSize = new System.Drawing.Size(0, 24);
			button3.Name = "button3";
			button3.Size = new System.Drawing.Size(75, 24);
			button3.TabIndex = 0;
			button3.Text = "OK";
			button3.UseVisualStyleBackColor = true;
			button3.Visible = false;
			// 
			// MessageBoxForm
			// 
			AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			ClientSize = new System.Drawing.Size(260, 102);
			Controls.Add(button3);
			Controls.Add(button2);
			Controls.Add(panel1);
			Controls.Add(button1);
			DataBindings.Add(new System.Windows.Forms.Binding("Text", MessageBoxFormBindingSource, "CaptionText", true));
			MaximizeBox = false;
			MinimizeBox = false;
			MinimumSize = new System.Drawing.Size(276, 140);
			Name = "MessageBoxForm";
			ShowIcon = false;
			SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show;
			StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			Text = "<Caption>";
			((System.ComponentModel.ISupportInitialize)(MessageBoxFormBindingSource)).EndInit();
			panel1.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(pictureBoxForIcon)).EndInit();
			ResumeLayout(false);
			PerformLayout();
		}

		private System.Windows.Forms.Button button1;
		private System.Windows.Forms.BindingSource MessageBoxFormBindingSource;
		private System.Windows.Forms.RichTextBox richTextBoxMessage;
		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.PictureBox pictureBoxForIcon;
		private System.Windows.Forms.Button button2;
		private System.Windows.Forms.Button button3;

		#endregion

		#region Private constants

		//These separators are used for the "copy to clipboard" standard operation, triggered by Ctrl + C (behavior and clipboard format is like in a standard MessageBox)
		private static readonly String STANDARD_MESSAGEBOX_SEPARATOR_LINES = "---------------------------\n";
		private static readonly String STANDARD_MESSAGEBOX_SEPARATOR_SPACES = "   ";

		//These are the possible buttons (in a standard MessageBox)
		private enum ButtonID
		{
			OK = 0,
			CANCEL,
			YES,
			NO,
			ABORT,
			RETRY,
			IGNORE
		};

		//These are the buttons texts for different languages. 
		//If you want to add a new language, add it here and in the GetButtonText-Function
		private enum TwoLetterISOLanguageID
		{
			en,
			de,
			es,
			it
		};

		private static readonly String[] BUTTON_TEXTS_ENGLISH_EN = {"OK", "Cancel", "&Yes", "&No", "&Abort", "&Retry", "&Ignore"}; //Note: This is also the fallback language
		private static readonly String[] BUTTON_TEXTS_GERMAN_DE = {"OK", "Abbrechen", "&Ja", "&Nein", "&Abbrechen", "&Wiederholen", "&Ignorieren"};
		private static readonly String[] BUTTON_TEXTS_SPANISH_ES = {"Aceptar", "Cancelar", "&Sí", "&No", "&Abortar", "&Reintentar", "&Ignorar"};
		private static readonly String[] BUTTON_TEXTS_ITALIAN_IT = {"OK", "Annulla", "&Sì", "&No", "&Interrompi", "&Riprova", "&Ignora"};

		#endregion

		#region Private members

		private MessageBoxDefaultButton defaultButton;
		private int visibleButtonsCount;

		#endregion

		#region Constructors
		/// <summary>
		/// Initializes a new instance of the <see cref="MessageBoxForm"/> class.
		/// </summary>
		private MessageBoxForm()
		{
			InitializeComponent();

			KeyPreview = true;
		}

		/// <summary>
		/// Constructs the specified message box.
		/// </summary>
		/// <param name="text">The text.</param>
		/// <param name="caption">The caption.</param>
		/// <param name="buttons">The buttons.</param>
		/// <param name="icon">The icon.</param>
		/// <param name="defaultButton">The default button.</param>
		public MessageBoxForm(string text, string caption, MessageBoxButtons buttons = MessageBoxButtons.OK, MessageBoxIcon icon = MessageBoxIcon.Warning,
			MessageBoxDefaultButton defaultButton = MessageBoxDefaultButton.Button1) : this()
		{
			ShowInTaskbar = false;
			CaptionText = caption;
			MessageText = text;

			MessageBoxFormBindingSource.DataSource = this;

			//Set the buttons visibilities and texts. Also set a default button.
			SetDialogButtons(buttons, defaultButton);

			//Set the dialogs icon. When no icon is used: Correct placement and width of rich text box.
			SetDialogIcon(icon);

			//Set the font for all controls
			Font = FONT;
			richTextBoxMessage.Font = FONT;

			//Calculate the dialogs start size (Try to auto-size width to show longest text row). Also set the maximum dialog size. 
			SetDialogSizes(text, caption);
		}
		#endregion

		#region Private helper functions

		/// <summary>
		/// Gets the string rows.
		/// </summary>
		/// <param name="message">The message.</param>
		/// <returns>The string rows as 1-dimensional array</returns>
		private static string[] GetStringRows(string message)
		{
			if (string.IsNullOrEmpty(message)) return null;

			var messageRows = message.Split(new[] {'\n'}, StringSplitOptions.None);
			return messageRows;
		}

		/// <summary>
		/// Ensure the given working area factor in the range of  0.2 - 1.0 where: 
		/// 
		/// 0.2 means:  20 percent of the working area height or width.
		/// 1.0 means:  100 percent of the working area height or width.
		/// </summary>
		/// <param name="workingAreaFactor">The given working area factor.</param>
		/// <returns>The corrected given working area factor.</returns>
		private static double GetCorrectedWorkingAreaFactor(double workingAreaFactor)
		{
			const double MIN_FACTOR = 0.2;
			const double MAX_FACTOR = 1.0;

			if (workingAreaFactor < MIN_FACTOR) return MIN_FACTOR;
			if (workingAreaFactor > MAX_FACTOR) return MAX_FACTOR;

			return workingAreaFactor;
		}

		/// <summary>
		/// Calculate the dialogs start size (Try to auto-size width to show longest text row).
		/// Also set the maximum dialog size. 
		/// </summary>
		/// <param name="text">The text (the longest text row is used to calculate the dialog width).</param>
		/// <param name="caption">The caption (this can also affect the dialog width).</param>
		private void SetDialogSizes(string text, string caption)
		{
			//First set the bounds for the maximum dialog size
			MaximumSize = new Size(Convert.ToInt32(SystemInformation.WorkingArea.Width * GetCorrectedWorkingAreaFactor(MAX_WIDTH_FACTOR)),
				Convert.ToInt32(SystemInformation.WorkingArea.Height * GetCorrectedWorkingAreaFactor(MAX_HEIGHT_FACTOR)));

			//Get rows. Exit if there are no rows to render...
			var stringRows = GetStringRows(text);
			if (stringRows == null) return;

			//Calculate whole text height
			var textHeight = TextRenderer.MeasureText(text, FONT, new Size(richTextBoxMessage.ClientRectangle.Width, Int32.MaxValue),
				TextFormatFlags.WordBreak).Height;

			//Calculate width for longest text line
			const int SCROLLBAR_WIDTH_OFFSET = 15;
			var longestTextRowWidth = stringRows.Max(textForRow => TextRenderer.MeasureText(textForRow, FONT).Width);
			var captionWidth = TextRenderer.MeasureText(caption, SystemFonts.CaptionFont).Width;
			var textWidth = Math.Max(longestTextRowWidth + SCROLLBAR_WIDTH_OFFSET, captionWidth);

			//Calculate margins
			var marginWidth = Width - richTextBoxMessage.Width;
			var marginHeight = Height - richTextBoxMessage.Height;

			//Set calculated dialog size (if the calculated values exceed the maximums, they were cut by windows forms automatically)
			Size = new Size(textWidth + marginWidth,
				textHeight + marginHeight);
		}

		/// <summary>
		/// Set the dialogs icon. 
		/// When no icon is used: Correct placement and width of rich text box.
		/// </summary>
		/// <param name="icon">The MessageBoxIcon.</param>
		private void SetDialogIcon(MessageBoxIcon icon)
		{
			switch (icon)
			{
				case MessageBoxIcon.Information:
					pictureBoxForIcon.Image = SystemIcons.Information.ToBitmap();
					break;
				case MessageBoxIcon.Warning:
					pictureBoxForIcon.Image = SystemIcons.Warning.ToBitmap();
					break;
				case MessageBoxIcon.Error:
					pictureBoxForIcon.Image = SystemIcons.Error.ToBitmap();
					break;
				case MessageBoxIcon.Question:
					pictureBoxForIcon.Image = SystemIcons.Question.ToBitmap();
					break;
				default:
					//When no icon is used: Correct placement and width of rich text box.
					pictureBoxForIcon.Visible = false;
					richTextBoxMessage.Left -= pictureBoxForIcon.Width;
					richTextBoxMessage.Width += pictureBoxForIcon.Width;
					break;
			}
		}

		/// <summary>
		/// Set dialog buttons visibilities and texts. 
		/// Also set a default button.
		/// </summary>
		/// <param name="buttons">The buttons.</param>
		/// <param name="defaultButton">The default button.</param>
		private void SetDialogButtons(MessageBoxButtons buttons, MessageBoxDefaultButton defaultButton)
		{
			//Set the buttons visibilities and texts
			switch (buttons)
			{
				case MessageBoxButtons.AbortRetryIgnore:
					visibleButtonsCount = 3;

					button1.Visible = true;
					button1.Text = GetButtonText(ButtonID.ABORT);
					button1.DialogResult = DialogResult.Abort;

					button2.Visible = true;
					button2.Text = GetButtonText(ButtonID.RETRY);
					button2.DialogResult = DialogResult.Retry;

					button3.Visible = true;
					button3.Text = GetButtonText(ButtonID.IGNORE);
					button3.DialogResult = DialogResult.Ignore;

					ControlBox = false;
					break;

				case MessageBoxButtons.OKCancel:
					visibleButtonsCount = 2;

					button2.Visible = true;
					button2.Text = GetButtonText(ButtonID.OK);
					button2.DialogResult = DialogResult.OK;

					button3.Visible = true;
					button3.Text = GetButtonText(ButtonID.CANCEL);
					button3.DialogResult = DialogResult.Cancel;

					CancelButton = button3;
					break;

				case MessageBoxButtons.RetryCancel:
					visibleButtonsCount = 2;

					button2.Visible = true;
					button2.Text = GetButtonText(ButtonID.RETRY);
					button2.DialogResult = DialogResult.Retry;

					button3.Visible = true;
					button3.Text = GetButtonText(ButtonID.CANCEL);
					button3.DialogResult = DialogResult.Cancel;

					CancelButton = button3;
					break;

				case MessageBoxButtons.YesNo:
					visibleButtonsCount = 2;

					button2.Visible = true;
					button2.Text = GetButtonText(ButtonID.YES);
					button2.DialogResult = DialogResult.Yes;

					button3.Visible = true;
					button3.Text = GetButtonText(ButtonID.NO);
					button3.DialogResult = DialogResult.No;

					ControlBox = false;
					break;

				case MessageBoxButtons.YesNoCancel:
					visibleButtonsCount = 3;

					button1.Visible = true;
					button1.Text = GetButtonText(ButtonID.YES);
					button1.DialogResult = DialogResult.Yes;

					button2.Visible = true;
					button2.Text = GetButtonText(ButtonID.NO);
					button2.DialogResult = DialogResult.No;

					button3.Visible = true;
					button3.Text = GetButtonText(ButtonID.CANCEL);
					button3.DialogResult = DialogResult.Cancel;

					CancelButton = button3;
					break;

				case MessageBoxButtons.OK:
				default:
					visibleButtonsCount = 1;
					button3.Visible = true;
					button3.Text = GetButtonText(ButtonID.OK);
					button3.DialogResult = DialogResult.OK;

					CancelButton = button3;
					break;
			}

			// Set default button (used in OnShown)
			this.defaultButton = defaultButton;
		}

		#endregion

		#region Private event handlers
		/// <summary>
		/// Handles the Shown event of the MessageBoxForm control.
		/// </summary>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		protected override void OnShown(EventArgs e)
		{
			base.OnShown(e);
		
			int buttonIndexToFocus = 1;
			Button buttonToFocus;

			//Set the default button...
			switch (defaultButton)
			{
				case MessageBoxDefaultButton.Button1:
				default:
					buttonIndexToFocus = 1;
					break;
				case MessageBoxDefaultButton.Button2:
					buttonIndexToFocus = 2;
					break;
				case MessageBoxDefaultButton.Button3:
					buttonIndexToFocus = 3;
					break;
			}

			if (buttonIndexToFocus > visibleButtonsCount) buttonIndexToFocus = visibleButtonsCount;

			if (buttonIndexToFocus == 3)
			{
				buttonToFocus = button3;
			}
			else if (buttonIndexToFocus == 2)
			{
				buttonToFocus = button2;
			}
			else
			{
				buttonToFocus = button1;
			}

			buttonToFocus.Focus();
		}

		/// <summary>
		/// Handles the LinkClicked event of the richTextBoxMessage control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.Windows.Forms.LinkClickedEventArgs"/> instance containing the event data.</param>
		private void richTextBoxMessage_LinkClicked(object sender, LinkClickedEventArgs e)
		{
			try
			{
				Cursor.Current = Cursors.WaitCursor;
				Process.Start(e.LinkText);
			}
			finally
			{
				Cursor.Current = Cursors.Default;
			}

		}

		/// <summary>
		/// Handles the KeyUp event of the richTextBoxMessage control.
		/// </summary>
		/// <param name="e">The <see cref="System.Windows.Forms.KeyEventArgs"/> instance containing the event data.</param>
		protected override void OnKeyUp(KeyEventArgs e)
		{
			base.OnKeyUp(e);
		
			//Handle standard key strikes for clipboard copy: "Ctrl + C" and "Ctrl + Insert"
			if (e.Control && (e.KeyCode == Keys.C || e.KeyCode == Keys.Insert))
			{
				var buttonsTextLine = (button1.Visible ? button1.Text + STANDARD_MESSAGEBOX_SEPARATOR_SPACES : string.Empty)
					+ (button2.Visible ? button2.Text + STANDARD_MESSAGEBOX_SEPARATOR_SPACES : string.Empty)
					+ (button3.Visible ? button3.Text + STANDARD_MESSAGEBOX_SEPARATOR_SPACES : string.Empty);

				//Build same clipboard text like the standard .Net MessageBox
				var textForClipboard = STANDARD_MESSAGEBOX_SEPARATOR_LINES
					+ Text + Environment.NewLine
					+ STANDARD_MESSAGEBOX_SEPARATOR_LINES
					+ richTextBoxMessage.Text + Environment.NewLine
					+ STANDARD_MESSAGEBOX_SEPARATOR_LINES
					+ buttonsTextLine.Replace("&", string.Empty) + Environment.NewLine
					+ STANDARD_MESSAGEBOX_SEPARATOR_LINES;

				//Set text in clipboard
				Clipboard.SetText(textForClipboard);
			}
		}

		#endregion

		#region Properties (only used for binding)

		/// <summary>
		/// The Caption.
		/// </summary>
		public string CaptionText { get; set; }

		/// <summary>
		/// The message text to display in the MessageBoxForm.
		/// </summary>
		public string MessageText { get; set; }

		#endregion
	} //class MessageBoxForm

}