#pragma checksum "F:\COde\QLCUNL.WebUI\Views\TinhThanh\All.cshtml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "86fea42578c85579c1a7da960d90918bfd1913d8"
// <auto-generated/>
#pragma warning disable 1591
[assembly: global::Microsoft.AspNetCore.Razor.Hosting.RazorCompiledItemAttribute(typeof(AspNetCore.Views_TinhThanh_All), @"mvc.1.0.view", @"/Views/TinhThanh/All.cshtml")]
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
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"86fea42578c85579c1a7da960d90918bfd1913d8", @"/Views/TinhThanh/All.cshtml")]
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"eb121bfd779b87ec9cf57d33affc75de17a3e4dc", @"/Views/_ViewImports.cshtml")]
    public class Views_TinhThanh_All : global::Microsoft.AspNetCore.Mvc.Razor.RazorPage<IEnumerable<QLCUNL.Models.User>>
    {
        private static readonly global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute __tagHelperAttribute_0 = new global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute("src", "~/js/user.js", global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
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
        private global::Microsoft.AspNetCore.Mvc.Razor.TagHelpers.UrlResolutionTagHelper __Microsoft_AspNetCore_Mvc_Razor_TagHelpers_UrlResolutionTagHelper;
        private global::Microsoft.AspNetCore.Mvc.TagHelpers.ScriptTagHelper __Microsoft_AspNetCore_Mvc_TagHelpers_ScriptTagHelper;
        #pragma warning disable 1998
        public async override global::System.Threading.Tasks.Task ExecuteAsync()
        {
            WriteLiteral("\r\n");
#nullable restore
#line 3 "F:\COde\QLCUNL.WebUI\Views\TinhThanh\All.cshtml"
  
    ViewData["Title"] = "Tất cả User";

#line default
#line hidden
#nullable disable
            WriteLiteral(@"
<h1>Tất cả danh mục</h1>

<table class=""table table-striped"">
    <thead>
        <tr>

            <th>
                Ngày tạo
            </th>
            <th>
                Tên danh mục
            </th>
            <th>
                Danh mục cha
            </th>
            <th>
                Danh sách danh mục cha
            </th>
            <th></th>
        </tr>
    </thead>
    <tbody id=""all_user"">
");
#nullable restore
#line 29 "F:\COde\QLCUNL.WebUI\Views\TinhThanh\All.cshtml"
         foreach (var item in Model)
        {

#line default
#line hidden
#nullable disable
            WriteLiteral("            <tr>\r\n\r\n                <td>\r\n                    ");
#nullable restore
#line 34 "F:\COde\QLCUNL.WebUI\Views\TinhThanh\All.cshtml"
               Write(Html.DisplayFor(modelItem => item.user_name));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n                </td>\r\n                <td>\r\n                    ");
#nullable restore
#line 37 "F:\COde\QLCUNL.WebUI\Views\TinhThanh\All.cshtml"
               Write(Html.DisplayFor(modelItem => item.full_name));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n                </td>\r\n                <td>\r\n                    ");
#nullable restore
#line 40 "F:\COde\QLCUNL.WebUI\Views\TinhThanh\All.cshtml"
               Write(Html.DisplayFor(modelItem => item.id_team));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n                </td>\r\n                \r\n                <td>\r\n                    <a");
            BeginWriteAttribute("href", " href=\"", 1003, "\"", 1031, 2);
            WriteAttributeValue("", 1010, "edit?id=", 1010, 8, true);
#nullable restore
#line 44 "F:\COde\QLCUNL.WebUI\Views\TinhThanh\All.cshtml"
WriteAttributeValue("", 1018, item.id_user, 1018, 13, false);

#line default
#line hidden
#nullable disable
            EndWriteAttribute();
            WriteLiteral(">Edit</a> |\r\n                    <a");
            BeginWriteAttribute("href", " href=\"", 1067, "\"", 1097, 2);
            WriteAttributeValue("", 1074, "detail?id=", 1074, 10, true);
#nullable restore
#line 45 "F:\COde\QLCUNL.WebUI\Views\TinhThanh\All.cshtml"
WriteAttributeValue("", 1084, item.id_user, 1084, 13, false);

#line default
#line hidden
#nullable disable
            EndWriteAttribute();
            WriteLiteral(">Detail</a> |\r\n                    ");
#nullable restore
#line 46 "F:\COde\QLCUNL.WebUI\Views\TinhThanh\All.cshtml"
               Write(Html.ActionLink("Delete", "category/delete", new {/* id=item._id */}));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n                </td>\r\n            </tr>\r\n");
#nullable restore
#line 49 "F:\COde\QLCUNL.WebUI\Views\TinhThanh\All.cshtml"
        }

#line default
#line hidden
#nullable disable
            WriteLiteral("    </tbody>\r\n\r\n</table>\r\n\r\n\r\n\r\n");
            __tagHelperExecutionContext = __tagHelperScopeManager.Begin("script", global::Microsoft.AspNetCore.Razor.TagHelpers.TagMode.StartTagAndEndTag, "86fea42578c85579c1a7da960d90918bfd1913d86725", async() => {
            }
            );
            __Microsoft_AspNetCore_Mvc_Razor_TagHelpers_UrlResolutionTagHelper = CreateTagHelper<global::Microsoft.AspNetCore.Mvc.Razor.TagHelpers.UrlResolutionTagHelper>();
            __tagHelperExecutionContext.Add(__Microsoft_AspNetCore_Mvc_Razor_TagHelpers_UrlResolutionTagHelper);
            __Microsoft_AspNetCore_Mvc_TagHelpers_ScriptTagHelper = CreateTagHelper<global::Microsoft.AspNetCore.Mvc.TagHelpers.ScriptTagHelper>();
            __tagHelperExecutionContext.Add(__Microsoft_AspNetCore_Mvc_TagHelpers_ScriptTagHelper);
#nullable restore
#line 56 "F:\COde\QLCUNL.WebUI\Views\TinhThanh\All.cshtml"
__Microsoft_AspNetCore_Mvc_TagHelpers_ScriptTagHelper.AppendVersion = true;

#line default
#line hidden
#nullable disable
            __tagHelperExecutionContext.AddTagHelperAttribute("asp-append-version", __Microsoft_AspNetCore_Mvc_TagHelpers_ScriptTagHelper.AppendVersion, global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
            __Microsoft_AspNetCore_Mvc_TagHelpers_ScriptTagHelper.Src = (string)__tagHelperAttribute_0.Value;
            __tagHelperExecutionContext.AddTagHelperAttribute(__tagHelperAttribute_0);
            await __tagHelperRunner.RunAsync(__tagHelperExecutionContext);
            if (!__tagHelperExecutionContext.Output.IsContentModified)
            {
                await __tagHelperExecutionContext.SetOutputContentAsync();
            }
            Write(__tagHelperExecutionContext.Output);
            __tagHelperExecutionContext = __tagHelperScopeManager.End();
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
        public global::Microsoft.AspNetCore.Mvc.Rendering.IHtmlHelper<IEnumerable<QLCUNL.Models.User>> Html { get; private set; }
    }
}
#pragma warning restore 1591
