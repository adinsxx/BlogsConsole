using System;
using NLog.Web;
using System.IO;
using System.Linq;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace BlogsConsole
{
    class Program
    {
        // create static instance of Logger
        private static NLog.Logger logger = NLogBuilder.ConfigureNLog(Directory.GetCurrentDirectory() + "\\nlog.config").GetCurrentClassLogger();
        static void Main(string[] args)
        {
            logger.Info("Program started");
            try
            {
                string choice;
                do
                {

                    Console.WriteLine("Enter your selection:");
                    Console.WriteLine("1) Display all blogs");
                    Console.WriteLine("2) Add Blog");
                    Console.WriteLine("3) Create Post");
                    Console.WriteLine("4) Display Posts");
                    Console.WriteLine("5) Delete Blog");
                    Console.WriteLine("6) Edit Blog");
                    Console.WriteLine("Enter q to quit");
                    choice = Console.ReadLine();


                    if (choice == "1")
                    {
                        var db = new BloggingContext();
                        logger.Info("Option 1 selected");
                        // Display all Blogs from the database
                        var bId = db.Blogs.OrderBy(b => b.BlogId);

                        if (bId != null)
                        {
                            Console.WriteLine("All blogs in the database:");
                            foreach (var blogName in bId)
                            {
                                Console.Write(blogName.BlogId + " ");
                                Console.WriteLine(blogName.Name);
                            }

                        }
                        else
                        {
                            {
                                logger.Info("No blogs available");
                            }

                        }
                    }
                    else if (choice == "2")
                    {
                        var db = new BloggingContext();
                        Blog blog = InputBlog(db);
                        if (blog != null){
                            //blog.BlogId = BlogId;
                            db.AddBlog(blog);
                            logger.Info("Blog added - {name}", blog.Name);
                        }

                //                             // Create and save a new Blog
                //     Console.Write("Enter a name for a new Blog: ");
                //     var name = Console.ReadLine();
                //     if (name != "null")
                //     {
                //         var blog = new Blog { Name = name };
                //         db.AddBlog(blog);
                //         logger.Info("Blog added - {name}", name);
                //     }
                //     else 
                //     {
                //         logger.Info("Blog name cannot be null");
                //     }

                // }
                    }

                    else if (choice == "3")
                    {
                        //Add title
                        var db = new BloggingContext();
                        Console.WriteLine("Enter the post title:");
                        var title = Console.ReadLine();
                        if (title != "")
                        {
                            var postTitle = new Post { Title = title };

                            logger.Info("Post added - {title}", title);
                            //Add post content
                            Console.WriteLine("Enter post content:");
                            var content = Console.ReadLine();
                            var postContent = new Post { Content = content };
                            db.AddPost(postTitle);
                            db.AddPost(postContent);
                            logger.Info("Post added - {content}", content);
                        }
                        else
                        {
                            logger.Info("Post title cannot be null");
                        }



                    }
                    else if (choice == "4")
                    {
                        var db = new BloggingContext();
                        logger.Info("Option 4 selected");
                        // Display all posts from the database
                        var pID = db.Posts.OrderBy(b => b.PostId);

                        if (pID != null)
                        {
                            Console.WriteLine("All {posts} in the database:");
                            foreach (var postContent in pID)
                            {
                                Console.Write(postContent.PostId + " ");
                                Console.WriteLine(postContent.Title);
                                Console.WriteLine(postContent.Content);
                            }

                        }
                        else
                        {
                            {
                                logger.Info("No posts available");
                            }

                        }

                    }

                    else if (choice == "5")
                    {
                        // delete blog
                        Console.WriteLine("Choose the blog to delete:");
                            var db = new BloggingContext();
                            var blog = GetBlog(db);
                            if (blog != null)
                            {
                                // TODO: delete blog
                                db.DeleteBlog(blog);
                                logger.Info($"Blog (id: {blog.BlogId}) deleted");
                            }
                    }
                    else if (choice == "6")
                    {
                        // edit blog
                        Console.WriteLine("Choose the blog to edit:");
                        var db = new BloggingContext();
                        var blog = GetBlog(db);
                        if (blog != null)
                        {
                            // input blog
                            Blog UpdatedBlog = InputBlog(db);
                            if (UpdatedBlog != null)
                            {
                                UpdatedBlog.BlogId = blog.BlogId;
                                db.EditBlog(UpdatedBlog);
                                logger.Info($"Blog (id: {blog.BlogId}) updated");
                            }
                        }
                    }
                } while (choice.ToLower() != "q") ;
                
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message);
            }

            logger.Info("Program ended");
        }

        public static Blog GetBlog(BloggingContext db)
        {
            // display all blogs
            var blogs = db.Blogs.OrderBy(b => b.BlogId);
            foreach (Blog b in blogs)
            {
                Console.WriteLine($"{b.BlogId}: {b.Name}");
            }
            if (int.TryParse(Console.ReadLine(), out int BlogId))
            {
                Blog blog = db.Blogs.FirstOrDefault(b => b.BlogId == BlogId);
                if (blog != null)
                {
                    return blog;
                }
            }
            logger.Error("Invalid Blog Id");
            return null;
        }
        
        public static Blog InputBlog(BloggingContext db)
        {
            Blog blog = new Blog();
            Console.WriteLine("Enter the Blog name");
            blog.Name = Console.ReadLine();

            ValidationContext context = new ValidationContext(blog, null, null);
            List<ValidationResult> results = new List<ValidationResult>();

            var isValid = Validator.TryValidateObject(blog, context, results, true);
            if (isValid)
            {
                // check for unique name
                if (db.Blogs.Any(b => b.Name == blog.Name))
                {
                    // generate validation error
                    isValid = false;
                    results.Add(new ValidationResult("Blog name exists", new string[] { "Name" }));
                }
                else
                {
                    logger.Info("Validation passed");
                }
            }
            if (!isValid)
            {
                foreach (var result in results)
                {
                    logger.Error($"{result.MemberNames.First()} : {result.ErrorMessage}");
                }
                return null;
            }
            return null;
        }
    }
}