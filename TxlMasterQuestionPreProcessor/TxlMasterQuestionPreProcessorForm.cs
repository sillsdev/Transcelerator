// ---------------------------------------------------------------------------------------------
#region // Copyright (c) 2023, SIL International.
// <copyright from='2018' to='2023' company='SIL International'>
//		Copyright (c) 2023, SIL International.   
//    
//		Distributable under the terms of the MIT License (http://sil.mit-license.org/)
// </copyright> 
#endregion
// 
// File: TxlMasterQuestionPreProcessorForm.cs
// ---------------------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows.Forms;
using Paratext.PluginInterfaces;
using SIL.Transcelerator;
using SIL.Transcelerator.Localization;
using SIL.Utils;
using File = System.IO.File;

namespace SIL.TxlMasterQuestionPreProcessor
{
	public partial class TxlMasterQuestionPreProcessorForm : Form
	{
		private readonly IVersification m_masterVersification;
		private readonly string m_sfmSourceLabelText;

		public TxlMasterQuestionPreProcessorForm(IVersification englishVersification)
		{
			InitializeComponent();

			m_masterVersification = englishVersification;
			m_sfmSourceLabelText = lblSource.Text;

			SetDefaultSfmSourceFile();
			txtXmlQuestionFile.Text = Path.Combine(@"c:\Projects\Transcelerator\Transcelerator", TxlData.kQuestionsFilename);
		}

		private void SetDefaultSfmSourceFile()
		{
			txtSourceFile.Text = @"c:\Projects\Transcelerator\TxlMasterQuestionPreProcessor\QTTallBooks.sfm";
		}

		private String ExpectedExtension => rdoSfmToXml.Checked ? ".sfm" : ".xml";
		private bool SourceHasExpectedExtension => Path.GetExtension(txtSourceFile.Text) == ExpectedExtension;
		private string DestinationDirectory
		{
			get
			{
				var baseFile = chkWriteTempFile.Checked ? txtSourceFile.Text : txtXmlQuestionFile.Text;
				return (String.IsNullOrWhiteSpace(baseFile)) ? String.Empty : Path.GetDirectoryName(baseFile);
			}
		}

		private void btnGenerate_Click(object sender, EventArgs e)
		{
			string missingFileName;
			btnGenerate.Enabled = false;
			string generatedFileName;
			using (new WaitCursor(this))
			{
				generatedFileName = rdoSfmToXml.Checked ? TryGenerateMasterQuestionFile(out missingFileName) : TryGenerateQuestionLocalizationsFile(out missingFileName);
			}
			if (generatedFileName != null)
				MessageBox.Show(this, "Created/Updated file:\n" + generatedFileName, TxlMasterQuestionPreProcessorPlugin.pluginName);
			else
				MessageBox.Show("Required file not found:\n" + missingFileName, Text);
			UpdateGenerateButtonEnabledState(null, null);
		}

		private string TryGenerateMasterQuestionFile(out string missingFileName)
		{
			missingFileName = null;
			string destQuestionsFilename = chkWriteTempFile.Checked ? Path.ChangeExtension(txtSourceFile.Text, "xml") :
				txtXmlQuestionFile.Text;
			string alternativesFilename = Path.Combine(Path.GetDirectoryName(txtSourceFile.Text) ?? string.Empty,
				Path.ChangeExtension(Path.GetFileNameWithoutExtension(txtSourceFile.Text) + " - AlternateFormOverrides", "xml"));
			FileInfo finfoSfmQuestions = new FileInfo(txtSourceFile.Text);
			FileInfo finfoXmlQuestions = new FileInfo(destQuestionsFilename);
			FileInfo finfoAlternatives = new FileInfo(alternativesFilename);

			if ((!finfoXmlQuestions.Exists ||
					(finfoSfmQuestions.Exists && finfoXmlQuestions.LastWriteTimeUtc < finfoSfmQuestions.LastWriteTimeUtc) ||
					(finfoSfmQuestions.Exists && finfoAlternatives.Exists && finfoXmlQuestions.LastWriteTimeUtc < finfoAlternatives.LastWriteTimeUtc)) ||
				MessageBox.Show("File " + destQuestionsFilename + " already exists and appears to be up-to-date relative to " + finfoSfmQuestions.Name +
					" and " + finfoAlternatives.Name + ". Do you want to re-generate it anyway?", Text, MessageBoxButtons.YesNo) == DialogResult.Yes)
			{
				if (!finfoSfmQuestions.Exists)
				{
					missingFileName = txtSourceFile.Text;
					return null;
				}
				if (!finfoAlternatives.Exists)
					alternativesFilename = null;
				QuestionSfmFileAccessor.Generate(txtSourceFile.Text, alternativesFilename, destQuestionsFilename);
				return destQuestionsFilename;
			}
			return null;
		}

