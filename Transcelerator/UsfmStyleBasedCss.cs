using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using Paratext.PluginInterfaces;
using static System.String;

namespace SIL.Transcelerator
{
	internal class UsfmStyleBasedCss
	{
		private readonly string m_defaultFont;
		private readonly string m_defaultFontFeature;
		private readonly string m_defaultLang;
		private readonly float m_defaultFontSize;
		private readonly bool m_rtl;
		private readonly IReadOnlyList<IMarkerInfo> m_markers;
		private readonly bool m_vertical;
		private static string s_css;
		private static string s_lastFont;
		private static string s_lastFontFeature;
		private static string s_lastLang;
		private static float s_lastFontSize;
		private static bool s_lastRtl;

		private const string bodyBeforeCss = @"
body::before {
    background-image: linear-gradient(to top, (color:background_window_dimmed) 0%, (color:background_window));
    content: '';
    position: fixed;
    top: 0px;
    width: 100%;
    height: 4px;
}";
        internal UsfmStyleBasedCss(string defaultFont, string defaultFontFeature, string defaultLang, float defaultFontSize, bool rtl,
            IReadOnlyList<IMarkerInfo> markers, bool vertical = false)
		{
			m_defaultFont = defaultFont;
			m_defaultFontFeature = defaultFontFeature;
			m_defaultLang = defaultLang;
			m_defaultFontSize = defaultFontSize;
			m_rtl = rtl;
			m_markers = markers;
			m_vertical = vertical;
		}

        /// <summary>
        /// Create a cascading stylesheet with the given information.
        /// </summary>
        public string CreateCSS()
		{
			if (s_css != null && m_defaultFont == s_lastFont &&
				m_defaultFontFeature == s_lastFontFeature &&
				m_defaultLang == s_lastLang &&
				m_defaultFontSize == s_lastFontSize &&
				m_rtl == s_lastRtl)
			{
				return s_css;
			}
            StringBuilder sb = new StringBuilder();

            // Emit body style
            sb.AppendLine("body {");
            if (m_vertical)
            {
                if (m_rtl)
                {
                    sb.AppendLine("direction: rtl;");
                    sb.AppendLine("writing-mode: vertical-rl;");
                }
                else
                    sb.AppendLine("writing-mode: vertical-lr;");
            }
            else
            {
                if (m_rtl)
                    sb.AppendLine("direction: rtl;");
            }
            sb.AppendLine("}");

            // Create floating styles
            sb.AppendLine(".leadingFloat");
            sb.AppendLine("{");
            sb.AppendLine(m_rtl ? "float:right;" : "float:left;");
            sb.AppendLine("}");

            // Emit floating-reset style (FB-15267)
            sb.AppendLine(".clearFloat {");
            sb.AppendLine(" clear:both;");
            sb.AppendLine("}");

            // Create start/center/end style for table cell alignment
            sb.AppendLine(".align_start");
            sb.AppendLine("{");
            sb.AppendLine(m_rtl ? "text-align:right;" : "text-align:left;");
            sb.AppendLine("}");
            sb.AppendLine(".align_center");
            sb.AppendLine("{");
            sb.AppendLine("text-align:center;");
            sb.AppendLine("}");
            sb.AppendLine(".align_end");
            sb.AppendLine("{");
            sb.AppendLine(m_rtl ? "text-align:left;" : "text-align:right;");
            sb.AppendLine("}");

            sb.Append(CreateUsfmCss());

            s_css = sb.ToString();
			s_lastFont = m_defaultFont;
			s_lastFontFeature = m_defaultFontFeature;
			s_lastLang = m_defaultLang;
			s_lastFontSize = m_defaultFontSize;
			s_lastRtl = m_rtl;
			return s_css;
		}

