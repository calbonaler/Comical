using System;
using System.Net;
using System.Threading.Tasks;

namespace Comical.Archivers.Manager
{
	public static class WebClientExtensions
	{
		public static async Task DownloadFileTaskAsync(this WebClient client, Uri address, string fileName, IProgress<int> progress)
		{
			if (address == null)
				throw new ArgumentNullException("address");
			TaskCompletionSource<bool> source = new TaskCompletionSource<bool>(address);
			DownloadProgressChangedEventHandler progressChanged = (s, ev) =>
			{
				if (address.Equals(ev.UserState) && progress != null)
					progress.Report(ev.ProgressPercentage);
			};
			System.ComponentModel.AsyncCompletedEventHandler completed = null;
			completed = (s, ev) =>
			{
				if (address.Equals(ev.UserState))
				{
					client.DownloadProgressChanged -= progressChanged;
					client.DownloadFileCompleted -= completed;
					if (ev.Cancelled)
						source.SetCanceled();
					else if (ev.Error != null)
						source.SetException(ev.Error);
					else
						source.SetResult(true);
				}
			};
			client.DownloadProgressChanged += progressChanged;
			client.DownloadFileCompleted += completed;
			client.DownloadFileAsync(address, fileName, address);
			await source.Task;
		}
	}
}
