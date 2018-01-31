using EmailSystem.com.bronto.api;
using System.Collections.Generic;
using System.Configuration;

namespace EmailSystem {
    public class BrontoApiWrapper : IBrontoApiWrapper {
        private BrontoSoapApiImplService api;

        public sessionHeader SessionHeaderValue {
            get {
                return api.sessionHeaderValue;
            }
            set {
                api.sessionHeaderValue = value;
            }
        }

        public BrontoApiWrapper() {
            api = new BrontoSoapApiImplService();
            api.Timeout = int.Parse(ConfigurationManager.AppSettings["SoapTimeout"]); // 360000
        }

        public string Login(string apiKey) {
            return api.login(apiKey);
        }

        public writeResult AddContacts(contactObject[] contacts) {
            return api.addContacts(contacts);
        }

        public writeResult UpdateContacts(contactObject[] contacts) {
            return api.updateContacts(contacts);
        }

        public writeResult DeleteContacts(contactObject[] contacts) {
            return api.deleteContacts(contacts);
        }

        public writeResult AddOrUpdateOrders(orderObject[] orders) {
            return api.addOrUpdateOrders(orders);
        }

        public contactObject[] ReadContacts(contactFilter filter) {
            return api.readContacts(new contactFilter(), false, false, null, 0, false, false, false, false, false, false, false, false, false, false);
        }

        public fieldObject[] ReadFields(fieldsFilter filter) {
            return api.readFields(filter, 0);
        }

        public contactObject[] ReadContacts(contactFilter filter, string[] contactFieldIds) {
            return api.readContacts(filter, false, false, contactFieldIds, 0, false, false, false, false, false, false, false, false, false, false);
        }

        public mailListObject[] ReadLists(mailListFilter filter) {
            return api.readLists(filter, 0);
        }

        public messageObject[] ReadMessages(messageFilter filter) {
            return api.readMessages(filter, false, 0);
        }

        public writeResult AddDeliveries(deliveryObject[] deliveries) {
            return api.addDeliveries(deliveries);
        }

        public deliveryObject[] ReadDeliveries(deliveryFilter filter) {
            return api.readDeliveries(filter, true, false, 0);
        }
    }
}
