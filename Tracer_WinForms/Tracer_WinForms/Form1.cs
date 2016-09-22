using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;

namespace Tracer_WinForms
{
    public struct myFile
    {
        public string shortFileName;
        public string fileName;
    }

    public partial class Form1 : Form
    {
        List<Trace>[] traceList = new List<Trace>[] { };
        public static bool changeNodeWindofIsOpen = false;
        public static bool createdTab = false;
        public List<myFile> fileNames = new List<myFile>();
        public Form1()
        {
            InitializeComponent();
            Array.Resize(ref traceList, traceList.Count() + 1);
        }

        private void button1_Click(object sender, EventArgs e)
        {

        }

        private void createNewTab()
        {
            string tabTitle = "XML Page " + (tabControl1.TabCount + 1).ToString();
            TabPage myTabPage = new TabPage(tabTitle);
            myTabPage.BackColor = Color.White;

            Array.Resize(ref traceList, traceList.Count() + 1);

            /* Add label for remove currency tab page */

            Label removePageLabel = new Label();
            removePageLabel.Text = removeLabel.Text;
            removePageLabel.Cursor = removeLabel.Cursor;
            removePageLabel.Click += removeLabel_Click;
            removePageLabel.Location = removeLabel.Location;
            myTabPage.Controls.Add(removePageLabel);

            /* Add treeView for new tab page */

            TreeView treeView = new TreeView();
            treeView.Name = "treeView" + tabControl1.TabCount;
            treeView.Location = treeView0.Location;
            treeView.Size = treeView0.Size;
            treeView.BorderStyle = treeView0.BorderStyle;
            myTabPage.Controls.Add(treeView);

            /* Initialize new tab page */

            tabControl1.TabPages.Add(myTabPage);
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void openAs_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.CheckFileExists = true;
            openFileDialog.Filter = "XML Files (*.xml)|*.xml";
            openFileDialog.Multiselect = false;
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                openFile(openFileDialog.FileName);
            }
        }

        private void openFile(string fileName, bool currentTab = false)
        {
            if (Path.GetExtension(fileName) != ".xml")
            {
                MessageBox.Show("Open error! This is not xml file");
                return;
            }
            TreeView currentTree;
            string xmlText = File.ReadAllText(fileName);
            XMLParser xmlParser = new XMLParser();
            if (!fileNames.Exists(x => x.fileName == fileName))
            {
                if ((createdTab || (tabControl1.TabCount <= 0)) && !currentTab)
                {
                    createNewTab();
                    tabControl1.SelectedTab = tabControl1.TabPages[tabControl1.TabCount - 1];
                    createdTab = true;
                }
                else
                    createdTab = true;
                tabControl1.TabPages[tabControl1.SelectedIndex].Text = Path.GetFileName(fileName);
            }
            else
            {
                MessageBox.Show("This file is open");
                return;
            }
            myFile tempFile = new myFile();
            tempFile.fileName = fileName;
            tempFile.shortFileName = Path.GetFileName(fileName);
            fileNames.Add(tempFile);

            if (traceList[tabControl1.SelectedIndex] != null)
            {
                //DialogResult result = ConfirmWindow(MessageBoxButtons.YesNo);
                //if (result == DialogResult.Yes)
                //{
                traceList[tabControl1.SelectedIndex] = null;
                traceList[tabControl1.SelectedIndex] = xmlParser.Parse(xmlText);
                currentTree = new TreeView();
                if (tabControl1.SelectedTab.Controls.ContainsKey("treeView" + tabControl1.SelectedIndex))
                    currentTree = (TreeView)tabControl1.SelectedTab.Controls["treeView" + tabControl1.SelectedIndex];
                MyTree myTree = new MyTree();
                currentTree.Nodes.Clear();
                myTree.GenerateTree(traceList[tabControl1.SelectedIndex], currentTree);
                currentTree.ExpandAll();
                currentTree.NodeMouseDoubleClick += new TreeNodeMouseClickEventHandler(treeNodeClick);
                //}
            }
            else
            {
                traceList[tabControl1.SelectedIndex] = xmlParser.Parse(xmlText);
                currentTree = new TreeView();
                if (tabControl1.SelectedTab.Controls.ContainsKey("treeView" + tabControl1.SelectedIndex))
                    currentTree = (TreeView)tabControl1.SelectedTab.Controls["treeView" + tabControl1.SelectedIndex];
                MyTree myTree = new MyTree();
                currentTree.Nodes.Clear();
                myTree.GenerateTree(traceList[tabControl1.SelectedIndex], currentTree);
                currentTree.NodeMouseDoubleClick += new TreeNodeMouseClickEventHandler(treeNodeClick);
                currentTree.ExpandAll();
            }
        }

