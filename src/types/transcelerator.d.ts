declare module 'transcelerator' {
  import { ScriptureReference } from 'papi-components';
  import { DataProviderDataType, IDataProvider } from '@papi/core';
  
  type PassageSelector = {
    projectId: string;
    scrRef: ScriptureReference;
  };

  /** Network event that informs subscribers when the command `transcelerator.open` is run */
  export type TransceleratorOpenedEvent = {
  };

  export type TBDDataProvider = IDataProvider<ExtensionVerseDataTypes>;
}

declare module 'papi-shared-types' {
  export interface CommandHandlers {
	/**
     * Opens a new Transcelerator WebView and returns the WebView id
     * @param projectId Project ID to open in Transcelerator. Prompts the user to
     * select project if not provided.
     * @returns WebView id for new word list WebView or `undefined` if the user canceled the dialog.
     */
    'paratextWordList.open': (projectId?: string) => Promise<string | undefined>;
    'transcelerator.open': (message: string) => { response: string };
  }
  
  export interface DataProviders {
    tbd: TBDDataProvider;
  }
}
