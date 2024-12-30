using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Events;
using UnityEngine.UI;

namespace NoaDebugger
{
    sealed class ApiLogDetailView : MonoBehaviour
    {
        [Serializable]
        public struct TabbedContent
        {
            public ToggleButtonBase _tabButton;

            public GameObject _viewRoot;
        }

        [SerializeField]
        TabbedContent _request;

        [SerializeField]
        TabbedContent _response;

        [SerializeField]
        ContextPanelView _apiDetailPanel;

        [SerializeField]
        ContextPanelView _requestHeaderPanel;

        [SerializeField]
        ContextPanelView _requestBodyPanel;

        [SerializeField]
        ContextPanelView _responseHeaderPanel;

        [SerializeField]
        ContextPanelView _responseBodyPanel;

        [SerializeField]
        Button _copyButton;

        UnityAction _onClickCopyButton;

        void Awake()
        {
            Assert.IsNotNull(_request._tabButton);
            Assert.IsNotNull(_request._viewRoot);
            Assert.IsNotNull(_response._tabButton);
            Assert.IsNotNull(_response._viewRoot);
            Assert.IsNotNull(_apiDetailPanel);
            Assert.IsNotNull(_requestHeaderPanel);
            Assert.IsNotNull(_requestBodyPanel);
            Assert.IsNotNull(_responseHeaderPanel);
            Assert.IsNotNull(_responseBodyPanel);
            Assert.IsNotNull(_copyButton);

            _request._tabButton._onClick.RemoveAllListeners();
            _request._tabButton._onClick.AddListener(_OnShowRequest);
            _response._tabButton._onClick.RemoveAllListeners();
            _response._tabButton._onClick.AddListener(_OnShowResponse);
            _copyButton.onClick.RemoveAllListeners();
            _copyButton.onClick.AddListener(_OnCopy);

            _request._tabButton.Init(true);
            _request._viewRoot.SetActive(true);
            _response._tabButton.Init(false);
            _response._viewRoot.SetActive(false);
        }

        public void ResetContent()
        {
            _apiDetailPanel.gameObject.SetActive(false);
            _requestHeaderPanel.gameObject.SetActive(false);
            _requestBodyPanel.gameObject.SetActive(false);
            _responseHeaderPanel.gameObject.SetActive(false);
            _responseBodyPanel.gameObject.SetActive(false);

            _onClickCopyButton = null;
        }

        public void SetCopyButton(UnityAction onClick)
        {
            _onClickCopyButton = onClick;
            _copyButton.gameObject.SetActive(true);
        }

        public void UpdateRequestTab(ApiLog log)
        {
            _apiDetailPanel.gameObject.SetActive(true);

            var apiDetailContexts = new Dictionary<string, string>()
            {
                {ApiLogPresenter.DETAIL_CONTENT_URL, log.Url.ToString()},
                {ApiLogPresenter.DETAIL_CONTENT_REQUEST_METHOD, log.Method},
                {ApiLogPresenter.DETAIL_CONTENT_STATUS_CODE, log.StatusCode.ToString()}
            };

            _apiDetailPanel.SetText(ApiLogPresenter.DETAIL_HEADER_GENERAL, apiDetailContexts);

            bool isShowRequestHeader = log.RequestHeaders != null && log.RequestHeaders.Any();
            _requestHeaderPanel.gameObject.SetActive(isShowRequestHeader);

            if (isShowRequestHeader)
            {
                var requestHeaderContexts = new Dictionary<string, string>();

                foreach ((string key, string value) in log.RequestHeaders)
                {
                    requestHeaderContexts.Add(key, value);
                }

                _requestHeaderPanel.SetText(ApiLogPresenter.DETAIL_HEADER_REQUEST_HEADERS, requestHeaderContexts);
            }

            bool isShowRequestBody = !string.IsNullOrEmpty(log.RequestBody);
            _requestBodyPanel.gameObject.SetActive(isShowRequestBody);

            if (isShowRequestBody)
            {
                _requestBodyPanel.SetText(
                    ApiLogPresenter.DETAIL_HEADER_REQUEST_BODY, context: null, suffix: log.PrettyPrintedRequestBody);
            }
        }

        public void UpdateResponseTab(ApiLog log)
        {
            bool isShowResponseHeader = log.ResponseHeaders != null && log.ResponseHeaders.Any();
            _responseHeaderPanel.gameObject.SetActive(isShowResponseHeader);

            if (isShowResponseHeader)
            {
                var responseHeaderContexts = new Dictionary<string, string>();

                foreach ((string key, string value) in log.ResponseHeaders)
                {
                    responseHeaderContexts.Add(key, value);
                }

                _responseHeaderPanel.SetText(ApiLogPresenter.DETAIL_HEADER_RESPONSE_HEADERS, responseHeaderContexts);
            }

            bool isShowResponseBody = !string.IsNullOrEmpty(log.ResponseBody);
            _responseBodyPanel.gameObject.SetActive(isShowResponseBody);

            if (isShowResponseBody)
            {
                _responseBodyPanel.SetText(header: null, context: null, suffix: log.PrettyPrintedResponseBody);
            }
        }

        void _OnShowRequest(bool showsContent) => _request._viewRoot.SetActive(showsContent);

        void _OnShowResponse(bool showsContent) => _response._viewRoot.SetActive(showsContent);

        void _OnCopy()
        {
            _onClickCopyButton?.Invoke();

            _copyButton.gameObject.SetActive(false);
        }
    }
}
