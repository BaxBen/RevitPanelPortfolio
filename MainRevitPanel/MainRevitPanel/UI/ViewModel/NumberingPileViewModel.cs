using MainRevitPanel.Base;
using MainRevitPanel.UI.EventsArgs;
using MainRevitPanel.UI.RelayCommands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MainRevitPanel.UI.ViewModel
{
    public class NumberingPileViewModel : ViewModelBase
    {
        public Dictionary<string, bool> SelectionTypes { get; set; }
        public Dictionary<string, List<bool>> Positions { get; set; }

        public RelayCommand ApplyCommand { get; }
        public bool? SelectedType { get; set; }
        public List<bool> SelectedPosition { get; set; }

        public event EventHandler<ApplRequestedEventArgs> ApplRequested;


        public NumberingPileViewModel()
        {
            SelectionTypes = new Dictionary<string, bool> { ["Все"] = false, ["Выборочно"] = true };
            Positions = new Dictionary<string, List<bool>>
            {
                ["Горизонтально Верхний левый угол"] = new List<bool> { true, false, true },
                ["Горизонтально Верхний правый угол"] = new List<bool> { true, true, true },
                ["Горизонтально Нижний левый угол"] = new List<bool> { true, false, false },
                ["Горизонтально Нижний правый угол"] = new List<bool> { true, true, false },
                ["Вертикально Верхний левый угол"] = new List<bool> { false, false, true },
                ["Вертикально Верхний правый угол"] = new List<bool> { false, true, true },
                ["Вертикально Нижний левый угол"] = new List<bool> { false, false, false },
                ["Вертикально Нижний правый угол"] = new List<bool> { false, true, false }
            };
            ApplyCommand = new RelayCommand(ExecuteApply, CanExecuteApply);
        }
        private bool CanExecuteApply(object parameter)
        {
            return (SelectedType != null && SelectedPosition!=null);
        }

        private void ExecuteApply(object parameter)
        {

            List<object> listParametrs = new List<object> { SelectedType, SelectedPosition };
            OnApplRequested(new ApplRequestedEventArgs(listParametrs));
        }

        protected virtual void OnApplRequested(ApplRequestedEventArgs e)
        {
            ApplRequested?.Invoke(this, e);
        }

    }
}
