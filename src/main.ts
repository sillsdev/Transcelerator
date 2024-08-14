import papi, { logger } from '@papi/backend';
import {
  ExecutionActivationContext,
  GetWebViewOptions,
  IWebViewProvider,
  SavedWebViewDefinition,
  WebViewContentType,
  WebViewDefinition,
} from '@papi/core';

import transceleratorMain from './transcelerator-main.web-view?inline';
import transceleratorMainReactStyles from './transcelerator-main.web-view.scss?inline';
import helpHtml from '../Docs/Help Topics/en/Home.htm?inline';
import addingQuestionsHelpHtml from '../Docs/Help Topics/en/addingquestions.htm?inline';
import adjustmentsHelpHtml from '../Docs/Help Topics/en/adjustments.htm?inline';
import biblicalTermsHelpHtml from '../Docs/Help Topics/en/biblicalterms.htm?inline';
import editingQuestionsHelpHtml from '../Docs/Help Topics/en/editingquestions.htm?inline';
import featureOverviewHelpHtml from '../Docs/Help Topics/en/featureoverview.htm?inline';
import filteringHelpHtml from '../Docs/Help Topics/en/filtering.htm?inline';
import generateScriptHelpHtml from '../Docs/Help Topics/en/generatescript.htm?inline';
import gettingStartedHelpHtml from '../Docs/Help Topics/en/gettingstarted.htm?inline';
import renderingSelectionRulesHelpHtml from '../Docs/Help Topics/en/renderingselectionrules.htm?inline';
import reorderingWordsHelpHtml from '../Docs/Help Topics/en/reorderingwords.htm?inline';

logger.info('Transcelerator is importing!');

const TRANSCELERATOR_MAIN_WEB_VIEW_TYPE = 'transcelerator.main';
const HTML_HELP_WEB_VIEW_TYPE = 'transceleratorHelp.html';

/**
 * Simple web view provider that provides Transcelerator Help in an html web view when papi requests
 * them
 */
const htmlWebViewProvider: IWebViewProvider = {
  async getWebView(
    savedWebView: SavedWebViewDefinition,
    options: GetWebViewOptions & { topic: string | undefined },
  ): Promise<WebViewDefinition | undefined> {
    if (savedWebView.webViewType !== HTML_HELP_WEB_VIEW_TYPE)
      throw new Error(
        `${HTML_HELP_WEB_VIEW_TYPE} provider received request to provide a ${savedWebView.webViewType} web view`,
      );

    let content = helpHtml;
    switch (options.topic) {
      case 'addingQuestions':
        content = addingQuestionsHelpHtml;
        break;
      case 'adjustments':
        content = adjustmentsHelpHtml;
        break;
      case 'biblicalTerms':
        content = biblicalTermsHelpHtml;
        break;
      case 'editingQuestions':
        content = editingQuestionsHelpHtml;
        break;
      case 'featureOverview':
        content = featureOverviewHelpHtml;
        break;
      case 'filteringHelp':
        content = filteringHelpHtml;
        break;
      case 'generateScript':
        content = generateScriptHelpHtml;
        break;
      case 'gettingStarted':
        content = gettingStartedHelpHtml;
        break;
      case 'renderingSelectionRules':
        content = renderingSelectionRulesHelpHtml;
        break;
      case 'reorderingWords':
        content = reorderingWordsHelpHtml;
        break;
      default:
        content = helpHtml;
    }

    return {
      ...savedWebView,
      title: 'Transcelerator Help',
      // Can't use the enum value from a definition file so assert the type from the string literal.
      // eslint-disable-next-line no-type-assertion/no-type-assertion
      contentType: 'html' as WebViewContentType.HTML,
      content,
    };
  },
};

/** Simple web view provider that provides React web views when papi requests them */
const transceleratorWebViewProvider: IWebViewProvider = {
  async getWebView(
    savedWebView: SavedWebViewDefinition,
    options: GetWebViewOptions & { projectId: string | undefined },
  ): Promise<WebViewDefinition | undefined> {
    if (savedWebView.webViewType !== TRANSCELERATOR_MAIN_WEB_VIEW_TYPE)
      throw new Error(
        `${TRANSCELERATOR_MAIN_WEB_VIEW_TYPE} provider received request to provide a ${savedWebView.webViewType} web view`,
      );

    const projectId = options.projectId || savedWebView.projectId;

    let projectName: string | undefined;
    try {
      if (projectId) {
        projectName = await (
          await papi.projectDataProviders.get('platform.base', projectId)
        ).getSetting('platform.name');
      }
    } catch (e) {
      logger.error(`Transcelerator web view provider error: Could not get project metadata: ${e}`);
    }

    return {
      ...savedWebView,
      title: projectName ? `Transcelerator (${projectName})` : 'Transcelerator',
      content: transceleratorMain,
      styles: transceleratorMainReactStyles,
      projectId,
    };
  },
};

export async function activate(context: ExecutionActivationContext) {
  logger.info('Transcelerator is activating!');

  /*   const questionWords = await papi.storage.readTextFileFromInstallDirectory(
    context.executionToken,
    '../Transcelerator/TxlQuestionWords.xml',
  ); */

  const htmlWebViewProviderPromise = papi.webViewProviders.register(
    HTML_HELP_WEB_VIEW_TYPE,
    htmlWebViewProvider,
  );

  const transceleratorWebViewProviderPromise = papi.webViewProviders.register(
    TRANSCELERATOR_MAIN_WEB_VIEW_TYPE,
    transceleratorWebViewProvider,
  );

  context.registrations.add(
    await papi.commands.registerCommand('transcelerator.open', async (projectId) => {
      let projectIdForWebView = projectId;

      // If projectId wasn't passed in, get from dialog
      if (!projectIdForWebView) {
        const userProjectIds = await papi.dialogs.showDialog('platform.selectProject', {
          title: '%dialogTitle_openTranscelerator%',
          prompt: '%dialogPrompt_selectProject%',
          includeProjectInterfaces: 'platformScripture.USFM_Book',
        });
        if (userProjectIds) projectIdForWebView = userProjectIds;
      }

      // If the user didn't select a project, return undefined and don't show Transcelerator
      if (!projectIdForWebView) return undefined;

      return papi.webViews.getWebView(
        TRANSCELERATOR_MAIN_WEB_VIEW_TYPE,
        { type: 'float', floatSize: { width: 775, height: 815 } },
        // Type assert because GetWebViewOptions is not yet typed to be generic and allow extra inputs
        // eslint-disable-next-line no-type-assertion/no-type-assertion
        {
          projectId: projectIdForWebView,
        } as GetWebViewOptions,
      );
    }),
  );

  context.registrations.add(
    await papi.commands.registerCommand('transcelerator.help', async (topic) => {
      return papi.webViews.getWebView(
        HTML_HELP_WEB_VIEW_TYPE,
        { type: 'float', floatSize: { width: 775, height: 815 } },
        // Type assert because GetWebViewOptions is not yet typed to be generic and allow extra inputs
        // eslint-disable-next-line no-type-assertion/no-type-assertion
        {
          topic: topic ?? 'home',
        } as GetWebViewOptions,
      );
    }),
  );

  // Await the data provider promise at the end so we don't hold everything else up
  context.registrations.add(
    await htmlWebViewProviderPromise,
    await transceleratorWebViewProviderPromise,
  );

  logger.info('Transcelerator is finished activating!');
}

export async function deactivate() {
  logger.info('Transcelerator is deactivating!');
  return true;
}
