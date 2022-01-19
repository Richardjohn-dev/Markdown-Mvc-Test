﻿using MarkdownMvc.Models;
using Microsoft.AspNetCore.Hosting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using TheBlogProject.Models;
using Markdig;

namespace MarkdownMvc
{
    public class FileService : IFileService
    {
        private static string _folder;


        private readonly IWebHostEnvironment _env;

        public FileService(IWebHostEnvironment env)
        {
            _env = env;
            _folder = _env.WebRootPath;

            EnsureFolder("/blog-posts/");
            EnsureFolder("/blog-images/");
            EnsureFolderYear(DateTime.UtcNow.Year);
        }

        private static void EnsureFolderYear(int year)
        {
            if (!Directory.Exists($"{_folder}{year}/"))
            {
                Directory.CreateDirectory($"{_folder}{year}/");
            }
        }

        private static void EnsureFolder(string folder)
        {
            if (!Directory.Exists(Path.Combine(_folder, folder)))
            {
                Directory.CreateDirectory($"{_folder}{folder}/");
            }
        }

        public static bool SavePost(Post post)
        {
            //make sure we have it first
            EnsureFolderYear(post.DateCreated.Year);

            try
            {
                //clean up the categories
                //var sb = new StringBuilder();
                //if (!string.IsNullOrWhiteSpace(post.))
                //{
                //    var skipFirst = false;
                //    foreach (var category in post.Categories.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries))
                //    {
                //        sb.Append((skipFirst ? "," : "") + category.Trim());
                //        skipFirst = true;
                //    }
                //}

                var filePath = Path.Combine(_folder + post.DateCreated.Year + "/", post.Slug + ".webinfo");

                var savePost = new XDocument(
                    new XElement("post",
                        new XElement("title", post.Title),
                        new XElement("slug", post.Slug),
                        new XElement("published_date", post.DateCreated.ToString("yyyy-MM-dd HH:mm:ss")),
                        new XElement("excerpt", post.Abstract),
                        new XElement("read_time", post.TimeToRead),
                        new XElement("content", post.Content),
                        new XElement("markdown_content", post.Content)
                    ));

                //save to file
                savePost.Save(filePath);

                //add to cache (or update if exists)
                //MemoryCache.AddPost(post);

                return true;
            }
            catch
            {
                //dont care really, will go into below
            }

            return false;
        }

        public List<Post> ReadAllPosts()
        {
            List<Post> PostList = new();
            //{
            //    //new Post(){
            //    //    Title = "First post",
            //    //    Abstract = "Define the underlying",
            //    //    Content = "Define the underlying principles that drive decisions and strategy for your design language bleeding edge onward and upward"
            //    //    // The above text was generated by Office Ipsum http://officeipsum.com/index.php
            //    //},
            //    //new Post(){
            //    //    Title = "Second post",
            //    //    Abstract = "so what's our",
            //    //    Content = "so what's our go to market strategy?. Customer centric all hands on deck yet where the metal hits the meat define"
            //    //},
            //    //new Post(){
            //    //    Title = "Not visible",
            //    //    Abstract = "not important",
            //    //    Content = "not important, you should not see this post"
            //    //},
            //    //new Post(){
            //    //    Title = "Third post",
            //    //    Abstract = "the underlying",
            //    //    Content = "the underlying principles that drive decisions and strategy for your design language not the long pole",
            //    //},
            //    //new Post(){
            //    //    Title = "Fourth post",                   
            //    //    Abstract = "in the future",
            //    //    Content = "Post scheduling made super easy",
            //    //    DateCreated = DateTime.Now.AddDays(3) // Post scheduling made easy
            //    //},





            //    new Post()
            //    {
            //        Title = "Markdown Test",
            //        Abstract = "Let's see what markdown can do",
            //        Content = "# Hello world\n" +
            //                    "**Lorem** ipsum dolor sit" +
            //                    "amet, *consectetur* adipiscing elit. Sed" +
            //                    "eu est nec metus luctus tempus. Pellentesque" +
            //                    "at elementum sapien, ac faucibus sem" +
            //                    "![surprise](https://media.giphy.com/media/fdyZ3qI0GVZC0/giphy.gif)"
            //    }
            //};
            //__folder = _env.WebRootPath;

            foreach (var file in Directory.EnumerateFiles(_folder, "*.md", SearchOption.AllDirectories))
            {
                var md = File.ReadAllText(file);
                //var doc = XElement.Load(file);
                var post = new Post
                {
                    Title = "test",
                    Slug = "Slug",
                    DateCreated = DateTime.Now,
                    Abstract = "blar",
                    TimeToRead = 2,
                    Content = Markdown.ToHtml(md)
                    //we dont care for having this in the cache, as edit post reads from disk regardless
                    //BodyMarkdown = null
                };

                PostList.Add(post);
            }

            return PostList;
        }


        //public static class XmlHelper
        //{
        //    public static string ReadValue(XElement doc, XName name, string defaultValue = "")
        //    {
        //        if (doc.Element(name) != null)
        //        {
        //            return doc.Element(name).Value;
        //        }

        //        return defaultValue;
        //    }

        //    public static bool ReadBool(XElement doc, XName name)
        //    {
        //        if (doc.Element(name) != null)
        //        {
        //            return doc.Element(name).Value == "True";
        //        }

        //        return false;
        //    }
        //}

    }
}