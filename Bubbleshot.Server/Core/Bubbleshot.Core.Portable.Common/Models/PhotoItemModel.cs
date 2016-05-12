﻿using System;
using Bubbleshot.Core.Portable.Common.Enums;

namespace Bubbleshot.Core.Portable.Common.Models
{
	public class PhotoItemModel
	{
		public int Source { get; set; }
		public string ImageLink { get; set; }
		public string ProfileLink { get; set; }
		public ChannelType ChannelType { get; set; }
		public DateTime TimeCreated { get; set; }
		public double Longitude { get; set; }
		public double Latitude { get; set; }
	}
}