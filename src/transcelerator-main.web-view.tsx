import { Button } from 'platform-bible-react';

globalThis.webViewComponent = function TransceleratorMain() {
  return (
    <>
      <div className="title">
        Transcelerator <span className="framework">React</span> webview
      </div>
      <Button>Show Help</Button>
    </>
  );
};
