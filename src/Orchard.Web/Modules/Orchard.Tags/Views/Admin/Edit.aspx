﻿<%@ Page Language="C#" Inherits="Orchard.Mvc.ViewPage<TagsAdminEditViewModel>" %>
<%@ Import Namespace="Orchard.Tags.ViewModels"%>
<h1><%: Html.TitleForPage(T("Edit a Tag").ToString()) %></h1>
<% using(Html.BeginFormAntiForgeryPost()) { %>
    <%: Html.ValidationSummary() %>
    <fieldset>
        <%: Html.HiddenFor(m => m.Id) %>
        <%: Html.LabelFor(m => m.TagName) %>
        <%: Html.TextBoxFor(m => m.TagName, new { @class = "text" })%> 
    </fieldset>
    <fieldset>
        <input class="button primaryAction" type="submit" value="<%:T("Save") %>" />
    </fieldset>
<% } %>