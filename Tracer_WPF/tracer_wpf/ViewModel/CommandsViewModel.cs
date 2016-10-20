using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Tracer_WPF.Model;
using Tracer_WPF.Utils;

namespace Tracer_WPF.ViewModel
{
    public class CommandsViewModel
    {
        public static List<string> files = new List<string>();

        private ICommand openFileCommand;
        public ICommand OpenFileCommand
        {
            get
            {
                if (openFileCommand == null)
                    openFileCommand = new RelayCommand(p => OpenFile());

                return openFileCommand;
            }
        }

        private ICommand saveAsFileCommand;
        public ICommand SaveAsFileCommand
        {
            get
            {
                if (saveAsFileCommand == null)
                    saveAsFileCommand = new RelayCommand(p => SaveAsFile());

                return saveAsFileCommand;
            }
        }

        private ICommand saveCommand;
        public ICommand SaveCommand
        {
            get
            {
                if (saveCommand == null)
                    saveCommand = new RelayCommand(p => SaveFile());

                return saveCommand;
            }
        }

        private ICommand deleteTabItemCommand;
        public ICommand DeleteTabItemCommand
        {
            get
            {
                if (deleteTabItemCommand == null)
                    deleteTabItemCommand = new RelayCommand(p => DeleteTabItem());

                return deleteTabItemCommand;
            }
        }

        private ICommand exitCommand;
        public ICommand ExitCommand
        {
            get
            {
                if (exitCommand == null)
                    exitCommand = new RelayCommand(p => Exit());

                return exitCommand;
            }
        }

        private void DeleteTabItem()
        {
            if (TabItemViewModel.Tabs.Count > 0)
            {
                if (TabItemViewModel.Tabs[MainWindow.tabControl.SelectedIndex].Header[TabItemViewModel.Tabs[MainWindow.tabControl.SelectedIndex].Header.Length - 1] == '*')
                {
                    if (MessageBox.Show("Close tab without saving?", "Close tab", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                    {
                        files.RemoveAt(MainWindow.tabControl.SelectedIndex);
                        TabItemViewModel.Tabs.RemoveAt(MainWindow.tabControl.SelectedIndex);
                    }                        
                }
                else
                {
                    files.RemoveAt(MainWindow.tabControl.SelectedIndex);
                    TabItemViewModel.Tabs.RemoveAt(MainWindow.tabControl.SelectedIndex);
                }
            }
        }

        private void Exit()
        {
            bool foundNotSaved = false;

            foreach (TabItemModel tab in TabItemViewModel.Tabs)
            {
                if (tab.Header[tab.Header.Length - 1] == '*')
                {
                    foundNotSaved = true;
                }
            }

            if (foundNotSaved)
            {
                if (MessageBox.Show("Close app without saving?", "Close application", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                    Application.Current.Shutdown();
            }
            else
                Application.Current.Shutdown();
        }

        private void SaveAsFile()
        {
            if (SystemTracer.SaveAsXML())
                MainWindow.ChangeTabItemState();
        }

        private void SaveFile()
        {
            if (SystemTracer.SaveXML())
                MainWindow.ChangeTabItemState();
        }

        private void OpenFile()
        {
            string fileName = SystemTracer.OpenFile();
            if (fileName.Length > 0)
            {
                ObservableCollection<Node> nodes = new ObservableCollection<Node>();
                nodes = SystemTracer.BuildNodes(SystemTracer.Parse(fileName));

                if (files.Contains(fileName))
                {
                    MessageBox.Show("You tried open the file which already is opened", "Open error");
                    return;
                }

                files.Add(fileName);
                
                TabItemViewModel.Tabs.Add(new TabItemModel { Header = Path.GetFileName(fileName), Nodes = nodes });
                MainWindow.tabControl.SelectedIndex = TabItemViewModel.Tabs.Count - 1;
            }
        }
    }
}
