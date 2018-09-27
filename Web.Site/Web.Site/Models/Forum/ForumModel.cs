using Cryptopia.Common.Validation;
using PagedList;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;


namespace Web.Site.Models
{
	public class ForumViewModel
	{
		public ForumViewModel()
		{
			Forums = new List<ForumModel>();
			LatestPosts = new List<ForumPostModel>();
		}
		public List<ForumModel> Forums { get; set; }
		public List<ForumPostModel> LatestPosts { get; set; }
	}

	public class ForumModel
	{
		public ForumModel()
		{
			Icon = "fa fa-list-ol";
			Categories = new List<ForumCategoryModel>();
		}
		public int Id { get; set; }
		public string Name { get; set; }
		public string Description { get; set; }
		public string Icon { get; set; }
		public List<ForumCategoryModel> Categories { get; set; }
	}

	public class ForumCategoryModel
	{
		public ForumCategoryModel()
		{
			Icon = "fa fa-newspaper-o";
			Threads = new List<ForumThreadModel>();
		}
		public int Id { get; set; }
		public string Name { get; set; }
		public string Description { get; set; }
		public string Icon { get; set; }

		public ForumModel Forum { get; set; }
		public List<ForumThreadModel> Threads { get; set; }
		public int ThreadCount { get; set; }
		public int PostCount { get; set; }
		public List<ForumPostModel> LatestPosts { get; set; }


	}

	public class ForumThreadModel
	{
		public ForumThreadModel()
		{
			Icon = "fa fa-file-text-o";
			//Posts = new PagedList<ForumPostModel>();
		}
		public int Id { get; set; }
		public string Name { get; set; }
		public string Description { get; set; }
		public string Icon { get; set; }
		public bool IsPinned { get; set; }

		public ForumModel Forum { get; set; }
		public ForumCategoryModel Category { get; set; }
		public IPagedList<ForumPostModel> Posts { get; set; }
		public int PostCount { get; set; }
		public int? PostId { get; set; }
		public ForumUser User { get; set; }

		public ForumPostModel LastPost { get; set; }




	}


	public class ForumPostModel
	{
		public ForumPostModel()
		{
			User = new ForumUser();
			Timestamp = DateTime.UtcNow;
		}

		public int Id { get; set; }
		public int PostNum { get; set; }
		public int ThreadId { get; set; }
		public string ThreadTitle { get; set; }
		public string Message { get; set; }
		public string ForumName { get; set; }
		public DateTime Timestamp { get; set; }
		public ForumUser User { get; set; }
		public bool IsFirst { get; set; }
		public DateTime? Edited { get; set; }
	}

	public class ForumUser
	{
		public string Signature { get; set; }
		public string Name { get; set; }
		public double Rating { get; set; }
		public string Gender { get; set; }
		public string Country { get; set; }
		public int ThreadCount { get; set; }
		public int PostCount { get; set; }
	}

	[Bind(Include = "Title, Description, Order")]
	public class CreateForumModel
	{
		[RequiredBase]
		[StringLengthBase(128)]
		[Display(Name = nameof(Resources.Forum.forumTitleLabel), ResourceType = typeof(Resources.Forum))]
		public string Title { get; set; }

		[RequiredBase]
		[StringLengthBase(512)]
		[Display(Name = nameof(Resources.Forum.forumDescriptionLabel), ResourceType = typeof(Resources.Forum))]
		public string Description { get; set; }

		[RequiredBase]
		[Display(Name = nameof(Resources.Forum.forumOrderLabel), ResourceType = typeof(Resources.Forum))]
		public int Order { get; set; }
	}

	[Bind(Include = "ForumId, Title, Description, Order")]
	public class UpdateForumModel
	{
		public int ForumId { get; set; }

		[RequiredBase]
		[StringLengthBase(128)]
		[Display(Name = nameof(Resources.Forum.forumTitleLabel), ResourceType = typeof(Resources.Forum))]
		public string Title { get; set; }

		[RequiredBase]
		[StringLengthBase(512)]
		[Display(Name = nameof(Resources.Forum.forumDescriptionLabel), ResourceType = typeof(Resources.Forum))]
		public string Description { get; set; }

		[RequiredBase]
		[Display(Name = nameof(Resources.Forum.forumOrderLabel), ResourceType = typeof(Resources.Forum))]
		public int Order { get; set; }
	}

	[Bind(Include = "ForumId, Title, Description, Order")]
	public class CreateCategoryModel
	{
		public int ForumId { get; set; }

