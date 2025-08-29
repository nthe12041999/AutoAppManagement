using AutoAppManagement.Models.Components;
using Microsoft.AspNetCore.Mvc;

namespace AutoAppManagement.WebApp.ViewComponents
{
    /// <summary>
    /// ViewComponent cho form controls
    /// </summary>
    public class FormControlViewComponent : ViewComponent
    {
        /// <summary>
        /// Invoke ViewComponent
        /// </summary>
        /// <param name="model">Form control model</param>
        /// <returns></returns>
        public IViewComponentResult Invoke(FormControlModel model)
        {
            return View(model);
        }

        /// <summary>
        /// Invoke với parameters
        /// </summary>
        /// <param name="id">Control ID</param>
        /// <param name="name">Control name</param>
        /// <param name="label">Label</param>
        /// <param name="type">Control type</param>
        /// <param name="value">Value</param>
        /// <param name="required">Required</param>
        /// <param name="placeholder">Placeholder</param>
        /// <param name="colSize">Column size</param>
        /// <returns></returns>
        public IViewComponentResult Invoke(
            string id,
            string name,
            string label,
            string type = "text",
            string? value = null,
            bool required = false,
            string placeholder = "",
            int colSize = 12)
        {
            var model = new FormControlModel
            {
                Id = id,
                Name = name,
                Label = label,
                Value = value,
                Required = required,
                Placeholder = placeholder,
                ColSize = colSize
            };

            // Parse control type
            if (Enum.TryParse<AutoAppManagement.Models.Enums.ControlType>(type, true, out var controlType))
            {
                model.Type = controlType;
            }

            return View(model);
        }
    }
}