		private string CreateUsfmCss()
        {
            var sb = new StringBuilder();

			// Emit usfm style
            sb.AppendLine(".usfm {");
            AppendCssFontDetails(sb);
            sb.AppendLine("}");

            // For each tag

            foreach (var mrkrInfo in m_markers.OfType<IStyledMarkerInfo>())
            {
                // Emit class
                sb.AppendFormat(".usfm_{0} {{ \n", mrkrInfo.Marker);
				if (mrkrInfo.Bold is bool bold)
				{
					sb.Append(" font-weight:");
					sb.AppendLine(bold ? "bold;" : "normal;");
				}

				if (mrkrInfo.Color is Color color && (color.R != 0 || color.G != 0 || color.B != 0))
					sb.AppendFormat(" color:{0};\n", ColorTranslator.ToHtml(color));
                
				IParagraphMarkerInfo paraInfo = mrkrInfo as IParagraphMarkerInfo;

				if (paraInfo?.FirstLineIndent is float firstLineIndent && firstLineIndent != 0)
                    sb.AppendFormat(" text-indent:{0}vw;\n", FormatScaledValueForVW(firstLineIndent));
                
				if (mrkrInfo.FontFamily?.Length > 0)
                    sb.AppendFormat(" font-family:\"{0}\";\n", mrkrInfo.FontFamily);

				if (mrkrInfo.FontSize > 0)
				{
					// Don't multiply by zoom. It's a percentage of the zoomed font size above.
					sb.AppendFormat(CultureInfo.InvariantCulture, " font-size:{0}%;\n", mrkrInfo.FontSize * 100 / 12);
				}

				if (mrkrInfo.SmallCaps is bool smallCaps)
				{
					sb.Append(" font-variant: ");
					sb.AppendLine(smallCaps ? "small-caps;" : "normal;");
				}

				if (mrkrInfo.Italic is bool italics)
				{
					sb.Append(" font-style: ");
					sb.AppendLine(italics ? "italic;" : "normal;");
				}

				if (paraInfo?.Justification is Justification justification)
				{
					switch (justification)
					{
						case Justification.Left:
							if (!m_rtl)
								sb.AppendLine(" text-align:left;");
							else
								sb.AppendLine(" text-align:right;");
							break;
						case Justification.Right:
							if (!m_rtl)
								sb.AppendLine(" text-align:right;");
							else
								sb.AppendLine(" text-align:left;");
							break;
						case Justification.Center:
							sb.AppendLine(" text-align:center;");
							break;
						case Justification.Both:
							sb.AppendLine(" text-align:justify;");
							break;
					}
				}

				if (paraInfo?.LeftMargin is float leftMargin)
                {
                    if (m_vertical)
                        sb.AppendFormat(" margin-top:{0}in;\n", FormatScaledValue(leftMargin));
                    else if (!m_rtl)
                        sb.AppendFormat(" margin-left:{0}vw;\n", FormatScaledValueForVW(leftMargin));
                    else
                        sb.AppendFormat(" margin-right:{0}vw;\n", FormatScaledValueForVW(leftMargin));
                }

				if (paraInfo?.LineSpacing != null)
				{
					switch (paraInfo?.LineSpacing)
					{
						case 1:
							sb.AppendLine(" line-height:1.5;");
							break;
						case 2:
							sb.AppendLine(" line-height:2;");
							break;
					}
				}

				if (paraInfo?.RightMargin is float rightMargin)
                {
                    if (m_vertical)
                        sb.AppendFormat(" margin-bottom:{0}in;\n", FormatScaledValue(rightMargin));
                    else if (!m_rtl)
                        sb.AppendFormat(" margin-right:{0}vw;\n", FormatScaledValueForVW(rightMargin));
                    else
                        sb.AppendFormat(" margin-left:{0}vw;\n", FormatScaledValueForVW(rightMargin));
                }

                if (paraInfo?.SpaceAfter is int spaceAfter && spaceAfter > 0)
                    sb.AppendFormat(CultureInfo.InvariantCulture, " margin-bottom:{0}pt;\n", spaceAfter);

                if (paraInfo?.SpaceBefore is int spaceBefore && spaceBefore > 0)
                    sb.AppendFormat(CultureInfo.InvariantCulture, " margin-top:{0}pt;\n", spaceBefore);

                if (mrkrInfo.Subscript is bool subscript)
                {
					if (subscript)
					{
						sb.AppendLine(" vertical-align: text-bottom;");
						sb.AppendLine(" font-size: 66%;");
					}
					else
					{
						sb.AppendLine(" vertical-align: baseline;");
						sb.AppendLine(" font-size: 100%;");
					}
				}
				else if (mrkrInfo.Superscript is bool superscript)
				{
					if (superscript)
					{
						sb.AppendLine(" vertical-align: text-top;");
						sb.AppendLine(" font-size: 66%;");
					}
					else
					{
						sb.AppendLine(" vertical-align: baseline;");
						sb.AppendLine(" font-size: 100%;");
					}
				}

				if (mrkrInfo.Underline is bool underline)
				{
					sb.Append(" text-decoration:");
					sb.AppendLine(underline ? "underline;" : "none;");
				}

				// Verses are no-break text
                if (mrkrInfo.Marker == "v")
                {
                    sb.AppendLine(" white-space:nowrap;");
                    // FB49055
                    sb.AppendLine(" unicode-bidi: embed;");
                }

                sb.AppendLine("}");
                sb.AppendLine("");
            }

            sb.AppendLine(bodyBeforeCss);

            return sb.ToString();
        }

