using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Tracer_WinForms
{
    public partial class ChangeNode : Form
    {
        protected TreeNode treeNode;
        protected double time;
        protected TabControl tabControl;
        public ChangeNode(ref TreeNode treeNode, ref TabControl tabControl)
        {
            this.treeNode = treeNode;
            this.tabControl = tabControl;
            InitializeComponent();
            InitializeData();
        }

        protected void InitializeData()
        {
            // Regular expression partterns
            string methodRegEx = @"\b([A-Za-z\d_]*)";
            string paramsRegEx = @"(?<=\(Params=)[\d]*";
            string packageRegEx = @"(?<=Package=)[A-Za-z\d_]*";
            string timeRegEx = @"(?<=time=)[\d]*";
            
            // Text in node
            string nodeText = treeNode.Text;

            // Get params from node with regular expressions
            Match methodMatch = Regex.Match(nodeText, methodRegEx);
            Match paramsMatch = Regex.Match(nodeText, paramsRegEx);
            Match packageMatch = Regex.Match(nodeText, packageRegEx);
            Match timeMatch = Regex.Match(nodeText, timeRegEx);

            // Initialize data to text fields
            textMethod.Text = methodMatch.Groups[0].Value;
            textPackage.Text = packageMatch.Groups[0].Value;
            textParams.Text = paramsMatch.Groups[0].Value;
            textTime.Text = timeMatch.Groups[0].Value;
            time = double.Parse(textTime.Text);
        }

        protected void SaveData()
        {
            string methodName = textMethod.Text;
            string packageName = textPackage.Text;
            //string paramsCount = textParams.Text;
            int paramsCount = 0;
            string newTime = textTime.Text;
            string nodeText = "";

            methodName = Regex.Replace(methodName, @"\s*", "");
            packageName = Regex.Replace(packageName, @"\s*", "");
            if ((methodName.Length < 1) || (packageName.Length < 1))
            {
                MessageBox.Show("Method and package name will not be empty");
                return;
            }

            try
            {
                time = double.Parse(newTime) - time;
                paramsCount = int.Parse(textParams.Text);
            }
            catch (Exception e)
            {
                MessageBox.Show("You type incorrect data");
                return;
            }

            nodeText = methodName + " (Params=" + paramsCount + ", Package=" + packageName + ", ";
            nodeText += "time=" + newTime + ")";

            if (!String.Equals(treeNode.Text, nodeText))
                tabControl.SelectedTab.Text = tabControl.SelectedTab.Text + "*";

            treeNode.Text = nodeText;

            recountTime(time, treeNode.Parent);

            Form1.changeNodeWindofIsOpen = false;
            Close();
        }

        private void recountTime(double newTime, TreeNode recountNode)
        {
            if (recountNode != null)
            {
                string nodeText = recountNode.Text;
                Match match = Regex.Match(nodeText, @"(?<=time=)[\d]*");
                double oldTime = double.Parse(match.Groups[0].Value.ToString());
                double recordTime = oldTime + newTime;
                nodeText = Regex.Replace(nodeText, @"time=([\d]*)", "time=" + recordTime);

                recountNode.Text = nodeText;

                recountTime(newTime, recountNode.Parent);
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("All data will be not saved", "Are you sure?", MessageBoxButtons.YesNo);
            if (result == DialogResult.Yes)
            {
                Close();
                Form1.changeNodeWindofIsOpen = false;
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            SaveData();
        }
    }
}
