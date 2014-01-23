using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FreeSrc.DevUtils.Commands
{
    public class OpenMQ : UiCommand<FreeSrc.DevUtils.Models.MQViewerVM>
    {
        public OpenMQ()
        {
            this.VMName = "MQViewer";
        }

        protected override void UpdateViewModel(Models.MQViewerVM viewModel)
        {
            viewModel.Open();
        }
    }
}
