using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using MainRevitPanel.Commands.Export;
using MainRevitPanel.Commands.VK;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace MainRevitPanel
{
    [Transaction(TransactionMode.Manual)]
    public class Main : IExternalApplication
    {
        private string AssemblyPath => Assembly.GetExecutingAssembly().Location;
        private Dictionary<string, RibbonPanel> _dictPanel = new Dictionary<string, RibbonPanel>();
        private string _tabName = "Portfolio";

        public Result OnShutdown(UIControlledApplication application)
        {
            return Result.Succeeded;
        }


        public Result OnStartup(UIControlledApplication application)
        {
            SetupPanel(application);
            return Result.Succeeded;
        }

        public void NavigatorPanel(UIControlledApplication application)
        {
            application.GetRibbonPanels(_tabName);
            RibbonPanel panel = application.CreateRibbonPanel(_tabName, "Навигация по разделам");

            ComboBoxData comboBoxData = new ComboBoxData("SectionNavigator");
            ComboBox comboBox = panel.AddItem(comboBoxData) as ComboBox;

            //В будущем заменю на данные из бд
            List<string> DB = new List<string>() { "Общие", "АР", "КР", "ОВ", "ВК", "Избранное", "Экспорт", "BIM" };

            foreach (var namePanel in DB)
            {
                comboBox.AddItem(new ComboBoxMemberData(namePanel, namePanel));
                _dictPanel.Add(namePanel, application.CreateRibbonPanel(_tabName, namePanel));
            }

            foreach (var chapter in DB)
            {
                if (chapter == "Общие") continue;
                _dictPanel[chapter].Visible = false;
            }

            comboBox.CurrentChanged += (sender, e) =>
            {
                ComboBox currentComboBox = sender as ComboBox;
                if (currentComboBox?.Current?.ItemText != null)
                {
                    foreach (var chapter in DB)
                    {
                        if (currentComboBox.Current.ItemText == chapter)
                        {
                            _dictPanel[chapter].Visible = true;
                        }
                        else
                        {
                            _dictPanel[chapter].Visible = false;
                        }
                    }
                }
            };

        }
        private void OVPanel(RibbonPanel panel)
        {
            PushButton button1 = CreateButton(
                panel,
                "Поиск ближайшего и дальнего крана",
                "Поиск ближайшего \nи дальнего крана",
                typeof(NearFarCraneCommand),
                "Пользователь выделяет один экземпляр семейства \"Шаровый кран с муфтой\". Плагин строит граф соединений и находит в системе два крана того же типа:\r\n\r\nБлижайший — минимальное количество соединений от исходного\r\nСамый дальний — максимальное количество соединений (длина пути)"
                );
        }
        private void ExportPanel(RibbonPanel panel)
        {
            PushButton button1 = CreateButton(
                panel,
                "Экспорт листов в DWG",
                "Экспорт листов в DWG",
                typeof(ExportToDWGCommand),
                "Пользователь выбирает листы для экспорта"
                );
        }

        private void SetupPanel(UIControlledApplication application)
        {
            application.CreateRibbonTab(_tabName);
            NavigatorPanel(application);
            OVPanel(_dictPanel["ОВ"]);
            ExportPanel(_dictPanel["Экспорт"]);
        }


        private PushButton CreateButton(RibbonPanel panel, string name, string text, Type commandType, string toolTip)
        {
            PushButtonData buttonData = new PushButtonData(name, text, AssemblyPath, commandType.FullName);

            buttonData.ToolTip = toolTip;

            PushButton button = panel.AddItem(buttonData) as PushButton;

            return button;
        }
    }
}
