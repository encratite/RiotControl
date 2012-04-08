using System;
using System.Net;

namespace RiotGear
{
	public interface IUpdateHandler
	{
		void UpdateDetected(int currentRevision, ApplicationVersion newVersion);
		void DownloadProgressUpdate(DownloadProgressChangedEventArgs arguments);
		void DownloadCompleted();
		void DownloadError(Exception exception);
	}
}
