import papi from 'papi-frontend';
import { useCallback, useState } from 'react';
import {
  DoStuffEvent,
  ExtensionVerseDataProvider,
  ExtensionVerseDataTypes,
} from 'transcelerator';
import { Button } from 'papi-components';
import { QuickVerseDataTypes } from 'quick-verse';

const {
  react: {
    hooks: { useData, useDataProvider, useEvent },
  },
  logger,
} = papi;

globalThis.webViewComponent = function Transcelerator2() {
  const [clicks, setClicks] = useState(0);

  useEvent<DoStuffEvent>(
    'transcelerator.doStuff',
    useCallback(({ count }) => setClicks(count), []),
  );

  const extensionVerseDataProvider = useDataProvider<ExtensionVerseDataProvider>(
    'transcelerator.quickVerse',
  );

  const [latestExtensionVerseText] = useData.Verse<ExtensionVerseDataTypes, 'Verse'>(
    extensionVerseDataProvider,
    'latest',
    'Loading latest Scripture text from Transcelerator ...',
  );

  const [latestQuickVerseText] = useData.Verse<QuickVerseDataTypes, 'Verse'>(
    'quickVerse.quickVerse',
    'latest',
    'Loading latest Scripture text from Transcelerator ...',
  );

  return (
    <>
      <div className="title">
        Transcelerator <span className="framework">React 2</span>
      </div>
      <div>{latestExtensionVerseText}</div>
      <div>{latestQuickVerseText}</div>
      <div>
        <Button
          onClick={async () => {
            const start = performance.now();
            const result = await papi.commands.sendCommand(
              'transcelerator.doStuff',
              'Transcelerator React Component',
            );
            setClicks(clicks + 1);
            logger.info(
              `command:transcelerator.doStuff '${result.response}' took ${
                performance.now() - start
              } ms`,
            );
          }}
        >
          Hi {clicks}
        </Button>
      </div>
    </>
  );
};