        private void treeNodeClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (!changeNodeWindofIsOpen)
            {
                MyTree myTree = new MyTree();
                changeNodeWindofIsOpen = true;
                myTree.changeNode(e, ref tabControl1);
            }
        }

        protected DialogResult ConfirmWindow(MessageBoxButtons buttons, string message = "All unsaved data will be deleted", string caption = "Are you sure")
        {
            buttons = MessageBoxButtons.YesNo;
            DialogResult result;

            result = MessageBox.Show(message, caption, buttons);

            return result;
        }

        private void removeLabel_Click(object sender, EventArgs e)
        {
            string warningMessage = "All unsaved data will be deleted.";
            string captionMessage = "Are you sure?";
            MessageBoxButtons buttons = MessageBoxButtons.YesNo;
            DialogResult result;

            result = MessageBox.Show(warningMessage, captionMessage, buttons);

            if (result == DialogResult.Yes)
            {
                TreeView currentTree;
                for (int i = tabControl1.SelectedIndex + 1; i < tabControl1.TabPages.Count; i++)
                {
                    if (tabControl1.TabPages[i].Controls.ContainsKey("treeView" + i))
                    {
                        currentTree = (TreeView)tabControl1.TabPages[i].Controls["treeView" + i];
                        currentTree.Name = "treeView" + (i - 1);
                    }
                    tabControl1.TabPages[i].Name = "tabPage" + i;
                    tabControl1.TabPages[i].Text = "XML Page " + i;
                }
                if (fileNames.Exists(x => x.shortFileName == tabControl1.SelectedTab.Text))
                    fileNames.Remove(fileNames.Find(x => x.shortFileName == tabControl1.SelectedTab.Text));
                tabControl1.TabPages.Remove(tabControl1.SelectedTab);
            }
        }

        private void saveAs_Click(object sender, EventArgs e)
        {
            string fileName = "";
            TreeView currentTree = new TreeView();
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "XML Files (*.xml)|*.xml|All Files (*.*)|*.*";
            saveFileDialog.FilterIndex = 1;
            
            if (tabControl1.TabCount > 0)
            {
                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    fileName = saveFileDialog.FileName;
                    if (Path.GetExtension(fileName) != ".xml")
                    {
                        MessageBox.Show("Save error! This is not XML file");
                        return;
                    }
                    XMLParser xmlParse = new XMLParser();
                    if (tabControl1.SelectedTab.Controls.ContainsKey("treeView" + tabControl1.SelectedIndex))
                    {
                        currentTree = (TreeView)tabControl1.SelectedTab.Controls["treeView" + tabControl1.SelectedIndex];
                        if (currentTree.Nodes.Count > 0)
                        {
                            xmlParse.Build(currentTree, fileName);
                            tabControl1.SelectedTab.Text = Regex.Replace(tabControl1.SelectedTab.Text, @"\*$", "");
                            if (fileNames.Exists(x => x.shortFileName == tabControl1.SelectedTab.Text))
                                fileNames.Remove(fileNames.Find(x => x.shortFileName == tabControl1.SelectedTab.Text));
                            openFile(fileName, true);
                        }
                        else
                            MessageBox.Show("Current treeview is empty");
                    }
                }
            }
            else
            {
                MessageBox.Show("Tab with xml not found");
            }
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.S)
            {
                e.SuppressKeyPress = true;
                saveAs.PerformClick();
            }
        }
    }
}
