using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Web.Site
{
	public static class ValidationExtensions
	{

		public static string ToPrizePlace(this int position)
		{
			if (position == 1)
				return "1st";
			if (position == 2)
				return "2nd";
			if (position == 3)
				return "3rd";
			return position + "th";
		}



		public static string GetTimeToGo(this DateTime end)
		{
			if (end > DateTime.UtcNow)
			{
				var timespan = end - DateTime.UtcNow;
				if (timespan.TotalDays > 0)
				{
					return new TimeSpan(timespan.Days, timespan.Hours, 0, 0).ToReadableString();
				}
				else if (timespan.TotalHours > 0)
				{
					return new TimeSpan(timespan.Days, timespan.Hours, timespan.Minutes, 0).ToReadableString();
				}
				return timespan.ToReadableString();
			}
			return string.Empty;
		}

		public static string ToReadableString(this TimeSpan span)
		{
			string formatted = string.Format("{0}{1}{2}{3}",
					span.Duration().Days > 0 ? string.Format("{0:0} day{1}, ", span.Days, span.Days == 1 ? String.Empty : "s") : string.Empty,
					span.Duration().Hours > 0 ? string.Format("{0:0} hour{1}, ", span.Hours, span.Hours == 1 ? String.Empty : "s") : string.Empty,
					span.Duration().Minutes > 0 ? string.Format("{0:0} minute{1}, ", span.Minutes, span.Minutes == 1 ? String.Empty : "s") : string.Empty,
					span.Duration().Seconds > 0 ? string.Format("{0:0} second{1}", span.Seconds, span.Seconds == 1 ? String.Empty : "s") : string.Empty);

			if (formatted.EndsWith(", ")) formatted = formatted.Substring(0, formatted.Length - 2);

			if (string.IsNullOrEmpty(formatted)) formatted = "0 seconds";

			return formatted;
		}

		public static string ToReadableStringShort(this TimeSpan span)
		{
			string formatted = string.Format("{0}{1}{2}{3}",
					span.Duration().Days > 0 ? string.Format("{0:0}d,", span.Days) : string.Empty,
					span.Duration().Hours > 0 ? string.Format("{0:0}h,", span.Hours) : string.Empty,
					span.Duration().Minutes > 0 ? string.Format("{0:0}m,", span.Minutes) : string.Empty,
					span.Duration().Seconds > 0 ? string.Format("{0:0}s", span.Seconds) : string.Empty);

			return !string.IsNullOrEmpty(formatted)
					? formatted.TrimEnd(',') : "0s";
		}

	

		public static string ElapsedTime(this DateTime dtEvent, string postfix = "ago", string prefix = "")
		{
			TimeSpan TS = DateTime.UtcNow - dtEvent;
			int intYears = DateTime.UtcNow.Year - dtEvent.Year;
			int intMonths = DateTime.UtcNow.Month - dtEvent.Month;
			int intDays = DateTime.UtcNow.Day - dtEvent.Day;
			int intHours = DateTime.UtcNow.Hour - dtEvent.Hour;
			int intMinutes = DateTime.UtcNow.Minute - dtEvent.Minute;
			int intSeconds = DateTime.UtcNow.Second - dtEvent.Second;
			if (intYears > 0) return String.Format("{0}{1} {2} {3}", prefix, intYears, (intYears == 1) ? "year" : "years", postfix);
			else if (intMonths > 0) return String.Format("{0}{1} {2} {3}", prefix, intMonths, (intMonths == 1) ? "month" : "months", postfix);
			else if (intDays > 0) return String.Format("{0}{1} {2} {3}", prefix, intDays, (intDays == 1) ? "day" : "days", postfix);
			else if (intHours > 0) return String.Format("{0}{1} {2} {3}", prefix, intHours, (intHours == 1) ? "hour" : "hours", postfix);
			else if (intMinutes > 0) return String.Format("{0}{1} {2} {3}", prefix, intMinutes, (intMinutes == 1) ? "minute" : "minutes", postfix);
			else if (intSeconds > 0) return String.Format("{0}{1} {2} {3}", prefix, intSeconds, (intSeconds == 1) ? "second" : "seconds", postfix);
			else
			{
				return String.Format("{0}{1} {2} {3}", prefix, dtEvent.ToShortDateString(), dtEvent.ToShortTimeString(), postfix);
			}
		}

		public static string ToTimeAgo(this double secondsAgo)
		{
			if (secondsAgo > 0)
			{
				return DateTime.UtcNow.AddSeconds(-secondsAgo).ElapsedTime();
			}
			return "Never";
		}

		public static string ToTimeRemaining(this DateTime time)
		{
			double seconds = (time - DateTime.UtcNow).TotalSeconds;
			if (seconds > 0)
			{
				return DateTime.UtcNow.AddSeconds(-seconds).ElapsedTime("");
			}
			return "Unknown";
		}

		public static string ToForumTime(this DateTime time)
		{
			bool today = time.Date == DateTime.UtcNow.Date;
			if (today)
			{
				return string.Format("Today {0}", time.ToString("h:mm:ss tt"));
			}
			bool yesterday = time.Date.AddDays(1) == DateTime.UtcNow.Date;
			if (yesterday)
			{
				return string.Format("Yesterday {0}", time.ToString("h:mm:ss tt"));
			}
			return time.ToString();
		}


		public static string FlattenErrors(this ModelStateDictionary modelState)
		{
			if (modelState != null && modelState.Any())
			{
				return string.Join("<br />", modelState.Where(x => x.Value.Errors.Count != 0)
												.SelectMany(x => x.Value.Errors)
												.Select(x => x.ErrorMessage)
												.ToArray());
			}
			return string.Empty;
		}

		public static string FirstError(this ModelStateDictionary modelState)
		{
			if (modelState != null && modelState.Any())
			{
				var error = modelState.Where(x => x.Value.Errors.Count != 0).SelectMany(x => x.Value.Errors).FirstOrDefault();
				return error != null ? error.ErrorMessage : "An unknown error occured, if problem persists please contact support.";
			}
			return string.Empty;
		}
	}

	public class ValidateFileAttribute : ValidationAttribute, IClientValidatable
	{
		public string ErrorFileSize { get; set; }
		public string ErrorFileType { get; set; }
		public int MaxSize { get; set; }
		public SizeType MaxSizeType { get; set; }
		public string[] AllowedFileExtensions { get; set; }

		public ValidateFileAttribute()
				: base()
		{
			AllowedFileExtensions = new string[] { };
			ErrorFileType = "File must be one of the following formats: {0}";
			ErrorFileType = "File must be smaller than {0}";
		}

		public override bool IsValid(object value)
		{
			var file = value as HttpPostedFileBase;
			if (file == null)
			{
				return false;
			}

			// Check file extension
			if (AllowedFileExtensions.Any())
			{
				if (!AllowedFileExtensions.Contains(file.FileName.Substring(file.FileName.LastIndexOf('.')).ToLower()))
				{
					ErrorMessage = string.Format(ErrorFileType, string.Join(", ", AllowedFileExtensions));
					return false;
				}
			}

			// Check filesize
			if (MaxSize > 0)
			{
				int size = 1024 * MaxSize; // KB
				if (MaxSizeType == ValidateFileAttribute.SizeType.MB)
				{
					size = 1024 * 1024 * MaxSize; // MB
				}
				else if (MaxSizeType == ValidateFileAttribute.SizeType.GB)
				{
					size = 1024 * 1024 * 1024 * MaxSize; // GB
				}

				if (file.ContentLength > size)
				{
					ErrorMessage = string.Format(ErrorFileSize, string.Format("{0}{1}", MaxSize, MaxSizeType));
					return false;
				}
			}
			return true;
		}

		public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
		{
			yield return new ModelClientValidationRule
			{
				ErrorMessage = this.ErrorMessage,
				ValidationType = "filevalidation"
			};

		}

		public enum SizeType
		{
			KB,
			MB,
			GB
		}
	}

	public class ImageFileAttribute : ValidateFileAttribute, IClientValidatable
	{
		public ImageFileAttribute()
				: base()
		{
			ErrorDimensions = "Image must be {0} or less.";
		}

		public int Width { get; set; }
		public int Height { get; set; }
		public string ErrorDimensions { get; set; }

		public override bool IsValid(object value)
		{
			if (base.IsValid(value))
			{
				var file = value as HttpPostedFileBase;
				if (file == null)
				{
					return false;
				}

				using (var image = System.Drawing.Image.FromStream(file.InputStream))
				{
					if ((Height > 0 && image.Height > Height) || (Width > 0 && image.Width > Width))
					{
						ErrorMessage = string.Format(ErrorDimensions, string.Format("{0}x{1}", Width, Height));
						return false;
					}
				}
				return true;
			}
			return false;
		}

		#region IClientValidatable Members

		//public override IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
		//{
		//    yield return new ModelClientValidationRule
		//    {
		//        ErrorMessage = this.ErrorMessage,
		//        ValidationType = "imagevalidation"
		//    };

		//}

		#endregion
	}

	public class HttpPostedFileExtensionAttribute : ValidationAttribute, IClientValidatable
	{
		private readonly FileExtensionsAttribute _fileExtensionsAttribute = new FileExtensionsAttribute();

		public HttpPostedFileExtensionAttribute()
		{
			ErrorMessage = _fileExtensionsAttribute.ErrorMessage;
		}

		public string Extensions
		{
			get { return _fileExtensionsAttribute.Extensions; }
			set { _fileExtensionsAttribute.Extensions = value; }
		}

		public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
		{
			var rule = new ModelClientValidationRule
			{
				ValidationType = "extension",
				ErrorMessage = FormatErrorMessage(metadata.GetDisplayName())
			};

			rule.ValidationParameters["extension"] =
					_fileExtensionsAttribute.Extensions
							.Replace(" ", string.Empty).Replace(".", string.Empty)
							.ToLowerInvariant();

			yield return rule;
		}

		public override string FormatErrorMessage(string name)
		{
			return _fileExtensionsAttribute.FormatErrorMessage(name);
		}

		public override bool IsValid(object value)
		{
			var file = value as HttpPostedFileBase;
			return _fileExtensionsAttribute.IsValid(file != null ? file.FileName : value);
		}
	}
	
	
}