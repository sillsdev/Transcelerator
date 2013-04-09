using System;
using System.IO;
using System.Text;
using System.Windows.Forms;
using SIL.Transcelerator;
using SILUBS.SharedScrUtils;

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

            if (!finfoXmlQuestions.Exists ||
                (finfoSfmQuestions.Exists && finfoXmlQuestions.LastWriteTimeUtc < finfoSfmQuestions.LastWriteTimeUtc) ||
                (finfoSfmQuestions.Exists && finfoAlternatives.Exists && finfoXmlQuestions.LastWriteTimeUtc < finfoAlternatives.LastWriteTimeUtc))
            {
                if (!finfoSfmQuestions.Exists)
                    MessageBox.Show("Required file not found:\n" + txtSfmQuestionFile.Text, Text);
                if (!finfoAlternatives.Exists)
                    alternativesFilename = null;
                QuestionSfmFileAccessor.Generate(txtSfmQuestionFile.Text, alternativesFilename, destQuestionsFilename);
            }
        }

        private void btnTextToSfm_Click(object sender, EventArgs e)
        {
            using (var dlg = new OpenFileDialog())
            {
                dlg.CheckFileExists = true;
                dlg.Multiselect = true;
                dlg.RestoreDirectory = true;
                dlg.Filter = "Standard Format Files (*.sfm)|*.sfm;*.txt|All Supported Files (*.sfm;*.txt)|*.sfm;*.txt";
                dlg.InitialDirectory = Path.Combine(Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "SoftDev"), "Transcelerator");
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    StringBuilder sb = new StringBuilder();
                    foreach (var filename in dlg.FileNames)
                    {
                        string fileToWrite = "QTT" + Path.GetFileName(filename);
                        string directory = Path.GetDirectoryName(filename);
                        if (directory != null)
                            fileToWrite = Path.Combine(directory, fileToWrite);

                        if (File.Exists(fileToWrite))
                        {
                            if (MessageBox.Show("File " + fileToWrite + " already exists. Do you want to overwrite it?",
                                "Transcelerator", MessageBoxButtons.YesNo) == DialogResult.No)
                            {
                                return;
                            }
                        }

                        int problemsFound;
                        using (var reader = new StreamReader(filename, Encoding.UTF8))
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
                    }

                    MessageBox.Show("Finished! Questions written to the following files:" + sb, "Transcelerator");
                }
            }
        }
    }
}
