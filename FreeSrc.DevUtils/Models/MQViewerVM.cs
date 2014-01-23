using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FreeSrc.DevUtils.Models
{
    public class MQMessage : ViewModelSkeleton 
    {
        public MQMessage(System.Messaging.Message m)
        {
            this.MessageId = m.Id; 
            this.ArrivedTime = m.ArrivedTime;
            this.Body = m.Body.ToString();
            this.Label = m.Label;
            this.SenderId = m.SenderId;
            this.SentTime = m.SentTime;
            this.TransactionId = m.TransactionId;
        }

        public DateTime ArrivedTime { get; set; }
        public string Body { get; set; }
        public string Label { get; set; }
        public byte[] SenderId { get; set; }
        public DateTime SentTime { get; set; }
        public string TransactionId { get; set; }
        public string MessageId { get; set; }
    }

    public class MQViewerVM : ViewModelSkeleton
    {
        public System.Messaging.MessageQueue _msgQ;

        public MQViewerVM()
        {
            this.Messages = new AsyncCollection<MQMessage>();
            this.QueuePath = @".\Private$\iapplyng_tests";
            this.IsOpen = false;
        }

        public bool IsOpen
        {
            get { return GetProperty<bool>("IsOpen"); }
            set
            {
                SetProperty<bool>("IsOpen", value);
            }
        }

        public string QueuePath
        {
            get { 
                return GetProperty<string>("QueuePath"); 
            }
            set { SetProperty<string>("QueuePath", value); }
        }

        public AsyncCollection<MQMessage> Messages { get; set; }

        public void Open()
        {
            if (!this.IsOpen || _msgQ == null)
            {
                var existingQueues = System.Messaging.MessageQueue.GetPrivateQueuesByMachine(Environment.MachineName);

                _msgQ = existingQueues.Where(x => this.QueuePath.EndsWith(x.Path, StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault();

                if (_msgQ == null)
                {
                    throw new ArgumentOutOfRangeException(string.Format("Queue '{0}' not found.", this.QueuePath));
                }
            }
        }
    }
}
