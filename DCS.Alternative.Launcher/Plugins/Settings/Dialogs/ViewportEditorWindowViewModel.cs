using System;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Windows.Media;
using DCS.Alternative.Launcher.Controls.MessageBoxEx;
using DCS.Alternative.Launcher.Modules;
using DCS.Alternative.Launcher.ServiceModel;
using DCS.Alternative.Launcher.Services;
using Reactive.Bindings;

namespace DCS.Alternative.Launcher.Plugins.Settings.Dialogs
{
    public class ViewportEditorWindowViewModel
    {
        private readonly Module _module;
        private readonly ISettingsService _settingsService;
        private readonly string _monitorId;

        public ViewportEditorWindowViewModel(IContainer container, bool isPreview, string monitorId, Module module, ViewportModel[] viewports)
        {
            _module = module;
            _monitorId = monitorId;
            _settingsService = container.Resolve<ISettingsService>();

            foreach (var viewport in viewports)
            {
                Viewports.Add(viewport);
            }

            IsPreview.Value = !isPreview;

            AddViewportCommand = new ReactiveCommand(IsPreview.AsObservable(), isPreview);
            DeleteViewportCommand = new ReactiveCommand<ViewportModel>(IsPreview.AsObservable(), isPreview);
            SaveCommand = new ReactiveCommand(IsPreview.AsObservable(), isPreview);
            CancelCommand = new ReactiveCommand(IsPreview.AsObservable(), isPreview);

            CancelCommand.Subscribe(OnAddViewport);
            DeleteViewportCommand.Subscribe(OnDeleteViewport);
            SaveCommand.Subscribe(OnSave);
            CancelCommand.Subscribe(OnCancel);
        }

        private void OnCancel()
        {
        }

        private void OnSave()
        {
        }


        public ReactiveProperty<bool> IsPreview
        {
            get;
        } = new ReactiveProperty<bool>();

        private void OnDeleteViewport(ViewportModel value)
        {
            var window = WindowAssist.GetWindow(this);

            if (MessageBoxEx.Show($"Are you sure you want to delete viewport {value.Name.Value}?", "Delete Viewport", System.Windows.MessageBoxButton.YesNo, parent: window) == System.Windows.MessageBoxResult.Yes)
            {
                Viewports.Remove(value);
            }
        }

        private void OnAddViewport()
        {
            var vp = new ViewportModel();

            vp.X.Value = 200;
            vp.Y.Value = 200;
            vp.Width.Value = 200;
            vp.Height.Value = 200;

            Viewports.Add(vp);
        }

        public ReactiveCommand SaveCommand
        {
            get;
        } 

        public ReactiveCommand CancelCommand
        {
            get;
        } 

        public ReactiveCommand AddViewportCommand
        {
            get;
        }

        public ReactiveCommand<ViewportModel> DeleteViewportCommand
        {
            get;
        } 

        public ReactiveCollection<ViewportModel> Viewports
        {
            get;
        } = new ReactiveCollection<ViewportModel>();
    }

    public class ViewportModel
    {
        public ReactiveProperty<double> X
        {
            get;
        } = new ReactiveProperty<double>();

        public ReactiveProperty<double> Y
        {
            get;
        } = new ReactiveProperty<double>();

        public ReactiveProperty<double> Width
        {
            get;
        } = new ReactiveProperty<double>();

        public ReactiveProperty<double> Height
        {
            get;
        } = new ReactiveProperty<double>();

        public ReactiveProperty<string> InitFile
        {
            get;
        } = new ReactiveProperty<string>();

        public ReactiveProperty<string> Name
        {
            get;
        } = new ReactiveProperty<string>();

        public ReactiveProperty<string> ImageUrl
        {
            get;
        } = new ReactiveProperty<string>();
    }
}