		/// <summary>
		/// Format a value which has been scaled by 1000. We don't use
		/// the default floating format because it generates a string with the
		/// current local settings (e.g. 1/4 = 0,25 in Holland) and this is not
		/// appreciated by CSS
		/// </summary>
		/// <param name="value">value times 1000 e.g. 1250</param>
		/// <returns>e.g 1.250</returns>
		private static string FormatScaledValue(float value)
        {
            double scaled = value / 1000.0;
            return scaled.ToString("0.000", CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// The value passed in has been scaled by 1000. Scale it back, but not all of the way
        /// because vw units are similar to % of the screen width so scaling the int down to a number
        /// close to 1 would be too small. We don't use the default floating format because it
        /// generates a string with the current local settings (e.g. 1/4 = 0,25 in Holland) and
        /// this is not appreciated by CSS
        /// </summary>
        /// <param name="value">value times 1000 e.g. 1250</param>
        /// <returns>e.g 12.50</returns>
        static string FormatScaledValueForVW(float value)
        {
            double scaled = (value / 50.0);
            return scaled.ToString("0.000", CultureInfo.InvariantCulture);
        }

		/// <summary>
		/// Output font-family, font-size, -moz-font-feature-settings and -moz-font-language-override.
		/// </summary>
		/// <param name="sb"></param>
		/// <returns>The StringBuilder instance.</returns>
		private void AppendCssFontDetails(StringBuilder sb)
		{
			sb.AppendFormat(" font-family:\"{0}\";\n", m_defaultFont);
			if (!IsNullOrEmpty(m_defaultFontFeature))
				sb.AppendFormat(" -moz-font-feature-settings:{0};\n", ConvertFontFeatures(m_defaultFontFeature));
			if (!IsNullOrEmpty(m_defaultLang))
				sb.AppendFormat(" -moz-font-language-override:\"{0}\";\n", m_defaultLang);
			sb.AppendFormat(CultureInfo.InvariantCulture, " font-size:{0}pt;\n", m_defaultFontSize.ToString("F1", CultureInfo.InvariantCulture));
		}

		/// <summary>
		/// Converts from old moz-Font-Feature-Settings syntax to Firefox15+ syntax.
		/// </summary>
		/// <param name="oldFontFeatureSyntax">Fonts features in the old FEAT=Val format</param>
		/// <returns>Font features in the new format</returns>
		public static string ConvertFontFeatures(string oldFontFeatureSyntax)
		{
			if (IsNullOrEmpty(oldFontFeatureSyntax))
				return oldFontFeatureSyntax;

			var newFontFeatureString = new StringBuilder();            

			var features = oldFontFeatureSyntax.Split(',');
			foreach (var option in features)
			{
				var featureParts = option.Split('=');
				newFontFeatureString.Append(featureParts.Length == 1
					? Format("\"{0}\"", featureParts[0])
					: Format("\"{0}\" {1}", featureParts[0], featureParts[1]));

				newFontFeatureString.Append(",");
			}

			newFontFeatureString.Remove(newFontFeatureString.Length - 1, 1);

			return newFontFeatureString.ToString();
		}
    }
}
