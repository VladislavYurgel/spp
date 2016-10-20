using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tracer_WPF.ViewModel
{
    public class MainContext
    {
        TabItemViewModel tabItemViewModel = new TabItemViewModel();
        CommandsViewModel commandsViewModel = new CommandsViewModel();
        //NodeViewModel nodeViewModel = new NodeViewModel();

        /*public NodeViewModel NodeViewModel
        {
            get { return nodeViewModel; }
        }*/

        public CommandsViewModel CommandsViewModel
        {
            get { return commandsViewModel; }
        }

        public TabItemViewModel TabItemViewModel
        {
            get { return tabItemViewModel; }
        }
    }
}
