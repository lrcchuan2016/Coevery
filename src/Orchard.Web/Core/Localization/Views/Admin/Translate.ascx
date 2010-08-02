<%@ Control Language="C#" Inherits="Orchard.Mvc.ViewUserControl<AddLocalizationViewModel>" %>
<%@ Import Namespace="Orchard.Core.Localization.ViewModels" %><%
Model.Content.Zones.AddRenderPartial("primary:before", "CultureSelection", Model); %>
<h1><%:Html.TitleForPage(T("Translate Content").ToString()) %></h1>
<% using (Html.BeginFormAntiForgeryPost()) { %>
<%:Html.ValidationSummary() %>
<%:Html.EditorForItem(m=>m.Content) %>
<%} %>
<% using (this.Capture("end-of-page-scripts")) { %>
<script type="text/javascript">
    (function ($) {
        // grab the slug input
        var slug = $("#Routable_Slug");
        if (slug) {
            // grab the current culture
            var culture = $("#SelectedCulture");
            var currentCulture = culture.val();
            // when the culture is changed update the slug
            culture.change(function () {
                var slugValue = slug.val();
                var newCulture = $(this).val();
                if (slugValue && slugValue.match(currentCulture + "$")) {
                    slug.val(slugValue.replace(new RegExp(currentCulture + "$", "i"), newCulture));
                    currentCulture = newCulture;
                }
            });
        }
    })(jQuery);</script>
<% } %>