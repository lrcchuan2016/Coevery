﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Coevery.Core.ClientRoute;
using Coevery.Core.Models;
using Coevery.Core.Services;
using Newtonsoft.Json.Linq;
using Orchard;
using Orchard.Environment.Extensions.Models;

namespace Coevery.Perspectives.Services
{
    public class ClientRouteProvider : IClientRouteProvider
    {
        public virtual Feature Feature { get; set; }

        public void Discover(ClientRouteBuilder builder)
        {
            builder.Create("PerspectiveList",
                           Feature,
                           route => route
                                        .Url("/Perspectives")
                                        .TemplateUrl("'SystemAdmin/Perspectives/List'")
                                        .Controller("PerspectiveListCtrl")
                                        .Dependencies("controllers/listcontroller"));

            builder.Create("PerspectiveCreate",
                           Feature,
                           route => route
                                        .Url("/Perspectives/Create")
                                        .TemplateUrl("'SystemAdmin/Perspectives/Create'")
                                        .Controller("PerspectiveEditCtrl")
                                        .Dependencies("controllers/editcontroller"));

            builder.Create("PerspectiveEdit",
                           Feature,
                           route => route
                                        .Url("/Perspectives/{Id:[0-9a-zA-Z]+}/Edit")
                                        .TemplateProvider(@"['$http', '$stateParams', function ($http, $stateParams) {
                                                var url = 'SystemAdmin/Perspectives/Edit/' + $stateParams.Id; 
                                                return $http.get(url).then(function(response) { return response.data; });
                                          }]")
                                        .Controller("PerspectiveEditCtrl")
                                        .Dependencies("controllers/editcontroller")
                );

            builder.Create("PerspectiveDetail",
                           Feature,
                           route => route
                                        .Url("/Perspectives/{Id:[0-9a-zA-Z]+}")
                                        .TemplateProvider(@"['$http', '$stateParams', function ($http, $stateParams) {
                                                var url = 'SystemAdmin/Perspectives/Detail/' + $stateParams.Id; 
                                                return $http.get(url).then(function(response) { return response.data; });
                                          }]")
                                        .Controller("PerspectiveDetailCtrl")
                                        .Dependencies("controllers/detailcontroller")
                );

            builder.Create("EditNavigationItem",
                           Feature,
                           route => route
                                        .Url("/Perspectives/{Id:[0-9a-zA-Z]+}/Navigation/{NId:[0-9a-zA-Z]+}")
                                        .TemplateProvider(@"['$http', '$stateParams', function ($http, $stateParams) {
                                                var url = 'SystemAdmin/Perspectives/EditNavigationItem/' + $stateParams.NId; 
                                                return $http.get(url).then(function(response) { return response.data; });
                                          }]")
                                        .Controller("NavigationItemEditCtrl")
                                        .Dependencies("controllers/navigationitemeditcontroller")
                );

            builder.Create("CreateNavigationItem",
                           Feature,
                           route => route
                                        .Url("/Perspectives/{Id:[0-9a-zA-Z]+}/Navigation/Create")
                                        .TemplateUrl("function(params) { return 'SystemAdmin/Perspectives/CreateNavigationItem/' + params.Id;}")
                                        .Controller("NavigationItemEditCtrl")
                                        .Dependencies("controllers/navigationitemeditcontroller")
                );

        }
    }
}