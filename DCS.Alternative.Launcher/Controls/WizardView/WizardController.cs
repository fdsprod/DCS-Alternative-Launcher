using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;
using DCS.Alternative.Launcher.ComponentModel;
using Reactive.Bindings;

namespace DCS.Alternative.Launcher.Controls
{
    public class WizardController : IDisposable, INotifyPropertyChanged
    {
        private readonly List<IDisposable> _disposables = new List<IDisposable>();
        private Guid _id = Guid.NewGuid();

        public WizardController()
        {
            CurrentStep.Subscribe(OnCurrentStepChanged);
        }

        public IWizardStep PreviousStep
        {
            get
            {
                var currentIndex = Steps.IndexOf(CurrentStep.Value);

                if (currentIndex == 0)
                {
                    return null;
                }

                return Steps[currentIndex - 1];
            }
        }

        public ReactiveProperty<bool> CanClose
        {
            get;
        } = new ReactiveProperty<bool>(true);

        public ReactiveProperty<bool> IsForwardOnlyWizard
        {
            get;
        } = new ReactiveProperty<bool>();

        public ReactiveProperty<bool> CanResize
        {
            get;
        } = new ReactiveProperty<bool>();


        public ReactiveProperty<bool> IsBackVisible
        {
            get;
        } = new ReactiveProperty<bool>(true);

        public ReactiveProperty<bool> IsNextVisible
        {
            get;
        } = new ReactiveProperty<bool>(true);

        public ReactiveProperty<bool> CanGoBack
        {
            get;
        } = new ReactiveProperty<bool>(true);

        public ReactiveProperty<bool> CanGoNext
        {
            get;
        } = new ReactiveProperty<bool>(true);

        public ReactiveProperty<bool> IsFooterVisible
        {
            get;
        } = new ReactiveProperty<bool>();

        public ReactiveProperty<bool> IsHeaderVisible
        {
            get;
        } = new ReactiveProperty<bool>();

        public ObservableCollection<IWizardStep> Steps
        {
            get;
        } = new ObservableCollection<IWizardStep>();

        public ReactiveProperty<IWizardStep> CurrentStep
        {
            get;
        } = new ReactiveProperty<IWizardStep>();

        internal WizardView Wizard
        {
            get;
            set;
        }

