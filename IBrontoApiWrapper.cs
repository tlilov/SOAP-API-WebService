using System;

namespace EmailSystem {
    public interface IBrontoApiWrapper {
        EmailSystem.com.bronto.api.writeResult AddContacts(EmailSystem.com.bronto.api.contactObject[] contacts);
        EmailSystem.com.bronto.api.writeResult AddDeliveries(EmailSystem.com.bronto.api.deliveryObject[] deliveries);
        EmailSystem.com.bronto.api.writeResult AddOrUpdateOrders(EmailSystem.com.bronto.api.orderObject[] orders);
        EmailSystem.com.bronto.api.writeResult DeleteContacts(EmailSystem.com.bronto.api.contactObject[] contacts);
        string Login(string apiKey);
        EmailSystem.com.bronto.api.contactObject[] ReadContacts(EmailSystem.com.bronto.api.contactFilter filter);
        EmailSystem.com.bronto.api.contactObject[] ReadContacts(EmailSystem.com.bronto.api.contactFilter filter, string[] contactFieldIds);
        EmailSystem.com.bronto.api.deliveryObject[] ReadDeliveries(EmailSystem.com.bronto.api.deliveryFilter filter);
        EmailSystem.com.bronto.api.fieldObject[] ReadFields(EmailSystem.com.bronto.api.fieldsFilter filter);
        EmailSystem.com.bronto.api.mailListObject[] ReadLists(EmailSystem.com.bronto.api.mailListFilter filter);
        EmailSystem.com.bronto.api.messageObject[] ReadMessages(EmailSystem.com.bronto.api.messageFilter filter);
        EmailSystem.com.bronto.api.sessionHeader SessionHeaderValue { get; set; }
        EmailSystem.com.bronto.api.writeResult UpdateContacts(EmailSystem.com.bronto.api.contactObject[] contacts);
    }
}
