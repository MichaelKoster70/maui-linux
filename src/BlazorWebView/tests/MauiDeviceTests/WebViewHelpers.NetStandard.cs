﻿using System;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Maui.MauiBlazorWebView.DeviceTests
{
#if !WINDOWS && !ANDROID && !IOS
	public static class WebViewHelpers
	{
		public static Task WaitForWebViewReady(object nativeWebView)
		{
			return Task.CompletedTask;
		}

		public static Task WaitForControlDiv(object nativeWebView, string controlValueToWaitFor)
		{
			return Task.CompletedTask;
		}

		public static Task<string> ExecuteScriptAsync(object nativeWebView, string script)
		{
			return Task.FromResult<string>(null);
		}
	}
#endif
}
