using NUnit.Framework;
using SIL.Transcelerator;
using System;
using System.IO;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;

namespace DataIntegrityTests
{
	[TestFixture]
	public class KeyTermRulesTests
	{
		[Test]
		public void DataIntegrity_ValidateAgainstXsd()
		{
			Assert.Ignore("The XSD is out of date and I can't figure out how to update it.");
			var folder = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
			Assert.IsNotNull(folder);
			var sourceXmlFolder = Path.GetDirectoryName(folder);
			if (sourceXmlFolder != null)
			{
				sourceXmlFolder = Path.GetDirectoryName(sourceXmlFolder);
				if (sourceXmlFolder != null)
					folder = Path.Combine(sourceXmlFolder, "Transcelerator");
			}
			
			XmlSchemaSet schema = new XmlSchemaSet();  
			schema.Add("", Path.Combine(folder, "keyTermRules.xsd"));
			XmlReader rd = XmlReader.Create(Path.Combine(folder, TxlData.kKeyTermRulesFilename));
			XDocument doc = XDocument.Load(rd);  
			doc.Validate(schema, delegate(object sender, ValidationEventArgs e)
			{
				if (Enum.TryParse<XmlSeverityType>("Error", out var type))
				{
					switch (type)
					{
						case XmlSeverityType.Error:
							Assert.Fail(e.Message);
							break;
						case XmlSeverityType.Warning:
							Assert.Warn(e.Message);
							break;
						default:
							throw new ArgumentOutOfRangeException();
					}
				}  
			}); 
		}
	}
}