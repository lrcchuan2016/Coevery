﻿<%@ Control Language="C#" Inherits="Orchard.Mvc.ViewUserControl<Orchard.Core.Localization.ViewModels.ContentLocalizationsViewModel>" %>
<%
Html.RegisterStyle("base.css");
if (Model.Localizations.Count() > 0) { %>
<div class="content-localization">
    <div class="content-localizations">
        <h4><%:T("Translations:") %></h4>
        <%:Html.UnorderedList(Model.Localizations, (c, i) => Html.ItemDisplayLink(c.Culture.Culture, c), "localizations") %>
    </div>
</div><%
} %>