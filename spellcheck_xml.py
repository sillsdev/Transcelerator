import xml.etree.ElementTree as ET
from xml.dom import minidom
from spellchecker import SpellChecker
import re
import os
import argparse

# List of transliterated foreign words; abbreviations; and unusual words, compounds or inflections.
# Note: "l8" (that's a lowercae 'L', not the numeral one) is skipped because we need to retain that typo in a hidden alternative.
words_to_skip = {"euangelion", "theos", "apostolos", "ethnê", "dunamis", "denarii", "diabolos", "katharizw", "hina", "thaumazw", "sheol", "logikos", "hierateuma", "peritemnw", "abussos", "km", "vv", "etc", "lbs", "floodwaters", "firepot", "firepans", "lampstand", "lampstands", "slingstones", "breastpiece", "algum", "spiritist", "spiritists", "versifications", "fowler", "fowlers", "dreamt", "waterer", "offerers", "righteousnesses", "backslidings", "compassions", "fornications", "revealer", "desolations", "l8"}

def spell_check_text(text, spell):
    def check_word(word):
        wordLc = word.lower()
        if wordLc in words_to_skip or 'ê' in wordLc or 'ô' in wordLc:
            return word
        if word[0].isalpha() and not (word[0].isupper() or word.endswith("s'") or word.endswith("'s") or '-' in word):  # Don't spellcheck capitalized words, possessives, or hyphenated words
            if wordLc in spell:
                corrected_word = spell.correction(wordLc)
                return corrected_word if corrected_word else f"***{word}***"
            else:
                return f"***{word}***"
        else:
            return word  # Return non-word tokens unchanged

    # Tokenize the text into words and non-words (spaces, numbers, punctuation)
    tokens = re.findall(r'\w[\w\'-]*|[^\w]+', text)
    
    corrected_tokens = []
    for token in tokens:
        corrected_tokens.append(check_word(token))

    # Rejoin the tokens to form the corrected text
    corrected_text = ''.join(corrected_tokens)
    return corrected_text

def spell_check_xml(file_path, max_elements=None, output_file=None):
    tree = ET.parse(file_path)
    root = tree.getroot()
    spell = SpellChecker()

    elements = list(root.iter())
    total_elements = len(elements)

    if max_elements is not None:
        elements = elements[:max_elements]
    
    checked_elements = len(elements)

    for i, elem in enumerate(elements):
        if elem.text:
            elem.text = spell_check_text(elem.text, spell)
        if elem.tail:
            elem.tail = spell_check_text(elem.tail, spell)

        # Display progress
        progress = (i + 1) / checked_elements * 100
        print(f"Progress: {progress:.2f}%", end='\r')

    if output_file is None:
        output_file = file_path

    rough_string = ET.tostring(root, encoding="utf-8")
    
    with open(output_file, "wb") as f:
        f.write(rough_string)
    
    print(f"\nSpell-checked file saved as {output_file}")

if __name__ == "__main__":
    default_file_path = os.path.join("Transcelerator", "TxlQuestions.xml")

    parser = argparse.ArgumentParser(description="Spell check an XML file.")
    parser.add_argument("file_path", nargs='?', default=default_file_path, help="Path to the XML file to spell check.")
    parser.add_argument("--max_elements", type=int, default=None, help="Maximum number of elements to check.")
    parser.add_argument("-o", "--output_file", type=str, default=None, help="Output file name. If not specified, the input file will be overwritten.")
    args = parser.parse_args()

    spell_check_xml(args.file_path, args.max_elements, args.output_file)