		private string TryGenerateQuestionLocalizationsFile(out string missingFileName)
		{
			LocalizationsFileGenerator txlLocalizationManager;
			try
			{
				txlLocalizationManager = new LocalizationsFileGenerator(DestinationDirectory, txtLocale.Text);
			}
			catch (FileNotFoundException e)
			{
				missingFileName = e.FileName;
				return null;
			}

			var existingTranslationsFilename = String.IsNullOrWhiteSpace(txtSourceFile.Text) ? null : txtSourceFile.Text;
			var finfoExistingTxlTranslations = existingTranslationsFilename == null ? null : new FileInfo(txtSourceFile.Text);
			var finfoMasterQuestionFile = new FileInfo(txtXmlQuestionFile.Text);

			if (!finfoMasterQuestionFile.Exists)
			{
				missingFileName = txtXmlQuestionFile.Text;
				return null;
			}
			missingFileName = null;

			if ((!txlLocalizationManager.Exists ||
					(finfoExistingTxlTranslations != null &&
					(finfoExistingTxlTranslations.Exists && txlLocalizationManager.FileInfo.LastWriteTimeUtc > finfoExistingTxlTranslations.LastWriteTimeUtc)) ||
					(finfoMasterQuestionFile.Exists && txlLocalizationManager.FileInfo.LastWriteTimeUtc > finfoMasterQuestionFile.LastWriteTimeUtc)) ||
				MessageBox.Show($"File {txlLocalizationManager.FileName} already exists and appears to be up-to-date relative to {txtXmlQuestionFile.Text}" +
					(finfoExistingTxlTranslations == null ? "" : $" and {txtSourceFile.Text}") + ". Do you want to re-generate it anyway?", Text, MessageBoxButtons.YesNo) == DialogResult.Yes)
			{
				txlLocalizationManager.GenerateOrUpdateFromMasterQuestions(txtXmlQuestionFile.Text, existingTranslationsFilename, m_chkRetainOnlyTranslated.Checked);
				return txlLocalizationManager.FileName;
			}
			return null;
		}

