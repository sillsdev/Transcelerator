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
		private bool m_isParagraphOpen;
		private Stack<string> m_pendingEndMarkers;

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
			m_isParagraphOpen = false;
			m_pendingEndMarkers = new Stack<string>();

			var doc = new XmlDocument();
			var fragment = doc.CreateDocumentFragment();

			using (var xmlw = fragment.CreateNavigator().AppendChild())
			{
				int chapter = m_startRef.ChapterNum == m_endRef.ChapterNum ? m_startRef.ChapterNum : 0;

				List<IUSFMMarkerToken> openingTokens = new List<IUSFMMarkerToken>();

				foreach (var tok in m_project.GetUSFMTokens(m_startRef.BookNum, chapter))
				{
					if (!tok.IsPublishableVernacular || tok.IsSpecial)
					{
						openingTokens.Clear();
						continue;
					}

					if (tok.IsFigure || tok.IsFootnoteOrCrossReference)
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
						if (!(tok is IUSFMMarkerToken markerToken) || markerToken.Type == MarkerType.Verse)
						{
							if (openingTokens.LastOrDefault() is IUSFMMarkerToken marker && marker.Type == MarkerType.Chapter)
								openingTokens.RemoveAt(openingTokens.Count - 1);
						}
						else if (markerToken.Type != MarkerType.End)
						{
							openingTokens.Add(markerToken);
						}

						continue;
					}

					foreach (var openingToken in openingTokens)
						ProcessToken(xmlw, openingToken);
					openingTokens.Clear();
					ProcessToken(xmlw, tok);
				}

				CloseOpenParagraph(xmlw);

				xmlw.Flush();
			}

			return fragment.InnerXml + Environment.NewLine;
		}

		private void CloseOpenParagraph(XmlWriter xmlw)
		{
			if (m_isParagraphOpen)
			{
				xmlw.WriteEndElement();
				m_isParagraphOpen = false;
			}
		}

		private void ProcessToken(XmlWriter xmlw, IUSFMToken tok)
		{
			if (tok is IUSFMMarkerToken markerToken)
			{
				switch (markerToken.Type)
				{
					case MarkerType.Paragraph:
						ClosePendingEndMarkers(xmlw);
						WritePendingVerse();
						CloseOpenParagraph(xmlw);

						WriteParagraph = () =>
						{
							xmlw.WriteStartElement("div");
							xmlw.WriteAttributeString("class", "usfm_" + markerToken.Marker);
							m_isParagraphOpen = true;
						};
						break;
					case MarkerType.Verse:
						ClosePendingEndMarkers(xmlw);
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
						ClosePendingEndMarkers(xmlw);
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
						if (markerToken.EndMarker != null)
							m_pendingEndMarkers.Push(markerToken.EndMarker);
						break;

					case MarkerType.End:
						if (m_pendingEndMarkers.Any(e => e == markerToken.Marker))
						{
							do
							{
								xmlw.WriteEndElement();
							} while (m_pendingEndMarkers.Pop() != markerToken.Marker);
						}
						break;

					default:
						if (tok.IsFigure || tok.IsFootnoteOrCrossReference)
						{
							ClosePendingEndMarkers(xmlw);
							if (markerToken.EndMarker != null)
								m_pendingEndMarkers.Push(markerToken.EndMarker);
						}

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

		private void ClosePendingEndMarkers(XmlWriter xmlw)
		{
			// In case data is bad...
			while (m_pendingEndMarkers.Count > 0)
			{
				xmlw.WriteEndElement();
				m_pendingEndMarkers.Pop();
			}
		}

		private void WritePendingVerse()
		{
			WriteVerse?.Invoke();
			WriteVerse = null;
		}
	}
}
