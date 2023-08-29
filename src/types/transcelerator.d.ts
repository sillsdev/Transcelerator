import type { DataProviderDataType } from 'shared/models/data-provider.model';
import type IDataProvider from 'shared/models/data-provider.interface';

declare module 'transcelerator' {
  export type ExtensionVerseSetData = string | { text: string; isHeresy: boolean };

  export type ExtensionVerseDataTypes = {
    Verse: DataProviderDataType<string, string | undefined, ExtensionVerseSetData>;
    Heresy: DataProviderDataType<string, string | undefined, string>;
    Chapter: DataProviderDataType<[book: string, chapter: number], string | undefined, never>;
  };

  /** Network event that informs subscribers when the command `transcelerator.doStuff` is run */
  export type DoStuffEvent = {
    /** How many times transcelerator has run the command `transcelerator.doStuff` */
    count: number;
  };

  export type ExtensionVerseDataProvider = IDataProvider<ExtensionVerseDataTypes>;
}

declare module 'papi-shared-types' {
  export interface CommandHandlers {
    'transcelerator.doStuff': (message: string) => { response: string; occurrence: number };
  }
}
