﻿using System;
using System.Linq;
using Bubbleshot.Core.Portable.Adapters.Base;
using Bubbleshot.Core.Portable.Adapters.EventArgs;
using Bubbleshot.Core.Portable.Adapters.Helpers;
using Bubbleshot.Core.Portable.Adapters.Rules;
using Bubbleshot.Core.Portable.Common.Extensions;
using Bubbleshot.Core.Portable.Common.Requests.Instagram;

namespace Bubbleshot.Core.Portable.Adapters.Instagram
{
	public class InstagramAdapter : BaseAdapter<InstagramAdapterConfig>, IAdapter
	{
		private InstagramPhotosSearchRequestParameters  _instagramPhotosSearchRequestParameters;
		private readonly InstagramPhotosSearchHttpRequest _instagramPhotosSearchHttpRequest;

		private readonly string _accessToken;

		public InstagramAdapter(InstagramAdapterConfig c) : base(c)
		{
			_accessToken = c.AccessToken;
			_instagramPhotosSearchHttpRequest = new InstagramPhotosSearchHttpRequest(c.ApiAddress);
		}

		public void Start(IAdapterRule rule)
		{
			Active = true;
			PollingManager.Start(TimeSpan.FromSeconds(5), () =>
			{
				var startTime = DateTime.UtcNow.AddSeconds(-5);
				var endTime = DateTime.UtcNow;

				_instagramPhotosSearchRequestParameters = new InstagramPhotosSearchRequestParameters
				{
					StartTime = startTime,
					EndTime = endTime,
					Latitude = rule.Latitude,
					Longitude = rule.Longitude,
					Distance = rule.Radius,
					AccessToken = _accessToken
				};
				var result = _instagramPhotosSearchHttpRequest.Execute(_instagramPhotosSearchRequestParameters);
				if (result?.Result?.Images.Count > 0)
				{
					result.Result.Images = result.Result.Images.Where(
						i =>
							i.DateUnixStyle >= startTime.ToUnixStyle() &&
							i.DateUnixStyle <= endTime.ToUnixStyle()).ToList();
				}
				if (result?.Result?.Images.Count > 0)
				{
					var mapper = new InstagramPhotoItemMapper();
					var genericResult = mapper.MapVkPhotoItems(result.Result.Images).ToList();
					OnNewPhotosReceived?.Invoke(this, new NewPhotoAlertEventArgs { Count = result.Result.Images.Count, Photos = genericResult });
				}

			});
		}

		public void Stop()
		{
			Active = false;
			PollingManager.Stop();
		}

		public bool IsActive => Active;
		public event EventHandler<NewPhotoAlertEventArgs> OnNewPhotosReceived;
	}
}
