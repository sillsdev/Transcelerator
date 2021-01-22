using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace SIL.Transcelerator
{
	[TestFixture]
	class UNSQuestionsDialogTests
	{
		[TestCase("2.0")]
		[TestCase("2.6")]
		[TestCase("3.0")]
		[TestCase("13.29")]
		public void ConvertUsxToHtml_VariousUsxVersions_UsxElementRemoved(string version)
		{
			Assert.AreEqual("..." + Environment.NewLine,
				UNSQuestionsDialog.ConvertUsxToHtml($"<usx version=\"{version}\">...</usx>", true));
		}

		[Test]
		public void ConvertUsxToHtml_ParagraphStylesAndVerseNumbers_ConvertedToDivAndSpanElements()
		{
			Assert.AreEqual("<div class=\"usfm_p\">" +
				"<span class=\"verse\" number=\"54\">54</span>Oyendo esto, se enfurecían y crujían los dientes. " +
				"<span class=\"verse\" number=\"55\">55</span>Pero Esteban vio la gloria de Dios, y a Jesús, " +
				"<span class=\"verse\" number=\"56\">56</span>y dijo:</div>" +
				"<div class=\"usfm_q\">" +
				"He aquí, veo los cielos abiertos, y al Hijo del Hombre que está a la diestra de Dios. " +
				"<span class=\"verse\" number=\"57\">57</span>Entonces ellos se taparon los oídos.</div>" + Environment.NewLine,
				UNSQuestionsDialog.ConvertUsxToHtml("<usx version=\"2.6\"><para style=\"p\">" +
					"<verse number=\"54\" style=\"v\" />Oyendo esto, se enfurecían y crujían los dientes. " +
					"<verse number=\"55\" style=\"v\" />Pero Esteban vio la gloria de Dios, y a Jesús, " +
					"<verse number=\"56\" style=\"v\" />y dijo:</para>" +
					"<para style=\"q\">" +
					"He aquí, veo los cielos abiertos, y al Hijo del Hombre que está a la diestra de Dios. " +
					"<verse number=\"57\" style=\"v\" />Entonces ellos se taparon los oídos.</para></usx>", true));
		}

		[Test] public void ConvertUsxToHtml_GlossaryWord_GlossaryJunkRemoved()
		{
			Assert.AreEqual("<div class=\"usfm_p\">" +
				"<span class=\"verse\" number=\"54\">54</span>Oyendo esto, se enfurecían y crujían los dientes. "+
				"</div>" + Environment.NewLine,
				UNSQuestionsDialog.ConvertUsxToHtml("<usx version=\"2.6\"><para style=\"p\">" +
					"<verse number=\"54\" style=\"v\" />Oyendo esto, se enfurecían y crujían los " +
					"<char style=\"w\">dientes|basically, we're talking teeth</char>. "+
					"</para></usx>", true));
		}

		[TestCase(true)]
		[TestCase(false)]
		public void ConvertUsxToHtml_EmptyVerses_Removed(bool includeEmptyVerseAtEnd)
		{
			var usx = "<usx version=\"3.0\"><para style=\"p\">" +
				"<verse number=\"1\" style=\"v\" />This is the " +
				"<char style=\"w\">genealogy|list of begats</char> " +
				"of Jesus Christ. " +
				"<verse number=\"2\" style=\"v\" /> " +
				"<verse number=\"3\" style=\"v\" /> " +
				"<verse number=\"4\" style=\"v\" />" +
				"<verse number=\"6\" style=\"v\" /> " +
				"<verse number=\"7\" style=\"v\" />Then spake He, saying, " +
				"<char style=\"wj\">This is my genealogy.</char>" +
				"<verse number=\"8\" style=\"v\" /> " +
				"<verse number=\"12\" style=\"v\" /> " +
				"<verse number=\"13\" style=\"v\" /></para>" +
				"<para style=\"q\">" +
				"This is the start of verse 13, but in a different paragraph. " +
				"<verse number=\"14\" style=\"v\" /> " +
				"<verse number=\"15\" style=\"v\" />" +
				"<verse number=\"16\" style=\"v\" />Blah.";
			if (includeEmptyVerseAtEnd)
				usx += "<verse number=\"17\" style=\"v\" />";
			usx += "</para></usx>";

			Assert.AreEqual("<div class=\"usfm_p\">" +
				"<span class=\"verse\" number=\"1\">1</span>This is the genealogy of Jesus Christ. " +
				"<span class=\"verse\" number=\"7\">7</span>Then spake He, saying, " +
				// REVIEW: Would we want to convert <char style="..."> to <span class="usfm_...">? 
				"<char style=\"wj\">This is my genealogy.</char>" +
				"<span class=\"verse\" number=\"13\">13</span></div>" +
				"<div class=\"usfm_q\">" +
				"This is the start of verse 13, but in a different paragraph. " +
				"<span class=\"verse\" number=\"16\">16</span>Blah." +
				"</div>" + Environment.NewLine,
				UNSQuestionsDialog.ConvertUsxToHtml(usx, true));
		}
	}
}