		[RequiredBase]
		[StringLengthBase(128)]
		[Display(Name = nameof(Resources.Forum.categoryTitleLabel), ResourceType = typeof(Resources.Forum))]
		public string Title { get; set; }

		[RequiredBase]
		[StringLengthBase(512)]
		[Display(Name = nameof(Resources.Forum.categoryDescriptionLabel), ResourceType = typeof(Resources.Forum))]
		public string Description { get; set; }

		[RequiredBase]
		[Display(Name = nameof(Resources.Forum.categoryOrderLabel), ResourceType = typeof(Resources.Forum))]
		public int Order { get; set; }
	}

	[Bind(Include = "CategoryId, Title, Description, Order")]
	public class UpdateCategoryModel
	{
		public int CategoryId { get; set; }

		[RequiredBase]
		[StringLengthBase(128)]
		[Display(Name = nameof(Resources.Forum.categoryTitleLabel), ResourceType = typeof(Resources.Forum))]
		public string Title { get; set; }

		[RequiredBase]
		[StringLengthBase(512)]
		[Display(Name = nameof(Resources.Forum.categoryDescriptionLabel), ResourceType = typeof(Resources.Forum))]
		public string Description { get; set; }

		[RequiredBase]
		[Display(Name = nameof(Resources.Forum.categoryOrderLabel), ResourceType = typeof(Resources.Forum))]
		public int Order { get; set; }
	}


	[Bind(Include = "CategoryId, Title, Message")]
	public class CreateThreadModel
	{
		public int CategoryId { get; set; }

		[RequiredBase]
		[StringLengthBase(128)]
		[Display(Name = nameof(Resources.Forum.threadTitleLabel), ResourceType = typeof(Resources.Forum))]
		public string Title { get; set; }

		[RequiredBase]
		[AllowHtml]
		[UIHint("tinymce_jquery_full")]
		[Display(Name = nameof(Resources.Forum.threadMessageLabel), ResourceType = typeof(Resources.Forum))]
		public string Message { get; set; }
	}

	[Bind(Include = "ThreadId, Title, Message")]
	public class UpdateThreadModel
	{
		public int ThreadId { get; set; }

		[RequiredBase]
		[StringLengthBase(128)]
		[Display(Name = nameof(Resources.Forum.threadTitleLabel), ResourceType = typeof(Resources.Forum))]
		public string Title { get; set; }

		[RequiredBase]
		[AllowHtml]
		[UIHint("tinymce_jquery_full")]
		[Display(Name = nameof(Resources.Forum.threadMessageLabel), ResourceType = typeof(Resources.Forum))]
		public string Message { get; set; }
	}

	[Bind(Include = "ThreadId, Message")]
	public class CreatePostModel
	{
		public int ThreadId { get; set; }

		[RequiredBase]
		[AllowHtml]
		[UIHint("tinymce_jquery_full")]
		[Display(Name = nameof(Resources.Forum.postMessageLabel), ResourceType = typeof(Resources.Forum))]
		public string Message { get; set; }

		public string ThreadName { get; set; }
	}

	[Bind(Include = "PostId, ThreadId, Message")]
	public class UpdatePostModel
	{
		public int PostId { get; set; }
		public int ThreadId { get; set; }

		[RequiredBase]
		[AllowHtml]
		[UIHint("tinymce_jquery_full")]
		[Display(Name = nameof(Resources.Forum.postMessageLabel), ResourceType = typeof(Resources.Forum))]
		public string Message { get; set; }
	}

	[Bind(Include = "PostId, Message")]
	public class ReportPostModel
	{
		public int PostId { get; set; }

		[RequiredBase]
		[Display(Name = nameof(Resources.Forum.postReportMessageLabel), ResourceType = typeof(Resources.Forum))]
		public string Message { get; set; }
	}

	[Bind(Include = "SearchText, SearchPosts, Page")]
	public class SearchForumModel
	{
		[MinLength(2)]
		public string SearchText { get; set; }
		public bool SearchPosts { get; set; }
		public int? Page { get; set; }
		public IPagedList<SearchForumResultModel> SearchResult { get; set; }
	}

	public class SearchForumResultModel
	{
		public string ResultTitle { get; set; }
		public string ResultText { get; set; }
		public int? PostId { get; set; }
		public int ThreadId { get; set; }
		public DateTime Timestamp { get; set; }
	}
}

