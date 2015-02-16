using System;
using System.Collections.Generic;
using System.Linq;
using InterpriseSuiteEcommerceCommon;
using InterpriseSuiteEcommerceCommon.Domain.Infrastructure;
using InterpriseSuiteEcommerceCommon.DTO;
using InterpriseSuiteEcommerceCommon.Extensions;

public partial class admin_ItemImages : System.Web.UI.Page
{
    #region Variables
    IProductService _productService = null;
    #endregion

    #region Initialization

    protected override void OnInit(EventArgs e)
    {
        InitializeDomainServices();
        base.OnInit(e);
    }

    protected override void OnUnload(EventArgs e)
    {
        base.OnUnload(e);
    }

    protected void Page_Load(object sender, EventArgs e)
    {
    }

    #endregion

    #region Methods

    private void InitializeDomainServices()
    {
        _productService = ServiceFactory.GetInstance<IProductService>();
    }

    public string GetInventoryItemsJSON()
    {
        var items = _productService.GetInventoryItems();
        var categories = _productService.GetInventoryCategories();

        foreach (var item in items)
        {
            string seName = (item.ItemDescription.IsNullOrEmptyTrimmed()) ? item.ItemName : item.ItemDescription;
            string entityID = item.Counter.ToString();

            item.ItemURL = SE.MakeEntityLink(DomainConstants.EntityProduct, entityID, seName);
            item.Categories = categories.Where(c => c.ItemCode == item.ItemCode)
                                        .ToList();
        }
        return items.OrderBy(i => i.ItemName)
                    .ToList()
                    .ToJSON();
    }

    public string GetInventoryItemsWithNoImagesJSON()
    {
        return _productService.GetInventoryItemsWithNoImages()
                              .OrderBy(i => i.ItemName)
                              .ToList()
                              .ToJSON();
    }

    public string GetSystemCategoriesJSON()
    {
        return _productService.GetSystemCategories()
                              .OrderBy(c => c.Description)
                              .ToList()
                              .ToJSON();
    }

    public string GetImageConfigJSON()
    {
        var configs = new List<GlobalConfig>();

        string key = String.Empty;
        string value = String.Empty;

        key = "UseImageResize"; value = AppLogic.AppConfig(key);
        configs.Add(new GlobalConfig(key, value.ToLower()));

        key = "DefaultHeight_icon"; value = AppLogic.AppConfig(key);
        configs.Add(new GlobalConfig(key, value.ToLower()));
        
        key = "DefaultHeight_medium"; value = AppLogic.AppConfig(key);
        configs.Add(new GlobalConfig(key, value.ToLower()));

        key = "DefaultHeight_large"; value = AppLogic.AppConfig(key);
        configs.Add(new GlobalConfig(key, value.ToLower()));

        key = "DefaultWidth_icon"; value = AppLogic.AppConfig(key);
        configs.Add(new GlobalConfig(key, value.ToLower()));

        key = "DefaultWidth_medium"; value = AppLogic.AppConfig(key);
        configs.Add(new GlobalConfig(key, value.ToLower()));

        key = "DefaultWidth_large"; value = AppLogic.AppConfig(key);
        configs.Add(new GlobalConfig(key, value.ToLower()));

        return configs.ToJSON();
    }

    #endregion
}