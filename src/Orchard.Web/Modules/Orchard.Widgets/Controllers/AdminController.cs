﻿using System;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Linq;
using Orchard.ContentManagement;
using Orchard.Core.Contents.Controllers;
using Orchard.DisplayManagement;
using Orchard.Localization;
using Orchard.Logging;
using Orchard.Mvc.Extensions;
using Orchard.UI.Admin;
using Orchard.UI.Notify;
using Orchard.Utility.Extensions;
using Orchard.Widgets.Models;
using Orchard.Widgets.Services;

namespace Orchard.Widgets.Controllers {

    [ValidateInput(false), Admin]
    public class AdminController : Controller, IUpdateModel {

        private const string NotAuthorizedManageWidgetsLabel = "Not authorized to manage widgets";

        private readonly IWidgetsService _widgetsService;

        public AdminController(
            IOrchardServices services,
            IWidgetsService widgetsService,
            IShapeFactory shapeFactory) {

            Services = services;
            _widgetsService = widgetsService;

            T = NullLocalizer.Instance;
            Logger = NullLogger.Instance;
            Shape = shapeFactory;
        }

        private IOrchardServices Services { get; set; }
        public Localizer T { get; set; }
        public ILogger Logger { get; set; }
        dynamic Shape { get; set; }

        public ActionResult Index(int? layerId) {
            IEnumerable<LayerPart> layers = _widgetsService.GetLayers();

            LayerPart currentLayer;
            IEnumerable<WidgetPart> widgets;

            if (layers.Count() > 0) {
                currentLayer = layerId == null ?
                               layers.First() :
                               layers.FirstOrDefault(layer => layer.Id == layerId);

                if (currentLayer == null &&
                    layerId != null) {
                    // Incorrect layer id passed
                    Services.Notifier.Error(T("Layer not found: {0}", layerId));
                    return RedirectToAction("Index");
                }

                widgets = _widgetsService.GetWidgets();
            }
            else {
                currentLayer = null;
                widgets = new List<WidgetPart>();
            }

            //WidgetsIndexViewModel widgetsIndexViewModel = new WidgetsIndexViewModel {
            //    WidgetTypes = _widgetsService.GetWidgetTypes(),
            //    Layers = layers,
            //    Zones = _widgetsService.GetZones(),
            //    CurrentLayer = currentLayer,
            //    CurrentLayerWidgets = currentLayerWidgets
            //};

            //return View(widgetsIndexViewModel);

            dynamic viewModel = Shape.ViewModel()
                .CurrentLayer(currentLayer)
                .Layers(layers)
                .Widgets(widgets)
                .Zones(_widgetsService.GetZones());

            // Casting to avoid invalid (under medium trust) reflection over the protected View method and force a static invocation.
            return View((object)viewModel);
        }

        [HttpPost, ActionName("Index")]
        public ActionResult IndexWidgetPOST(int? id, string returnUrl) {
            const string moveDownString = "submit.MoveDown.";
            const string moveUpString = "submit.MoveUp.";

            if (!Services.Authorizer.Authorize(Permissions.ManageWidgets, T(NotAuthorizedManageWidgetsLabel)))
                return new HttpUnauthorizedResult();

            try {
                string moveDownAction = HttpContext.Request.Form.AllKeys.FirstOrDefault(key => key.StartsWith(moveDownString));
                if (moveDownAction != null) {
                    moveDownAction = moveDownAction.Substring(moveDownString.Length, moveDownAction.IndexOf(".", moveDownString.Length) - moveDownString.Length);
                    _widgetsService.MoveWidgetDown(int.Parse(moveDownAction));
                }
                else {
                    string moveUpAction = HttpContext.Request.Form.AllKeys.FirstOrDefault(key => key.StartsWith(moveUpString));
                    if (moveUpAction != null) {
                        moveUpAction = moveUpAction.Substring(moveUpString.Length, moveUpAction.IndexOf(".", moveUpString.Length) - moveUpString.Length);
                        _widgetsService.MoveWidgetUp(int.Parse(moveUpAction));
                    }
                }
            } catch (Exception exception) {
                this.Error(exception, T("Moving widget failed: {0}", exception.Message), Logger, Services.Notifier);
            }

            return this.RedirectLocal(returnUrl, () => RedirectToAction("Index"));
        }

        public ActionResult AddWidget(int layerId, string widgetType) {
            if (!Services.Authorizer.Authorize(Permissions.ManageWidgets, T(NotAuthorizedManageWidgetsLabel)))
                return new HttpUnauthorizedResult();

            try {
                WidgetPart widgetPart = Services.ContentManager.New<WidgetPart>(widgetType);
                if (widgetPart == null)
                    return HttpNotFound();

                int widgetPosition = _widgetsService.GetWidgets(layerId).Count() + 1;
                widgetPart.Position = widgetPosition.ToString();

                widgetPart.LayerPart = _widgetsService.GetLayer(layerId);
                dynamic model = Services.ContentManager.BuildEditor(widgetPart);
                // Casting to avoid invalid (under medium trust) reflection over the protected View method and force a static invocation.
                return View((object)model);
            }
            catch (Exception exception) {
                this.Error(exception, T("Creating widget failed: {0}", exception.Message), Logger, Services.Notifier);

                return RedirectToAction("Index", "Admin", new { id = layerId });
            }
        }

