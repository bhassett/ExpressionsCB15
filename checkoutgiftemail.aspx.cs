using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using InterpriseSuiteEcommerce;
using InterpriseSuiteEcommerceCommon.InterpriseIntegration;
using InterpriseSuiteEcommerceCommon;
using InterpriseSuiteEcommerceCommon.Extensions;
using InterpriseSuiteEcommerceCommon.Domain.Model;
using InterpriseSuiteEcommerceCommon.Domain.Infrastructure;
using System.Text;
using InterpriseSuiteEcommerceCommon.Domain.Model.CustomModel;

public partial class checkoutgiftemail : SkinBase
{
    #region Private Members

    InterpriseShoppingCart _cart = null;
    private IEnumerable<ShoppingCartGiftEmailCustomModel> _cartGiftItemEmails = null;

    #endregion

    #region DomainServices
    
    IShoppingCartService _shoppingCartService = null;
    INavigationService _navigationService = null;
    IAuthenticationService _authenticationService = null;
    IStringResourceService _stringResourceService = null;

    #endregion 

    #region Events

    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);

        InitializeDomainServices();

        SetCacheability();
        RequireSecurePage();
        RequireCustomerRecord();
        InitializeShoppingCart();
        PerformPageAccessLogic();

        InitializeGiftItems();
        LoadGiftItems();

        InitializeStringResources();

        this.btnCheckoutTop.Click += new EventHandler(btnCheckoutTop_Click);
        this.btnCheckoutBottom.Click += new EventHandler(btnCheckoutBottom_Click);
    }
    
    protected override void OnUnload(EventArgs e)
    {
        if (_cart != null) { _cart.Dispose(); }
        base.OnUnload(e);
    }

    protected void Page_Load(object sender, EventArgs e)
    {

    }

    protected void btnCheckoutBottom_Click(object sender, EventArgs e)
    {
        ProcessGiftEmail();
    }

    protected void btnCheckoutTop_Click(object sender, EventArgs e)
    {
        ProcessGiftEmail();
    }

    #endregion

    #region Private Methods

    private void SetCacheability()
    {
        Response.CacheControl = "private";
        Response.Expires = 0;
        Response.AddHeader("pragma", "no-cache");
    }

    private void InitializeDomainServices()
    {
        _navigationService = ServiceFactory.GetInstance<INavigationService>();
        _shoppingCartService = ServiceFactory.GetInstance<IShoppingCartService>();
        _authenticationService = ServiceFactory.GetInstance<IAuthenticationService>();
        _stringResourceService = ServiceFactory.GetInstance<IStringResourceService>();
    }

    private void InitializeStringResources()
    {
        this.lblHeader.Text = _stringResourceService.GetString("checkoutgiftemail.aspx.1");
        this.SectionTitle = _stringResourceService.GetString("checkoutgiftemail.aspx.1");
        this.btnCheckoutTop.Text = _stringResourceService.GetString("checkoutgiftemail.aspx.3");
        this.btnCheckoutBottom.Text = _stringResourceService.GetString("checkoutgiftemail.aspx.3");
    }

    private void InitializeShoppingCart()
    {
        _cart = new InterpriseShoppingCart(base.EntityHelpers, ThisCustomer.SkinID, ThisCustomer, CartTypeEnum.ShoppingCart, string.Empty, false, true);
        if (!_cart.IsEmpty()) { _cart.BuildSalesOrderDetails(true); }
    }

    private void PerformPageAccessLogic()
    {
        if (!_cart.HasGiftItems()) 
        {
            _navigationService.NavigateToShoppingCart();
        }
    }

    private IEnumerable<CartItem> GetCartGiftItems()
    {
        //get gift certificate and gift card items only
        return _cart.CartItems.Where(item => item.ItemType == Interprise.Framework.Base.Shared.Const.ITEM_TYPE_GIFT_CARD ||
                                            item.ItemType == Interprise.Framework.Base.Shared.Const.ITEM_TYPE_GIFT_CERTIFICATE);
    }

    private IEnumerable<ShoppingCartGiftEmailCustomModel> GetStoredGiftItemEmails()
    {
        if (_cartGiftItemEmails == null) { _cartGiftItemEmails = _shoppingCartService.GetShoppingCartGiftEmails(); }
        return _cartGiftItemEmails;
    }

    private void LoadGiftItems()
    {
        rptGiftItemsEmail.DataSource = GetStoredGiftItemEmails().OrderBy(i => i.Counter);
        rptGiftItemsEmail.DataBind();
    }

    private void InitializeGiftItems()
    {
        var storedGiftItemEmails = GetStoredGiftItemEmails();
        var cartGiftItems = GetCartGiftItems();
        var customer = _authenticationService.GetCurrentLoggedInCustomer();
        bool hasChanges = false;

        foreach (var cartItem in cartGiftItems)
        {
            decimal giftCeiling = cartItem.m_Quantity * cartItem.UnitMeasureQty;

            //get current giftitem email
            int giftCount = storedGiftItemEmails.Where(i => i.ShoppingCartRecID == cartItem.m_ShoppingCartRecordID).Count();

            //check if cart line item ordered quantity increased
            if (giftCount < giftCeiling)
            {
                var giftItemEmailsToBeAdded = new List<ShoppingCartGiftEmailCustomModel>();
                for (int i = 0; i < giftCeiling - giftCount; i++)
                {
                    giftItemEmailsToBeAdded.Add(new ShoppingCartGiftEmailCustomModel()
                    {
                        ShoppingCartRecID = cartItem.m_ShoppingCartRecordID,
                        ItemCode = cartItem.ItemCode,
                        LineNum = GetCartItemLineNum(cartItem.m_ShoppingCartRecordID),
                        EmailRecipient = customer.EMail
                    });
                }

                //bulk insert new gift emails
                _shoppingCartService.CreateShoppingCartGiftEmail(giftItemEmailsToBeAdded);
                hasChanges = true;
            }

            //check if cart line item ordered quantity decreased
            if (giftCount > giftCeiling)
            {
                int deleteCount = Convert.ToInt32(giftCount - giftCeiling);
                _shoppingCartService.DeleteShoppingCartGiftEmailTopRecords(cartItem.m_ShoppingCartRecordID, deleteCount);
                hasChanges = true;
            }
        }

        if (hasChanges) { _cartGiftItemEmails = _shoppingCartService.GetShoppingCartGiftEmails(); }
    }

    private void ProcessGiftEmail()
    {
        var storedGiftItemEmails = GetStoredGiftItemEmails();
        var giftItemEmailsToBeUpdated = new List<ShoppingCartGiftEmailCustomModel>();
        foreach (RepeaterItem item in rptGiftItemsEmail.Items)
        {
            int giftID = Convert.ToInt32(((HiddenField)item.FindControl("txtGiftID")).Value);
            string email = ((TextBox)item.FindControl("txtEmail")).Text.Trim();

            var giftItemRow = storedGiftItemEmails.FirstOrDefault(i => i.Counter == giftID);
            int cartRecID = (giftItemRow != null) ? giftItemRow.ShoppingCartRecID : 0;
            int lineNum = GetCartItemLineNum(cartRecID);

            bool isEmailUpdated = email != giftItemRow.EmailRecipient;
            bool isLineNumUpdated = lineNum != giftItemRow.LineNum;
            if (isEmailUpdated || isLineNumUpdated) { giftItemEmailsToBeUpdated.Add(new ShoppingCartGiftEmailCustomModel() { Counter = giftID, EmailRecipient = email, LineNum = lineNum }); }
        }

        //bulk update modified gift emails
        _shoppingCartService.UpdateShoppingCartGiftEmail(giftItemEmailsToBeUpdated);

        if (AppLogic.AppConfigBool("SkipShippingOnCheckout") || !_cart.HasShippableComponents())
        {
            _cart.MakeShippingNotRequired();
            _navigationService.NavigateToCheckOutPayment();
        }

        if ((_cart.HasMultipleShippingAddresses() && _cart.NumItems() <= AppLogic.MultiShipMaxNumItemsAllowed() && _cart.CartAllowsShippingMethodSelection) || _cart.HasRegistryItems())
        {
            _navigationService.NavigateToCheckoutMult();
        }
        else
        {
            _navigationService.NavigateToCheckoutShipping();
        }
    }

    private int GetCartItemLineNum(int cartRecID)
    {
        return _cart.CartItems.FindIndex(x => x.m_ShoppingCartRecordID == cartRecID) + 1;
    }

    #endregion
}