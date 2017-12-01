<%@ Page Language="C#" AutoEventWireup="true" CodeFile="findgiftregistry.aspx.cs"
    Inherits="InterpriseSuiteEcommerce.findgiftregistry" %>
<%@ OutputCache Location="None" %>
<%@ Import Namespace="InterpriseSuiteEcommerceCommon" %>
<%@ Import Namespace="InterpriseSuiteEcommerceCommon.Extensions" %>
<%@ Import Namespace="InterpriseSuiteEcommerceCommon.DTO" %>
<head>
<title></title>
</head>
<body>
<form id="form1" runat="server">
  <asp:Panel ID="pnlMain" runat="server" CssClass="pnlmain">
    
    <!-- Find Gift Registry Form -->
    <div class="sections-place-holder row">
      <div class="col-md-6 col-md-offset-3">
        <div class="entity-header">
          <h1>
            <asp:Literal runat="server" ID="litRegistryHeader"></asp:Literal>
          </h1>
        </div>
        <div class="form-group">
          <label><%= AppLogic.GetString("findregistry.aspx.aspx.4")%></label>
          <input type="text" id="txtLastName" class="form-control" />
        </div>
        <div class="form-group">
          <label><%= AppLogic.GetString("findregistry.aspx.aspx.3")%></label>
          <input type="text" id="txtFirstName" class="form-control" />
        </div>
        <div class="form-group">
          <label><%= AppLogic.GetString("findregistry.aspx.aspx.5")%></label>
          <input type="text" id="txtEventTitle" class="form-control" />
        </div>
        <div class="button-place-holder">
          <input type="button" 
                        class="btn btn-primary content" 
                        id="btnFind"
                        data-contentKey="findregistry.aspx.aspx.1"
                        data-contentValue="<%= AppLogic.GetString("findregistry.aspx.aspx.1", true) %>"
                        data-contentType="string resource"
                        value="<%= AppLogic.GetString("findregistry.aspx.aspx.1", true) %>" />
          &nbsp;&nbsp;&nbsp;
          <div class="clear-both height-17"></div>
        </div>
      </div>
    </div>
    
    <!-- Search Result-->
    <div class="sections-place-holder" id="divSearchResult" style="display:none;">
      <div class="page-sections-head"><%= AppLogic.GetString("findregistry.aspx.aspx.2") %>: <span id="recFound"></span></div>
      <div id="detailLoader" style="width:100%; display:none; text-align:center; padding:20px; "> <img title="" alt="" src="images/ajax-loader2.gif" /> </div>
      <div id="searchOutput" style="display:none;">
        <%-- <div class="gridHeader">
                <div class="Paging">
                    <span class="pagingprevappender"></span>
                    <span class="numberappender"></span>
                    <span class="pagingnextappender"></span>
                </div>
                </div>--%>
        <table class="gridBody" width="100%" style="border:none !important; font-size:9pt !important;">
        </table>
        <br />
        <div class="gridFooter" style="border:none !important; border-top:solid 1px #cebfbf !important; border-radius:0 !important;">
          <div class="Paging"> <span class="pagingprevappender"></span> <span class="numberappender"></span> <span class="pagingnextappender"></span> </div>
        </div>
      </div>
    </div>
    <script type="text/javascript">
            
            var searchResults = null;
            var maxDisplayPages = 5;
            var headerResult = null;
            var currentRow = 0;
            var totalSet = 1;
            var noRecordNumber = -1;
            var defaultRecordCountPerPage = <%= AppLogic.AppConfigNativeInt("GiftRegistry.DefaulSearchPageSize") %>;

            var firstName = '';
            var lastName = '';
            var title = '';

            $(document).ready(function () {

                $('#txtFirstName').keypress(function(e){
                    if(e.which == 13){ Find(); }
                });

                $('#txtLastName').keypress(function(e){
                    if(e.which == 13){ Find(); }
                });

                $('#txtEventTitle').keypress(function(e){
                    if(e.which == 13){ Find(); }
                });

                $('#btnFind').click(function () { 
                    if($(this).hasClass("editable-content")) return false;
                    Find();
                });

            });

            function Find()
            {
                firstName = $('#txtFirstName').val();
                lastName = $('#txtLastName').val();
                title = $('#txtEventTitle').val();

                currentRow = 0;
                    
                if(!IsValidSearchInput()) {
                    alert('<%= AppLogic.GetString("findregistry.aspx.aspx.6", true) %>');
                    return;
                };

                Search();
            }

            function NextSet(currentRecordNumber)
            {
                if(currentRecordNumber == -1) return;
                currentRow = currentRecordNumber;

                Search();
            }

            function PrevSet(currentRecordNumber)
            {
                if(currentRecordNumber == -1) return;
                currentRow = currentRecordNumber;

                Search();
            }

            function InitPageSetLayout()
            {
                var preObject = null;

                $(".pagingprevappender a").remove();
                $(".pagingnextappender a").remove();

                if(totalSet == 1)
                {
                    preObject = $.parseJSON('{"CurrentStyle":' + '"PagingPrevInactive"' + ', "CurrentRecord":' + noRecordNumber + '}');
                    $.tmpl("pagingprev", preObject).appendTo(".pagingprevappender");

                    preObject = $.parseJSON('{"CurrentStyle":' + '"PagingNextInactive"' + ', "CurrentRecord":' + noRecordNumber + '}');
                    $.tmpl("pagingnext", preObject).appendTo(".pagingnextappender");
                    return;
                }
                else
                {
                    //it will not be greater than
                    if(currentRow == headerResult.TotalRecord) 
                    {
                        //this will be 50

                        var prevRecordNumber = (Math.floor(currentRow / headerResult.DefaultRecordPerSet) * headerResult.DefaultRecordPerSet) - (headerResult.DefaultRecordPerSet);
                        //currentRow - (headerResult.DefaultRecordPerSet * 2);

                        if(prevRecordNumber < 0) prevRecordNumber = 0;

                        preObject = $.parseJSON('{"CurrentStyle":' + '"PagingPrev"' + ', "CurrentRecord":' + prevRecordNumber + '}');
                        $.tmpl("pagingprev", preObject).appendTo(".pagingprevappender");

                        preObject = $.parseJSON('{"CurrentStyle":' + '"PagingNextInactive"' + ', "CurrentRecord":' + noRecordNumber + '}');
                        $.tmpl("pagingnext", preObject).appendTo(".pagingnextappender");
                    }
                    else //less the current record
                    {
                        var nextRecordNumber = currentRow;
                        var prevClass= 'PagingPrev';

                        var prevRecordNumber = currentRow - (headerResult.DefaultRecordPerSet * 2);
                        if(prevRecordNumber < 0 && headerResult.DefaultRecordPerSet <= nextRecordNumber)
                        {
                            prevClass = 'PagingPrevInactive';
                            prevRecordNumber = -1;
                            currentRow = headerResult.DefaultRecordPerSet;
                            nextRecordNumber = headerResult.DefaultRecordPerSet;
                        }

                        preObject = $.parseJSON('{"CurrentStyle":' + JSON.stringify(prevClass) + ', "CurrentRecord":' + prevRecordNumber + '}');
                        $.tmpl("pagingprev", preObject).appendTo(".pagingprevappender");

                        preObject = $.parseJSON('{"CurrentStyle":' + '"PagingNext"' + ', "CurrentRecord":' + nextRecordNumber + '}');
                        $.tmpl("pagingnext", preObject).appendTo(".pagingnextappender");
                    }
                }

            }

            function MovePage (startAt, currentActive)
            {
                var id = "#lnkpage_" + currentActive;
                if($(id).attr('class') == 'PagingActive') return;

                var lnkControl = $('.PagingActive');
                $(lnkControl).removeClass("PagingActive");
                $(lnkControl).addClass("PagingNum");

                $(id).removeClass("PagingNum");
                $(id).addClass("PagingActive");

                $("#searchOutput").hide();
                $("#detailLoader").show();
                $(".gridBody tr").remove();

                var range = (startAt==0)? defaultRecordCountPerPage : startAt + defaultRecordCountPerPage;
                var recToDisplay = searchResults.slice(startAt, range);
                $.tmpl("detailTemplate", recToDisplay).appendTo(".gridBody");

                $("#detailLoader").hide();
                $("#searchOutput").show();
            }

            $.template(
                "detailTemplate",
                "<tr>" +
                "<td style='width: 10%; height: 50px;'>" +
                "<div class='imgContainer'><img src='images/giftregistry/${PictureFileName}' class='imgContainer' /></div>" +
                "</td>" +
                "<td>" +
                    "<a href='${URLForViewing}'>${Title}</a><br />" + 
                    "<span><%= AppLogic.GetString("giftregistry.aspx.4", true) %>: ${StartDate} To ${EndDate}</span><br />" +
                    "<span>${OwnersFullName}</span>" +
                "</td>" +
                "</tr>"
            );

            $.template(
                "headerTemplate",
                "<a class='${CurrentStyle}' id='lnkpage_${CurrentNum}' href='javascript:void(0);' onclick='MovePage(${StartingCount},${CurrentNum});'>${CurrentNum}</a>"
            );

            $.template(
                "pagingprev",
                "<a class='${CurrentStyle}' onclick='PrevSet(${CurrentRecord})' href='javascript:void(0)'></a>"
            );

            $.template(
                "pagingnext",
                "<a class='${CurrentStyle}' onclick='NextSet(${CurrentRecord})' href='javascript:void(0)' title='next'></a>"
            );

            function Search () 
            {
                $("#searchOutput").hide();
                $("#detailLoader").show();
                $(".gridBody tr").remove();
                $("#divSearchResult").show();
                    
                searchResults = "[]";

                $.ajax({
                    type: "POST",
                    url: "ActionService.asmx/FindRegistriesReturnJSON",
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    data: '{"firstName":' + JSON.stringify(firstName) +
                            ', "lastName":' + JSON.stringify(lastName) +
                            ', "eventTitle":' + JSON.stringify(title) + 
                            ', "currentRow":' + JSON.stringify(currentRow) + '}',
                    success: function (result) {
                        headerResult = $.parseJSON(result.d);
                        searchResults = headerResult.Items;

                        if(searchResults.length > 0)
                        {
                            $("#searchOutput").show();

                            totalSet = headerResult.TotalSet;
                            currentRow = headerResult.CurrentRecord;

                            InitPageSetLayout();

                            var recToDisplay = searchResults.slice(0, defaultRecordCountPerPage);
                            $.tmpl("detailTemplate", recToDisplay).appendTo(".gridBody");

                            //remove the paging since it will be recreated and will not append to the next display
                            $(".PagingActive").remove();
                            $(".PagingNum").remove();

                            PopulatePageNum();
                        }

                        $("#detailLoader").hide();
                        $("#recFound").html(headerResult.TotalRecord + ' ' + '<%= AppLogic.GetString("findregistry.aspx.aspx.7")%>');

                    },
                    error: function (result, textStatus, errorThrown) {
                        alert(errorThrown);
                    }
                });
            }

            function IsValidSearchInput()
            {
                return ($.trim(firstName).length > 0 || $.trim(lastName).length > 0 || $.trim(title).length > 0)
            }

            function PopulatePageNum()
            {
                var totalSize = searchResults.length;
                var totalPages = 0;
                if(totalSize > defaultRecordCountPerPage)
                {
                    var totalPages = totalSize / defaultRecordCountPerPage;
                }
                else
                {
                    totalPages = 1;
                    $('.PagingNext').hide();
                }

                var currnumLst = "[";
                for (var i = 0; i < totalPages; i++) 
                {
                    var curNm = (i + 1);
                    if(i == 0)
                    {
                        currnumLst += '{"CurrentNum":' + curNm + ', "CurrentStyle":' + '"PagingActive"' + ', "StartingCount":' + 0 + '},';
                    }
                    else
                    {
                        currnumLst += '{"CurrentNum":' + curNm + ', "CurrentStyle":' + '"PagingNum"' + ', "StartingCount":' + (i * defaultRecordCountPerPage) + '},';
                    }
                }

                currnumLst = currnumLst.substr(0, currnumLst.length-1);
                currnumLst += "]";

                var numObjlst = $.parseJSON(currnumLst);
                $.tmpl("headerTemplate", numObjlst).appendTo(".numberappender");
            }

        </script>
    </asp:Panel>
</form>
</body>