        [HttpPost, ActionName("AddWidget")]
        public ActionResult AddWidgetPOST(int layerId, string widgetType) {
            if (!Services.Authorizer.Authorize(Permissions.ManageWidgets, T(NotAuthorizedManageWidgetsLabel)))
                return new HttpUnauthorizedResult();

            try {
                WidgetPart widgetPart = _widgetsService.CreateWidget(layerId, widgetType, "", "", "");
                if (widgetPart == null)
                    return HttpNotFound();

                var model = Services.ContentManager.UpdateEditor(widgetPart, this);
                if (!ModelState.IsValid) {
                    Services.TransactionManager.Cancel();
                    // Casting to avoid invalid (under medium trust) reflection over the protected View method and force a static invocation.
                    return View((object)model);
                }

                Services.Notifier.Information(T("Your {0} has been created.", widgetPart.TypeDefinition.DisplayName));
            } catch (Exception exception) {
                this.Error(exception, T("Creating widget failed: {0}", exception.Message), Logger, Services.Notifier);
            }

            return RedirectToAction("Index", "Admin", new { id = layerId });
        }

        public ActionResult AddLayer() {
            if (!Services.Authorizer.Authorize(Permissions.ManageWidgets, T(NotAuthorizedManageWidgetsLabel)))
                return new HttpUnauthorizedResult();

            try {
                LayerPart layerPart = Services.ContentManager.New<LayerPart>("Layer");
                if (layerPart == null)
                    return HttpNotFound();

                dynamic model = Services.ContentManager.BuildEditor(layerPart);
                // Casting to avoid invalid (under medium trust) reflection over the protected View method and force a static invocation.
                return View((object)model);
            } catch (Exception exception) {
                this.Error(exception, T("Creating layer failed: {0}", exception.Message), Logger, Services.Notifier);
                return RedirectToAction("Index");
            }
        }

        [HttpPost, ActionName("AddLayer")]
        public ActionResult AddLayerPOST() {
            if (!Services.Authorizer.Authorize(Permissions.ManageWidgets, T(NotAuthorizedManageWidgetsLabel)))
                return new HttpUnauthorizedResult();

            try {
                LayerPart layerPart = _widgetsService.CreateLayer("", "", "");
                if (layerPart == null)
                    return HttpNotFound();

                var model = Services.ContentManager.UpdateEditor(layerPart, this);

                if (!ModelState.IsValid) {
                    Services.TransactionManager.Cancel();
                    // Casting to avoid invalid (under medium trust) reflection over the protected View method and force a static invocation.
                    return View((object)model);
                }

                Services.Notifier.Information(T("Your {0} has been created.", layerPart.TypeDefinition.DisplayName));
                return RedirectToAction("Index", "Admin", new { id = layerPart.Id });
            } catch (Exception exception) {
                this.Error(exception, T("Creating layer failed: {0}", exception.Message), Logger, Services.Notifier);
                return RedirectToAction("Index");
            }
        }

        public ActionResult EditLayer(int id) {
            if (!Services.Authorizer.Authorize(Permissions.ManageWidgets, T(NotAuthorizedManageWidgetsLabel)))
                return new HttpUnauthorizedResult();

            try {
                LayerPart layerPart = _widgetsService.GetLayer(id);
                if (layerPart == null) {
                    return HttpNotFound();
                }

                dynamic model = Services.ContentManager.BuildEditor(layerPart);
                // Casting to avoid invalid (under medium trust) reflection over the protected View method and force a static invocation.
                return View((object)model);
            } catch (Exception exception) {
                this.Error(exception, T("Editing layer failed: {0}", exception.Message), Logger, Services.Notifier);

                return RedirectToAction("Index", "Admin", new { id });
            }
        }

        [HttpPost, ActionName("EditLayer")]
        [FormValueRequired("submit.Save")]
        public ActionResult EditLayerSavePOST(int id, string returnUrl) {
            if (!Services.Authorizer.Authorize(Permissions.ManageWidgets, T(NotAuthorizedManageWidgetsLabel)))
                return new HttpUnauthorizedResult();

            try {
                LayerPart layerPart = _widgetsService.GetLayer(id);
                if (layerPart == null)
                    return HttpNotFound();

                var model = Services.ContentManager.UpdateEditor(layerPart, this);

                if (!ModelState.IsValid) {
                    Services.TransactionManager.Cancel();
                    // Casting to avoid invalid (under medium trust) reflection over the protected View method and force a static invocation.
                    return View((object)model);
                }

                Services.Notifier.Information(T("Your {0} has been saved.", layerPart.TypeDefinition.DisplayName));
            } catch (Exception exception) {
                this.Error(exception, T("Editing layer failed: {0}", exception.Message), Logger, Services.Notifier);
            }

            return this.RedirectLocal(returnUrl, () => RedirectToAction("Index"));
        }

