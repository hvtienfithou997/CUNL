#pragma checksum "F:\COde\QLCUNL.WebUI\Views\NhaTuyenDung\Edit.cshtml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "12316450da25dbb22f7be50befa09f0d3a68a910"
// <auto-generated/>
#pragma warning disable 1591
[assembly: global::Microsoft.AspNetCore.Razor.Hosting.RazorCompiledItemAttribute(typeof(AspNetCore.Views_NhaTuyenDung_Edit), @"mvc.1.0.view", @"/Views/NhaTuyenDung/Edit.cshtml")]
namespace AspNetCore
{
    #line hidden
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using Microsoft.AspNetCore.Mvc.ViewFeatures;
#nullable restore
#line 1 "F:\COde\QLCUNL.WebUI\Views\_ViewImports.cshtml"
using QLCUNL.WebUI;

#line default
#line hidden
#nullable disable
#nullable restore
#line 2 "F:\COde\QLCUNL.WebUI\Views\_ViewImports.cshtml"
using QLCUNL.WebUI.Models;

#line default
#line hidden
#nullable disable
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"12316450da25dbb22f7be50befa09f0d3a68a910", @"/Views/NhaTuyenDung/Edit.cshtml")]
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"eb121bfd779b87ec9cf57d33affc75de17a3e4dc", @"/Views/_ViewImports.cshtml")]
    public class Views_NhaTuyenDung_Edit : global::Microsoft.AspNetCore.Mvc.Razor.RazorPage<QLCUNL.Models.NhaTuyenDung>
    {
        private static readonly global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute __tagHelperAttribute_0 = new global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute("id", new global::Microsoft.AspNetCore.Html.HtmlString("validate-menu"), global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
        private static readonly global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute __tagHelperAttribute_1 = new global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute("asp-action", "All", global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
        private static readonly global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute __tagHelperAttribute_2 = new global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute("src", "~/js/nha_tuyen_dung_edit.js", global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
        #line hidden
        #pragma warning disable 0649
        private global::Microsoft.AspNetCore.Razor.Runtime.TagHelpers.TagHelperExecutionContext __tagHelperExecutionContext;
        #pragma warning restore 0649
        private global::Microsoft.AspNetCore.Razor.Runtime.TagHelpers.TagHelperRunner __tagHelperRunner = new global::Microsoft.AspNetCore.Razor.Runtime.TagHelpers.TagHelperRunner();
        #pragma warning disable 0169
        private string __tagHelperStringValueBuffer;
        #pragma warning restore 0169
        private global::Microsoft.AspNetCore.Razor.Runtime.TagHelpers.TagHelperScopeManager __backed__tagHelperScopeManager = null;
        private global::Microsoft.AspNetCore.Razor.Runtime.TagHelpers.TagHelperScopeManager __tagHelperScopeManager
        {
            get
            {
                if (__backed__tagHelperScopeManager == null)
                {
                    __backed__tagHelperScopeManager = new global::Microsoft.AspNetCore.Razor.Runtime.TagHelpers.TagHelperScopeManager(StartTagHelperWritingScope, EndTagHelperWritingScope);
                }
                return __backed__tagHelperScopeManager;
            }
        }
        private global::Microsoft.AspNetCore.Mvc.TagHelpers.FormTagHelper __Microsoft_AspNetCore_Mvc_TagHelpers_FormTagHelper;
        private global::Microsoft.AspNetCore.Mvc.TagHelpers.RenderAtEndOfFormTagHelper __Microsoft_AspNetCore_Mvc_TagHelpers_RenderAtEndOfFormTagHelper;
        private global::Microsoft.AspNetCore.Mvc.TagHelpers.OptionTagHelper __Microsoft_AspNetCore_Mvc_TagHelpers_OptionTagHelper;
        private global::Microsoft.AspNetCore.Mvc.TagHelpers.AnchorTagHelper __Microsoft_AspNetCore_Mvc_TagHelpers_AnchorTagHelper;
        private global::Microsoft.AspNetCore.Mvc.Razor.TagHelpers.UrlResolutionTagHelper __Microsoft_AspNetCore_Mvc_Razor_TagHelpers_UrlResolutionTagHelper;
        private global::Microsoft.AspNetCore.Mvc.TagHelpers.ScriptTagHelper __Microsoft_AspNetCore_Mvc_TagHelpers_ScriptTagHelper;
        #pragma warning disable 1998
        public async override global::System.Threading.Tasks.Task ExecuteAsync()
        {
            WriteLiteral("\r\n");
#nullable restore
#line 3 "F:\COde\QLCUNL.WebUI\Views\NhaTuyenDung\Edit.cshtml"
  
    Layout = "~/Views/Shared/_Layout.cshtml";
    ViewData["Title"] = "Chỉnh sửa nhà tuyển dụng";

#line default
#line hidden
#nullable disable
            WriteLiteral(@"
<div class=""row"">
    <div class=""col-md-6"">
        <h3>Cập nhật nhà tuyển dụng</h3>
    </div>
    <div class=""col-md-6"">
        <div class=""form-group text-right"">
            <button class=""btn-size btn btn-primary"" onclick=""onUpdate(event)""><i class=""icon icon-add""></i>Cập nhật</button>
        </div>
    </div>
</div>
<hr />
");
            __tagHelperExecutionContext = __tagHelperScopeManager.Begin("form", global::Microsoft.AspNetCore.Razor.TagHelpers.TagMode.StartTagAndEndTag, "12316450da25dbb22f7be50befa09f0d3a68a9105316", async() => {
                WriteLiteral(@"
    <div class=""row"">
        <div class=""col-md-6"">
            <div class=""form-group"">
                <label class=""control-label"">Email nhà tuyển dụng</label>
                <input class=""form-control"" disabled id=""id_nha_tuyen_dung"" />
                <span class=""text-danger""></span>
            </div>
            <div class=""form-group"">
                <label class=""control-label"">JOB</label>
                <select class=""form-control"" id=""id_job"" name=""id_job"" onchange=""SearchNoteUvJob(1)"">                    
");
#nullable restore
#line 30 "F:\COde\QLCUNL.WebUI\Views\NhaTuyenDung\Edit.cshtml"
                     if (ViewBag.all_job != null)
                    {
                        foreach (var item in ViewBag.all_job)
                        {

#line default
#line hidden
#nullable disable
                WriteLiteral("                            ");
                __tagHelperExecutionContext = __tagHelperScopeManager.Begin("option", global::Microsoft.AspNetCore.Razor.TagHelpers.TagMode.StartTagAndEndTag, "12316450da25dbb22f7be50befa09f0d3a68a9106497", async() => {
#nullable restore
#line 34 "F:\COde\QLCUNL.WebUI\Views\NhaTuyenDung\Edit.cshtml"
                                                    Write(item.chuc_danh);

#line default
#line hidden
#nullable disable
                    WriteLiteral(" (");
#nullable restore
#line 34 "F:\COde\QLCUNL.WebUI\Views\NhaTuyenDung\Edit.cshtml"
                                                                     Write(item.id_auto);

#line default
#line hidden
#nullable disable
                    WriteLiteral(")");
                }
                );
                __Microsoft_AspNetCore_Mvc_TagHelpers_OptionTagHelper = CreateTagHelper<global::Microsoft.AspNetCore.Mvc.TagHelpers.OptionTagHelper>();
                __tagHelperExecutionContext.Add(__Microsoft_AspNetCore_Mvc_TagHelpers_OptionTagHelper);
                BeginWriteTagHelperAttribute();
#nullable restore
#line 34 "F:\COde\QLCUNL.WebUI\Views\NhaTuyenDung\Edit.cshtml"
                               WriteLiteral(item.id_job);

#line default
#line hidden
#nullable disable
                __tagHelperStringValueBuffer = EndWriteTagHelperAttribute();
                __Microsoft_AspNetCore_Mvc_TagHelpers_OptionTagHelper.Value = __tagHelperStringValueBuffer;
                __tagHelperExecutionContext.AddTagHelperAttribute("value", __Microsoft_AspNetCore_Mvc_TagHelpers_OptionTagHelper.Value, global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
                await __tagHelperRunner.RunAsync(__tagHelperExecutionContext);
                if (!__tagHelperExecutionContext.Output.IsContentModified)
                {
                    await __tagHelperExecutionContext.SetOutputContentAsync();
                }
                Write(__tagHelperExecutionContext.Output);
                __tagHelperExecutionContext = __tagHelperScopeManager.End();
                WriteLiteral("\r\n");
#nullable restore
#line 35 "F:\COde\QLCUNL.WebUI\Views\NhaTuyenDung\Edit.cshtml"
                        }
                    }

#line default
#line hidden
#nullable disable
                WriteLiteral(@"                </select>
                <span class=""text-danger""></span>
            </div>
            <div class=""form-group d-none"">
                <label class=""control-label"">User JOB</label>
                <input class=""form-control"" id=""id_user_job"" />
                <span class=""text-danger""></span>
            </div>
            
            
        </div>
        <div class=""col-md-6""> </div>
        <div class=""col-md-12"">
            <div class=""form-group"">
                <label class=""control-label"">Nội dung</label>
                <textarea rows=""8"" class=""form-control"" id=""noi_dung""></textarea>
                <span class=""text-danger""></span>
            </div>
        </div>
    </div>
");
            }
            );
            __Microsoft_AspNetCore_Mvc_TagHelpers_FormTagHelper = CreateTagHelper<global::Microsoft.AspNetCore.Mvc.TagHelpers.FormTagHelper>();
            __tagHelperExecutionContext.Add(__Microsoft_AspNetCore_Mvc_TagHelpers_FormTagHelper);
            __Microsoft_AspNetCore_Mvc_TagHelpers_RenderAtEndOfFormTagHelper = CreateTagHelper<global::Microsoft.AspNetCore.Mvc.TagHelpers.RenderAtEndOfFormTagHelper>();
            __tagHelperExecutionContext.Add(__Microsoft_AspNetCore_Mvc_TagHelpers_RenderAtEndOfFormTagHelper);
            __tagHelperExecutionContext.AddHtmlAttribute(__tagHelperAttribute_0);
            await __tagHelperRunner.RunAsync(__tagHelperExecutionContext);
            if (!__tagHelperExecutionContext.Output.IsContentModified)
            {
                await __tagHelperExecutionContext.SetOutputContentAsync();
            }
            Write(__tagHelperExecutionContext.Output);
            __tagHelperExecutionContext = __tagHelperScopeManager.End();
            WriteLiteral("\r\n<div class=\"box col-md-4 d-none\">\r\n    <div class=\"form-group search-by-id \">\r\n        <input placeholder=\"ID User\" id=\"id_user\" class=\"form-control\" />\r\n        <input placeholder=\"ID Job\" id=\"id_job1\" class=\"form-control\"");
            BeginWriteAttribute("value", " value=\"", 2342, "\"", 2350, 0);
            EndWriteAttribute();
            WriteLiteral(" />\r\n        <input placeholder=\"ID Ứng Viên\" id=\"id_ung_vien\" class=\"form-control\"");
            BeginWriteAttribute("value", " value=\"", 2434, "\"", 2442, 0);
            EndWriteAttribute();
            WriteLiteral(@" />
    </div>
    <div class=""form-group search-by-datetime"">
        <input placeholder=""Ngày giờ phỏng vấn (từ)"" id=""ngay_gio_phong_van_from"" class=""form-control"" />
        <input placeholder=""Ngày giờ phỏng vấn (đến)"" id=""ngay_gio_phong_van_to"" class=""form-control"" />
    </div>
    <div class=""form-group search-by-daywork"">
        <input placeholder=""Ngày đi làm (từ)"" id=""ngay_di_lam_from"" class=""form-control"" />
        <input placeholder=""Ngày đi làm (đến)"" id=""ngay_di_lam_to"" class=""form-control"" />
    </div>
    <div class=""form-group search-by-salary"">
        <input placeholder=""Lương thử việc (từ)"" id=""luong_thu_viec_from"" class=""form-control"" />
        <input placeholder=""Lương thử việc (đến)"" id=""luong_thu_viec_to"" class=""form-control"" />
    </div>
    <div class=""form-group search-by-salarymain"">
        <input placeholder=""Lương chính thức (từ)"" id=""luong_chinh_thuc_from"" class=""form-control"" />
        <input placeholder=""Lương chính thức (đến)"" id=""luong_chinh_thuc_to"" c");
            WriteLiteral(@"lass=""form-control"" />
    </div>
</div>
<div class=""row"">
    <div class=""col-md-12 border-info"">
        <h3>Note ứng viên đã chọn: <span id=""count-selected""></span></h3>
        <div id=""box-uv"">
            <div class=""border-info"" id=""ung-vien-selected"">
            </div>
        </div>
    </div>
    <div class=""col-sm-12 table-responsive"">
        <h3>Chọn Note ứng viên JOB</h3>
        <input placeholder=""Tìm kiếm"" id=""term"" name=""term"" class=""form-control d-none""");
            BeginWriteAttribute("value", " value=\"", 3957, "\"", 4023, 1);
#nullable restore
#line 91 "F:\COde\QLCUNL.WebUI\Views\NhaTuyenDung\Edit.cshtml"
WriteAttributeValue("", 3965, Html.Raw(System.Web.HttpUtility.HtmlDecode(ViewBag.term)), 3965, 58, false);

#line default
#line hidden
#nullable disable
            EndWriteAttribute();
            WriteLiteral(@" />
        <div class=""totalRecs text-right""></div>
        <table id=""table_data"" class=""display table  table-striped"" style=""width:100%"">
            <thead class=""thead-light"">
                <tr>
                    <th>Ứng viên</th>
                    <th>
                        Trạng Thái
                    </th>
                    <th>
                        Giờ phỏng vấn
                    </th>
                    <th>
                        Đi làm
                    </th>
                    <th>
                        Lương thử / chính
                    </th>
");
            WriteLiteral(@"
                    <th>Chọn ứng viên</th>

                </tr>
            </thead>
            <tbody id=""div_data"">
            </tbody>
        </table>
    </div>
    <div class=""form-group"">
        <button class=""btn-size btn btn-primary"" onclick=""onUpdate(event)""><i class=""icon icon-add""></i>Cập nhật</button>
    </div>
    <div style=""text-align: center; width: 100%; display: inline-flex; justify-content: center"">
        <div class=""d-flex text-center data-container"">
            <div class=""paging"">
            </div>
        </div>
    </div>

</div>
<div>
    ");
            __tagHelperExecutionContext = __tagHelperScopeManager.Begin("a", global::Microsoft.AspNetCore.Razor.TagHelpers.TagMode.StartTagAndEndTag, "12316450da25dbb22f7be50befa09f0d3a68a91014572", async() => {
                WriteLiteral("Quay lại danh sách");
            }
            );
            __Microsoft_AspNetCore_Mvc_TagHelpers_AnchorTagHelper = CreateTagHelper<global::Microsoft.AspNetCore.Mvc.TagHelpers.AnchorTagHelper>();
            __tagHelperExecutionContext.Add(__Microsoft_AspNetCore_Mvc_TagHelpers_AnchorTagHelper);
            __Microsoft_AspNetCore_Mvc_TagHelpers_AnchorTagHelper.Action = (string)__tagHelperAttribute_1.Value;
            __tagHelperExecutionContext.AddTagHelperAttribute(__tagHelperAttribute_1);
            await __tagHelperRunner.RunAsync(__tagHelperExecutionContext);
            if (!__tagHelperExecutionContext.Output.IsContentModified)
            {
                await __tagHelperExecutionContext.SetOutputContentAsync();
            }
            Write(__tagHelperExecutionContext.Output);
            __tagHelperExecutionContext = __tagHelperScopeManager.End();
            WriteLiteral("\r\n</div>\r\n\r\n");
            DefineSection("Scripts", async() => {
                WriteLiteral("\r\n");
#nullable restore
#line 137 "F:\COde\QLCUNL.WebUI\Views\NhaTuyenDung\Edit.cshtml"
      await Html.RenderPartialAsync("_ValidationScriptsPartial");

#line default
#line hidden
#nullable disable
            }
            );
            WriteLiteral("\r\n");
            __tagHelperExecutionContext = __tagHelperScopeManager.Begin("script", global::Microsoft.AspNetCore.Razor.TagHelpers.TagMode.StartTagAndEndTag, "12316450da25dbb22f7be50befa09f0d3a68a91016111", async() => {
            }
            );
            __Microsoft_AspNetCore_Mvc_Razor_TagHelpers_UrlResolutionTagHelper = CreateTagHelper<global::Microsoft.AspNetCore.Mvc.Razor.TagHelpers.UrlResolutionTagHelper>();
            __tagHelperExecutionContext.Add(__Microsoft_AspNetCore_Mvc_Razor_TagHelpers_UrlResolutionTagHelper);
            __Microsoft_AspNetCore_Mvc_TagHelpers_ScriptTagHelper = CreateTagHelper<global::Microsoft.AspNetCore.Mvc.TagHelpers.ScriptTagHelper>();
            __tagHelperExecutionContext.Add(__Microsoft_AspNetCore_Mvc_TagHelpers_ScriptTagHelper);
#nullable restore
#line 140 "F:\COde\QLCUNL.WebUI\Views\NhaTuyenDung\Edit.cshtml"
__Microsoft_AspNetCore_Mvc_TagHelpers_ScriptTagHelper.AppendVersion = true;

#line default
#line hidden
#nullable disable
            __tagHelperExecutionContext.AddTagHelperAttribute("asp-append-version", __Microsoft_AspNetCore_Mvc_TagHelpers_ScriptTagHelper.AppendVersion, global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
            __Microsoft_AspNetCore_Mvc_TagHelpers_ScriptTagHelper.Src = (string)__tagHelperAttribute_2.Value;
            __tagHelperExecutionContext.AddTagHelperAttribute(__tagHelperAttribute_2);
            await __tagHelperRunner.RunAsync(__tagHelperExecutionContext);
            if (!__tagHelperExecutionContext.Output.IsContentModified)
            {
                await __tagHelperExecutionContext.SetOutputContentAsync();
            }
            Write(__tagHelperExecutionContext.Output);
            __tagHelperExecutionContext = __tagHelperScopeManager.End();
            WriteLiteral("\r\n<script>\r\n    let id_ntd = \"");
#nullable restore
#line 142 "F:\COde\QLCUNL.WebUI\Views\NhaTuyenDung\Edit.cshtml"
             Write(ViewBag.id);

#line default
#line hidden
#nullable disable
            WriteLiteral("\";    \r\n    $(`[name=\'term\']`).keypress(function (event) {\r\n        if (event.keyCode == 13) {\r\n            SearchNoteUvJob(1);\r\n        }\r\n    });\r\n    SearchNoteUvJob(1);\r\n</script>");
        }
        #pragma warning restore 1998
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.ViewFeatures.IModelExpressionProvider ModelExpressionProvider { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.IUrlHelper Url { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.IViewComponentHelper Component { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.Rendering.IJsonHelper Json { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.Rendering.IHtmlHelper<QLCUNL.Models.NhaTuyenDung> Html { get; private set; }
    }
}
#pragma warning restore 1591
