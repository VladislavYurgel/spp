using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using Tracer_WPF.Model;
using Tracer_WPF.Utils;
using Tracer_WPF.ViewModel;

namespace Tracer_WPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static TabControl tabControl;
        public static ObservableCollection<Node> tempNodeList;
        public static Node tempNode;

        public MainWindow()
        {
            InitializeComponent();
            tabControl = TracerTabControl;
        }

        public void SelectNode(object sender, RoutedEventArgs e)
        {
            TreeView treeView = sender as TreeView;
            tempNodeList = treeView.ItemsSource as ObservableCollection<Node>;
            tempNode = treeView.SelectedItem as Node;

            if (tempNode.ParentId > 0)
            {
                EditMethodName.Text = tempNode.MethodName;
                EditMethodName.IsEnabled = true;
                EditPackageName.Text = tempNode.Package;
                EditPackageName.IsEnabled = true;
                EditParamsCount.Text = tempNode.ParamsCount.ToString();
                EditParamsCount.IsEnabled = true;
                EditTime.Text = tempNode.Time.ToString();
                EditTime.IsEnabled = true;
                SaveEditableData.IsEnabled = true;
            }
            else
            {
                EditMethodName.Clear();
                EditMethodName.IsEnabled = false;
                EditPackageName.Clear();
                EditPackageName.IsEnabled = false;
                EditParamsCount.Clear();
                EditParamsCount.IsEnabled = false;
                EditTime.Clear();
                EditTime.IsEnabled = false;
                SaveEditableData.IsEnabled = false;
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            TabItemViewModel.Tabs.Add(new TabItemModel { Header = "New tab" });

        }

        private void SaveEditableData_MouseDown(object sender, RoutedEventArgs e)
        {
            try
            {
                int prevTime = tempNode.Time;

                tempNode.MethodName = EditMethodName.Text;
                tempNode.Package = EditPackageName.Text;
                tempNode.Time = int.Parse(EditTime.Text);
                tempNode.ParamsCount = int.Parse(EditParamsCount.Text);

                int newTime = tempNode.Time - prevTime;

                SetNode(ref tempNodeList, tempNode.NodeId, tempNode);

                RecountTime(tempNodeList, newTime, tempNode.ParentId);

                if (!TabItemIsUnsaved())
                    TabItemViewModel.Tabs[tabControl.SelectedIndex].Header += '*';
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        public static bool TabItemIsUnsaved(int index = -1)
        {
            int tabIndex = (index < 0) ? tabControl.SelectedIndex : index;

            if (TabItemViewModel.Tabs[tabIndex].Header[TabItemViewModel.Tabs[tabIndex].Header.Length - 1] == '*')
                return true;
            else
                return false;
        }

        public static void ChangeTabItemState(int index = -1)
        {
            int tabIndex = (index < 0) ? tabControl.SelectedIndex : index;

            if (!TabItemIsUnsaved(tabIndex))
                TabItemViewModel.Tabs[tabIndex].Header += "*";
            else
                TabItemViewModel.Tabs[tabIndex].Header = Path.GetFileName(CommandsViewModel.files[tabIndex]);
        }

        private static void SetNode(ref ObservableCollection<Node> nodes, int neededId, Node insertedNode)
        {
            for (int i = 0; i < nodes.Count; i++)
            {
                if (nodes[i].NodeId == neededId)
                {
                    nodes[i] = insertedNode;

                    return;
                }
                if (nodes[i].Nodes.Count > 0)
                    SetNode(ref nodes[i].nodes, neededId, insertedNode);
            }
        }

        private static void SetTime(ref ObservableCollection<Node> nodes, ref int neededId, int time)
        {
            for (int i = 0; i < nodes.Count; i++)
            {
                if (nodes[i].NodeId == neededId)
                {
                    nodes[i].Time += time;
                    neededId = nodes[i].ParentId;

                    return;
                }
                if (nodes[i].Nodes.Count > 0)
                    SetTime(ref nodes[i].nodes, ref neededId, time);
            }
        }

        private static void RecountTime(ObservableCollection<Node> nodes, int timeDifference, int parentId)
        {
            while (parentId != 0)
            {
                SetTime(ref nodes, ref parentId, timeDifference);
            }
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
