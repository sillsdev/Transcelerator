import xml.etree.ElementTree as ET
import os
import sys

def count_words(text):
    """Count the number of words in a given text."""
    return len(text.split())

def main(paratext_project_id):
    # Construct the file path
    xml_file = os.path.join(
        "C:\\", "My Paratext 9 Projects", paratext_project_id, 
        "pluginData", "Transcelerator", "Transcelerator", "Translations of Checking Questions.xml"
    )

    if not os.path.exists(xml_file):
        print(f"File not found: {xml_file}")
        return

    # Parse the XML file
    tree = ET.parse(xml_file)
    root = tree.getroot()

    original_phrase_word_count = 0
    translation_word_count = 0

    # Iterate through each Translation element
    for translation in root.findall('Translation'):
        original_phrase = translation.find('OriginalPhrase').text
        translation_text = translation.find('Translation').text

        # Count words in the OriginalPhrase and Translation
        original_phrase_word_count += count_words(original_phrase)
        translation_word_count += count_words(translation_text)

    print(f'Total word count based on original English questions: {original_phrase_word_count}')
    print(f'Total word count based on localized questions: {translation_word_count}')

if __name__ == "__main__":
    # Check if a project ID was passed as a command-line argument
    if len(sys.argv) > 1:
        paratext_project_id = sys.argv[1]
    else:
        paratext_project_id = input("Enter the Paratext project ID: ")

    main(paratext_project_id)
