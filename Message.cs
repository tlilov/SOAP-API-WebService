using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EmailSystem {
    public class Message {
        public int MessageId { get; set; }
        public string TransactionId { get; set; }
        public string ContactId { get; set; }
        public string ContactType { get; set; }
        public int MessageTypeId { get; set; }
        public string SiteCode { get; set; }
        public string To { get; set; }
        public string From { get; set; }
        public string FromName { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime? DateSent { get; set; }
        public string Status { get; set; }
        public string TemplateId { get; set; }
        public string TemplateName { get; set; }
        public bool? Transactional { get; set; }
        public int Attempts { get; set; }
        [NotMapped] public bool ReplyTrackingEnabled { get; set; }

        [NotMapped]
        public string MessageFormat {
            get {
                return "html";
            }
        }

        public List<EmailToken> Tokens { get; set; }

        public Message() {
            DateCreated = DateTime.Now;
            Tokens = new List<EmailToken>();
        }
    }

}