        [HttpPost, ActionName("EditLayer")]
        [FormValueRequired("submit.Delete")]
        public ActionResult EditLayerDeletePOST(int id) {
            if (!Services.Authorizer.Authorize(Permissions.ManageWidgets, T(NotAuthorizedManageWidgetsLabel)))
                return new HttpUnauthorizedResult();

            try {
                _widgetsService.DeleteLayer(id);
                Services.Notifier.Information(T("Layer was successfully deleted"));
            } catch (Exception exception) {
                this.Error(exception, T("Removing Layer failed: {0}", exception.Message), Logger, Services.Notifier);
            }

            return RedirectToAction("Index");
        }

        public ActionResult EditWidget(int id) {
            if (!Services.Authorizer.Authorize(Permissions.ManageWidgets, T(NotAuthorizedManageWidgetsLabel)))
                return new HttpUnauthorizedResult();

            WidgetPart widgetPart = null;
            try {
                widgetPart = _widgetsService.GetWidget(id);
                if (widgetPart == null) {
                    Services.Notifier.Error(T("Widget not found: {0}", id));
                    return RedirectToAction("Index");
                }

                dynamic model = Services.ContentManager.BuildEditor(widgetPart);
                // Casting to avoid invalid (under medium trust) reflection over the protected View method and force a static invocation.
                return View((object)model);
            }
            catch (Exception exception) {
                this.Error(exception, T("Editing widget failed: {0}", exception.Message), Logger, Services.Notifier);

                if (widgetPart != null)
                    return RedirectToAction("Index", "Admin", new { id = widgetPart.LayerPart.Id });

                return RedirectToAction("Index");
            }
        }

        [HttpPost, ActionName("EditWidget")]
        [FormValueRequired("submit.Save")]
        public ActionResult EditWidgetSavePOST(int id, int layerId) {
            if (!Services.Authorizer.Authorize(Permissions.ManageWidgets, T(NotAuthorizedManageWidgetsLabel)))
                return new HttpUnauthorizedResult();

            WidgetPart widgetPart = null;
            try {
                widgetPart = _widgetsService.GetWidget(id);
                if (widgetPart == null)
                    return HttpNotFound();

                widgetPart.LayerPart = _widgetsService.GetLayer(layerId);
                var model = Services.ContentManager.UpdateEditor(widgetPart, this);
                if (!ModelState.IsValid) {
                    Services.TransactionManager.Cancel();
                    // Casting to avoid invalid (under medium trust) reflection over the protected View method and force a static invocation.
                    return View((object)model);
                }

                Services.Notifier.Information(T("Your {0} has been saved.", widgetPart.TypeDefinition.DisplayName));
            } catch (Exception exception) {
                this.Error(exception, T("Editing widget failed: {0}", exception.Message), Logger, Services.Notifier);
            }

            return widgetPart != null ?
                RedirectToAction("Index", "Admin", new { id = widgetPart.LayerPart.Id }) :
                RedirectToAction("Index");
        }

        [HttpPost, ActionName("EditWidget")]
        [FormValueRequired("submit.Delete")]
        public ActionResult EditWidgetDeletePOST(int id) {
            if (!Services.Authorizer.Authorize(Permissions.ManageWidgets, T(NotAuthorizedManageWidgetsLabel)))
                return new HttpUnauthorizedResult();

            WidgetPart widgetPart = null;
            try {
                widgetPart = _widgetsService.GetWidget(id);
                if (widgetPart == null)
                    return HttpNotFound();

                _widgetsService.DeleteWidget(widgetPart.Id);
                Services.Notifier.Information(T("Widget was successfully deleted"));
            } catch (Exception exception) {
                this.Error(exception, T("Removing Widget failed: {0}", exception.Message), Logger, Services.Notifier);
            }

            return widgetPart != null ?
                RedirectToAction("Index", "Admin", new { id = widgetPart.LayerPart.Id }) : 
                RedirectToAction("Index");
        }

        bool IUpdateModel.TryUpdateModel<TModel>(TModel model, string prefix, string[] includeProperties, string[] excludeProperties) {
            return base.TryUpdateModel(model, prefix, includeProperties, excludeProperties);
        }

        void IUpdateModel.AddModelError(string key, LocalizedString errorMessage) {
            ModelState.AddModelError(key, errorMessage.ToString());
        }
    }
}