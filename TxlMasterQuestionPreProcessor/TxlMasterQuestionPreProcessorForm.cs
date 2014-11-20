using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows.Forms;
using SIL.ScriptureUtils;
using SIL.Transcelerator;
using SIL.Utils;

namespace SIL.TxlMasterQuestionPreProcessor
{
    public partial class TxlMasterQuestionPreProcessorForm : Form
    {
        private readonly IScrVers m_masterVersification;

        public TxlMasterQuestionPreProcessorForm(IScrVers englishVersification)
        {
            InitializeComponent();

            m_masterVersification = englishVersification;

            txtSfmQuestionFile.Text = @"c:\Projects\Transcelerator\TxlMasterQuestionPreProcessor\QTTallBooks.sfm";
            txtXmlQuestionFile.Text = Path.Combine(@"c:\Projects\Transcelerator\Transcelerator", TxlCore.questionsFilename);
        }

        private void btnGenerateMasterQuestionFile_Click(object sender, EventArgs e)
        {
            string destQuestionsFilename = chkWriteTempFile.Checked ? Path.ChangeExtension(txtSfmQuestionFile.Text, "xml") :
                txtXmlQuestionFile.Text;
            string alternativesFilename = Path.Combine(Path.GetDirectoryName(txtSfmQuestionFile.Text) ?? string.Empty,
                Path.ChangeExtension(Path.GetFileNameWithoutExtension(txtSfmQuestionFile.Text) + " - AlternateFormOverrides", "xml"));
            FileInfo finfoSfmQuestions = new FileInfo(txtSfmQuestionFile.Text);
            FileInfo finfoXmlQuestions = new FileInfo(destQuestionsFilename);
            FileInfo finfoAlternatives = new FileInfo(alternativesFilename);

            if ((!finfoXmlQuestions.Exists ||
                (finfoSfmQuestions.Exists && finfoXmlQuestions.LastWriteTimeUtc < finfoSfmQuestions.LastWriteTimeUtc) ||
                (finfoSfmQuestions.Exists && finfoAlternatives.Exists && finfoXmlQuestions.LastWriteTimeUtc < finfoAlternatives.LastWriteTimeUtc)) ||
                MessageBox.Show("File " + destQuestionsFilename + " already exists and appears to be up-to-date relative to " + finfoSfmQuestions.Name +
                " and " + finfoAlternatives.Name + ". Do you want to re-generate it anyway?", Text, MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                if (!finfoSfmQuestions.Exists)
                    MessageBox.Show("Required file not found:\n" + txtSfmQuestionFile.Text, Text);
                if (!finfoAlternatives.Exists)
                    alternativesFilename = null;
                QuestionSfmFileAccessor.Generate(txtSfmQuestionFile.Text, alternativesFilename, destQuestionsFilename);
            }

            MessageBox.Show(this, "Done!", TxlMasterQuestionPreProcessorPlugin.pluginName);
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
			                using (var writer = new StreamWriter(txtSfmQuestionFile.Text, true))
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
    }
}