        public virtual void Dispose()
        {
            foreach (var disposable in _disposables)
            {
                disposable.Dispose();
            }

            _disposables.Clear();

            foreach (var step in Steps)
            {
                step.Dispose();
            }

            Steps.Clear();

            CurrentStep.Value = null;
            Wizard = null;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public event EventHandler BeforeNext;

        public event EventHandler BeforeBack;


        private void OnCurrentStepChanged(IWizardStep obj)
        {
            OnCurrentStepChanged();
        }

        public virtual Task<bool> GoNextAsync(bool allowStepToOverride = true)
        {
            if (UiDispatcher.Current.CheckAccess())
            {
                return goNextAsync(allowStepToOverride);
            }

            return UiDispatcher.Current.InvokeAsync(() => goNextAsync(allowStepToOverride)).Result;
        }

        public virtual Task<bool> GoBackAsync(bool allowStepToOverride = true)
        {
            if (UiDispatcher.Current.CheckAccess())
            {
                return goBackAsync(allowStepToOverride);
            }

            return UiDispatcher.Current.InvokeAsync(() => goBackAsync(allowStepToOverride)).Result;
        }

        private async Task<bool> goNextAsync(bool allowStepToOverride)
        {
            var currentStep = CurrentStep.Value;

            if (!CanGoNext.Value)
            {
                return false;
            }

            if (currentStep != null)
            {
                if (allowStepToOverride && !currentStep.OnNext())
                {
                    OnNextCancelled();
                    return false;
                }

                if (!currentStep.Validate())
                {
                    OnValidationFailed();
                    return false;
                }

                if (!currentStep.Commit())
                {
                    OnCommitCancelled();
                    return false;
                }
            }

            var nextIndex = 0;

            if (currentStep != null)
            {
                nextIndex = Steps.IndexOf(currentStep) + 1;
            }

            if (nextIndex < Steps.Count)
            {
                var deactivate = currentStep as IDeactivate;

                if (deactivate != null && deactivate.IsActivated)
                {
                    await deactivate.DeactivateAsync();
                }

                BeforeNext?.Invoke(this, EventArgs.Empty);

                var nextStep = Steps[nextIndex];
                CurrentStep.Value = nextStep;

                var activate = nextStep as IActivate;

                if (activate != null)
                {
                    await activate.ActivateAsync();
                }

                if (IsForwardOnlyWizard.Value)
                {
                    for (var i = 1; i < nextIndex; i++)
                    {
                        var step = Steps[0];
                        step.Dispose();
                        Steps.RemoveAt(0);
                    }
                }
            }
            else
            {
                OnDone();
            }

            return true;
        }

        private async Task<bool> goBackAsync(bool allowStepToOverride)
        {
            var currentStep = CurrentStep.Value;

            if (!CanGoBack.Value)
            {
                return false;
            }

            var previousIndex = 0;

            if (currentStep != null)
            {
                if (allowStepToOverride && !currentStep.OnBack())
                {
                    OnBackCancelled();
                    return false;
                }

                previousIndex = Steps.IndexOf(currentStep) - 1;
            }

            if (previousIndex >= 0)
            {
                var previousStep = Steps[previousIndex];

                var deactivate = currentStep as IDeactivate;

                if (deactivate != null && deactivate.IsActivated)
                {
                    await deactivate.DeactivateAsync();
                }

                BeforeBack?.Invoke(this, EventArgs.Empty);

                CurrentStep.Value = previousStep;

                var activate = previousStep as IActivate;

                if (activate != null)
                {
                    await activate?.ActivateAsync();
                }
            }
            else
            {
                OnDone();
            }

            return true;
        }

        public bool HasStep(Type wizardStepType)
        {
            foreach (var step in Steps)
            {
                if (step.GetType() == wizardStepType)
                {
                    return true;
                }
            }

            return false;
        }

        public virtual void Initialize(IContainer container)
        {
        }

        public void InsertAfter(Type wizardStepType, IWizardStep wizardStep)
        {
            var steps = Steps;
            var index = GetWizardStepIndexByType(wizardStepType);

            if (steps.Count == ++index)
            {
                steps.Add(wizardStep);
            }
            else
            {
                steps.Insert(index, wizardStep);
            }

            var disposableStep = wizardStep as IDisposable;
            if (disposableStep != null)
            {
                _disposables.Add(disposableStep);
            }
        }

        public void ClearAfter(Type wizardStepType)
        {
            var steps = Steps;
            var index = GetWizardStepIndexByType(wizardStepType);

            for (var i = steps.Count - 1; i > index; i--)
            {
                steps.RemoveAt(i);
            }
        }

        public virtual async Task<bool> CloseAsync(bool allowStepToOverride = true)
        {
            if (!CanClose.Value)
            {
                return false;
            }

            var currentStep = CurrentStep.Value;
            var deactivate = currentStep as IDeactivate;

            if (currentStep != null && allowStepToOverride && !currentStep.OnComplete())
            {
                OnCompleteCancelled();
                return false;
            }

            try
            {
                if (deactivate != null)
                {
                    await deactivate.DeactivateAsync();
                }

                OnDone();
            }
            catch (Exception ex)
            {
                Debug.Print(ex.ToString());
            }

            return true;
        }

        public event EventHandler Complete;

        protected virtual void OnCompleteCancelled()
        {
        }

        protected virtual void OnCurrentStepChanged()
        {
        }

        protected virtual void OnNextCancelled()
        {
        }

        protected virtual void OnBackCancelled()
        {
        }

        protected virtual void OnCommitCancelled()
        {
        }

        protected virtual void OnValidationFailed()
        {
        }

        protected virtual void OnDone()
        {
            var handler = Complete;
            handler?.Invoke(this, EventArgs.Empty);

            Window.GetWindow(Wizard)?.Close();
        }

        private int GetWizardStepIndexByType(Type wizardStepType)
        {
            var index = -1;
            var steps = Steps;

            for (var i = 0; i < steps.Count; i++)
            {
                if (steps[i].GetType() == wizardStepType)
                {
                    index = i;
                    break;
                }
            }

            if (index == -1)
            {
                throw new Exception(string.Format("Unable to find wizard step {0}", wizardStepType));
            }

            return index;
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}