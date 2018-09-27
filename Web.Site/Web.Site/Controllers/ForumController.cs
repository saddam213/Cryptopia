using System;
using System.Threading.Tasks;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;

using Web.Site.Models;
using System.Linq;
using Cryptopia.Data.DataContext;
using System.Collections.Generic;
using PagedList;
using Ganss.XSS;
using System.Data.Entity;
using Cryptopia.Entity;

namespace Web.Site.Controllers
{
	public class ForumController : BaseController
	{
		#region Forum

		[HttpGet]
		[AllowAnonymous]
		public async Task<ActionResult> Index()
		{
			using (var context = new ApplicationDbContext())
			{
				var model = new ForumViewModel
				{
					Forums = await context.Forums.Where(x => !x.IsDeleted).OrderBy(x => x.Order).Select(x => new ForumModel
					{
						Id = x.Id,
						Name = x.Title,
						Description = x.Description,
						Icon = x.Icon,
						Categories = x.Categories.Where(c => !c.IsDeleted).OrderBy(c => c.Order).Select(c => new ForumCategoryModel
						{
							Id = c.Id,
							Name = c.Name,
							Icon = c.Icon,
							Description = c.Description,
							ThreadCount = c.Threads.Count(g => !g.IsDeleted),
							PostCount = c.Threads.SelectMany(g => g.Posts).Count(g => !g.IsDeleted),
						}).ToList(),
					}).ToListAsync(),
					LatestPosts = await context.ForumPosts.Where(x => !x.IsDeleted).OrderByDescending(x => x.Id).Take(20).Select(x => new ForumPostModel
					{
						Id = x.Id,
						Message = x.Message,
						ThreadTitle = x.Thread.Title,
						ThreadId = x.ThreadId,
						ForumName = x.Thread.Category.Name
					}).ToListAsync()
				};

				var sanitizer = new HtmlSanitizer(allowedTags: new List<string> { "p" });
				foreach (var message in model.LatestPosts)
				{
					message.Message = sanitizer.Sanitize(message.Message).Replace("<p>","").Replace("</p>","").Trim();
				}

				return View("Forum", model);
			}
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		[Authorize(Roles = "Admin, Moderator")]
		public ActionResult CreateForum()
		{
			return View("CreateForum", new CreateForumModel());
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		[Authorize(Roles = "Admin, Moderator")]
		public ActionResult SubmitForum(CreateForumModel model)
		{
			if (ModelState.IsValid)
			{
				using (var context = new ApplicationDbContext())
				{
					context.Forums.Add(new Forum
					{
						Title = model.Title,
						Description = model.Description,
						Order = model.Order
					});
					context.SaveChanges();
				}
			}
			return RedirectToAction("Index");
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		[Authorize(Roles = "Admin, Moderator")]
		public ActionResult UpdateForum(int forumId)
		{
			using (var context = new ApplicationDbContext())
			{
				var forum = context.Forums.FirstOrDefault(x => x.Id == forumId && !x.IsDeleted);
				if (forum == null)
				{
					return View("ViewMessageModal", new ViewMessageModel(ViewMessageType.Warning, "Forum Not Found", string.Format("The following forum #{0} could not be found.", forumId)));
				}

				return View("UpdateForum", new UpdateForumModel
				{
					ForumId = forum.Id,
					Title = forum.Title,
					Description = forum.Description,
					Order = forum.Order
				});
			}
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		[Authorize(Roles = "Admin, Moderator")]
		public ActionResult SubmitUpdateForum(UpdateForumModel model)
		{
			if (ModelState.IsValid)
			{
				using (var context = new ApplicationDbContext())
				{
					var forum = context.Forums.FirstOrDefault(x => x.Id == model.ForumId && !x.IsDeleted);
					if (forum != null)
					{
						forum.Title = model.Title;
						forum.Description = model.Description;
						forum.Order = model.Order;
						context.SaveChanges();
					}
				}
			}
			return RedirectToAction("Index");
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		[Authorize(Roles = "Admin, Moderator")]
		public ActionResult DeleteForum(int forumId)
		{
			using (var context = new ApplicationDbContext())
			{
				var forum = context.Forums.FirstOrDefault(x => x.Id == forumId && !x.IsDeleted);
				if (forum != null)
				{
					foreach (var category in forum.Categories)
					{
						foreach (var thread in category.Threads)
						{
							foreach (var post in thread.Posts)
							{
								post.IsDeleted = true;
							}
							thread.IsDeleted = true;
						}
						category.IsDeleted = true;
					}
					forum.IsDeleted = true;
					context.SaveChanges();
					return JsonSuccess();
				}
				return JsonError(string.Format(Resources.Forum.forumDeleteNotFoundError, forumId));
			}
		}

		#endregion

		#region Category

		[HttpGet]
		[AllowAnonymous]
		public async Task<ActionResult> Category(int id)
		{
			using (var context = new ApplicationDbContext())
			{
				var category = await context.ForumCategories.FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted);
				if (category == null)
				{
					return View("ViewMessage", new ViewMessageModel(ViewMessageType.Warning, Resources.Forum.categoryNotFoundErrorTitle, string.Format(Resources.Forum.categoryNotFoundErrorMessage, id)));
				}

				var model = new ForumCategoryModel
				{
					Id = category.Id,
					Name = category.Name,
					Icon = category.Icon,
					Forum = new ForumModel
					{
						Id = category.Forum.Id,
						Name = category.Forum.Title,
						Icon = category.Forum.Icon
					},
					Threads = category.Threads.Where(x => !x.IsDeleted).OrderByDescending(x => x.IsPinned).ThenByDescending(x => x.LastUpdate).Select(x => new ForumThreadModel
					{
						Id = x.Id,
						Name = x.Title,
						Description = x.Description,
						PostCount = x.Posts.Count(g => !g.IsDeleted),
						IsPinned = x.IsPinned,
						Icon = x.Icon
					}).ToList(),
					LatestPosts = category.Threads.Where(x => !x.IsDeleted).SelectMany(x => x.Posts).Where(x => !x.IsDeleted).OrderByDescending(x => x.Id).Take(20).Select(x => new ForumPostModel
					{
						Id = x.Id,
						Message = x.Message,
						ThreadTitle = x.Thread.Title,
						ThreadId = x.ThreadId,
						ForumName = x.Thread.Category.Name
					}).ToList()
				};

				var sanitizer = new HtmlSanitizer(allowedTags: new List<string> { "p" });
				foreach (var message in model.LatestPosts)
				{
					message.Message = sanitizer.Sanitize( message.Message).Replace("<p>", "").Replace("</p>", "").Trim();
				}

				foreach (var threadModel in model.Threads)
				{
					var thread = category.Threads.Where(x => !x.IsDeleted).FirstOrDefault(x => x.Id == threadModel.Id);
					if (thread != null)
					{
						var lastPost = thread.Posts.Where(x => !x.IsDeleted).OrderByDescending(x => x.Id).FirstOrDefault();
						if (lastPost != null)
						{
							threadModel.LastPost = new ForumPostModel
							{
								Id = lastPost.Id,
								Timestamp = lastPost.Timestamp,
								User = new ForumUser
								{
									Name = lastPost.User.UserName
								}
							};
						}
					}
				}

				return View("Category", model);
			}
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		[Authorize(Roles = "Admin, Moderator")]
		public ActionResult CreateCategory(int forumId)
		{
			return View("CreateCategory", new CreateCategoryModel { ForumId = forumId });
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		[Authorize(Roles = "Admin, Moderator")]
		public async Task<ActionResult> SubmitCategory(CreateCategoryModel model)
		{
			if (ModelState.IsValid)
			{
				using (var context = new ApplicationDbContext())
				{
					var category = new ForumCategory
					{
						ForumId = model.ForumId,
						Name = model.Title,
						Description = model.Description,
						Order = model.Order
					};
					context.ForumCategories.Add(category);
					await context.SaveChangesAsync().ConfigureAwait(false);
					return RedirectToAction("Category", new { id = category.Id });
				}
			}
			return RedirectToAction("Index");
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		[Authorize(Roles = "Admin, Moderator")]
		public async Task<ActionResult> UpdateCategory(int categoryId)
		{
			using (var context = new ApplicationDbContext())
			{
				var category = await context.ForumCategories.FirstOrDefaultAsync(x => x.Id == categoryId && !x.IsDeleted);
				if (category == null)
				{
					return View("ViewMessageModal", new ViewMessageModel(ViewMessageType.Warning, Resources.Forum.categoryNotFoundErrorTitle, string.Format(Resources.Forum.categoryNotFoundErrorMessage, categoryId)));
				}

				return View("UpdateCategory", new UpdateCategoryModel
				{
					CategoryId = category.Id,
					Title = category.Name,
					Description = category.Description,
					Order = category.Order
				});
			}
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		[Authorize(Roles = "Admin, Moderator")]
		public async Task<ActionResult> SubmitUpdateCategory(UpdateCategoryModel model)
		{
			if (ModelState.IsValid)
			{
				using (var context = new ApplicationDbContext())
				{
					var category = await context.ForumCategories.FirstOrDefaultAsync(x => x.Id == model.CategoryId && !x.IsDeleted);
					if (category != null)
					{
						category.Name = model.Title;
						category.Description = model.Description;
						category.Order = model.Order;
						await context.SaveChangesAsync().ConfigureAwait(false);
					}
				}
			}
			return RedirectToAction("Category", new { id = model.CategoryId });
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		[Authorize(Roles = "Admin, Moderator")]
		public async Task<ActionResult> DeleteCategory(int categoryId)
		{
			using (var context = new ApplicationDbContext())
			{
				var category = await context.ForumCategories.FirstOrDefaultAsync(x => x.Id == categoryId && !x.IsDeleted);
				foreach (var thread in category.Threads)
				{
					foreach (var post in thread.Posts)
					{
						post.IsDeleted = true;
					}
					thread.IsDeleted = true;
				}
				category.IsDeleted = true;
				await context.SaveChangesAsync().ConfigureAwait(false);
				return JsonSuccess();
			}
		}

		#endregion

		#region Thread

		[HttpGet]
		[AllowAnonymous]
		public async Task<ActionResult> Thread(int id, int? page, int? postId)
		{
			using (var context = new ApplicationDbContext())
			{
				var thread = await context.ForumThreads.FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted);
				if (thread == null)
				{
					return ViewMessage(new ViewMessageModel(ViewMessageType.Info, Resources.Forum.threadNotFoundErrorTitle, Resources.Forum.threadNotFoundErrorMessage));
				}

				var count = thread.Posts.Where(x => !x.IsDeleted).Count();
				if (postId.HasValue)
				{
					var index = thread.Posts.Where(x => !x.IsDeleted).OrderBy(x => x.Id).Select((x, i) => new { Index = i, Item = x }).FirstOrDefault(x => x.Item.Id == postId.Value);
					if (index != null)
					{
						page = 1 + (index.Index / 25);
					}
				}
				var sanitizer = new HtmlSanitizer();
				var skip = Math.Max(page.HasValue ? page.Value - 1 : 0, 0) * 25;
				var posts = thread.Posts.Where(x => !x.IsDeleted).OrderBy(x => x.Id).Skip(skip).Take(25).Select((x, i) => new ForumPostModel
				{
					Id = x.Id,
					PostNum = skip + i,
					ThreadId = x.ThreadId,
					ThreadTitle = x.Thread.Title,
					IsFirst = x.IsFirstPost,
					Message = sanitizer.Sanitize(x.Message),
					Timestamp = x.Timestamp,
					Edited = x.LastUpdate != x.Timestamp ? x.LastUpdate : default(DateTime?),
					User = new ForumUser
					{
						Gender = x.User.Profile.IsPublic ? x.User.Profile.Gender : Resources.Forum.userGenderUnknown,
						Name = x.User.UserName,
						Country = x.User.Profile.IsPublic ? x.User.Profile.Country : Resources.Forum.userCountryUnknown,
						Rating = x.User.TrustRating,
						PostCount = x.User.ForumPosts.Count,
						ThreadCount = x.User.ForumThreads.Count,
						Signature = sanitizer.Sanitize(x.User.ForumSignature)
					}
				}).ToList();

				var pagedList = new StaticPagedList<ForumPostModel>(posts, Math.Max(page.HasValue ? page.Value : 0, 1), 25, count);
				return View("Thread", new ForumThreadModel
				{
					Id = thread.Id,
					Name = thread.Title,
					PostId = postId,
					Forum = new ForumModel
					{
						Id = thread.Category.Forum.Id,
						Name = thread.Category.Forum.Title
					},
					Category = new ForumCategoryModel
					{
						Id = thread.Category.Id,
						Name = thread.Category.Name
					},
					Posts = pagedList
				});
			}
		}

		[HttpPost]
		[Authorize]
		[ValidateAntiForgeryToken]
		public ActionResult CreateThread(int categoryId)
		{
			return View("CreateThread", new CreateThreadModel { CategoryId = categoryId });
		}

		[HttpPost]
		[Authorize]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> SubmitThread(CreateThreadModel model)
		{
			if (ModelState.IsValid)
			{
				using (var context = new ApplicationDbContext())
				{
					var thread = new ForumThread
					{
						CategoryId = model.CategoryId,
						Title = model.Title,
						UserId = User.Identity.GetUserId()
					};

					var sanitizer = new HtmlSanitizer();
					var post = new ForumPost
					{
						UserId = User.Identity.GetUserId(),
						Message = sanitizer.Sanitize(model.Message),
						IsFirstPost = true,
					};
					thread.Posts = new List<ForumPost>();
					thread.Posts.Add(post);
					context.ForumThreads.Add(thread);
					await context.SaveChangesAsync().ConfigureAwait(false);
					return RedirectToAction("Thread", new { id = thread.Id });
				}
			}
			return View("CreateThread", model);
		}

		[HttpGet]
		[Authorize]
		public async Task<ActionResult> UpdateThread(int threadId)
		{
			using (var context = new ApplicationDbContext())
			{
				var sanitizer = new HtmlSanitizer();
				var thread = await context.ForumThreads.FirstOrDefaultAsync(x => x.Id == threadId && !x.IsDeleted);
				if (thread == null)
				{
					return ViewMessage(new ViewMessageModel(ViewMessageType.Warning, Resources.Forum.threadNotFoundErrorTitle, Resources.Forum.threadNotFoundErrorMessage));
				}

				if (!(User.IsInRole("Admin") || User.IsInRole("Moderator")))
				{
					if (!thread.UserId.Equals(User.Identity.GetUserId(), StringComparison.OrdinalIgnoreCase))
					{
						return ViewMessage(new ViewMessageModel(ViewMessageType.Warning, Resources.Forum.threadUpdatePermitionErrorTitle, Resources.Forum.threadUpdatePermitionErrorMessage));
					}
				}

				var firstPost = thread.Posts.FirstOrDefault(x => x.IsFirstPost && !x.IsDeleted);
				if (firstPost == null)
				{
					return ViewMessage(new ViewMessageModel(ViewMessageType.Warning, Resources.Forum.threadNotFoundErrorTitle, Resources.Forum.threadNotFoundErrorMessage));
				}

				return View("UpdateThread", new UpdateThreadModel
				{
					ThreadId = thread.Id,
					Title = thread.Title,
					Message = sanitizer.Sanitize(firstPost.Message)
				});
			}
		}

		[HttpPost]
		[Authorize]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> SubmitUpdateThread(UpdateThreadModel model)
		{
			if (ModelState.IsValid)
			{
				var sanitizer = new HtmlSanitizer();
				using (var context = new ApplicationDbContext())
				{
					var thread = await context.ForumThreads.FirstOrDefaultAsync(x => x.Id == model.ThreadId && !x.IsDeleted);
					if (thread == null)
					{
						return ViewMessage(new ViewMessageModel(ViewMessageType.Warning, Resources.Forum.threadNotFoundErrorTitle, Resources.Forum.threadNotFoundErrorMessage));
					}

					if (!(User.IsInRole("Admin") || User.IsInRole("Moderator")))
					{
						if (!thread.UserId.Equals(User.Identity.GetUserId(), StringComparison.OrdinalIgnoreCase))
						{
							return ViewMessage(new ViewMessageModel(ViewMessageType.Warning, Resources.Forum.threadUpdatePermitionErrorTitle, Resources.Forum.threadUpdatePermitionErrorMessage));
						}
					}

					var firstPost = thread.Posts.FirstOrDefault(x => x.IsFirstPost && !x.IsDeleted);
					if (firstPost == null)
					{
						return ViewMessage(new ViewMessageModel(ViewMessageType.Warning, Resources.Forum.threadNotFoundErrorTitle, Resources.Forum.threadNotFoundErrorMessage));
					}

					firstPost.LastUpdate = DateTime.UtcNow;
					thread.LastUpdate = DateTime.UtcNow;
					firstPost.Message = sanitizer.Sanitize(model.Message);
					thread.Title = model.Title;
					await context.SaveChangesAsync().ConfigureAwait(false);
					return RedirectToAction("Thread", new { id = model.ThreadId });
				}
			}
			return View("UpdateThread", model);
		}

		[HttpPost]
		[Authorize(Roles = "Admin, Moderator")]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> DeleteThread(int threadId)
		{
			using (var context = new ApplicationDbContext())
			{
				var thread = await context.ForumThreads.FirstOrDefaultAsync(x => x.Id == threadId && !x.IsDeleted);
				foreach (var post in thread.Posts)
				{
					post.IsDeleted = true;
				}
				thread.IsDeleted = true;
				await context.SaveChangesAsync().ConfigureAwait(false);
				return RedirectToAction("Index");
			}
		}

		[HttpPost]
		[Authorize(Roles = "Admin, Moderator")]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> PinThread(int threadId)
		{
			using (var context = new ApplicationDbContext())
			{
				var thread = await context.ForumThreads.FirstOrDefaultAsync(x => x.Id == threadId && !x.IsDeleted);
				if (thread != null)
				{
					thread.IsPinned = !thread.IsPinned;
					await context.SaveChangesAsync().ConfigureAwait(false);
				}
				return JsonSuccess();
			}
		}

		#endregion

		#region Post

		[HttpGet]
		[Authorize]
		public async Task<ActionResult> CreatePost(int threadId, int? quoteId)
		{
			using (var context = new ApplicationDbContext())
			{
				var template = string.Empty;
				var sanitizer = new HtmlSanitizer();
				var thread = await context.ForumThreads.FirstOrDefaultAsync(x => x.Id == threadId && !x.IsDeleted);
				if (thread == null)
				{
					return ViewMessage(new ViewMessageModel(ViewMessageType.Warning, Resources.Forum.threadNotFoundErrorTitle, Resources.Forum.threadNotFoundErrorMessage));
				}

				if (quoteId.HasValue)
				{
					var post = thread.Posts.FirstOrDefault(x => x.Id == quoteId.Value && !x.IsDeleted);
					if (post != null)
					{
						template = string.Format("<blockquote><div>{0}</div><small><i>{1}: {2}</i></small></blockquote><p></p>", post.Message, post.User.UserName, post.Timestamp);
					}
				}

				return View("CreatePost", new CreatePostModel
				{
					ThreadId = threadId,
					Message = sanitizer.Sanitize(template),
					ThreadName = thread.Title
				});
			}
		}

		[HttpPost]
		[Authorize]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> SubmitPost(CreatePostModel model)
		{
			if (ModelState.IsValid)
			{
				using (var context = new ApplicationDbContext())
				{
					var sanitizer = new HtmlSanitizer();
					var post = new ForumPost
					{
						ThreadId = model.ThreadId,
						Message = sanitizer.Sanitize(model.Message),
						UserId = User.Identity.GetUserId()
					};
					context.ForumPosts.Add(post);
					await context.SaveChangesAsync().ConfigureAwait(false);
					return RedirectToAction("Thread", new { id = model.ThreadId, postId = post.Id });
				}
			}
			return View("CreatePost", model);
		}

		[HttpGet]
		[Authorize]
		public async Task<ActionResult> UpdatePost(int postId)
		{
			using (var context = new ApplicationDbContext())
			{
				var sanitizer = new HtmlSanitizer();
				var post = await context.ForumPosts.FirstOrDefaultAsync(x => x.Id == postId);
				if (post == null)
				{
					return ViewMessage(new ViewMessageModel(ViewMessageType.Warning, Resources.Forum.postNotFoundErrorTitle, Resources.Forum.postNotFoundErrorMessage));
				}

				if (!(User.IsInRole("Admin") || User.IsInRole("Moderator")))
				{
					if (!post.UserId.Equals(User.Identity.GetUserId(), StringComparison.OrdinalIgnoreCase))
					{
						return ViewMessage(new ViewMessageModel(ViewMessageType.Warning, Resources.Forum.postUpdatePermitionErrorTitle, Resources.Forum.postUpdatePermitionErrorMessage));
					}
				}

				return View("UpdatePost", new UpdatePostModel
				{
					ThreadId = post.ThreadId,
					PostId = post.Id,
					Message = sanitizer.Sanitize(post.Message)
				});
			}
		}

		[HttpPost]
		[Authorize]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> UpdatePost(UpdatePostModel model)
		{
			if (ModelState.IsValid)
			{
				using (var context = new ApplicationDbContext())
				{
					var sanitizer = new HtmlSanitizer();
					var post = await context.ForumPosts.FirstOrDefaultAsync(x => x.Id == model.PostId && !x.IsDeleted);
					if (post == null)
					{
						return ViewMessage(new ViewMessageModel(ViewMessageType.Warning, Resources.Forum.postNotFoundErrorTitle, Resources.Forum.postNotFoundErrorMessage));
					}

					if (!(User.IsInRole("Admin") || User.IsInRole("Moderator")))
					{
						if (!post.UserId.Equals(User.Identity.GetUserId(), StringComparison.OrdinalIgnoreCase))
						{
							return ViewMessage(new ViewMessageModel(ViewMessageType.Warning, Resources.Forum.postUpdatePermitionErrorTitle, Resources.Forum.postUpdatePermitionErrorMessage));
						}
					}

					post.LastUpdate = DateTime.UtcNow;
					post.Message = sanitizer.Sanitize(model.Message);
					await context.SaveChangesAsync().ConfigureAwait(false);
					return RedirectToAction("Thread", new { id = model.ThreadId, postId = post.Id });
				}
			}
			return View("UpdatePost", model);
		}

		[HttpPost]
		[Authorize]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> DeletePost(int postId)
		{
			using (var context = new ApplicationDbContext())
			{
				var post = await context.ForumPosts.FirstOrDefaultAsync(x => x.Id == postId && !x.IsDeleted);
				if (post == null)
				{
					return ViewMessage(new ViewMessageModel(ViewMessageType.Warning, Resources.Forum.postNotFoundErrorTitle, Resources.Forum.postNotFoundErrorMessage));
				}

				if (!(User.IsInRole("Admin") || User.IsInRole("Moderator")))
				{
					if (!post.UserId.Equals(User.Identity.GetUserId(), StringComparison.OrdinalIgnoreCase))
					{
						return ViewMessage(new ViewMessageModel(ViewMessageType.Warning, Resources.Forum.postDeletePermitionErrorTitle, Resources.Forum.postDeletePermitionErrorMessage));
					}
				}

				post.IsDeleted = true;
				await context.SaveChangesAsync().ConfigureAwait(false);
				return JsonSuccess();
			}
		}

		[HttpPost]
		[Authorize]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> ReportPost(int postId)
		{
			using (var context = new ApplicationDbContext())
			{
				var post = await context.ForumPosts.FirstOrDefaultAsync(x => x.Id == postId && !x.IsDeleted);
				if (post == null)
				{
					return View("ViewMessageModal", new ViewMessageModel(ViewMessageType.Warning, Resources.Forum.postNotFoundErrorTitle, Resources.Forum.postNotFoundErrorMessage));
				}
				return View("ReportPost", new ReportPostModel { PostId = post.Id });
			}
		}

		[HttpPost]
		[Authorize]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> SubmitReportPost(ReportPostModel model)
		{
			using (var context = new ApplicationDbContext())
			{
				var post = await context.ForumPosts.FirstOrDefaultAsync(x => x.Id == model.PostId && !x.IsDeleted);
				if (post == null)
				{
					return ViewMessage(new ViewMessageModel(ViewMessageType.Warning, Resources.Forum.postNotFoundErrorTitle, Resources.Forum.postNotFoundErrorMessage));
				}

				var report = new ForumReport
				{
					PostId = model.PostId,
					UserId = User.Identity.GetUserId(),
					Message = model.Message
				};

				context.ForumReports.Add(report);
				await context.SaveChangesAsync().ConfigureAwait(false);
				return RedirectToAction("Thread", new { id = post.ThreadId, postId = post.Id });
			}
		}

		#endregion

		#region Search

		[HttpGet]
	//	[OutputCache(Duration = 60, VaryByParam = "SearchText;SearchPosts;Page;")]
		public async Task<ActionResult> SearchResult(SearchForumModel model)
		{

			if (ModelState.IsValid)
			{
				using (var context = new ApplicationDbContext())
				{
					var totalCount = 0;
					var sanitizer = new HtmlSanitizer(allowedTags: new List<string> { "p" });
					var results = new List<SearchForumResultModel>();
					var pageNum = model.Page.HasValue ? model.Page.Value : 1;
					if (model.SearchPosts)
					{
						totalCount = context.ForumPosts.Count(x => !x.IsDeleted && x.Message.Contains(model.SearchText));
						var postResults = await context.ForumPosts.Where(x => !x.IsDeleted && x.Message.Contains(model.SearchText)).OrderByDescending(x => x.LastUpdate).Skip((pageNum - 1) * 25).Take(25).ToListAsync();
						foreach (var post in postResults)
						{
							results.Add(new SearchForumResultModel
							{
								PostId = post.Id,
								ThreadId = post.ThreadId,
								Timestamp = post.LastUpdate,
								ResultTitle = post.Thread.Title,
								ResultText = sanitizer.Sanitize(post.Message).Replace("<p>", "").Replace("</p>", "")
							});
						}
					}
					else
					{
						totalCount = context.ForumThreads.Count(x => !x.IsDeleted && x.Title.Contains(model.SearchText));
						var threadResults = await context.ForumThreads.Where(x => !x.IsDeleted && x.Title.Contains(model.SearchText)).OrderByDescending(x => x.LastUpdate).Skip((pageNum - 1) * 25).Take(25).ToListAsync();
						foreach (var thread in threadResults)
						{
							results.Add(new SearchForumResultModel
							{
								ThreadId = thread.Id,
								ResultText = thread.Title,
								Timestamp = thread.LastUpdate
							});
						}
					}
					model.SearchResult = new PagedList<SearchForumResultModel>(results, pageNum, 25);
				}
			}
			return View("SearchResult", model);
		}

		#endregion
	}
}