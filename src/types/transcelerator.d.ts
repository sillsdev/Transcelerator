import { VerseRef } from '@sillsdev/scripture';

declare module 'transcelerator' {
  /**
   * Network event that informs subscribers when the user has selected a question/phrase in a
   * different verse.
   */
  export type SelectionChangedEvent = { scrRef: VerseRef };

  /**
   * Network event that informs subscribers when a question/phrase has been translated by the user
   * or confirmed by the user as correctly translated.
   */
  export type TranslationConfirmedEvent = {
    origEnglishQuestion: string;
    translation: string;
    scrRef: VerseRef;
    modifiedEnglishQuestion?: string;
    localizedQuestion?: string;
    localizationBcp47Code?: string;
  };

  /**
   * Network event that informs subscribers when the user has selected a biblical term for a
   * question/phrase.
   */
  export type BiblicalTermSelectedEvent = { term: string; scrRef: VerseRef };
}

declare module 'papi-shared-types' {
  export interface CommandHandlers {
    /**
     * Opens a new Transcelerator WebView and returns the WebView id
     *
     * @param projectId Project ID to open. Prompts the user to select a project if not provided.
     * @returns WebView id for new text collection WebView or `undefined` if the user canceled the
     *   dialog
     */
    'transcelerator.open': (projectId: string) => Promise<string | undefined>;
  }
}
