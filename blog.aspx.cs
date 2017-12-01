using InterpriseSuiteEcommerceCommon;
using InterpriseSuiteEcommerceCommon.Domain.Infrastructure;
using InterpriseSuiteEcommerceCommon.Extensions;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml.Linq;

public partial class blog : InterpriseSuiteEcommerce.SkinBase
{
    IBlogService _blogService = null;
    IAppConfigService _appConfigService = null;
    INavigationService _navigationService = null;
    private const string XmlHelper = "helper.blog.xml.config";

    public int PostPerPage { get; private set; }
    public bool ShowDescriptionInPostList { get; private set; }
    public int DescriptionCharacters { get; private set; }

    protected override void OnInit(EventArgs e)
    {
        SectionTitle = "Blogs";
        _blogService = ServiceFactory.GetInstance<IBlogService>();
        _appConfigService = ServiceFactory.GetInstance<IAppConfigService>();
        _navigationService = ServiceFactory.GetInstance<INavigationService>();

        InitializeContent();
        base.OnInit(e);
    }

    protected void Page_Load(object sender, EventArgs e)
    {
    }

    private void InitializeContent()
    {
        try
        {
            if (_appConfigService.BlogEngineApiUrl.IsNullOrEmptyTrimmed()) return; // configure appsetting BlogEngineApiUrl in web.config

            LoadSettings();
            LoadBlogCategories();

            var postid = HttpContext.Current.Request.QueryString["postid"];
            if (postid != null)
            {
                LoadBlogPost(postid);
                return;
            }

            var categoryid = HttpContext.Current.Request.QueryString["categoryid"];
            if (categoryid != null)
            {
                LoadBlogPosts(categoryid);
                return;
            }

            // load all published blog posts...
            LoadBlogPosts();
        }
        catch (Exception)
        {
        }
    }

    private void LoadSettings()
    {
        var settings = _blogService.GetSettings().ToList();
        if (settings == null) return;

        var setting = settings.Find(x => x.Name.EqualsIgnoreCase("PostsPerPage"));
        if (setting != null) PostPerPage = setting.Value.TryParseInt().Value;

        setting = settings.Find(x => x.Name.EqualsIgnoreCase("ShowDescriptionInPostList"));
        if (setting != null) ShowDescriptionInPostList = setting.Value.TryParseBool().Value;

        setting = settings.Find(x => x.Name.EqualsIgnoreCase("DescriptionCharacters"));
        if (setting != null) DescriptionCharacters = setting.Value.TryParseInt().Value;

    }

    private void LoadBlogPost(string blogid)
    {
        if (blogid.IsNullOrEmptyTrimmed()) return;

        var blog = _blogService.GetPost(blogid);
        if (blogid == null) _navigationService.NavigateToPageError("Blog Not Found");

        var xml = new XElement(DomainConstants.XML_ROOT_NAME);
        xml.Add(new XElement(DomainConstants.XML_SECTION_TYPE, "BLOGPOSTDETAIL"));
        xml.Add(new XElement("ID", blog.ID));
        xml.Add(new XElement("Title", blog.Title));
        xml.Add(new XElement("RelativeLink", GetBlogPostLink(blog.Slug)));
        xml.Add(new XElement("Content", blog.Content));
        xml.Add(new XElement("Author", blog.Author));
        xml.Add(new XElement("PostDate", blog.DateCreated.TryParseDateTime().Value.ToString("dd MMMM yyyy")));

        var xmlCategories = new XElement("BLOGPOST_CATEGORIES");
        blog.Categories.ForEach(x => {
            var xmlCategory = new XElement("BLOGPOST_CATEGORY");
            xmlCategory.Add(new XElement("Title", x.Title));
            xmlCategory.Add(new XElement("RelativeLink", GetBlogCategoryLink(x.Title)));
            xmlCategories.Add(xmlCategory);
        });

        xml.Add(xmlCategories);


        var xmlpackage = new XmlPackage2(XmlHelper, xml);
        BlogPostDetail.Text = xmlpackage.TransformString();

        SectionTitle = "<a href='blog.aspx'>Blogs</a> / " + blog.Title;
    }

