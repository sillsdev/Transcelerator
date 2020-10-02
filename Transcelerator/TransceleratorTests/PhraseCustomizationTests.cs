// ---------------------------------------------------------------------------------------------
#region // Copyright (c) 2020, SIL International.
// <copyright from='2020' to='2020' company='SIL International'>
//		Copyright (c) 2020, SIL International.   
//    
//		Distributable under the terms of the MIT License (http://sil.mit-license.org/)
// </copyright> 
#endregion
// 
// File: PhraseCustomizationTests.cs
// ---------------------------------------------------------------------------------------------
using System;
using NUnit.Framework;

namespace SIL.Transcelerator
{
	/// ----------------------------------------------------------------------------------------
	/// <summary>
	/// Tests the PhraseCustomization implementation
	/// </summary>
	/// ----------------------------------------------------------------------------------------
	[TestFixture]
	public class PhraseCustomizationTests
	{
		[Test]
		public void Key_CustomizationBasedOnModifiedTranslatablePhrase_ValueIsBasedOnOriginalTpQuestion()
		{
			var tp = new TranslatablePhrase(new Question("TST 6:1-2", 100006001, 100006002, "Is this the original?", "Yes"), 1, 2, 3);
			tp.ModifiedPhrase = "This is the modified version of it";
			var pc = new PhraseCustomization(tp);
			Assert.AreEqual("Is this the original?", pc.ImmutableKey);
			Assert.AreEqual(pc.ImmutableKey, pc.ImmutableKey_PublicForSerializationOnly);
		}
		
		[TestCase(PhraseCustomization.CustomizationType.AdditionAfter)]
		[TestCase(PhraseCustomization.CustomizationType.InsertionBefore)]
		public void Key_CustomizationBasedOnAddedQuestionWithEnglishVersion_KeyBasedOnEnglishQuestion(PhraseCustomization.CustomizationType type)
		{
			var pc = new PhraseCustomization($"Is this the {GetAdjectiveForBaseQuestion(type)} question?",
				new Question("TST 8:9", 100008009, 100008009, "Is this a user-added question?", "Yes"), type);
			Assert.AreEqual("Is this a user-added question?", pc.ImmutableKey);
			Assert.IsNull(pc.ImmutableKey_PublicForSerializationOnly);
		}
		
		[TestCase(PhraseCustomization.CustomizationType.AdditionAfter)]
		[TestCase(PhraseCustomization.CustomizationType.InsertionBefore)]
		public void Key_CustomizationBasedOnAddedQuestionWithoutEnglishVersion_KeyAssignedToNewGuidWithPrefix(PhraseCustomization.CustomizationType type)
		{
			var pc = new PhraseCustomization($"Is this the {GetAdjectiveForBaseQuestion(type)} question?",
				new Question("TST 8:9", 100008009, 100008009, null, "Tal vez"), type);
			Assert.IsTrue(pc.ImmutableKey.StartsWith(Question.kGuidPrefix));
			Assert.IsTrue(Guid.TryParse(pc.ImmutableKey.Substring(Question.kGuidPrefix.Length), out _));
		}
		
		[TestCase(PhraseCustomization.CustomizationType.AdditionAfter, "Is this a user-added question?")]
		[TestCase(PhraseCustomization.CustomizationType.InsertionBefore, "Is this a user-added question?")]
		[TestCase(PhraseCustomization.CustomizationType.AdditionAfter, null)]
		[TestCase(PhraseCustomization.CustomizationType.InsertionBefore, "")] // FWIW: In real life, it can't be an empty string.
		public void Key_CustomizationBasedOnAddedQuestionWithExplicitKey_KeyBasedOnExplicitKey(
			PhraseCustomization.CustomizationType type, string questionInEnglish)
		{
			var explicitKey = Question.kGuidPrefix + Guid.NewGuid();
			var pc = new PhraseCustomization($"Is this the {GetAdjectiveForBaseQuestion(type)} question?",
				new Question("TST 8:9", 100008009, 100008009, questionInEnglish, "Tal vez", explicitKey), type);
			Assert.AreEqual(explicitKey, pc.ImmutableKey);
			pc.ModifiedPhrase = "Changed it!";
			Assert.AreEqual(explicitKey, pc.ImmutableKey);
			Assert.AreEqual(pc.ImmutableKey, pc.ImmutableKey_PublicForSerializationOnly);
		}

		private static string GetAdjectiveForBaseQuestion(PhraseCustomization.CustomizationType type) =>
			type == PhraseCustomization.CustomizationType.AdditionAfter ? "preceding" : "following";
	}
}