		private void btnTextToSfm_Click(object sender, EventArgs e)
		{
			using (var dlg = new OpenFileDialog())
			{
				dlg.CheckFileExists = true;
				dlg.Multiselect = true;
				dlg.RestoreDirectory = true;
				dlg.Filter = "Standard Format Files (*.sfm)|*.sfm|All Supported Files (*.sfm;*.txt)|*.sfm;*.txt";
				dlg.InitialDirectory = Path.Combine(Path.Combine(
					Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "SoftDev"), "Transcelerator");
				if (dlg.ShowDialog() == DialogResult.OK)
				{
					StringBuilder sb = new StringBuilder("Finished! Questions written to the following files:");
					bool overwriteAll = false;
					bool skipAll = false;
					List<string> successfullyProcessedFiles = new List<string>(dlg.FileNames.Length);
					foreach (var filename in dlg.FileNames)
					{
						string srcFile;
						// Rename *.sfm.txt to *.sfm (MS Word insists on adding .txt extension when saving as plain text)
						if (filename.EndsWith(".sfm.txt"))
						{
							srcFile = filename.Remove(filename.Length - 4);
							if (File.Exists(srcFile))
								srcFile = filename;
							else
							{
								try
								{
									File.Move(filename, srcFile);
								}
								catch (Exception)
								{
									srcFile = filename;
								}
							}
						}
						else
							srcFile = filename;

						string fileToWrite = "QTT" + Path.GetFileName(srcFile);

						// Just in case rename failed above...
						if (fileToWrite.EndsWith(".sfm.txt"))
							fileToWrite = fileToWrite.Remove(filename.Length - 4);

						string directory = Path.GetDirectoryName(srcFile);
						if (directory != null)
							fileToWrite = Path.Combine(directory, fileToWrite);

						if (File.Exists(fileToWrite))
						{
							if (skipAll)
								continue;

							if (!overwriteAll)
							{
								using (var overwriteDlg = new ConfirmFileOverwriteDlg(fileToWrite,
									TxlMasterQuestionPreProcessorPlugin.pluginName, true))
								{
									if (overwriteDlg.ShowDialog(this) == DialogResult.Yes)
									{
										overwriteAll = overwriteDlg.ApplyToAll;
									}
									else
									{
										skipAll = overwriteDlg.ApplyToAll;
										continue;
									}
								}
							}
						}

						int problemsFound;
						using (var reader = new StreamReader(srcFile, Encoding.UTF8))
						{
							using (var writer = new StreamWriter(fileToWrite))
							{
								problemsFound = QuestionSfmFileAccessor.MakeStandardFormatQuestions(reader,
									writer, m_masterVersification);
							}
						}
						sb.Append("\n");
						sb.Append(fileToWrite);
						if (problemsFound > 0)
							sb.Append(string.Format(", with {0} problems!", problemsFound));
						else
							successfullyProcessedFiles.Add(fileToWrite);
					}

					if (successfullyProcessedFiles.Count == 0)
					{
						MessageBox.Show(sb.ToString(), TxlMasterQuestionPreProcessorPlugin.pluginName);
					}
					else
					{
						sb.Append("\n\n");
						sb.Append("Include questions from successfully processed files in master SF file?");
						if (MessageBox.Show(sb.ToString(), TxlMasterQuestionPreProcessorPlugin.pluginName,
							MessageBoxButtons.YesNo) == DialogResult.Yes)
						{
							using (var writer = new StreamWriter(txtSourceFile.Text, true))
							{
								foreach (string file in successfullyProcessedFiles)
								{
									using (var reader = new StreamReader(file, Encoding.UTF8))
									{
										writer.WriteLine(reader.ReadToEnd());
									}
								}
							}
						}
					}
				}
			}
		}

		private void HandleOptionChanged(object sender, EventArgs e)
		{
			if (rdoSfmToXml.Checked)
			{
				SetDefaultSfmSourceFile();
				lblLocale.Enabled = txtLocale.Enabled = false;
				lblSource.Text = m_sfmSourceLabelText;
				chkWriteTempFile.Enabled = true;
				m_chkRetainOnlyTranslated.Visible = false;
			}
			else
			{
				if (!SourceHasExpectedExtension)
					txtSourceFile.Text = String.Empty;
				lblLocale.Enabled = txtLocale.Enabled = true;
				lblSource.Text = "Existing Translations from Transcelerator (optional):";
				chkWriteTempFile.Enabled = chkWriteTempFile.Checked = false;
				m_chkRetainOnlyTranslated.Visible = true;
			}
		}

		private void UpdateGenerateButtonEnabledState(object sender, EventArgs e)
		{
			if (rdoSfmToXml.Checked)
			{
				btnGenerate.Enabled = !String.IsNullOrWhiteSpace(txtSourceFile.Text) && SourceHasExpectedExtension;
			}
			else
			{
				btnGenerate.Enabled = (String.IsNullOrWhiteSpace(txtSourceFile.Text) || SourceHasExpectedExtension) &&
					!String.IsNullOrWhiteSpace(txtLocale.Text);
			}
			btnGenerate.Enabled &= Directory.Exists(DestinationDirectory);
		}

		private void btnNavigateToSourceFile_Click(object sender, EventArgs e)
		{
			using (var dlg = new OpenFileDialog())
			{
				dlg.FileName = txtSourceFile.Text;
				dlg.DefaultExt = ExpectedExtension;
				if (dlg.ShowDialog() == DialogResult.OK)
					txtSourceFile.Text = dlg.FileName;
			}
		}
	}
}
