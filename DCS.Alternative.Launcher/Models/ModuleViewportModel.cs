using System.Collections.Generic;
using DCS.Alternative.Launcher.DomainObjects;
using DCS.Alternative.Launcher.Modules;
using Reactive.Bindings;

namespace DCS.Alternative.Launcher.Models
{
    public class ModuleViewportModel
    {
        public ModuleViewportModel(string name, string imageUrl, Module module, IEnumerable<MonitorDefinition> monitors, IEnumerable<Viewport> viewports)
        {
            Name.Value = name;
            ImageUrl.Value = imageUrl;
            Module.Value = module;

            foreach (var monitor in monitors)
            {
                MonitorIds.Add(monitor.MonitorId);
            }

            if (viewports != null)
            {
                foreach (var viewport in viewports)
                {
                    Viewports.Add(viewport);
                }
            }
        }

        public ReactiveProperty<bool> IsSelected
        {
            get;

        } = new ReactiveProperty<bool>();

        public ReactiveProperty<string> Name
        {
            get;

        } = new ReactiveProperty<string>();

        public ReactiveProperty<string> ImageUrl
        {
            get;

        } = new ReactiveProperty<string>();

        public ReactiveProperty<Module> Module
        {
            get;

        } = new ReactiveProperty<Module>();

        public ReactiveCollection<string> MonitorIds
        {
            get;

        } = new ReactiveCollection<string>();

        public ReactiveCollection<Viewport> Viewports
        {
            get;

        } = new ReactiveCollection<Viewport>();
    }
}