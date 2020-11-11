﻿using System;
using NLog.Web;
using System.IO;
using System.Linq;

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
                Console.WriteLine("Enter your selection");
                Console.WriteLine("1) Display all blogs");
                Console.WriteLine("2) Add Blog");
                Console.WriteLine("3) Create Post");
                Console.WriteLine("4) Display Posts");
                Console.WriteLine("Enter q to quit");
                var choice = Console.ReadLine();
                var db = new BloggingContext();

                if (choice == "1")
                {
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
                    // Create and save a new Blog
                    Console.Write("Enter a name for a new Blog: ");
                    var name = Console.ReadLine();
                    if (name != "null")
                    {
                        var blog = new Blog { Name = name };
                        db.AddBlog(blog);
                        logger.Info("Blog added - {name}", name);
                    }
                    else 
                    {
                        logger.Info("Blog name cannot be null");
                    }

                }
                else if (choice == "3")
                {
                    Console.WriteLine(":");
                    var title = Console.ReadLine();
                    var postTitle = new Post { Title = title };
                    db.AddPost(postTitle);
                    logger.Info("Post added - {title}", title);


                }
                else if (choice == "4")
                {

                }

            }
            catch (Exception ex)
            {
                logger.Error(ex.Message);
            }

            logger.Info("Program ended");
        }
    }
}