    private void LoadBlogPosts(string categoryid = null)
    {
        var blogs = _blogService.GetPosts()
                            .Where(x => x.IsPublished)
                            .ToList();

        if(!categoryid.IsNullOrEmptyTrimmed())
        {
            SectionTitle = "<a href='blog.aspx'>Blogs</a>";
            var category = _blogService.GetCategory(categoryid);
            if (category != null) SectionTitle += " / " + category.Title;

            blogs = blogs.Where(x => x.Categories.Any(y => categoryid.EqualsIgnoreCase(CommonLogic.RemoveIllegalCharacters(y.Title))))
                         .ToList();
        }


        var xml = new XElement(DomainConstants.XML_ROOT_NAME);
        xml.Add(new XElement(DomainConstants.XML_SECTION_TYPE, "BLOGPOSTS"));
        xml.Add(new XElement("ShowDescriptionOnly", ShowDescriptionInPostList));
        var xmlBlogs = new XElement("BLOGPOSTS");
        foreach (var blog in blogs)
        {
            string relativelink = GetBlogPostLink(blog.Slug);

            var xmlBlog = new XElement("BLOGPOST");
            xmlBlog.Add(new XElement("ID", blog.ID));
            xmlBlog.Add(new XElement("Title", blog.Title));
            xmlBlog.Add(new XElement("RelativeLink", relativelink));
            xmlBlog.Add(new XElement("Author", blog.Author));
            xmlBlog.Add(new XElement("PostDate", blog.DateCreated.TryParseDateTime().Value.ToString("dd MMMM yyyy")));
            xmlBlog.Add(new XElement("Content", blog.Content));
            xmlBlog.Add(new XElement("Description", blog.Content.Substring(0, DescriptionCharacters)));

            var xmlBlogCategories = new XElement("BLOGPOST_CATEGORIES");
            blog.Categories.ForEach(x => {
                var xmlBlogCategory = new XElement("BLOGPOST_CATEGORY");
                xmlBlogCategory.Add(new XElement("Title", x.Title));
                xmlBlogCategory.Add(new XElement("RelativeLink", GetBlogCategoryLink(x.Title)));
                xmlBlogCategories.Add(xmlBlogCategory);
            });

            xmlBlog.Add(xmlBlogCategories);
            xmlBlogs.Add(xmlBlog);
        }
        xml.Add(xmlBlogs);

        var xmlpackage = new XmlPackage2(XmlHelper, xml);
        BlogPosts.Text = xmlpackage.TransformString();
    }

    private void LoadBlogCategories()
    {
        var categories = _blogService.GetCategories()
                                     .ToList()
                                     .Where(x => x.Count > 0);

        var xml = new XElement(DomainConstants.XML_ROOT_NAME);
        xml.Add(new XElement(DomainConstants.XML_SECTION_TYPE, "BLOGCATEGORIES"));
        var xmlCategories = new XElement("BLOGCATEGORIES");
        foreach (var category in categories)
        {
            var xmlCategory = new XElement("BLOGCATEGORY");
            xmlCategory.Add(new XElement("ID", category.ID));
            xmlCategory.Add(new XElement("Title", category.Title));
            xmlCategory.Add(new XElement("Count", category.Count));
            xmlCategory.Add(new XElement("RelativeLink", GetBlogCategoryLink(category.Title)));
            xmlCategories.Add(xmlCategory);
        }
        xml.Add(xmlCategories);

        var xmlpackage = new XmlPackage2(XmlHelper, xml);
        BlogCategories.Text = xmlpackage.TransformString();
    }

    private string GetBlogPostLink(string slug)
    {
        if (slug.IsNullOrEmptyTrimmed()) return String.Empty;
        return "b-{0}.aspx".FormatWith(slug);
    }

    private string GetBlogCategoryLink(string title)
    {
        if (title.IsNullOrEmptyTrimmed()) return String.Empty;
        return "bc-{0}.aspx".FormatWith(CommonLogic.RemoveIllegalCharacters(title).ToLower());
    }
}