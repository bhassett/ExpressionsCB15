<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:ise="urn:ise" exclude-result-prefixes="ise">
  <xsl:output method="html" encoding="utf-8" indent="yes" />
  <xsl:template match="/">
    <xsl:variable name="RenderType" select="FIELD/RENDERTYPE" />
    <xsl:variable name="VatEnabled" select="FIELD/VATENABLED" />
    <xsl:variable name="VatInclusive" select="FIELD/VATINCLUSIVE" />
    <xsl:variable name="HasCoupon" select="FIELD/HASCOUPON" />
    <xsl:variable name="IsCouponTypeOrders" select="FIELD/IS_COUPON_TYPE_ORDERS" />
    <xsl:variable name="IsFreeShipping" select="FIELD/ISFREESHIPPING" />
    <xsl:variable name="ShowStockHints" select="FIELD/SHOWSTOCKHINTS" />
    <xsl:variable name="ShowShipDateInCart" select="FIELD/SHOWSHIPDATEINCART" />
    <xsl:variable name="ShowPicInCart" select="ise:AppConfigBool('ShowPicsInCart')" />
    <xsl:variable name="IsCustomerRegistered" select="FIELD/ISREGISTERED" />
    <xsl:variable name="DecimalPlaces" select="ise:GetInventoryPreferencePlaces()" />
    <xsl:variable name="HideOutOfStockProducts" select="ise:AppConfigBool('HideOutOfStockProducts')" />
    <xsl:variable name="ShowTaxBreakDown" select="FIELD/SHOW_TAX_BREAK_DOWN" />
    <xsl:variable name="HideUnitMeasure" select="ise:AppConfigBool('HideUnitMeasure')" />
    <xsl:choose>
      <xsl:when test="EMPTY_CART_TEXT">
        <div class="tableHeaderArea">
          <xslvalue-of select="FIELD/EMPTY_CART_TEXT" disable-output-escaping="yes" />
        </div>
      </xsl:when>
      <xsl:otherwise>
        <xsl:if test="$RenderType != 'REVIEW'">
          <xsl:call-template name="HeaderTemplate">
            <xsl:with-param name="RenderType" select="$RenderType" />
          </xsl:call-template>
        </xsl:if>
        <div>
          <xsl:choose>
            <xsl:when test="$RenderType = 'REVIEW'">
              <xsl:attribute name="class">
                <xsl:text>review-multiship-cartitem-expander cart-items-wrapper</xsl:text>
              </xsl:attribute>
            </xsl:when>
            <xsl:otherwise>
              <xsl:attribute name="class">
                <xsl:text>cart-items-wrapper </xsl:text>
              </xsl:attribute>
            </xsl:otherwise>
          </xsl:choose>
          <table class="cart-item">
            <tr>
              <td class="cart-col-header content" data-contentKey="shoppingcart.cs.1" data-contentValue="{ise:StringResourceTextOnly('shoppingcart.cs.1')}" data-contentType="string resource">
                <xsl:if test="$ShowPicInCart = 'false'">
                  <xsl:attribute name="colspan">2</xsl:attribute>
                </xsl:if>
                <xsl:value-of select="ise:StringResourceTextOnly('shoppingcart.cs.1')">
                </xsl:value-of>
              </td>
              <xsl:if test="$ShowPicInCart = 'true'">
                <td class="cart-col-header">
                </td>
              </xsl:if>
              <td class="cart-col-header">
                <xsl:value-of select="ise:StringResourceTextOnly('common.cs.25')">
                </xsl:value-of>
              </td>
              <xsl:if test="$ShowStockHints = 'true' and $ShowShipDateInCart = 'true'">
                <td class="cart-col-header content" data-contentKey="shoppingcart.aspx.17" data-contentValue="{ise:StringResourceTextOnly('shoppingcart.aspx.17')}" data-contentType="string resource">
                  <xsl:value-of select="ise:StringResourceTextOnly('shoppingcart.aspx.17')">
                  </xsl:value-of>
                </td>
              </xsl:if>
              <xsl:if test="$HideUnitMeasure = 'false'">
                <td class="cart-col-header content" data-contentKey="shoppingcart.cs.37" data-contentValue="{ise:StringResourceTextOnly('shoppingcart.cs.37')}" data-contentType="string resource">
                  <xsl:value-of select="ise:StringResourceTextOnly('shoppingcart.cs.37')">
                  </xsl:value-of>
                </td>
              </xsl:if>
              <td class="cart-col-header cart-quantity-expander content" data-contentKey="shoppingcart.cs.2" data-contentValue="{ise:StringResourceTextOnly('shoppingcart.cs.2')}" data-contentType="string resource">
                <xsl:value-of select="ise:StringResourceTextOnly('shoppingcart.cs.2')">
                </xsl:value-of>
              </td>
              <xsl:if test="$HasCoupon = 'true' and $IsCouponTypeOrders = 'false'">
                <td class="cart-col-header cart-quantity-expander content" data-contentKey="shoppingcart.cs.42" data-contentValue="{ise:StringResourceTextOnly('shoppingcart.cs.42')}" data-contentType="string resource">
                  <xsl:value-of select="ise:StringResourceTextOnly('shoppingcart.cs.42')">
                  </xsl:value-of>
                </td>
              </xsl:if>
              <td class="cart-col-header cart-subtotal-expander gotextright-basic content" data-contentKey="shoppingcart.cs.70" data-contentValue="{ise:StringResourceTextOnly('shoppingcart.cs.70')}" data-contentType="string resource">
                <xsl:value-of select="ise:StringResourceTextOnly('shoppingcart.cs.70')">
                </xsl:value-of>
              </td>
              <td class="cart-col-header">
              </td>
            </tr>
            <tr>
              <td>
                <xsl:choose>
                  <xsl:when test="$HasCoupon = 'true' and $IsCouponTypeOrders = 'false'">
                    <xsl:attribute name="colspan">8</xsl:attribute>
                  </xsl:when>
                  <xsl:otherwise>
                    <xsl:attribute name="colspan">7</xsl:attribute>
                  </xsl:otherwise>
                </xsl:choose>
                <hr class="cart-item-divider">
                </hr>
              </td>
            </tr>
            <!--start-->
            <xsl:for-each select="FIELD/CART_ITEMS">
              <xsl:choose>
                <xsl:when test="ITEM/BUNDLECODE = ''">
                  <xsl:for-each select="ITEM">
                    <xsl:variable name="ItemType" select="current()/ITEMTYPE" />
                    <xsl:variable name="ItemCode" select="current()/ITEMCODE" />
                    <xsl:variable name="IsOutOfStock" select="current()/ISOUTOFSTOCK" />
                    <xsl:variable name="CartStatus" select="current()/CARTSTATUS" />
                    <xsl:variable name="POStatus" select="current()/POSTATUS" />
                    <xsl:variable name="AllocQty" select="current()/ALLOCQTY" />
                    <xsl:variable name="ReserveCol" select="current()/RESERVECOL" />
                    <xsl:variable name="IsCBNItem" select="current()/ISCBNITEM" />
                    <xsl:variable name="IsDropShip" select="current()/ISDROPSHIP" />
                    <xsl:variable name="ItemNotes" select="current()/PRODUCTNOTES" />
                    <tr>
                      <!--<td>
                            <xsl:value-of select="BUNDLECODE" disable-output-escaping="yes" />
                          </td>-->
                      <td class="cart_picture_layout">
                        <xsl:if test="SHOWPICSINCART = 'true'">
                          <xsl:choose>
                            <xsl:when test="LINKBACK = 'true'">
                              <a href="{PRODUCTLINKHREF}">
                                <img class="mobileimagesize" title="{PRODUCTIMAGETITLE}" alt="{PRODUCTIMAGEALT}" src="{PRODUCTIMAGEPATH}" />
                              </a>
                            </xsl:when>
                            <xsl:otherwise>
                              <img title="{PRODUCTIMAGETITLE}" alt="{PRODUCTIMAGEALT}" src="{PRODUCTIMAGEPATH}" />
                            </xsl:otherwise>
                          </xsl:choose>
                        </xsl:if>
                      </td>
                      <td class="cart-col kit_container">
                        <xsl:choose>
                          <xsl:when test="LINKBACK = 'true'">
                            <xsl:choose>
                              <xsl:when test="ISCHECKOUTOPTION = 'false'">
                                <a href="{PRODUCTLINKHREF}">
                                  <b>
                                    <xsl:value-of select="PRODUCTLINKNAME" disable-output-escaping="yes" />
                                  </b>
                                </a>
                              </xsl:when>
                              <xsl:otherwise>
                                <span class="product_description">
                                  <xsl:value-of select="PRODUCTLINKNAME" disable-output-escaping="yes" />
                                </span>
                              </xsl:otherwise>
                            </xsl:choose>
                          </xsl:when>
                          <xsl:otherwise>
                            <span class="product_description">
                              <xsl:value-of select="PRODUCTLINKNAME" disable-output-escaping="yes" />
                            </span>
                          </xsl:otherwise>
                        </xsl:choose>
                        <xsl:if test="KIT_ITEMS and ($RenderType = 'SHOPPINGCART' or $RenderType = 'SHIPPING' or $RenderType = 'REVIEW')">
                          <br />
                          <xsl:if test="LINKBACK = 'true' and ISREGISTRYITEM != 'true'">
                            <a href="{KIT_ITEMS/KIT_EDIT_HREF}">
                              <img align="absmiddle" border="0" alt="{ise:StringResourceTextOnly('mobile.shoppingcart.cs.4')}" src="{concat('skins/Skin_', ../../SKINID,'/images/edit.gif')}" />
                            </a>
                          </xsl:if>
                          <ul>
                            <xsl:for-each select="KIT_ITEMS/KITITEM">
                              <li>
                                    - <xsl:value-of select="current()" disable-output-escaping="yes" /></li>
                            </xsl:for-each>
                          </ul>
                        </xsl:if>
                        <xsl:if test="string-length($ItemNotes) &gt; 0">
                          <br />
                          <br />
                          <span class="small">
                            <xsl:value-of select="concat(ise:StringResourceTextOnly('shoppingcart.cs.23'),' ', $ItemNotes)" disable-output-escaping="yes" />
                          </span>
                        </xsl:if>
                        <xsl:if test="ISREGISTRYITEM = 'true'">
                          <br />
                          <br />
                          <span class="registry-notification">
                            <xsl:value-of select="ise:StringResource('giftregistry.aspx.10')" />
                          </span>
                          <br />
                          <span class="registry-notification">
                            <xsl:value-of select="ise:StringResource('giftregistry.aspx.12')" />:
                                <xsl:value-of select="REGISTRYITEMQUANTITY" /></span>
                          <xsl:if test="REGISTRYITEMQUANTITY = 0">
                            <br />
                            <br />
                            <span class="errorLg">
                              <xsl:value-of select="ise:StringResource('editgiftregistry.error.16')" />
                            </span>
                            <br />
                          </xsl:if>
                          <xsl:if test="ISREGISTRYITEMHASCONFLICT = 'true'">
                            <br />
                            <br />
                            <span class="errorLg">
                              <xsl:value-of select="ise:StringResource('editgiftregistry.error.17')" />
                            </span>
                            <br />
                          </xsl:if>
                        </xsl:if>
                        <xsl:if test="HAS_MULTIPLE_ADDRESSES and HAS_MULTIPLE_ADDRESSES = 'true' and ITEMISDOWNLOAD and ITEMISDOWNLOAD = 'false'">
                          <br />
                          <div class="shippinaddress_content">
                            <b>
                              <xsl:value-of select="ise:StringResource('mobile.shoppingcart.cs.24')" />
                            </b> : <xsl:value-of select="SHIP_ITEM_TO_VALUE" disable-output-escaping="yes" /><br /><br /><xsl:value-of select="SHIP_ITEM_DETAIL" disable-output-escaping="yes" /><br /><br /><b><xsl:value-of select="ise:StringResource('order.cs.23')" disable-output-escaping="yes" /></b><br /><xsl:value-of select="SHIPING_METHOD_VALUE" disable-output-escaping="yes" /></div>
                        </xsl:if>
                        <xsl:if test="($RenderType = 'PAYMENT' or $RenderType = 'REVIEW' or $RenderType = 'SHOPPINGCART')">
                          <xsl:if test="IS_STOREPICKUP = 'true'">
                            <br />
                            <br />
                            <div class="shippinaddress_content">
                              <span>
                                <b>
                                  <xsl:value-of select="ise:StringResourceTextOnly('checkoutshipping.aspx.11')" disable-output-escaping="yes" />:
                                    </b>
                                <xsl:value-of select="ITEM_SHIPPINGMETHOD" disable-output-escaping="yes" />
                              </span>
                              <br />
                              <br />
                              <span>
                                <b>
                                  <xsl:value-of select="ise:StringResourceTextOnly('checkoutstore.aspx.23')" disable-output-escaping="yes" />:
                                    </b>
                              </span>
                              <br />
                              <xsl:value-of select="WAREHOUSE_NAME" disable-output-escaping="yes" />
                              <br />
                              <xsl:value-of select="WAREHOUSE_ADDRESS" disable-output-escaping="yes" />
                            </div>
                          </xsl:if>
                        </xsl:if>
                        <xsl:if test="$IsOutOfStock = 'true' and $CartStatus = 'P'">
                          <br />
                          <br />
                          <img title="{PRODUCTIMAGETITLE}" src="{concat('skins/Skin_',current()/parent::node()/SKINID,'/images/outofstock.gif')}" />
                        </xsl:if>
                        <xsl:if test="$HideOutOfStockProducts = 'true'">
                          <xsl:if test="$CartStatus = 'A' and $IsOutOfStock = 'true' and ($POStatus != 'Open' or $POStatus = 'Partial')">
                            <br />
                            <br />
                            <img title="{PRODUCTIMAGETITLE}" src="{concat('skins/Skin_',current()/parent::node()/SKINID,'/images/outofstock.gif')}" />
                          </xsl:if>
                        </xsl:if>
                      </td>
                      <td class="cart-col">
                        <xsl:value-of select="SALES_PRICE" disable-output-escaping="yes" />
                      </td>
                      <xsl:if test="$ShowStockHints = 'true' and $ShowShipDateInCart = 'true'">
                        <xsl:choose>
                          <xsl:when test="$ItemType = 'Non-Stock' or $ItemType = 'Service' or $ItemType = 'Electronic Download' or $ItemType = 'Gift Card' or $ItemType = 'Gift Certificate'">
                            <td class="cart-col">
                            </td>
                          </xsl:when>
                          <xsl:when test="$IsOutOfStock = 'true' and $CartStatus = 'P'">
                            <td class="cart-col">
                            </td>
                          </xsl:when>
                          <!--has allocation and has or no reservation-->
                          <xsl:when test="$AllocQty &gt; 0 and $ReserveCol &gt;= 0">
                            <td class="cart-col">
                              <div class="cart_shipping">
                                <span class="addSpace">
                                  <!--<xsl:value-of select="$AllocQty" disable-output-escaping="yes" />-->
                                  <xsl:value-of select="ise:FormatDecimal($AllocQty,$DecimalPlaces)" disable-output-escaping="yes" />
                                </span>
                                <span>
                                  <xsl:value-of select="ise:StringResource('shoppingcart.cs.61')" disable-output-escaping="yes" />
                                </span>
                              </div>
                              <!--has reservation-->
                              <xsl:if test="$ReserveCol &gt; 0">
                                <br />
                                <xsl:for-each select="RESERVATIONITEM">
                                  <xsl:call-template name="RESERVATIONITEM">
                                    <xsl:with-param name="ItemCode" select="$ItemCode" />
                                  </xsl:call-template>
                                </xsl:for-each>
                              </xsl:if>
                              <xsl:if test="NOTAVAILABLEQTYWITHRESERVATION &gt; 0">
                                <br />
                                <span class="addSpace">
                                  <xsl:value-of select="NOTAVAILABLEQTYWITHRESERVATION" disable-output-escaping="yes" />
                                </span>
                                <span>
                                  <xsl:value-of select="ise:StringResource('shoppingcart.cs.47')" disable-output-escaping="yes" />
                                </span>
                              </xsl:if>
                            </td>
                          </xsl:when>
                          <!--possible values of shipping date column for CBN items-->
                          <xsl:when test="$IsCBNItem ='true' and $IsDropShip ='true'">
                            <td class="cart-col">
                              <xsl:if test="AVAILABLEQTY &gt; 0">
                                <span class="addSpace">
                                  <xsl:value-of select="AVAILABLEQTY" disable-output-escaping="yes" />
                                </span>
                                <span>
                                  <xsl:value-of select="ise:StringResource('shoppingcart.cs.61')" disable-output-escaping="yes" />
                                </span>
                                <br />
                              </xsl:if>
                              <xsl:if test="NOTAVAILABLEQTY &gt; 0">
                                <span class="addSpace">
                                  <xsl:value-of select="NOTAVAILABLEQTY" disable-output-escaping="yes" />
                                </span>
                                <span>
                                  <xsl:value-of select="ise:StringResource('shoppingcart.cs.47')" disable-output-escaping="yes" />
                                </span>
                              </xsl:if>
                            </td>
                          </xsl:when>
                          <!--0 allocation but has reservation-->
                          <xsl:when test="$AllocQty = 0 and $ReserveCol &gt; 0">
                            <td class="cart-col">
                              <!--has reservation-->
                              <xsl:for-each select="RESERVATIONITEM">
                                <xsl:call-template name="RESERVATIONITEM">
                                  <xsl:with-param name="ItemCode" select="$ItemCode" />
                                </xsl:call-template>
                              </xsl:for-each>
                              <xsl:if test="NOTAVAILABLEQTYWITHRESERVATION &gt; 0">
                                <br />
                                <span class="addSpace">
                                  <xsl:value-of select="NOTAVAILABLEQTYWITHRESERVATION" disable-output-escaping="yes" />
                                </span>
                                <span>
                                  <xsl:value-of select="ise:StringResource('shoppingcart.cs.47')" disable-output-escaping="yes" />
                                </span>
                              </xsl:if>
                            </td>
                          </xsl:when>
                          <!--0 allocation and no reservation = stock not available-->
                          <xsl:otherwise>
                            <td class="cart-col">
                              <div class="cart_shipping">
                                <span class="addSpace">
                                  <xsl:value-of select="ise:StringResource('shoppingcart.cs.47')" disable-output-escaping="yes" />
                                </span>
                              </div>
                            </td>
                          </xsl:otherwise>
                        </xsl:choose>
                      </xsl:if>
                      <xsl:if test="$HideUnitMeasure = 'false'">
                        <td class="cart-col">
                          <xsl:if test="NOT_HIDE_UNIT_MEASURE">
                            <xsl:choose>
                              <xsl:when test="AVAILABLEUNITMESSURE_GREATER_ONE = 'false'">
                                <span>
                                  <xsl:value-of select="UNITMEASURECODESPANDISPLAY" disable-output-escaping="yes" />
                                </span>
                                <input type="hidden" name="{UNITMEASURECODEID}" id="{UNITMEASURECODENAME}" value="{UNITMEASURECODEVALUE}" />
                              </xsl:when>
                              <xsl:otherwise>
                                <xsl:choose>
                                  <xsl:when test="$RenderType = 'SHOPPINGCART'">
                                    <select size="1" class="showproduct_limitunitmeasure" name="{MULTIPLE_UNITMEASURECODENAME}" id="{MULTIPLE_UNITMEASURECODEID}">
                                      <xsl:for-each select="./UNITMEASSURE_ITEM">
                                        <option value="{VALUE}" unitmeasurequantity="{UNITMEASUREQUANTITY}" minorderquantityid="{MINORDERQUANTITYID}">
                                          <xsl:if test="SELECTED = 'true'">
                                            <xsl:attribute name="selected">selected</xsl:attribute>
                                          </xsl:if>
                                          <xsl:value-of select="TEXT" disable-output-escaping="yes" />
                                        </option>
                                      </xsl:for-each>
                                    </select>
                                  </xsl:when>
                                  <xsl:otherwise>
                                    <xsl:value-of select="UNITMEASURECODESPANDISPLAY" disable-output-escaping="yes" />
                                  </xsl:otherwise>
                                </xsl:choose>
                              </xsl:otherwise>
                            </xsl:choose>
                          </xsl:if>
                        </td>
                      </xsl:if>
                      <td class="cart-col">
                        <xsl:choose>
                          <xsl:when test="$RenderType = 'SHOPPINGCART'">
                            <xsl:choose>
                              <xsl:when test="ISRESTRICTEDQUANTITIES = 'false'">
                                <input type="text" class="inputQuantityLimit addSpace" id="{INPUTQUANTITYID}" name="{INPUTQUANTITYID}" value="{INPUTQUANTITYVALUE}" />
                              </xsl:when>
                              <xsl:otherwise>
                                <select size="1" class="showproduct_limit-restricted-qty addSpace" id="{concat('Quantity_',QUANTITYLISTID)}" name="{concat('Quantity_',QUANTITYLISTID)}">
                                  <option value="0">DELETE</option>
                                  <xsl:for-each select="RESTRICTEDQUANTITIES">
                                    <option value="{QTY}">
                                      <xsl:if test="SELECTED = 'true'">
                                        <xsl:attribute name="selected">selected</xsl:attribute>
                                      </xsl:if>
                                      <xsl:value-of select="QTY" disable-output-escaping="yes" />
                                    </option>
                                  </xsl:for-each>
                                </select>
                              </xsl:otherwise>
                            </xsl:choose>
                            <input type="hidden" name="{MINORDERQUANTITYID}" id="{MINORDERQUANTITYNAME}" value="{MINORDERQUANTITYVALUE}" />
                            <input type="hidden" name="Base_{MINORDERQUANTITYID}" id="Base_{MINORDERQUANTITYNAME}" value="{MINORDERQUANTITYVALUE}" />
                          </xsl:when>
                          <xsl:otherwise>
                            <span>
                              <xsl:value-of select="INPUTQUANTITYVALUE" disable-output-escaping="yes" />
                            </span>
                          </xsl:otherwise>
                        </xsl:choose>
                      </td>
                      <xsl:if test="$HasCoupon = 'true' and $IsCouponTypeOrders = 'false'">
                        <td class="cart-col">
                          <xsl:choose>
                            <xsl:when test="COUPON_DISCOUNT_TYPE = 'Percent'">
                              <span>
                                    (<xsl:value-of select="DISCOUNT_COUPON_RATE_VALUE" disable-output-escaping="yes" />) (<xsl:value-of select="DISCOUNT_COUPON_PERCENTAGE" disable-output-escaping="yes" />)
                                  </span>
                            </xsl:when>
                            <xsl:otherwise>
                              <span>
                                    (<xsl:value-of select="DISCOUNT_COUPON_RATE_VALUE" disable-output-escaping="yes" />)
                                  </span>
                            </xsl:otherwise>
                          </xsl:choose>
                        </td>
                      </xsl:if>
                      <td class="cart-col gotextright-basic">
                        <div class="cart_price">
                          <span class="showproduct_Price addSpace">
                            <xsl:value-of select="PRICEFORMATTED" disable-output-escaping="yes" />
                          </span>
                          <xsl:if test="$VatEnabled = 'true'">
                            <span>
                              <b>
                                <xsl:choose>
                                  <xsl:when test="$VatInclusive = 'false'">
                                    <xsl:value-of select="ise:StringResource('showproduct.aspx.37')" disable-output-escaping="yes" />
                                  </xsl:when>
                                  <xsl:otherwise>
                                    <xsl:value-of select="ise:StringResource('showproduct.aspx.38')" disable-output-escaping="yes" />
                                  </xsl:otherwise>
                                </xsl:choose>
                              </b>
                            </span>
                            <br />
                            <hr />
                            <span>
                              <b>
                                <xsl:value-of select="ise:StringResource('showproduct.aspx.41')" disable-output-escaping="yes" />
                              </b>
                            </span>
                            <span class="showproduct_Price addSpace">
                              <xsl:value-of select="TAX_RATE_VALUE" />
                            </span>
                          </xsl:if>
                        </div>
                      </td>
                      <td class="cart-col cart-col-delete-icon">
                        <xsl:if test="SHOWCARTDELETEITEMBUTTON and $RenderType = 'SHOPPINGCART'">
                          <input type="submit" class="cart-delete-custom content" name="bt_Delete" onclick="{concat('this.form.Quantity_',CART_ITEM_ID,'.value=0;')}" value="" />
                        </xsl:if>
                      </td>
                    </tr>
                    <xsl:call-template name="CartItemsDivider">
                      <xsl:with-param name="HasCoupon" select="$HasCoupon" />
                      <xsl:with-param name="IsCouponTypeOrders" select="$IsCouponTypeOrders" />
                    </xsl:call-template>
                  </xsl:for-each>
                </xsl:when>
                <xsl:otherwise>
                  <!--For Bundle Item-->
                  <tr>
                    <!--column 1-->
                    <td class="cart_picture_layout">
                      <xsl:if test="SHOWPICSINCART = 'true'">
                        <xsl:choose>
                          <xsl:when test="LINKBACK = 'true'">
                            <a href="{PRODUCTLINKHREF}">
                              <img id="img-shopping-cart-{COUNTER}" class="mobileimagesize" title="{PRODUCTIMAGETITLE}" alt="{PRODUCTIMAGEALT}" src="{PRODUCTIMAGEPATH}" />
                            </a>
                          </xsl:when>
                          <xsl:otherwise>
                            <img title="{PRODUCTIMAGETITLE}" alt="{PRODUCTIMAGEALT}" src="{PRODUCTIMAGEPATH}" />
                          </xsl:otherwise>
                        </xsl:choose>
                      </xsl:if>
                      <center>
                        <xsl:choose>
                          <xsl:when test="ITEM/BUNDLEHEADERID = ''">
                            <button class="btn btn-info site-button content" type="button" id="btn-bundle-view-details-{COUNTER}">
                              <xsl:value-of select="ise:StringResource('showproduct.aspx.88')" disable-output-escaping="yes" />
                            </button>
                          </xsl:when>
                          <xsl:otherwise>
                            <button class="btn btn-info site-button content" type="button" id="btn-bundle-view-details-{COUNTER}-{ITEM/BUNDLEHEADERID}">
                              <xsl:value-of select="ise:StringResource('showproduct.aspx.88')" disable-output-escaping="yes" />
                            </button>
                          </xsl:otherwise>
                        </xsl:choose>
                        <xsl:value-of select="ise:GetBundleProductDetails('shoppingcart',COUNTER,ITEM/BUNDLECODE,'',ITEM/BUNDLEHEADERID)" disable-output-escaping="yes">
                        </xsl:value-of>
                      </center>
                    </td>
                    <!--column 2-->
                    <td class="cart-col kit_container">
                      <xsl:choose>
                        <xsl:when test="LINKBACK = 'true'">
                          <xsl:choose>
                            <xsl:when test="ISCHECKOUTOPTION = 'false'">
                              <a href="{PRODUCTLINKHREF}">
                                <b>
                                  <xsl:value-of select="PRODUCTLINKNAME" disable-output-escaping="yes" />
                                </b>
                              </a>
                            </xsl:when>
                            <xsl:otherwise>
                              <span class="product_description">
                                <xsl:value-of select="PRODUCTLINKNAME" disable-output-escaping="yes" />
                              </span>
                            </xsl:otherwise>
                          </xsl:choose>
                        </xsl:when>
                        <xsl:otherwise>
                          <span class="product_description">
                            <xsl:value-of select="PRODUCTLINKNAME" disable-output-escaping="yes" />
                          </span>
                        </xsl:otherwise>
                      </xsl:choose>
                    </td>
                    <!--column 3-->
                    <td class="cart-col" id="bundle-total-price-{ITEM/BUNDLECODE}-{ITEM/BUNDLEHEADERID}">
                      <xsl:value-of select="SALES_PRICE" disable-output-escaping="yes" />
                    </td>
                    <!--column 4-->
                    <td>
                    </td>
                    <!--column 5-->
                    <xsl:if test="$HideUnitMeasure = 'false'">
                      <td class="cart-col">
                        <xsl:if test="NOT_HIDE_UNIT_MEASURE">
                          <xsl:choose>
                            <xsl:when test="AVAILABLEUNITMESSURE_GREATER_ONE = 'false'">
                              <span>
                                <xsl:value-of select="UNITMEASURECODESPANDISPLAY" disable-output-escaping="yes" />
                              </span>
                              <input type="hidden" name="{UNITMEASURECODEID}" id="{UNITMEASURECODENAME}" value="{UNITMEASURECODEVALUE}" />
                            </xsl:when>
                          </xsl:choose>
                        </xsl:if>
                      </td>
                    </xsl:if>
                    <!--column 6-->
                    <td class="cart-col">
                      <xsl:choose>
                        <xsl:when test="$RenderType = 'SHOPPINGCART'">
                          <xsl:choose>
                            <xsl:when test="ISRESTRICTEDQUANTITIES = 'false'">
                              <input type="text" class="inputQuantityLimit addSpace" id="{INPUTQUANTITYID}_{ITEM/BUNDLEHEADERID}" name="{INPUTQUANTITYID}_{ITEM/BUNDLEHEADERID}" value="{INPUTQUANTITYVALUE}" />
                            </xsl:when>
                            <xsl:otherwise>
                              <select size="1" class="showproduct_limit-restricted-qty addSpace" id="{concat('Quantity_',QUANTITYLISTID)}" name="{concat('Quantity_',QUANTITYLISTID)}">
                                <option value="0">DELETE</option>
                                <xsl:for-each select="RESTRICTEDQUANTITIES">
                                  <option value="{QTY}">
                                    <xsl:if test="SELECTED = 'true'">
                                      <xsl:attribute name="selected">selected</xsl:attribute>
                                    </xsl:if>
                                    <xsl:value-of select="QTY" disable-output-escaping="yes" />
                                  </option>
                                </xsl:for-each>
                              </select>
                            </xsl:otherwise>
                          </xsl:choose>
                          <input type="hidden" name="{MINORDERQUANTITYID}" id="{MINORDERQUANTITYNAME}" value="{MINORDERQUANTITYVALUE}" />
                          <input type="hidden" name="Base_{MINORDERQUANTITYID}" id="Base_{MINORDERQUANTITYNAME}" value="{MINORDERQUANTITYVALUE}" />
                        </xsl:when>
                        <xsl:otherwise>
                          <span>
                            <xsl:value-of select="INPUTQUANTITYVALUE" disable-output-escaping="yes" />
                          </span>
                        </xsl:otherwise>
                      </xsl:choose>
                    </td>
                    <!--column 7-->
                    <td class="cart-col" id="bundle-total-price-ext-{ITEM/BUNDLECODE}-{ITEM/BUNDLEHEADERID}">
                      <xsl:value-of select="SALES_EXTPRICE" disable-output-escaping="yes" />
                    </td>
                    <!--column 8-->
                    <td class="cart-col cart-col-delete-icon">
                      <xsl:if test="SHOWCARTDELETEITEMBUTTON and $RenderType = 'SHOPPINGCART'">
                        <input type="submit" class="cart-delete-custom content" name="bt_Delete" onclick="$('{concat('#Quantity_',ITEM/BUNDLECODE)}_{ITEM/BUNDLEHEADERID}').val('0');" value="0" />
                      </xsl:if>
                    </td>
                  </tr>
                  <xsl:call-template name="CartItemsDivider">
                    <xsl:with-param name="HasCoupon" select="$HasCoupon" />
                    <xsl:with-param name="IsCouponTypeOrders" select="$IsCouponTypeOrders" />
                  </xsl:call-template>
                </xsl:otherwise>
              </xsl:choose>
            </xsl:for-each>
            <!--end-->
            <tr>
              <td colspan="2" class="cart-col">
              </td>
              <td class="cart-col">
                <xsl:choose>
                  <xsl:when test="$HasCoupon = 'true' and $IsCouponTypeOrders = 'false'">
                    <xsl:attribute name="colspan">6</xsl:attribute>
                  </xsl:when>
                  <xsl:otherwise>
                    <xsl:attribute name="colspan">5</xsl:attribute>
                  </xsl:otherwise>
                </xsl:choose>
                <!-- Summary Rendering -->
                <div>
                  <xsl:choose>
                    <xsl:when test="$RenderType = 'SHOPPINGCART'">
                      <xsl:attribute name="class">
                        <xsl:text>summary</xsl:text>
                      </xsl:attribute>
                    </xsl:when>
                    <xsl:otherwise>
                      <xsl:attribute name="class">
                        <xsl:text>summary-smaller</xsl:text>
                      </xsl:attribute>
                    </xsl:otherwise>
                  </xsl:choose>
                  <span class="summary-captions">
                    <xsl:value-of select="ise:StringResource('shoppingcart.cs.27')" disable-output-escaping="yes" />
                  </span>
                  <span class="summary-values">
                    <xsl:value-of select="FIELD/SUBTOTAL_VALUE" />
                  </span>
                  <xsl:if test="$VatEnabled = 'true'">
                    <span class="summary-captions leftSpace">
                      <xsl:choose>
                        <xsl:when test="$VatInclusive = 'false'">
                          <xsl:value-of select="ise:StringResource('showproduct.aspx.37')" disable-output-escaping="yes" />
                        </xsl:when>
                        <xsl:otherwise>
                          <xsl:value-of select="ise:StringResource('showproduct.aspx.38')" disable-output-escaping="yes" />
                        </xsl:otherwise>
                      </xsl:choose>
                    </span>
                  </xsl:if>
                  <div class="checkout-summary-clr">
                  </div>
                  <span class="summary-captions">
                    <xsl:value-of select="ise:StringResource('shoppingcart.aspx.10')" disable-output-escaping="yes" />
                  </span>
                  <xsl:if test="($RenderType = 'PAYMENT' or $RenderType = 'REVIEW') and $IsFreeShipping = 'false'">
                    <xsl:if test="$VatEnabled = 'true'">
                      <span class="summary-captions leftSpace">
                        <xsl:choose>
                          <xsl:when test="$VatInclusive = 'false'">
                            <xsl:value-of select="ise:StringResource('showproduct.aspx.37')" disable-output-escaping="yes" />
                          </xsl:when>
                          <xsl:otherwise>
                            <xsl:value-of select="ise:StringResource('showproduct.aspx.38')" disable-output-escaping="yes" />
                          </xsl:otherwise>
                        </xsl:choose>
                      </span>
                    </xsl:if>
                  </xsl:if>
                  <xsl:choose>
                    <xsl:when test="$RenderType = 'SHOPPINGCART' or $RenderType = 'SHIPPING'">
                      <span class="summary-values">
                        <xsl:value-of select="ise:StringResource('shoppingcart.aspx.12')" disable-output-escaping="yes" />
                      </span>
                    </xsl:when>
                    <xsl:otherwise>
                      <span class="summary-values">
                        <xsl:choose>
                          <xsl:when test="$IsFreeShipping = 'true'">
                            <xsl:value-of select="ise:StringResource('shoppingcart.aspx.13')" disable-output-escaping="yes" />
                          </xsl:when>
                          <xsl:otherwise>
                            <xsl:value-of select="FIELD/FREIGHT" disable-output-escaping="yes" />
                          </xsl:otherwise>
                        </xsl:choose>
                      </span>
                    </xsl:otherwise>
                  </xsl:choose>
                  <!--Regardless if vat is enabled/disabled. display the tax-->
                  <xsl:if test="$VatInclusive = 'false'">
                    <br />
                    <span class="summary-captions">
                      <xsl:value-of select="ise:StringResource('shoppingcart.aspx.11')" disable-output-escaping="yes" />
                    </span>
                    <xsl:choose>
                      <xsl:when test="$RenderType = 'SHOPPINGCART' and $IsCustomerRegistered = 'false'">
                        <span class="summary-values">
                          <xsl:value-of select="ise:StringResource('shoppingcart.aspx.12')" disable-output-escaping="yes" />
                        </span>
                      </xsl:when>
                      <xsl:otherwise>
                        <xsl:if test="$ShowTaxBreakDown = 'false' or $RenderType = 'SHIPPING' or $RenderType = 'SHOPPINGCART'">
                          <span class="summary-values">
                            <xsl:value-of select="FIELD/TAX_RATE_VALUE" disable-output-escaping="yes" />
                          </span>
                        </xsl:if>
                        <xsl:if test="$ShowTaxBreakDown = 'true' and ($RenderType = 'PAYMENT' or $RenderType = 'REVIEW')">
                          <div>
                            <span class="summary-values">
                              <a class="aTaxRateValue" href="javascript:void(1);" data-title="{ise:StringResource('shoppingcart.aspx.66')}" title="{ise:StringResource('shoppingcart.aspx.65')}" data-mode="show">
                                <xsl:value-of select="FIELD/TAX_RATE_VALUE" disable-output-escaping="yes" />
                              </a>
                            </span>
                            <div class="clear-both">
                            </div>
                            <div class="divTaxBreakdownWrapper">
                              <span id="title">
                                <xsl:value-of select="ise:StringResource('shoppingcart.aspx.67')" disable-output-escaping="yes" />
                              </span>
                              <ul>
                                <li>
                                  <xsl:value-of select="ise:StringResource('shoppingcart.aspx.68')" disable-output-escaping="yes" />
                                </li>
                                <li style="float:right;">
                                  <xsl:value-of select="FIELD/FREIGHTTAX" disable-output-escaping="yes" />
                                </li>
                              </ul>
                              <ul>
                                <li>
                                  <xsl:value-of select="ise:StringResource('shoppingcart.aspx.69')" disable-output-escaping="yes" />
                                </li>
                                <li style="float:right;">
                                  <xsl:value-of select="FIELD/LINEITEMTAX" disable-output-escaping="yes" />
                                </li>
                              </ul>
                            </div>
                          </div>
                        </xsl:if>
                      </xsl:otherwise>
                    </xsl:choose>
                  </xsl:if>
                  <!-- Coupon Discount -->
                  <xsl:if test="$HasCoupon = 'true' and $IsCouponTypeOrders = 'true'">
                    <br />
                    <span class="summary-captions">
                      <xsl:value-of select="ise:StringResource('shoppingcart.cs.38')" disable-output-escaping="yes" />
                    </span>
                    <span class="summary-values">
                      <xsl:value-of select="FIELD/APPLIED_COUPON_DISCOUNT" disable-output-escaping="yes" />
                    </span>
                  </xsl:if>
                  <!-- Gift Card -->
                  <xsl:if test="FIELD/APPLIED_CREDIT_GIFT_CARD != ''">
                    <br />
                    <span class="summary-captions">
                      <xsl:value-of select="ise:StringResource('shoppingcart.aspx.31')" disable-output-escaping="yes" />
                    </span>
                    <span class="summary-values">
                      <xsl:value-of select="FIELD/APPLIED_CREDIT_GIFT_CARD" disable-output-escaping="yes" />
                    </span>
                  </xsl:if>
                  <!-- Gift Certificate -->
                  <xsl:if test="FIELD/APPLIED_CREDIT_GIFT_CERTIFICATE != ''">
                    <br />
                    <span class="summary-captions">
                      <xsl:value-of select="ise:StringResource('shoppingcart.aspx.32')" disable-output-escaping="yes" />
                    </span>
                    <span class="summary-values">
                      <xsl:value-of select="FIELD/APPLIED_CREDIT_GIFT_CERTIFICATE" disable-output-escaping="yes" />
                    </span>
                  </xsl:if>
                  <!-- Loyalty Points -->
                  <xsl:if test="FIELD/APPLIED_LOYALTYPOINTS_AMOUNT != ''">
                    <br />
                    <span class="summary-captions">
                      <xsl:value-of select="ise:StringResource('shoppingcart.aspx.44')" disable-output-escaping="yes" />
                    </span>
                    <span class="summary-values">
                      <xsl:value-of select="FIELD/APPLIED_LOYALTYPOINTS_AMOUNT" disable-output-escaping="yes" />
                    </span>
                  </xsl:if>
                  <!-- Credit Memos -->
                  <xsl:if test="FIELD/APPLIED_CREDITMEMOS_AMOUNT != ''">
                    <br />
                    <span class="summary-captions">
                      <xsl:value-of select="ise:StringResource('shoppingcart.aspx.63')" disable-output-escaping="yes" />
                    </span>
                    <span class="summary-values">
                      <xsl:value-of select="FIELD/APPLIED_CREDITMEMOS_AMOUNT" disable-output-escaping="yes" />
                    </span>
                  </xsl:if>
                  <div class="checkout-summary-clr checkout-summary-clr-with-border hide-on-tax-breakdown-display">
                  </div>
                  <span class="summary-captions summary-captions-bold">
                    <xsl:value-of select="ise:StringResource('shoppingcart.cs.11')" disable-output-escaping="yes" />
                  </span>
                  <span class="summary-values summary-values-bold">
                    <xsl:value-of select="FIELD/TOTAL" />
                  </span>
                </div>
                <!-- End Summary Rendering -->
              </td>
            </tr>
          </table>
        </div>
      </xsl:otherwise>
    </xsl:choose>
    <script>
          $(document).ready(function(){

          setMinQuantityByUnitMeasure = function (selection) {
            var $this = selection.find(":selected");
            var minOrderQtyID = $this.attr("minorderquantityid");
            var umQty = $this.attr("unitmeasurequantity");

            if (minOrderQtyID==undefined){return;}
            if (minOrderQtyID.toString().length == 0){return;}
            var $baseMinOrderQtyHolder = $("#"+ "Base_" + minOrderQtyID );

            if ($baseMinOrderQtyHolder == undefined){return;}
            var baseMinOrderQty =$baseMinOrderQtyHolder.val();

            if(umQty==undefined){return;}
            if (baseMinOrderQty==undefined){return;}

            umQty = Number(umQty);
            baseMinOrderQty = Number(baseMinOrderQty);

            if (baseMinOrderQty == 0){return;}

            // DOZEN to EACH Vv
            // check if allow fractional before using Math.ceil
            <xsl:choose><xsl:when test="ALLOWFRACTIONAL = 'false'">
                umQty=  Math.ceil(baseMinOrderQty/umQty);
              </xsl:when><xsl:otherwise>
                umQty=  Math.round((baseMinOrderQty/umQty)*100)/100;
              </xsl:otherwise></xsl:choose>
          var $minQuantityHolder = $("#"+ minOrderQtyID);
          if ($minQuantityHolder != undefined){$minQuantityHolder.val(Number(umQty)); }
          }

          $(".showproduct_limitunitmeasure").change(function(){
            var $this = $(this);
            setMinQuantityByUnitMeasure($this);
          });


          });
        </script>
  </xsl:template>
  <xsl:template name="HeaderTemplate">
    <xsl:param name="RenderType" />
    <div class="section-header section-header-top" style="text-align:left;">
      <span>
        <xsl:value-of select="ise:StringResource('checkoutshipping.aspx.13')" disable-output-escaping="yes" />
      </span>
      <xsl:if test="$RenderType != 'SHOPPINGCART'">
        <span>
          <xsl:value-of select="ise:StringResource('checkoutcard.aspx.1')" disable-output-escaping="yes" />
        </span>
        <a href="shoppingcart.aspx">
          <xsl:value-of select="ise:StringResource('checkoutcard.aspx.2')" disable-output-escaping="yes" />
        </a>
      </xsl:if>
    </div>
  </xsl:template>
  <xsl:template name="RESERVATIONITEM">
    <xsl:param name="ItemCode" />
    <xsl:if test="RESERVE_ITEMCODE = $ItemCode">
      <span class="addSpace">
        <xsl:value-of select="RESERVE_QTY" disable-output-escaping="yes" />
      </span>
      <span class="addSpace">
        <xsl:value-of select="ise:StringResource('shoppingcart.cs.60')" disable-output-escaping="yes" />
      </span>
      <br />
      <span>
        <xsl:value-of select="RESERVE_SHIPDATE" disable-output-escaping="yes" />
      </span>
      <br />
    </xsl:if>
  </xsl:template>
  <xsl:template name="CartItemsDivider">
    <tr>
      <td>
        <xsl:choose>
          <xsl:when test="HasCoupon = 'true' and IsCouponTypeOrders = 'false'">
            <xsl:attribute name="colspan">8</xsl:attribute>
          </xsl:when>
          <xsl:otherwise>
            <xsl:attribute name="colspan">7</xsl:attribute>
          </xsl:otherwise>
        </xsl:choose>
        <hr class="cart-item-divider">
        </hr>
      </td>
    </tr>
  </xsl:template>
</xsl:stylesheet>
