using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using Paratext.PluginInterfaces;

namespace SIL.Transcelerator
{
	public interface IHtmlScriptureExtractor
	{
		bool IncludeVerseNumbers { get; set; }

		string GetAsHtmlFragment(int startRef, int endRef);
	}

	public class ScriptureExtractor : IHtmlScriptureExtractor
	{
		private readonly IProject m_project;
		private readonly Func<int, IVerseRef> m_makeVerseRef;

		private IVerseRef m_startRef;
		private IVerseRef m_endRef;
		private int m_openElements;

		// We need to defer writing paragraphs and verse/chapter numbers until we actually get some text.
		private Action WriteParagraph { get; set; }
		private Action WriteVerse { get; set; }
		private Action WriteChapter { get; set; }

		public ScriptureExtractor(IProject project, Func<int, IVerseRef> makeVerseRef)
		{
			m_project = project;
			m_makeVerseRef = makeVerseRef;
		}

		public bool IncludeVerseNumbers { get; set; }

		public string GetAsHtmlFragment(int startRef, int endRef)
		{
			m_startRef = m_makeVerseRef(startRef);
			m_endRef = m_makeVerseRef(endRef);
			WriteParagraph = null;
			WriteVerse = null;
			WriteChapter = null;
			m_openElements = 0;

			var doc = new XmlDocument();
			var fragment = doc.CreateDocumentFragment();

			using (var xmlw = fragment.CreateNavigator().AppendChild())
			{
				int chapter = m_startRef.ChapterNum == m_endRef.ChapterNum ? m_startRef.ChapterNum : 0;

				List<IUSFMToken> openingTokens = new List<IUSFMToken>();

				foreach (var tok in m_project.GetUSFMTokens(m_startRef.BookNum, chapter))
				{
					if (!tok.IsPublishableVernacular || tok.IsFigure ||
						tok.IsFootnoteOrCrossReference && tok.IsSpecial)
					{
						openingTokens.Clear();
						continue;
					}
					
					if (!tok.IsScripture)
						continue;

					if (tok.VerseRef.CompareTo(m_endRef) > 0)
						break;

					if (tok.VerseRef.CompareTo(m_startRef) < 0)
					{
						if (!(tok is IUSFMMarkerToken) || ((IUSFMMarkerToken)tok).Type == MarkerType.Verse)
						{
							if (openingTokens.LastOrDefault() is IUSFMMarkerToken marker && marker.Type == MarkerType.Chapter)
								openingTokens.RemoveAt(openingTokens.Count - 1);
						}
						else
							openingTokens.Add(tok);
						continue;
					}

					foreach (var openingToken in openingTokens)
						ProcessToken(xmlw, openingToken);
					openingTokens.Clear();
					ProcessToken(xmlw, tok);
				}

				while (m_openElements > 0)
				{
					xmlw.WriteEndElement();
					m_openElements--;
				}

				xmlw.Flush();
			}

			return fragment.InnerXml + Environment.NewLine;
		}

		private void ProcessToken(XmlWriter xmlw, IUSFMToken tok)
		{
			if (tok is IUSFMMarkerToken markerToken)
			{
				switch (markerToken.Type)
				{
					case MarkerType.Paragraph:
						WritePendingVerse();
						while (m_openElements > 0)
						{
							xmlw.WriteEndElement();
							m_openElements--;
						}

						WriteParagraph = () =>
						{
							xmlw.WriteStartElement("div");
							xmlw.WriteAttributeString("class", "usfm_" + markerToken.Marker);
							m_openElements++;
						};
						break;
					case MarkerType.Verse:
						if (IncludeVerseNumbers)
						{
							WriteVerse = () =>
							{
								WriteChapter?.Invoke();
								WriteChapter = null;
								WriteParagraph?.Invoke();
								WriteParagraph = null;
								xmlw.WriteStartElement("span");
								xmlw.WriteAttributeString("class", "verse");
								xmlw.WriteAttributeString("number", markerToken.Data);
								xmlw.WriteValue(markerToken.Data);
								xmlw.WriteEndElement();
							};
						}
						break;
					case MarkerType.Chapter:
						if (xmlw.WriteState != WriteState.Start && IncludeVerseNumbers)
						{
							WriteChapter = () =>
							{
								xmlw.WriteStartElement("div");
								xmlw.WriteAttributeString("class", "chapter");
								xmlw.WriteAttributeString("number", markerToken.Data);
								xmlw.WriteValue(markerToken.Data);
								xmlw.WriteEndElement();
							};
						}
						break;
					case MarkerType.Character:
						WritePendingVerse();
						xmlw.WriteStartElement("span");
						xmlw.WriteAttributeString("class", "usfm_" + markerToken.Marker);
						break;
					case MarkerType.End:
						xmlw.WriteEndElement();
						break;
				}
			}
			else if (tok is IUSFMTextToken textToken)
			{
				WritePendingVerse();
				WriteParagraph?.Invoke();
				WriteParagraph = null;
				xmlw.WriteValue(textToken.Text);
			}
		}

		private void WritePendingVerse()
		{
			WriteVerse?.Invoke();
			WriteVerse = null;
		}
	}
}
