using EmailSystem.com.bronto.api;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmailSystem.Tests.Infrastructure {
    public class TestableBrontoApiWrapper : IBrontoApiWrapper {
        private List<deliveryObject> deliveries = new List<deliveryObject>();
        private List<deliveryObject> readDeliveries = new List<deliveryObject>();
        private List<messageObject> messages = new List<messageObject>(); 

        public writeResult WriteResult { get; set; }
        public fieldObject[] Fields { get; set; }
        public contactObject[] Contacts { get; set; }
        public bool AddContactsCalled { get; set; }
        public List<contactObject> AddedContacts { get; set; }
        public bool UpdateContactsCalled { get; set; }
        public List<mailListObject> Lists { get; set; }
        public deliveryFilter ReadDeliveriesFilter { get; set; }
        public int AddDeliveriesCallCounter = 0;
        public bool ShouldThrowExceptionOnAddDeliveries = false;
        public int ReadDeliveriesCallCounter = 0;

        public com.bronto.api.writeResult AddContacts(com.bronto.api.contactObject[] contacts) {
            this.AddContactsCalled = true;
            this.AddedContacts = contacts.ToList();
            return WriteResult;
        }

        public com.bronto.api.writeResult AddDeliveries(com.bronto.api.deliveryObject[] deliveries) {
            if (ShouldThrowExceptionOnAddDeliveries)
                throw new NotImplementedException();

            this.deliveries.AddRange(deliveries.ToList());
            AddDeliveriesCallCounter++;

            return WriteResult;
        }

        public List<deliveryObject> GetGeliveries() {
            return this.deliveries;
        } 

        public com.bronto.api.writeResult AddOrUpdateOrders(com.bronto.api.orderObject[] orders) {
            return WriteResult;
        }

        public com.bronto.api.writeResult DeleteContacts(com.bronto.api.contactObject[] contacts) {
            throw new NotImplementedException();
        }

        public string Login(string apiKey) {
            return "1";
        }

        public com.bronto.api.contactObject[] ReadContacts(com.bronto.api.contactFilter filter) {
            return Contacts;
        }

        public com.bronto.api.contactObject[] ReadContacts(com.bronto.api.contactFilter filter, string[] contactFieldIds) {
            return Contacts;
        }

        public com.bronto.api.deliveryObject[] ReadDeliveries(com.bronto.api.deliveryFilter filter) {
            ReadDeliveriesFilter = filter;
            ReadDeliveriesCallCounter++;
            return readDeliveries.ToArray();
        }

        public void SetReadDeliveries(List<deliveryObject> readDeliveries) {
            this.readDeliveries = readDeliveries;
        }

        public com.bronto.api.fieldObject[] ReadFields(com.bronto.api.fieldsFilter filter) {
            return Fields;
        }

        public com.bronto.api.mailListObject[] ReadLists(com.bronto.api.mailListFilter filter) {
            return Lists.ToArray();
        }

        public com.bronto.api.messageObject[] ReadMessages(com.bronto.api.messageFilter filter) {
            return messages.ToArray();
        }

        public void SetMessages(List<messageObject> messages) {
            this.messages = messages;
        }

        public com.bronto.api.sessionHeader SessionHeaderValue { get; set; }

        public com.bronto.api.writeResult UpdateContacts(com.bronto.api.contactObject[] contacts) {
            this.UpdateContactsCalled = true;
            return WriteResult;
        }
    }
}
